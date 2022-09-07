using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tekla.Structures.Model;

namespace TeklaHierarchicDefinitions.Models
{
    public class SteelBOMPart : INotifyPropertyChanged
    {
        #region Параметры
        private Part part;
        #endregion

        public SteelBOMPart(Part part)
        {
            this.part = part;
        }

        #region Свойства
        public Part Part
        {
            get
            { return part; }
            set
            {
                part = value;
                OnPropertyChanged();
            }
        }

        public string Material
        {
            get { return part.Material.MaterialString; }
        }

        public string Profile
        {
            get 
            { 
                string profileType = string.Empty;
                double profileWidth = 0;
                var ct = part as ContourPlate;
                var bt = part as BentPlate;
                if (ct != null | bt != null)
                    return part.Profile.ProfileString;
                string pattern = @"^[a-zA-Z\-]*";
                if (part.GetReportProperty("PROFILE_TYPE", ref profileType))
                {
                    if (profileType.Equals("B"))
                    {
                        if (part.GetReportProperty("PROFILE.WIDTH", ref profileWidth))
                            return Regex.Match(part.Profile.ProfileString, pattern).Value + profileWidth.ToString();
                    }
                }
                string profileTplName = string.Empty;
                //if (part.GetReportProperty("PROFILE.TPL_NAME", ref profileTplName) && profileTplName.Length>0)
                //    return profileTplName;
                return part.Profile.ProfileString; 
            }
        }

        public double Weight
        {
            get
            {                
                double weight = 0;
                part.GetReportProperty("WEIGHT", ref weight);
                return weight;
            }
        }

        public double WeightGross
        {
            get
            {
                double weight = 0;
                part.GetReportProperty("WEIGHT_GROSS", ref weight);
                return weight;
            }
        }

        public string ProfileGost
        {
            get
            {
                string profileName = string.Empty;
                string profileNameGost = string.Empty;
                if (part.GetReportProperty("PROFILE.GOST_NOTE", ref profileName))
                {
                    part.GetReportProperty("PROFILE.GOST_NAME", ref profileNameGost);
                }
                string profileType = string.Empty;
                part.GetReportProperty("PROFILE_TYPE", ref profileType);
                if (profileType.Equals("B"))
                {
                    string elementType = string.Empty;
                    part.GetReportProperty("USERDEFINED.ru_tip_elementa", ref elementType);
                    if (elementType == "Настил")
                        return "ГОСТ 8568-77. Листы стальные с ромбическим и чечевичным рифлением";
                    else
                        return "ГОСТ 19903-2015. Сталь листовая горячекатанная";
                }
                return profileNameGost + ". " + profileName;
            }
        }

        public float IsInElementList
        {
            get
            {
                if (TeklaAPIUtils.TeklaDB.IsModelObjectBoundToEL(Part))
                    return 1;
                else
                    return 0;
            }
        }

        public string Category
        {
            get
            {
                string profileName = string.Empty;
                part.GetReportProperty("ASSEMBLY.MAINPART.USERDEFINED.RU_BOM_CTG", ref profileName);
                return profileName;
            }
        }

        public string MaterialGost
        {
            get
            {
                string material = string.Empty;
                string materialGost = string.Empty;
                part.GetReportProperty("MATERIAL.USERDEFINED.GOST_NOTE", ref material);
                part.GetReportProperty("MATERIAL.USERDEFINED.GOST_NAME", ref materialGost);
                return material + ". " + materialGost;
            }
        }
        
        public string GroupingCode
        {
            get { return Material + Category + Profile; }
        }
        #endregion

        #region Обработка изменения свойств
        /// <summary>
        /// Отслеживает изменения свойств
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
    public class SteelBOMPosition : INotifyPropertyChanged
    {
        #region Параметры
        ObservableCollection<SteelBOMPart> parts= new ObservableCollection<SteelBOMPart>();
        #endregion

        #region Конструктор
        internal SteelBOMPosition(List<SteelBOMPart> parts)
        {
            this.parts = new ObservableCollection<SteelBOMPart>(parts);
        }
        #endregion

        #region Свойства
        public ObservableCollection<SteelBOMPart> Parts
        {
            get
            { return parts; }
            set
            {
                parts = value;
                OnPropertyChanged();
            }
        }

        public string AttachedToEL
        {
            get
            {
                var proportion = Parts.Select(t=>t.IsInElementList).Sum()/ Parts.Count;
                if (proportion == 0)
                    return "Нет";
                if (proportion < 1)
                    return "Частично";
                return "Все";
            }
        }

        public string Material 
        { 
            get { return parts.FirstOrDefault().Material; } 
        }

        public string Profile 
        { 
            get { return parts.FirstOrDefault().Profile; } 
        }

        public double Weight 
        { 
            get { return parts.Select(t =>t.Weight).Sum(); } 
        }

        public double WeightGross
        {
            get { return parts.Select(t => t.WeightGross).Sum(); }
        }

        public double WeightRounded
        {
            get
            {
                return double.Parse( Weight.ToString("F"));
            }
        }

        public double WeightGrossRounded
        {
            get
            {
                return double.Parse(WeightGross.ToString("F"));
            }
        }

        public string ProfileGost 
        { 
            get {return parts.FirstOrDefault().ProfileGost; } 
        }

        public string Category
        {
            get 
            {
                string cat = parts.FirstOrDefault().Category;
                if (cat.Length > 0)
                    return cat;
                else
                    return "-";
            }
        }

        public string MaterialGost
        {
            get => parts.FirstOrDefault().MaterialGost;
        }
        #endregion

        #region Обработка изменения свойств
        /// <summary>
        /// Отслеживает изменения свойств
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion
    }
}
