using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;

namespace TeklaHierarchicDefinitions.Models
{
    internal class SteelBOMPart : INotifyPropertyChanged
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
            get { return part.Profile.ProfileString; }
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

        public string ProfileGost
        {
            get
            {
                string profileName = string.Empty;
                string profileNameGost = string.Empty;
                part.GetReportProperty("PROFILE.USERDEFINED.GOST_NOTE", ref profileNameGost);
                part.GetReportProperty("PROFILE.USERDEFINED.GOST_NAME", ref profileName);
                return profileNameGost + ". " + profileName;
            }
        }

        public string Category
        {
            get
            {
                string profileName = string.Empty;
                part.GetReportProperty("USERDEFINED.RU_BOM_CTG", ref profileName);
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
    internal class SteelBOMPosition : INotifyPropertyChanged
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

        public string ProfileGost 
        { 
            get {return parts.FirstOrDefault().ProfileGost; } 
        }

        public string Category
        {
            get => parts.FirstOrDefault().Category;
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
