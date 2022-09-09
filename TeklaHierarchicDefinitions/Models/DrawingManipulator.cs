using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tekla.Structures.Drawing;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using TeklaHierarchicDefinitions.TeklaAPIUtils;
using Tekla.Structures.Model;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;
using System.Text;

namespace TeklaHierarchicDefinitions.Models
{
    public class DrawingManipulator : INotifyPropertyChanged
    {
        [Ignore]
        internal Drawing Drawing { get; set; }

        [Ignore]
        public string Mark
        {
            get { return Drawing.Mark; }
        }

        [Name("Ключ документа")]
        public string Code
        {
            get => $"{ModelManipulator.projectInfo.ProjectNumber}-{Album}_{Drawing.Name}";
        }

        [Name("Комплект")]
        public string Complex
        {
            get => ModelManipulator.projectInfo.ProjectNumber + "-" + Album;
        }

        [Name("Объект строительства 1")]
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

        [Name("Объект строительства 2")]
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

        [Name("Объект строительства 3")]
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

        [Name("Объект строительства 4")]
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

        [Name("Название объекта 1")]
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

        [Name("Название объекта 2")]
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

        [Name("Название объекта 3")]
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

        [Name("Название объекта 4")]
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

        [Ignore]
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

        [Name("Листов")]
        public string Lists
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("lists", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("lists", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Стадия")]
        public string Stage
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_stadiya", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_stadiya", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }
        
        [Name("Лист")]
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

        [Name("Альбом")]
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

        [Name("Название")]
        public string Name
        {
            get
            {
                return $"{Title1} {Title2} {Title3}";
            }
        }

        [Ignore]
        public string Title1
        {
            get => Drawing.Title1; set
            {
                Drawing.Title1 = value;

                if (Drawing.Modify())
                    TeklaAPIUtils.TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Ignore]
        public string Title2
        {
            get => Drawing.Title2; set
            {
                Drawing.Title2 = value;
                if (Drawing.Modify())
                    TeklaAPIUtils.TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Ignore]
        public string Title3
        {
            get => Drawing.Title3; set
            {
                Drawing.Title3 = value;
                if (Drawing.Modify())
                    TeklaAPIUtils.TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Должность 1")]
        public string ru_6
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_6_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_6_dop", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Фамилия 1")]
        public string ru_6_fam
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_6_fam_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_6_fam_dop", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Должность 2")]
        public string ru_7
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_7_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_7_dop", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Фамилия 2")]
        public string ru_7_fam
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_7_fam_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_7_fam_dop", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Должность 3")]
        public string ru_8
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_8_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_8_dop", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Фамилия 3")]
        public string ru_8_fam
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_8_fam_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_8_fam_dop", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Должность 4")]
        public string ru_9
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_9_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_9_dop", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Фамилия 4")]
        public string ru_9_fam
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_9_fam_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_9_fam_dop", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Должность 5")]
        public string ru_10
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_10_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_10_dop", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Фамилия 5")]
        public string ru_10_fam
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_10_fam_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_10_fam_dop", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Должность 6")]
        public string ru_11
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_11_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_11_dop", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Фамилия 6")]
        public string ru_11_fam
        {
            get
            {
                var s = string.Empty;
                Drawing.GetUserProperty("ru_11_fam_dop", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("ru_11_fam_dop", value);
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

    public sealed class DrawingManipulatorMap : ClassMap<DrawingManipulator>
    {
        public DrawingManipulatorMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            //Map(m => m.ConstructionObject1).Name("ColumnA");
            //Map(m => m.ObjectName1).Name("ColumnB");
        }
    }

    public static class DrawingGroup
    {
        private static PropertyRooting filler = PropertyRooting.Model;

        public static List<DrawingManipulator> DrawingManipulators { get; set; } = new List<DrawingManipulator>();

        public static List<ModelManipulator> ModelManipulators
        {
            get
            {
                var modelManipulators = new List<ModelManipulator>();
                modelManipulators.Add(new ModelManipulator());
                return modelManipulators;
            }
        }

        public static string album { get; set; }

        public static ObservableCollection<PropertyFillers> PropertyFillersList
        {
            get
            {
                var res = new ObservableCollection<PropertyFillers>();
                var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                if (objectList.Count > 0)
                {
                    res.Add(new PropertyFillers() { InternalPropertyName = "Album", PropertyName = "Комплект", ReferencedObjectList = objectList });
                    if (Filler == PropertyRooting.Model)
                    {
                        res.Add(new PropertyFillers() { InternalPropertyName = "Stage", PropertyName = "Стадия комплекта", ReferencedObjectList = objectList });
                    }
                    else if(Filler == PropertyRooting.Album)
                    {
                        res.Add(new PropertyFillers() { InternalPropertyName = "Stage", PropertyName = "Стадия комплекта", ReferencedObjectList = objectList });
                    }
                    else 
                    {
                        res.Add(new PropertyFillers() { InternalPropertyName = "Stage", PropertyName = "Стадия чертежа", ReferencedObjectList = objectList });
                    }
                    res.Add(new PropertyFillers() { InternalPropertyName = "Lists", PropertyName = "Листов", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName1", PropertyName = "Имя объекта: 1" , ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName2", PropertyName = "Имя объекта: 2", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName3", PropertyName = "Имя объекта: 3", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName4", PropertyName = "Имя объекта: 4", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName5", PropertyName = "Имя объекта: 5", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject1", PropertyName = "Объект строительства: 1", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject2", PropertyName = "Объект строительства: 2", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject3", PropertyName = "Объект строительства: 3", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject4", PropertyName = "Объект строительства: 4", ReferencedObjectList = objectList });
                }
                return res;
            }
        }

        public static ObservableCollection<PropertyFillers> ModelPropertyFillersList
        {
            get
            {
                var res = new ObservableCollection<PropertyFillers>();
                var objectList = DrawingGroup.ModelManipulators.Cast<object>().ToList();
                res.Add(new PropertyFillers() { InternalPropertyName = "ProjectCode", PropertyName = "Код проекта", ReferencedObjectList = objectList });
                res.Add(new PropertyFillers() { InternalPropertyName = "Description", PropertyName = "Описание", ReferencedObjectList = objectList });
                res.Add(new PropertyFillers() { InternalPropertyName = "Object", PropertyName = "Объект", ReferencedObjectList = objectList });
                res.Add(new PropertyFillers() { InternalPropertyName = "Designer", PropertyName = "Компания-разработчик", ReferencedObjectList = objectList });
                res.Add(new PropertyFillers() { InternalPropertyName = "DesignerCompany1", PropertyName = "Компания строка 1", ReferencedObjectList = objectList });
                res.Add(new PropertyFillers() { InternalPropertyName = "DesignerCompany2", PropertyName = "Компания строка 2", ReferencedObjectList = objectList });
                res.Add(new PropertyFillers() { InternalPropertyName = "DesignerCompany3", PropertyName = "Компания строка 3", ReferencedObjectList = objectList });
                
                return res;
            }
        }

        public static ObservableCollection<PropertyFillers> AlbumDesigners
        {
            get
            {
                var res = new ObservableCollection<PropertyFillers>();

                if (Filler == PropertyRooting.Model)
                {
                    var objectList = DrawingGroup.ModelManipulators.Cast<object>().ToList();
                    res.Add(new PropertyFillers() { 
                        InternalPropertyName = "ru_6", PropertyName = "Строка в модели: 1",
                        InternalPropertyName2 = "ru_6_fam", PropertyName2 = "Фамилия: 1",
                        ReferencedObjectList = objectList });

                    res.Add(new PropertyFillers() { InternalPropertyName = "ru_7", 
                        PropertyName = "Строка в модели: 2",
                        InternalPropertyName2 = "ru_7_fam",  PropertyName2 = "Фамилия: 2",
                        ReferencedObjectList = objectList });

                    res.Add(new PropertyFillers() { 
                        InternalPropertyName = "ru_8", 
                        PropertyName = "Строка в модели: 3",
                        InternalPropertyName2 = "ru_8_fam",  PropertyName2 = "Фамилия: 3",
                        ReferencedObjectList = objectList });

                    res.Add(new PropertyFillers() { 
                        InternalPropertyName = "ru_9", 
                        PropertyName = "Строка в модели: 4",
                        InternalPropertyName2 = "ru_9_fam",
                        PropertyName2 = "Фамилия: 4",
                        ReferencedObjectList = objectList });

                    res.Add(new PropertyFillers() { 
                        InternalPropertyName = "ru_10", 
                        PropertyName = "Строка в модели: 5",
                        InternalPropertyName2 = "ru_10_fam",PropertyName2 = "Фамилия: 5",
                        ReferencedObjectList = objectList });

                    res.Add(new PropertyFillers() { 
                        InternalPropertyName = "ru_11", 
                        PropertyName = "Строка в модели: 6",
                        InternalPropertyName2 = "ru_11_fam", PropertyName2 = "Фамилия: 6",
                        ReferencedObjectList = objectList });
                }
                else if(Filler == PropertyRooting.Album)
                {
                    var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_6",
                        PropertyName = "Строка в альбоме: 1",
                        InternalPropertyName2 = "ru_6_fam",
                        PropertyName2 = "Фамилия в альбоме: 1",
                        ReferencedObjectList = objectList
                    });

                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_7",
                        PropertyName = "Строка в альбоме: 2",
                        InternalPropertyName2 = "ru_7_fam",
                        PropertyName2 = "Фамилия в альбоме: 2",
                        ReferencedObjectList = objectList
                    });

                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_8",
                        PropertyName = "Строка в альбоме: 3",
                        InternalPropertyName2 = "ru_8_fam",
                        PropertyName2 = "Фамилия в альбоме: 3",
                        ReferencedObjectList = objectList
                    });

                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_9",
                        PropertyName = "Строка в альбоме: 4",
                        InternalPropertyName2 = "ru_9_fam",
                        PropertyName2 = "Фамилия в альбоме: 4",
                        ReferencedObjectList = objectList
                    });

                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_10",
                        PropertyName = "Строка в альбоме: 5",
                        InternalPropertyName2 = "ru_10_fam",
                        PropertyName2 = "Фамилия в альбоме: 5",
                        ReferencedObjectList = objectList
                    });

                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_11",
                        PropertyName = "Строка в альбоме: 6",
                        InternalPropertyName2 = "ru_11_fam",
                        PropertyName2 = "Фамилия в альбоме: 6",
                        ReferencedObjectList = objectList
                    });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_6", PropertyName = "Должность в альбоме: 1", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_6_fam", PropertyName = "Фамилия в альбоме: 1", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_7", PropertyName = "Должность в альбоме: 2", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_7_fam", PropertyName = "Фамилия в альбоме: 2", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_8", PropertyName = "Должность в альбоме: 3", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_8_fam", PropertyName = "Фамилия в альбоме: 3", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_9", PropertyName = "Должность в альбоме: 4", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_9_fam", PropertyName = "Фамилия в альбоме: 4", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_10", PropertyName = "Должность в альбоме: 5", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_10_fam", PropertyName = "Фамилия в альбоме: 5", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_11", PropertyName = "Должность в альбоме: 6", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ru_11_fam", PropertyName = "Фамилия в альбоме: 6", ReferencedObjectList = objectList });
                }
                else if(Filler == PropertyRooting.Drawing)
                {
                    var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                    if(objectList.Count > 0)
                    {

                        res.Add(new PropertyFillers()
                        {
                            InternalPropertyName = "ru_6",
                            PropertyName = "Строка в чертеже: 1",
                            InternalPropertyName2 = "ru_6_fam",
                            PropertyName2 = "Фамилия в чертеже: 1",
                            ReferencedObjectList = objectList
                        });

                        res.Add(new PropertyFillers()
                        {
                            InternalPropertyName = "ru_7",
                            PropertyName = "Строка в чертеже: 2",
                            InternalPropertyName2 = "ru_7_fam",
                            PropertyName2 = "Фамилия в чертеже: 2",
                            ReferencedObjectList = objectList
                        });

                        res.Add(new PropertyFillers()
                        {
                            InternalPropertyName = "ru_8",
                            PropertyName = "Строка в чертеже: 3",
                            InternalPropertyName2 = "ru_8_fam",
                            PropertyName2 = "Фамилия в чертеже: 3",
                            ReferencedObjectList = objectList
                        });

                        res.Add(new PropertyFillers()
                        {
                            InternalPropertyName = "ru_9",
                            PropertyName = "Строка в чертеже: 4",
                            InternalPropertyName2 = "ru_9_fam",
                            PropertyName2 = "Фамилия в чертеже: 4",
                            ReferencedObjectList = objectList
                        });

                        res.Add(new PropertyFillers()
                        {
                            InternalPropertyName = "ru_10",
                            PropertyName = "Строка в чертеже: 5",
                            InternalPropertyName2 = "ru_10_fam",
                            PropertyName2 = "Фамилия в чертеже: 5",
                            ReferencedObjectList = objectList
                        });

                        res.Add(new PropertyFillers()
                        {
                            InternalPropertyName = "ru_11",
                            PropertyName = "Строка в чертеже: 6",
                            InternalPropertyName2 = "ru_11_fam",
                            PropertyName2 = "Фамилия в чертеже: 6",
                            ReferencedObjectList = objectList
                        });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_6", PropertyName = "Должность в чертеже: 1", ReferencedObjectList = objectList });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_6_fam", PropertyName = "Фамилия в чертеже: 1", ReferencedObjectList = objectList });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_7", PropertyName = "Должность в чертеже: 2", ReferencedObjectList = objectList });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_7_fam", PropertyName = "Фамилия в чертеже: 2", ReferencedObjectList = objectList });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_8", PropertyName = "Должность в чертеже: 3", ReferencedObjectList = objectList });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_8_fam", PropertyName = "Фамилия в чертеже: 3", ReferencedObjectList = objectList });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_9", PropertyName = "Должность в чертеже: 4", ReferencedObjectList = objectList });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_9_fam", PropertyName = "Фамилия в чертеже: 4", ReferencedObjectList = objectList });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_10", PropertyName = "Должность в чертеже: 5", ReferencedObjectList = objectList });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_10_fam", PropertyName = "Фамилия в чертеже: 5", ReferencedObjectList = objectList });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_11", PropertyName = "Должность в чертеже: 6", ReferencedObjectList = objectList });
                        //res.Add(new PropertyFillers() { InternalPropertyName = "ru_11_fam", PropertyName = "Фамилия в чертеже: 6", ReferencedObjectList = objectList });
                    }
                    }

                else
                {
                }
                return res;
            }
        }

        public static async Task<bool> CsvExport(ObservableCollection<DrawingManipulator> drawings)
        {
            try
            {
                var project = TeklaDB.model.GetInfo();
                var tt = Path.Combine(project.ModelPath, "#"+Path.GetFileNameWithoutExtension(project.ModelName));
                var path = $"{tt}.csv";
                using (var writer = new StreamWriter(path,false, Encoding.UTF8))
                {
                    var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };
                using (var csv = new CsvWriter(writer, config) )
                {
                    csv.Context.RegisterClassMap<DrawingManipulatorMap>();
                    
                    csv.WriteRecords(drawings);
                }

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }           
        }

        public static PropertyRooting Filler
        {
            get => filler; 
            set
            { 
                filler = value;
                OnGlobalPropertyChanged("PropertyFillersList");
                OnGlobalPropertyChanged("AlbumDesigners");
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

    public class ModelManipulator  : INotifyPropertyChanged
    {
        public static ProjectInfo projectInfo = TeklaDB.model.GetProjectInfo();
        public string Object
        {
            get
            {                
                return projectInfo.Object;
            }
            set
            {
                projectInfo.Object = value;
                projectInfo.Modify();
                TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string ProjectCode
        {
            get
            {
                projectInfo.Info1 = $"#{Path.GetFileNameWithoutExtension(TeklaDB.model.GetInfo().ModelName)}";
                return projectInfo.ProjectNumber;
            }
            set
            {
                projectInfo.ProjectNumber = value;
                projectInfo.SetUserProperty("ru_shifr", value);
                projectInfo.Modify();
                TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return projectInfo.Description;
            }
            set
            {
                projectInfo.Description = value;
                projectInfo.Modify();
                TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string Designer
        {
            get
            {
                return projectInfo.Designer;
            }
            set
            {
                projectInfo.Designer = value;
                projectInfo.Modify();
                TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        public string DesignerCompany1
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_nazvanie_org_1", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_nazvanie_org_1", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string DesignerCompany2
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_nazvanie_org_2", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_nazvanie_org_2", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string DesignerCompany3
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_nazvanie_org_3", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_nazvanie_org_3", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_6
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_6", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_6", value);
                projectInfo.Modify();
                OnPropertyChanged();                
            }
        }

        public string ru_6_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_6_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_6_fam", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_7
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_7", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_7", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_7_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_7_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_7_fam", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_8
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_8", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_8", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_8_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_8_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_8_fam", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_9
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_9", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_9", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_9_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_9_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_9_fam", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_10
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_10", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_10", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_10_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_10_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_10_fam", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_11
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_11", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_11", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_11_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserProperty("ru_11_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserProperty("ru_11_fam", value);
                projectInfo.Modify();
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

    public enum PropertyRooting
    {
        Drawing,
        Album,
        Model
    }

    public class PropertyFillers
    {
        List<object> referencedObjectList = new List<object>();
        public string PropertyName { get; set; }
        public string InternalPropertyName { get; set; }

        public string PropertyName2 { get; set; }
        public string InternalPropertyName2 { get; set; }

        internal List<object> ReferencedObjectList
        {
            get
            {
                return referencedObjectList;
            }
            set
            {
                referencedObjectList = value;
                OnPropertyChanged();
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
                    var sv = ReferencedObjectList.Select(x => prop.GetValue(x).ToString()).Distinct().Where(t=>t.Length > 0).ToList();
                    if (sv.Count > 1)
                        return $"[{string.Join("; ", sv)}]";
                    else
                        return sv.FirstOrDefault();
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


        public string PropertyValue2
        {
            get
            {
                object obj = ReferencedObjectList.FirstOrDefault();
                if(null != obj)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(InternalPropertyName2, BindingFlags.Public | BindingFlags.Instance);
                    if (prop.CanRead)
                    {
                        return prop.GetValue(obj).ToString();
                    }

                }
                return null;
            }
            set
            {
                foreach (var obj in ReferencedObjectList)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(InternalPropertyName2, BindingFlags.Public | BindingFlags.Instance);
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