using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using TeklaHierarchicDefinitions.TeklaAPIUtils;
using TeklaHierarchicDefinitions;
using System.Windows;

namespace TeklaHierarchicDefinitions.ViewsModels
{
    class VM
    {
        public DataGridHierarchialData DataGrid;
        public void ReadObjects()
        {
            //Получаем таблицу
            var list = TeklaAPIUtils.TeklaDB.GetHierarchicObjectsWithHierarchicDefinitionName("ElementList"); //Talo80Beams
            DataTable accTable = TeklaAPIUtils.ToDataTableConverter.CreateDataTable(list);


            accTable.DefaultView.Sort = "Name";

            DataGridHierarchialData DataGrid = new DataGridHierarchialData();

            // Метод упаковки детей
            //Action<DataRowView, DataGridHierarchialDataModel> Sort = null;
            //Sort = new Action<DataRowView, DataGridHierarchialDataModel>((row, parent) =>
            //{
            //    DataGridHierarchialDataModel t = new DataGridHierarchialDataModel() { Data = row, DataManager = data };
            //    if (row["iGroup"].ToString() == "1")
            //    {
            //        foreach (DataRowView r in accTable.DefaultView.FindRows(row["iSmajId"]))
            //            Sort(r, t);
            //    }
            //    parent.AddChild(t);
            //});


            //foreach (DataRowView r in accTable.DefaultView.FindRows(0))
            //{


            //    DataGridHierarchialDataModel t = new DataGridHierarchialDataModel() { 
            //        Data = r, // обеспечивает управление отображаемостью
            //        DataManager = data // Управляет иерархией
            //    };


            //    if (r["iGroup"].ToString() == "1")
            //    {
            //        foreach (DataRowView rf in accTable.DefaultView.FindRows(r["iSmajId"]))
            //            Sort(rf, t);
            //    }

            //    t.IsVisible = true; // first layer
            //    data.RawData.Add(t);
            //}

            foreach (DataRowView r in accTable.DefaultView)
            {
                DGHDModel t = new DGHDModel()
                {
                    Data = r, // обеспечивает управление отображаемостью
                    DataManager = DataGrid // Управляет иерархией
                };

                t.IsVisible = true; // first layer
                DataGrid.RawData.Add(t);
            }

            DataGrid.Initialize();
            //dg.ItemsSource = DataGrid;
        }
    }

    /// <summary>
    /// Управление
    /// </summary>
    public class DGHDModel
    {
        // Узел раскрыт/нет
        private bool _expanded = false;

        // Узел видимый или нет.
        private bool _visible = false;

        // Список детей
        public DGHDModel() { Children = new List<DGHDModel>(); }

        //Родитель
        public DGHDModel Parent { get; set; }

        //Управляет иерархией
        public DataGridHierarchialData DataManager { get; set; }

        //Добавляет детей
        public void AddChild(DGHDModel t)
        {
            t.Parent = this;
            Children.Add(t);
        }


        #region LEVEL
        private int _level = -1;
        public int Level
        {
            get
            {
                if (_level == -1)
                {
                    _level = (Parent != null) ? Parent.Level + 1 : 0;
                }
                return _level;
            }
        }

        #endregion
        public bool IsExpanded
        {
            get { return _expanded; }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    if (_expanded == true)
                        Expand();
                    else
                        Collapse();
                }
            }
        }

        public bool IsVisible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    if (_visible)
                        ShowChildren();
                    else
                        HideChildren();
                }
            }
        }
        public bool HasChildren { get { return Children.Count > 0; } }
        public List<DGHDModel> Children { get; set; }

        public object Data { get; set; } // the Data (Specify Binding as such {Binding Data.Field})

        public IEnumerable<DGHDModel> VisibleDescendants
        {
            get
            {
                return Children
                    .Where(x => x.IsVisible)
                    .SelectMany(x => (new[] { x }).Concat(x.VisibleDescendants));
            }
        }

        private void PropModify(string name, string val)
        {
            var modifyableObj = Data as HierarchicObjectInTekla;

        }


        // Expand Collapse
        private void Collapse()
        {
            DataManager.RemoveChildren(this);
            foreach (DGHDModel d in Children)
                d.IsVisible = false;
        }

        private void Expand()
        {
            DataManager.AddChildren(this);
            foreach (DGHDModel d in Children)
                d.IsVisible = true;
        }


        // Only if this is Expanded
        private void HideChildren()
        {
            if (IsExpanded)
            {
                // Following Order is Critical
                DataManager.RemoveChildren(this);
                foreach (DGHDModel d in Children)
                    d.IsVisible = false;
            }
        }
        private void ShowChildren()
        {
            if (IsExpanded)
            {
                // Following Order is Critical
                DataManager.AddChildren(this);
                foreach (DGHDModel d in Children)
                    d.IsVisible = true;
            }
        }
    }


    // Управляет иерархически детишками в виде модели
    public class DataGridHierarchialData : ObservableCollection<DGHDModel>
    {

        public List<DGHDModel> RawData { get; set; } // 
        public DataGridHierarchialData() { RawData = new List<DGHDModel>(); }

        public void Initialize()
        {
            this.Clear();
            foreach (DGHDModel m in RawData.Where(c => c.IsVisible).SelectMany(x => new[] { x }.Concat(x.VisibleDescendants)))
            {
                this.Add(m);
            }
        }

        public void AddChildren(DGHDModel d)
        {
            if (!this.Contains(d))
                return;
            int parentIndex = this.IndexOf(d);
            foreach (DGHDModel c in d.Children)
            {
                parentIndex += 1;
                this.Insert(parentIndex, c);
            }
        }

        public void RemoveChildren(DGHDModel d)
        {
            foreach (DGHDModel c in d.Children)
            {
                if (this.Contains(c))
                    this.Remove(c);
            }
        }
    }


}


