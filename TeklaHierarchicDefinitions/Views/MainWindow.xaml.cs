﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Tekla.Structures;
using Tekla.Structures.Dialog;
using Tekla.Structures.Model;
using TeklaHierarchicDefinitions.Models;
using TeklaHierarchicDefinitions.TeklaAPIUtils;
using TeklaHierarchicDefinitions.ViewModels;
using DrawingGroup = TeklaHierarchicDefinitions.Models.DrawingGroup;

namespace TeklaHierarchicDefinitions
{
    // https://stackoverflow.com/questions/3457107/handle-editable-hierarchical-data-treeviewdatagrid-hybrid
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ApplicationWindowBase
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new BillOfElementsViewModel();
            this.ShowInTaskbar = true;
            //string assembly = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            //string traductionFile = Path.GetDirectoryName(assembly) + @"\TeklaHierarchicDefinitions.ail";

            //if (File.Exists(traductionFile))
            //{
            //    string lang = GetShortLanguage();

            //    Dialogs.SetSettings(string.Empty);
            //    Tekla.Structures.Dialog.Localization localization = new Tekla.Structures.Dialog.Localization(traductionFile, lang);

            //    //localization.Localize(this);

            //}

            Title = Title + " " + AppInfo.GetVersion();


        }

        public static string GetLongLanguage()
        {
            var tempValue = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XS_LANGUAGE", ref tempValue);
            return tempValue;
        }

        ///  <summary>
        /// Returns 3-letter equivalent of language/ Obsolete
        ///  </summary>
        /// <returns> 3-letter language definition. </returns>
        public static string GetShortLanguage()
        {
            if (string.IsNullOrEmpty(GetLongLanguage())) return "enu";
            switch (GetLongLanguage())
            {
                case "ENGLISH":
                    return "enu";
                case "DUTCH":
                    return "nld";
                case "FRENCH":
                    return "fra";
                case "GERMAN":
                    return "deu";
                case "ITALIAN":
                    return "ita";
                case "SPANISH":
                    return "esp";
                case "JAPANESE":
                    return "jpn";
                case "CHINESE SIMPLIFIED":
                    return "chs";
                case "CHINESE TRADITIONAL":
                    return "cht";
                case "CZECH":
                    return "csy";
                case "PORTUGUESE BRAZILIAN":
                    return "ptb";
                case "HUNGARIAN":
                    return "hun";
                case "POLISH":
                    return "plk";
                case "RUSSIAN":
                    return "rus";
                default:
                    return "enu";
            }
        }

        private void HODataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((bool)HightlightObjects.IsChecked)
            {
                var boe = (sender as System.Windows.Controls.DataGrid).SelectedItem as BillOfElements;
                //if (boe != null)
                //    boe.GetSelectedObjects();
                var boes = (sender as System.Windows.Controls.DataGrid).SelectedItems.Cast<BillOfElements>().SelectMany(t =>
                {
                    var arr = new List<ModelObject>();
                    var xx = t.HierarchicObjectInTekla.HierarchicObject.GetChildren().GetEnumerator();
                    while (xx.MoveNext())
                        arr.Add(xx.Current as ModelObject);
                    return arr;
                }).ToArray();
                TeklaDB.SelectObjectsInModelView(new ArrayList(boes));
            }
        }

        private void SBOMDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var boe = (sender as System.Windows.Controls.DataGrid).SelectedItem as SteelBOMPosition;
            var dc = (this.DataContext as BillOfElementsViewModel);
            var si = (sender as System.Windows.Controls.DataGrid).SelectedItems;
            var filtered = si.Cast<SteelBOMPosition>().SelectMany(t =>
            {
                var arr = new List<ModelObject>();
                IEnumerator<Part> xx = t.Parts.Where(m => m.IsInElementList < 1).Select(m => m.Part).GetEnumerator();
                while (xx.MoveNext())
                    arr.Add(xx.Current as ModelObject);
                return arr;
            }).ToArray();

            var all = si.Cast<SteelBOMPosition>().SelectMany(t =>
            {
                var arr = new List<ModelObject>();
                IEnumerator<Part> xx = t.Parts.Select(m => m.Part).GetEnumerator();
                while (xx.MoveNext())
                    arr.Add(xx.Current as ModelObject);
                return arr;
            }).ToArray();

            if ((sender as System.Windows.Controls.DataGrid).Name == "Materials")
                dc.SBOMMaterials = si.Cast<SteelBOMPosition>().Select(t => t.Material).Distinct().ToList();

            if ((sender as System.Windows.Controls.DataGrid).Name == "Categories")
                dc.SBOMCategories = si.Cast<SteelBOMPosition>().Select(t => t.Category).Distinct().ToList();

            if ((sender as System.Windows.Controls.DataGrid).Name == "Profiles")
                dc.SBOMProfiles = si.Cast<SteelBOMPosition>().Select(t => t.Profile).Distinct().ToList();

            dc.OnPropertyChanged("SteelBOMPositions");

            if ((bool)HightlightSBOMObjects.IsChecked)
            {
                if ((bool)SelectUnattachedObjectsOnly.IsChecked)
                    TeklaDB.SelectObjectsInModelView(new ArrayList(filtered));
                else
                    TeklaDB.SelectObjectsInModelView(new ArrayList(all));
            }
        }

        private void SBOMDataGridSimple_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var boe = (sender as System.Windows.Controls.DataGrid).SelectedItem as SteelBOMPosition;
            var dc = (this.DataContext as BillOfElementsViewModel);
            var si = (sender as System.Windows.Controls.DataGrid).SelectedItems;
            var filtered = si.Cast<SteelBOMPosition>().SelectMany(t =>
            {
                var arr = new List<ModelObject>();
                IEnumerator<Part> xx = t.Parts.Where(m => m.IsInElementList < 1).Select(m => m.Part).GetEnumerator();
                while (xx.MoveNext())
                    arr.Add(xx.Current as ModelObject);
                return arr;
            }).ToArray();

            var all = si.Cast<SteelBOMPosition>().SelectMany(t =>
            {
                var arr = new List<ModelObject>();
                IEnumerator<Part> xx = t.Parts.Select(m => m.Part).GetEnumerator();
                while (xx.MoveNext())
                    arr.Add(xx.Current as ModelObject);
                return arr;
            }).ToArray();

            if ((bool)HightlightSBOMObjects.IsChecked)
            {
                if ((bool)SelectUnattachedObjectsOnly.IsChecked)
                    TeklaDB.SelectObjectsInModelView(new ArrayList(filtered));
                else
                    TeklaDB.SelectObjectsInModelView(new ArrayList(all));
            }
        }


        //private void MaterialCatalog_SelectClicked(object sender, EventArgs e)
        //{
        //    var obj = (BillOfElementsViewModel)sender;
        //    var ddd = sender;
        //    var fsfd = sender as Tekla.Structures.Dialog.UIControls.MaterialCatalog;
        //    //this.materialCatalog.SelectedMaterial;
        //    var dsf = (HODataGrid.SelectedItem);
        //    var erer = (HODataGrid.Columns[1]);
        //    //((HODataGrid.Columns[1]) as Tekla.Structures.Dialog.UIControls.MaterialCatalog).SelectedMaterial = (this.HODataGrid.SelectedItem as BillOfElements).Material;
        //}

        private void MaterialCatalog_SelectionDone(object sender, EventArgs e)
        {
            var selectedItems = ((ObservableCollection<BillOfElements>)(HODataGrid).Items.SourceCollection)
                .Where(x => x.Selection == true).ToList();
            foreach (var i in selectedItems)
            {
                i.Material = materialCatalog.SelectedMaterial;
            }
            HODataGrid.UpdateLayout();
        }

        private void MaterialCatalog_SelectClicked(object sender, EventArgs e)
        {
            var selectedItems = ((ObservableCollection<BillOfElements>)(HODataGrid).Items.SourceCollection)
                .Where(x => x.Selection == true)
                .ToList();
            materialCatalog.SelectedMaterial = ((selectedItems[0]).Material);

        }

        //private void SelectedCheckboxes()
        //{
        //    var selectedItems = ((ObservableCollection<BillOfElements>)(HODataGrid).Items.SourceCollection).Where(x => x.Selection == true).ToList();

        //    //materialCatalog.IsEnabled = (selectedItems.Count > 0);
        //    //profileCatalog.IsEnabled = (selectedItems.Count > 0);
        //}

        //private void HODataGrid_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    SelectedCheckboxes();
        //}

        //private void HODataGrid_CurrentCellChanged(object sender, EventArgs e)
        //{
        //    SelectedCheckboxes();
        //}

        //private void HODataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        //{
        //    SelectedCheckboxes();
        //}

        private void profileCatalog_SelectClicked(object sender, EventArgs e)
        {
            var selectedItems = ((ObservableCollection<BillOfElements>)(HODataGrid).Items.SourceCollection)
    .Where(x => x.Selection == true)
    .ToList();
            profileCatalog.SelectedProfile = ((selectedItems[0]).Profile);

        }

        private void profileCatalog_SelectionDone(object sender, EventArgs e)
        {
            var selectedItems = ((ObservableCollection<BillOfElements>)(HODataGrid).Items.SourceCollection)
    .Where(x => x.Selection == true).ToList();
            foreach (var i in selectedItems)
            {
                i.Profile = profileCatalog.SelectedProfile;
            }
            HODataGrid.UpdateLayout();
        }

        private void HODataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var initialCollection = (BillOfElementsViewModel)DataContext;
            initialCollection.BillOfElementsList = null;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dms = (sender as System.Windows.Controls.DataGrid).SelectedItems.Cast<DrawingManipulator>().ToList();
            DrawingGroup.DrawingManipulators = dms;
            if (this.DataContext != null)
                (this.DataContext as BillOfElementsViewModel).UpdateProps();
        }
    }

    public class LevelToIndentConverter : IValueConverter
    {
        private static readonly LevelToIndentConverter DefaultInstance = new LevelToIndentConverter();

        public static LevelToIndentConverter Default
        {
            get { return DefaultInstance; }
        }

        private const double IndentSize = 20.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Thickness((int)value * IndentSize, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    //private void dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    //{
    //    if (e.EditAction == DataGridEditAction.Commit)
    //    {
    //        var column = e.Column as DataGridBoundColumn;
    //        if (column != null)
    //        {
    //            var bindingPath = (column.Binding as Binding).Path.Path;
    //            var dfff = e.Row.Item as DGHDModel;
    //            var ttt = dfff.Data as DataRowView;
    //            int rowIndex = e.Row.GetIndex();
    //            var el = (e.EditingElement as TextBox).Text;
    //            var dgfdg = column.Header.ToString();
    //            var fgh = ttt[5];

    //            //var manipulator = (dfff.Data as DataRowView).Row as BackEnd.HierarchicObjectInTeklaView;



    //            // rowIndex has the row index
    //            // bindingPath has the column's binding
    //            // el.Text has the new, user-entered value

    //        }
    //    }
    //}

    /// <summary>
    /// формиует отступ в иерархическом представлении
    /// </summary>
}
