using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tekla.Structures.Drawing;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;
using TeklaHierarchicDefinitions.TeklaAPIUtils;

namespace TeklaHierarchicDefinitions.Models
{
    public class DrawingManipulator: INotifyPropertyChanged
    {
        internal Drawing Drawing { get; set; }

        public string Mark
        {
            get { return Drawing.Mark; }
        }

        public string Album
        {
            get 
            {
                var album = string.Empty;
                Drawing.GetUserProperty("ru_marka_komp_chert", ref album);
                return album; 
            }
            set 
            {
                Drawing.SetUserProperty("ru_marka_komp_chert", value);
                if (Drawing.CommitChanges())
                    TeklaAPIUtils.TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string List
        {
            get => Drawing.Name;
            set
            {
                Drawing.Name = value;
                if (Drawing.Modify())
                    TeklaAPIUtils.TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string Title1 { get => Drawing.Title1; set
            {
                Drawing.Title1 = value;
                
                if (Drawing.Modify())
                    TeklaAPIUtils.TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string Title2 { get => Drawing.Title2; set
            {
                Drawing.Title2 = value;
                if (Drawing.Modify())
                    TeklaAPIUtils.TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string Title3 { get => Drawing.Title3; set
            {
                Drawing.Title3 = value;
                if (Drawing.Modify())
                    TeklaAPIUtils.TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string ObjectName1
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_naimen_stroit_1", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_naimen_stroit_1", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string ObjectName2
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_naimen_stroit_2", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_naimen_stroit_2", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string ObjectName3
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_naimen_stroit_3", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_naimen_stroit_3", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string ObjectName4
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_naimen_stroit_4", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_naimen_stroit_4", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string ObjectName5
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_naimen_stroit_5", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_naimen_stroit_5", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string ConstructionObject1
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_objekt_stroit_1", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_objekt_stroit_1", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string ConstructionObject2
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_objekt_stroit_2", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_objekt_stroit_2", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string ConstructionObject3
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_objekt_stroit_3", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_objekt_stroit_3", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string ConstructionObject4
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_objekt_stroit_4", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_objekt_stroit_4", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

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

    public static class DrawingGroup
    {
        private static PropertyRooting filler = PropertyRooting.Model;

        public static List<DrawingManipulator> DrawingManipulators { get; set; } = new List<DrawingManipulator>();

        public static string album { get; set; }

        public static ObservableCollection<PropertyFillers> PropertyFillersList
        {
            get
            {
                var res = new ObservableCollection<PropertyFillers>();
                if(Filler == PropertyRooting.Model)
                {

                }
                else if(Filler == PropertyRooting.Album)
                {
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName1", PropertyName = "Имя объекта: 1" });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName2", PropertyName = "Имя объекта: 2" });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName3", PropertyName = "Имя объекта: 3" });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName4", PropertyName = "Имя объекта: 4" });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName5", PropertyName = "Имя объекта: 5" });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject1", PropertyName = "Объект строительства: 1" });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject2", PropertyName = "Объект строительства: 2" });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject3", PropertyName = "Объект строительства: 3" });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject4", PropertyName = "Объект строительства: 4" });
                }   
                else
                {
                }
                return res;
            }
        }

        public static PropertyRooting Filler
        {
            get => filler; 
            set
            { 
                filler = value;
                OnGlobalPropertyChanged("PropertyFillersList");
            }
        }

        #region Обработка изменения свойств
        /// <summary>
        /// Отслеживает изменения свойств
        /// </summary>

        static event PropertyChangedEventHandler GlobalPropertyChanged = delegate { };

        static void OnGlobalPropertyChanged(string propertyName)
        {
            GlobalPropertyChanged(
                typeof(DrawingGroup),
                new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }



    public enum PropertyRooting
    {
        Drawing,
        Album,
        Model
    }

    public class PropertyFillers
    {
        public string PropertyName { get; set; }
        public string InternalPropertyName { get; set; }

        internal List<object> ReferencedObjectList
        {
            get
            {
                List<object> dm = new List<object>();
                dm.Add(TeklaDB.model);
                if (DrawingGroup.Filler != PropertyRooting.Model)
                    dm = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                return dm;
            }
        }

        public string PropertyValue
        {
            get
            {
                object obj = ReferencedObjectList.FirstOrDefault();
                PropertyInfo prop = obj.GetType().GetProperty(InternalPropertyName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanRead)
                {
                    return prop.GetValue(obj).ToString();
                }
                else
                    return null;
            }
            set
            {
                foreach(var obj in ReferencedObjectList)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(InternalPropertyName, BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanWrite)
                    {
                        prop.SetValue(obj, value, null);
                    }
                }
                OnPropertyChanged();
            }
        }

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