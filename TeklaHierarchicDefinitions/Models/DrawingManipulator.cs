using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using TeklaHierarchicDefinitions.TeklaAPIUtils;

namespace TeklaHierarchicDefinitions.Models
{

    public class DrawingManipulator : INotifyPropertyChanged
    {
        [Ignore]
        internal DrawingEnvelop Drawing { get; set; } = new DrawingEnvelop();

        [Ignore]
        public string Mark
        {
            get { return Drawing.Mark; }
        }

        [Name("Ключ документа")]
        [Index(0)]
        public string Code
        {
            get => $"{ModelManipulator.projectInfo.ProjectNumber}-{Album}_{Drawing.Name}";
        }

        [Name("Комплект")]
        [Index(1)]
        public string Complex
        {
            get => ModelManipulator.projectInfo.ProjectNumber + "-" + Album;
        }

        [Name("Объект строительства 1")]
        [Index(2)]
        public string ConstructionObject1
        {
            get
            {
                var s = " ";
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
        [Index(3)]
        public string ConstructionObject2
        {
            get
            {
                var s = " ";
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
        [Index(4)]
        public string ConstructionObject3
        {
            get
            {
                var s = " ";
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
        [Index(5)]
        public string ConstructionObject4
        {
            get
            {
                var s = " ";
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
        [Index(6)]
        public string ObjectName1
        {
            get
            {
                var s = " ";
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
        [Index(7)]
        public string ObjectName2
        {
            get
            {
                var s = " ";
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
        [Index(8)]
        public string ObjectName3
        {
            get
            {
                var s = " ";
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
        [Index(9)]
        public string ObjectName4
        {
            get
            {
                var s = " ";
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

        //[Ignore]
        //public string ObjectName5
        //{
        //    get
        //    {
        //        var s = " ";
        //        Drawing.GetUserProperty("ru_naimen_stroit_5", ref s);
        //        return s;
        //    }
        //    set
        //    {
        //        Drawing.SetUserProperty("ru_naimen_stroit_5", value);
        //        Drawing.CommitChanges();
        //        OnPropertyChanged();
        //    }
        //}

        [Name("Листов")]
        [Index(10)]
        public string Lists
        {
            get
            {
                var s = " ";
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
        [Index(11)]
        public string Stage
        {
            get
            {
                var s = " ";
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
        [Index(12)]
        public string List
        {
            get
            {
                if (Drawing.Name.Length == 0)
                    return " ";
                else
                    return Drawing.Name;
            }
            set
            {
                Drawing.Name = value;
                if (Drawing.Modify())
                    TeklaAPIUtils.TeklaDB.model.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Name("Альбом")]
        [Index(14)]
        public string Album
        {
            get
            {
                var album = " ";
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
        [Index(13)]
        public string Name
        {
            get
            {
                string[] vs = { Title1, Title2, Title3 };
                return string.Join(" ", vs);
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
        [Index(15)]
        public string ru_6
        {
            get
            {
                var s = " ";
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
        [Index(16)]
        public string ru_6_fam
        {
            get
            {
                var s = " ";
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
        [Index(17)]
        public string ru_7
        {
            get
            {
                var s = " ";
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
        [Index(18)]
        public string ru_7_fam
        {
            get
            {
                var s = " ";
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
        [Index(19)]
        public string ru_8
        {
            get
            {
                var s = " ";
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
        [Index(20)]
        public string ru_8_fam
        {
            get
            {
                var s = " ";
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
        [Index(21)]
        public string ru_9
        {
            get
            {
                var s = " ";
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
        [Index(22)]
        public string ru_9_fam
        {
            get
            {
                var s = " ";
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
        [Index(23)]
        public string ru_10
        {
            get
            {
                var s = " ";
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
        [Index(24)]
        public string ru_10_fam
        {
            get
            {
                var s = " ";
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
        [Index(25)]
        public string ru_11
        {
            get
            {
                var s = " ";
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
        [Index(26)]
        public string ru_11_fam
        {
            get
            {
                var s = " ";
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

        [Name("Дата утверждения чертежа")]
        [Index(27)]
        public string DrawingDate
        {
            get
            {
                var s = " ";
                Drawing.GetUserProperty("DR_APPROVAL_DATE", ref s);
                return s;
            }
            set
            {
                Drawing.SetUserProperty("DR_APPROVAL_DATE", value);
                Drawing.CommitChanges();
                OnPropertyChanged();
            }
        }

        [Ignore]
        public string Tooltip
        {
            get
            {
                var propInfos = GetType().GetProperties().Where(n => n.Name != "Tooltip").Select(t => t.Name + " : " + t.GetValue(this, null)).ToList();
                return string.Join("\r\n", propInfos);
            }
        }

        public void BorrowProperties(DrawingManipulator otherDrawingManipulator)
        {
            this.Album = otherDrawingManipulator.Album;
            this.ConstructionObject1 = otherDrawingManipulator.ConstructionObject1;
            this.ConstructionObject2 = otherDrawingManipulator.ConstructionObject2;
            this.ConstructionObject3 = otherDrawingManipulator.ConstructionObject3;
            this.ConstructionObject4 = otherDrawingManipulator.ConstructionObject4;
            this.ObjectName1 = otherDrawingManipulator.ObjectName1;
            this.ObjectName2 = otherDrawingManipulator.ObjectName2;
            this.ObjectName3 = otherDrawingManipulator.ObjectName3;
            this.ObjectName4 = otherDrawingManipulator.ObjectName4;
            this.Stage = otherDrawingManipulator.Stage;
            this.ru_11 = otherDrawingManipulator.ru_11;
            this.ru_11_fam = otherDrawingManipulator.ru_11_fam;
            this.ru_10 = otherDrawingManipulator.ru_10;
            this.ru_10_fam = otherDrawingManipulator.ru_10_fam;
            this.ru_9 = otherDrawingManipulator.ru_9;
            this.ru_9_fam = otherDrawingManipulator.ru_9_fam;
            this.ru_8 = otherDrawingManipulator.ru_8;
            this.ru_8_fam = otherDrawingManipulator.ru_8_fam;
            this.ru_7 = otherDrawingManipulator.ru_7;
            this.ru_7_fam = otherDrawingManipulator.ru_7_fam;
            this.ru_6 = otherDrawingManipulator.ru_6;
            this.ru_6_fam = otherDrawingManipulator.ru_6_fam;

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

        internal class DrawingEnvelop
        {
            public DrawingEnvelop() { }

            public DrawingEnvelop(Drawing myDrawing)
            {
                Drawing = myDrawing;
            }

            private Dictionary<string, string> drawingData = new Dictionary<string, string>();
            private string mark;
            private string name;
            private string title1;
            private string title2;
            private string title3;

            public string Mark
            {
                get
                {
                    if (Drawing != null)
                        return Drawing.Mark;
                    else
                        return mark;
                }
            }
            public string Name
            {
                get
                {
                    if (Drawing != null)
                        return Drawing.Name;
                    else
                        return name;
                }
                set
                {
                    if (Drawing != null)
                    {
                        Drawing.Name = value;
                        Drawing.Modify();
                        TeklaDB.model.CommitChanges();
                    }
                    else
                        name = value;

                }
            }


            public string Title1
            {
                get
                {
                    if (Drawing != null)
                        return Drawing.Title1;
                    else
                        return title1;
                }
                set
                {
                    if (Drawing != null)
                    {
                        Drawing.Title1 = value;
                        Drawing.Modify();
                        TeklaDB.model.CommitChanges();
                    }
                    else
                        title1 = value;
                }
            }
            public string Title2
            {
                get
                {
                    if (Drawing != null)
                        return Drawing.Title2;
                    else
                        return title2;
                }
                set
                {
                    if (Drawing != null)
                    {
                        Drawing.Title2 = value;
                        Drawing.Modify();
                        TeklaDB.model.CommitChanges();
                    }
                    else
                        title2 = value;
                }
            }

            public string Title3
            {
                get
                {
                    if (Drawing != null)
                        return Drawing.Title3;
                    else
                        return title3;
                }
                set
                {
                    if (Drawing != null)
                    {
                        Drawing.Title3 = value;
                        Drawing.Modify();
                        TeklaDB.model.CommitChanges();
                    }
                    else
                        title3 = value;
                }
            }

            public Drawing Drawing { get; }

            internal bool CommitChanges()
            {
                if (Drawing != null)
                {
                    return Drawing.CommitChanges();
                }
                return false;
            }

            internal bool GetUserProperty(string v, ref string s)
            {
                if (Drawing != null)
                {
                    return Drawing.GetUserPropertyWrapper(v, ref s);
                }
                else
                {
                    return drawingData.TryGetValue(v, out s);
                }
            }

            internal bool Modify()
            {
                if (Drawing != null)
                {
                    return Drawing.Modify();
                }
                return false;
            }

            internal void SetUserProperty(string v, string value)
            {
                if (Drawing != null & value != null)
                {
                    Drawing.SetUserPropertyWrapper(v, value);
                }
                else
                {
                    drawingData[v] = value;
                }
            }
        }
        #endregion
    }

    public sealed class DrawingManipulatorMap : ClassMap<DrawingManipulator>
    {
        public DrawingManipulatorMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Code);
            Map(m => m.Complex);
            Map(m => m.ConstructionObject1);
            Map(m => m.ConstructionObject2);
            Map(m => m.ConstructionObject3);
            Map(m => m.ConstructionObject4);
            Map(m => m.ObjectName1);
            Map(m => m.ObjectName2);
            Map(m => m.ObjectName3);
            Map(m => m.ObjectName4);
            Map(m => m.Lists);
            Map(m => m.Stage);
            Map(m => m.Name);

        }
    }

    public static class DrawingGroup
    {
        private static PropertyRooting filler = PropertyRooting.Model;

        public static List<DrawingManipulator> DrawingManipulators { get; set; } = new List<DrawingManipulator>();

        public static List<DrawingManipulator> BorrowedListFromCsv { get; set; } = new List<DrawingManipulator>();

        public static List<ModelManipulator> ModelManipulators
        {
            get
            {
                var modelManipulators = new List<ModelManipulator>();
                modelManipulators.Add(new ModelManipulator());
                return modelManipulators;
            }
        }

        public static PropertyFillers DrawingsAlbum
        {
            get
            {
                var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                return new PropertyFillers() { InternalPropertyName = "Album", PropertyName = "Комплект", ReferencedObjectList = objectList };
            }
        }

        public static PropertyFillers AlbumPhase
        {
            get
            {
                var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                if (Filler == PropertyRooting.Model)
                {
                    return new PropertyFillers() { InternalPropertyName = "Stage", PropertyName = "Стадия комплекта", ReferencedObjectList = objectList };
                }
                else if (Filler == PropertyRooting.Album)
                {
                    return new PropertyFillers() { InternalPropertyName = "Stage", PropertyName = "Стадия комплекта", ReferencedObjectList = objectList };
                }
                else
                {
                    return new PropertyFillers() { InternalPropertyName = "Stage", PropertyName = "Стадия чертежа", ReferencedObjectList = objectList };
                }
            }
        }


        public static PropertyFillers ListNumber
        {
            get
            {
                var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                if (Filler == PropertyRooting.Model)
                {
                    return new PropertyFillers() { InternalPropertyName = "List", PropertyName = "Лист", ReferencedObjectList = objectList };
                }
                else if (Filler == PropertyRooting.Album)
                {
                    return new PropertyFillers() { InternalPropertyName = "List", PropertyName = "Лист", ReferencedObjectList = objectList };
                }
                else
                {
                    return new PropertyFillers() { InternalPropertyName = "List", PropertyName = "Лист", ReferencedObjectList = objectList };
                }
            }
        }

        public static PropertyFillers ListsInAlbumTotal
        {
            get
            {
                var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                if (Filler == PropertyRooting.Model)
                {
                    return new PropertyFillers() { InternalPropertyName = "Lists", PropertyName = "Листов", ReferencedObjectList = objectList };
                }
                else if (Filler == PropertyRooting.Album)
                {
                    return new PropertyFillers() { InternalPropertyName = "Lists", PropertyName = "Листов", ReferencedObjectList = objectList };
                }
                else
                {
                    return new PropertyFillers() { InternalPropertyName = "Lists", PropertyName = "Листов", ReferencedObjectList = objectList };
                }
            }
        }

        public static ObservableCollection<PropertyFillers> ObjectPropertyFillersList
        {
            get
            {
                var res = new ObservableCollection<PropertyFillers>();
                var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                if (objectList.Count > 0)
                {
                    //res.Add(new PropertyFillers() { InternalPropertyName = "Album", PropertyName = "Комплект", ReferencedObjectList = objectList });
                    //if (Filler == PropertyRooting.Model)
                    //{
                    //    res.Add(new PropertyFillers() { InternalPropertyName = "Stage", PropertyName = "Стадия комплекта", ReferencedObjectList = objectList });
                    //}
                    //else if (Filler == PropertyRooting.Album)
                    //{
                    //    res.Add(new PropertyFillers() { InternalPropertyName = "Stage", PropertyName = "Стадия комплекта", ReferencedObjectList = objectList });
                    //}
                    //else
                    //{
                    //    res.Add(new PropertyFillers() { InternalPropertyName = "Stage", PropertyName = "Стадия чертежа", ReferencedObjectList = objectList });
                    //}
                    //res.Add(new PropertyFillers() { InternalPropertyName = "Lists", PropertyName = "Листов", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName1", PropertyName = "Имя объекта: 1", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName2", PropertyName = "Имя объекта: 2", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName3", PropertyName = "Имя объекта: 3", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName4", PropertyName = "Имя объекта: 4", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ObjectName5", PropertyName = "Имя объекта: 5", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject1", PropertyName = "Объект строительства: 1", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject2", PropertyName = "Объект строительства: 2", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject3", PropertyName = "Объект строительства: 3", ReferencedObjectList = objectList });
                    //res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject4", PropertyName = "Объект строительства: 4", ReferencedObjectList = objectList });
                }
                return res;
            }
        }

        public static ObservableCollection<PropertyFillers> ConstructionObjectPropertyFillersList
        {
            get
            {
                var res = new ObservableCollection<PropertyFillers>();
                var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                if (objectList.Count > 0)
                {
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject1", PropertyName = "Объект строительства: 1", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject2", PropertyName = "Объект строительства: 2", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject3", PropertyName = "Объект строительства: 3", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "ConstructionObject4", PropertyName = "Объект строительства: 4", ReferencedObjectList = objectList });
                }
                return res;
            }
        }


        public static ObservableCollection<PropertyFillers> TitleFillerList
        {
            get
            {
                var res = new ObservableCollection<PropertyFillers>();
                var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();

                if (objectList.Count == 1)
                {
                    res.Add(new PropertyFillers() { InternalPropertyName = "Title1", PropertyName = "Заголовок 1", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "Title2", PropertyName = "Заголовок 2", ReferencedObjectList = objectList });
                    res.Add(new PropertyFillers() { InternalPropertyName = "Title3", PropertyName = "Заголовок 3", ReferencedObjectList = objectList });
                }
                return res;
            }
        }

        public static PropertyFillers ModelCode
        {
            get
            {
                var objectList = DrawingGroup.ModelManipulators.Cast<object>().ToList();
                return new PropertyFillers() { InternalPropertyName = "ProjectCode", PropertyName = "Код проекта", ReferencedObjectList = objectList };
            }
        }

        public static PropertyFillers DrawingsDates
        {
            get
            {
                var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                return new PropertyFillers() { InternalPropertyName = "DrawingDate", PropertyName = "Дата утверждения", ReferencedObjectList = objectList };
            }
        }

        public static ObservableCollection<PropertyFillers> ModelPropertyFillersList
        {
            get
            {
                var res = new ObservableCollection<PropertyFillers>();
                var objectList = DrawingGroup.ModelManipulators.Cast<object>().ToList();
                //res.Add(new PropertyFillers() { InternalPropertyName = "ProjectCode", PropertyName = "Код проекта", ReferencedObjectList = objectList });
                res.Add(new PropertyFillers() { InternalPropertyName = "Description", PropertyName = "Описание", ReferencedObjectList = objectList });
                res.Add(new PropertyFillers() { InternalPropertyName = "Object", PropertyName = "Объект", ReferencedObjectList = objectList });
                res.Add(new PropertyFillers() { InternalPropertyName = "Designer", PropertyName = "Компания-разработчик", ReferencedObjectList = objectList });
                //res.Add(new PropertyFillers() { InternalPropertyName = "DesignerCompany1", PropertyName = "Компания строка 1", ReferencedObjectList = objectList });
                //res.Add(new PropertyFillers() { InternalPropertyName = "DesignerCompany2", PropertyName = "Компания строка 2", ReferencedObjectList = objectList });
                //res.Add(new PropertyFillers() { InternalPropertyName = "DesignerCompany3", PropertyName = "Компания строка 3", ReferencedObjectList = objectList });

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
                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_6",
                        PropertyName = "Строка в модели: 1",
                        InternalPropertyName2 = "ru_6_fam",
                        PropertyName2 = "Фамилия: 1",
                        ReferencedObjectList = objectList
                    });

                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_7",
                        PropertyName = "Строка в модели: 2",
                        InternalPropertyName2 = "ru_7_fam",
                        PropertyName2 = "Фамилия: 2",
                        ReferencedObjectList = objectList
                    });

                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_8",
                        PropertyName = "Строка в модели: 3",
                        InternalPropertyName2 = "ru_8_fam",
                        PropertyName2 = "Фамилия: 3",
                        ReferencedObjectList = objectList
                    });

                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_9",
                        PropertyName = "Строка в модели: 4",
                        InternalPropertyName2 = "ru_9_fam",
                        PropertyName2 = "Фамилия: 4",
                        ReferencedObjectList = objectList
                    });

                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_10",
                        PropertyName = "Строка в модели: 5",
                        InternalPropertyName2 = "ru_10_fam",
                        PropertyName2 = "Фамилия: 5",
                        ReferencedObjectList = objectList
                    });

                    res.Add(new PropertyFillers()
                    {
                        InternalPropertyName = "ru_11",
                        PropertyName = "Строка в модели: 6",
                        InternalPropertyName2 = "ru_11_fam",
                        PropertyName2 = "Фамилия: 6",
                        ReferencedObjectList = objectList
                    });
                }
                else if (Filler == PropertyRooting.Album)
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
                else if (Filler == PropertyRooting.Drawing)
                {
                    var objectList = DrawingGroup.DrawingManipulators.Cast<object>().ToList();
                    if (objectList.Count > 0)
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
                var tt = Path.Combine(project.ModelPath, "#" + Path.GetFileNameWithoutExtension(project.ModelName));
                var path = $"{tt}.csv";
                using (var writer = new StreamWriter(path, false, Encoding.GetEncoding(1251)))
                {
                    var config = new CsvConfiguration(CultureInfo.CurrentCulture)
                    {
                        ShouldQuote = args => false,
                        Delimiter = ";",
                        Encoding = Encoding.GetEncoding(1251)
                    };
                    using (var csv = new CsvWriter(writer, config))
                    {
                        csv.Context.RegisterClassMap<DrawingManipulatorMap>();

                        csv.WriteRecords(drawings);
                    }

                }
                //if (File.Exists(path))
                //{                    
                //    File.WriteAllText(path, File.ReadAllText(path), Encoding.GetEncoding(1251));
                //}
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

        public static ObservableCollection<PropertyFillers> CompanyNamePropertyFillers
        {
            get
            {
                var res = new ObservableCollection<PropertyFillers>();
                var objectList = DrawingGroup.ModelManipulators.Cast<object>().ToList();
                res.Add(new PropertyFillers() { InternalPropertyName = "DesignerCompany1", PropertyName = "Компания строка 1", ReferencedObjectList = objectList });
                res.Add(new PropertyFillers() { InternalPropertyName = "DesignerCompany2", PropertyName = "Компания строка 2", ReferencedObjectList = objectList });
                res.Add(new PropertyFillers() { InternalPropertyName = "DesignerCompany3", PropertyName = "Компания строка 3", ReferencedObjectList = objectList });

                return res;

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

        internal static void LoadCsv(string path)
        {
            try
            {
                using (var reader = new StreamReader(path, Encoding.GetEncoding(1251)))
                {
                    var config = new CsvConfiguration(CultureInfo.CurrentCulture)
                    {
                        ShouldQuote = args => false,
                        Delimiter = ";",
                        Encoding = Encoding.GetEncoding(1251)
                    };
                    using (var csv = new CsvReader(reader, config))
                    {
                        var rc = new DrawingManipulator();
                        var records = csv.GetRecords<DrawingManipulator>().GroupBy(t => t.Complex).Select(t => t.FirstOrDefault());
                        BorrowedListFromCsv.Clear();
                        BorrowedListFromCsv = records.ToList();
                    }
                }
            }
            catch (Exception ex) { }

        }
        #endregion
    }

    public class ModelManipulator : INotifyPropertyChanged
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
                projectInfo.SetUserPropertyWrapper("ru_shifr", value);
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
                projectInfo.GetUserPropertyWrapper("ru_nazvanie_org_1", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_nazvanie_org_1", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string DesignerCompany2
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_nazvanie_org_2", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_nazvanie_org_2", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string DesignerCompany3
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_nazvanie_org_3", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_nazvanie_org_3", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_6
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_6", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_6", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_6_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_6_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_6_fam", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_7
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_7", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_7", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_7_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_7_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_7_fam", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_8
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_8", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_8", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_8_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_8_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_8_fam", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_9
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_9", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_9", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_9_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_9_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_9_fam", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_10
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_10", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_10", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_10_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_10_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_10_fam", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_11
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_11", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_11", value);
                projectInfo.Modify();
                OnPropertyChanged();
            }
        }

        public string ru_11_fam
        {
            get
            {
                var s = string.Empty;
                projectInfo.GetUserPropertyWrapper("ru_11_fam", ref s);
                return s;
            }
            set
            {
                projectInfo.SetUserPropertyWrapper("ru_11_fam", value);
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
                if (null != obj)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(InternalPropertyName, BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanRead)
                    {
                        var sv = ReferencedObjectList.Select(x => prop.GetValue(x).ToString())
                            .Distinct()
                            //.Where(t => t.Length > 0)
                            .ToList();
                        if (sv.Count > 1)
                            return $"[{string.Join("; ", sv)}]";
                        else
                            return sv.FirstOrDefault();
                    }
                }
                return null;
            }
            set
            {
                foreach (var obj in ReferencedObjectList)
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
                if (null != obj)
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