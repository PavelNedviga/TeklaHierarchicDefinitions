﻿using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tekla.Structures.Drawing;
using TeklaHierarchicDefinitions.Models;
using TeklaHierarchicDefinitions.TeklaAPIUtils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using DataGrid = System.Windows.Controls.DataGrid;
using Part = Tekla.Structures.Model.Part;
using Task = System.Threading.Tasks.Task;
using Tekla.Structures.Model;
using NPOI.SS.Format;
using NPOI.HSSF.Util;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Syncfusion.Windows.Controls.Grid;
using NPOI.SS.Util;
using System.Collections;
using ControlzEx.Standard;
using static TeklaHierarchicDefinitions.Models.DrawingManipulator;
using NLog;
using TeklaHierarchicDefinitions.Logging;

namespace TeklaHierarchicDefinitions.ViewModels
{
    [CustomLog]
    public class BillOfElementsViewModel : INotifyPropertyChanged
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region Ведомость элементов
        //internal WpfMaterialCatalog mc;
        #region Параметры
        private MyObservableCollection<BillOfElements> _billOfElements;

        private List<string> _billOfElementsList;
        private string _selectedBOE = "КМ";

        private bool _buttonIsEnabled = false;
        private bool _instantUpdate = false;

        private bool _modificationBlocked = true;
        private bool _windowOnTop = true;


        private BillOfElements _selectedItem;

        #endregion

        #region События
        //private Tekla.Structures.Model.Events _events = new Tekla.Structures.Model.Events();
        //private object _selectionEventHandlerLock = new object();
        //private object _changedObjectHandlerLock = new object();

        private Tekla.Structures.Model.Events ModelEvents { get; set; }
        private object _changedObjectHandlerLock = new object();

        internal void RegisterHandlers()
        {
            try
            {
                ModelEvents = new Tekla.Structures.Model.Events();
                //ModelEvents.SelectionChange += this.ModelEvents_SelectionChanged;
                ModelEvents.ModelObjectChanged += this.ModelEvents_ModelObjectChanged;
                //ModelEvents.ModelSave += this.ModelEvents_ModelSave;
                //ModelEvents.Interrupted += this.OnInterrupted;
                //ModelEvents.TeklaStructuresExit += this.ModelEvents_TeklaExit;

                ModelEvents.Register();
                //MessageBox.Show("Events activated");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ModelEvents_ModelObjectChanged(System.Collections.Generic.List<ChangeData> changes)
        {
            lock (_changedObjectHandlerLock)
            {
                new System.Threading.Tasks.Task(delegate
                {
                    //MessageBox.Show("ModelObject changed: " + changes[0].Type.ToString() + " Id: " + changes[0].Object.Identifier.ID.ToString() + " type: " + changes[0].Object.GetType().ToString());
                    if(changes.Where(t => t.Object is Part).Any(t => t.Type.ToString() == "OBJECT_INSERT"))
                    {
                        var objs = changes.Where(t => t.Type.ToString() == "OBJECT_INSERT")
                                          .Where(t => t.Object is Part)
                                          .Select(t=>t.Object as Part)
                                          .ToList();
                        var mos = new Tekla.Structures.Model.UI.ModelObjectSelector();
                        var moe = mos.GetSelectedObjects();
                        List<Tekla.Structures.Model.ModelObject> smo = new List<Tekla.Structures.Model.ModelObject>();
                        while (moe.MoveNext()) { smo.Add(moe.Current); }

                        var dictHO = BillOfElementsDict;
                        List<KeyValuePair<BillOfElements, Part>> kvPairs = new List<KeyValuePair<BillOfElements, Part>>();
                        string album = string.Empty;
                        foreach (var mo in objs)
                        {
                            if (mo.GetUserProperty("Album", ref album))
                            {
                                var part = TeklaDB.model.SelectModelObject( mo.Identifier) as Part;
                                BillOfElements billOfElements;
                                if (dictHO.TryGetValue(album + "_" + part.Class, out billOfElements))
                                {
                                    kvPairs.Add(new KeyValuePair<BillOfElements, Part>(billOfElements, part));                                
                                }
                            }
                        }
                        var dct = kvPairs.GroupBy(g => g.Key).ToDictionary(k=>k.Key,v=>v.Select(t=>t.Value).ToList()); 
                        foreach(var kv in dct)
                            kv.Key.AttachObjects(kv.Value);
                    }
                }).Start();
            };
        }

        private void ModelEvents_SelectionChanged()
        {
            MessageBox.Show("SelectionChanged");
        }

        private void ModelEvents_ModelSave()
        {
            MessageBox.Show("ModelSave");
        }

        private void OnInterrupted()
        {
            MessageBox.Show("Interrupted");
        }

        private void ModelEvents_TeklaExit()
        {
            ModelEvents.UnRegister();
        }

        //private void ModelEvents_TrackEvent(System.Collections.Generic.List<ChangeData> changes)
        //{
        //    MessageBox.Show("SelectionChanged");
        //}

        //public void RegisterEventHandler()
        //{
        //    _events.SelectionChange += Events_SelectionChangeEvent;
        //    _events.ModelObjectChanged += Events_ModelObjectChangedEvent;
        //    _events.Register();
        //}

        //public void UnRegisterEventHandler()
        //{
        //    _events.UnRegister();
        //}

        //void Events_SelectionChangeEvent()
        //{
        //    /* Make sure that the inner code block is running synchronously */
        //    lock (_selectionEventHandlerLock)
        //    {

        //        MessageBox.Show("Selection changed event received.");
        //    }
        //}

        //void Events_ModelObjectChangedEvent(List<ChangeData> changes)
        //{
        //    /* Make sure that the inner code block is running synchronously */
        //    lock (_changedObjectHandlerLock)
        //    {
        //        foreach (ChangeData data in changes)
        //            MessageBox.Show("Changed event received " + ":" + data.Object.ToString() + ":" + " Type" + ":" + data.Type.ToString() + " guid: " + data.Object.Identifier.GUID.ToString());
        //        MessageBox.Show("Changed event received for " + changes.Count.ToString() + " objects");
        //    }
        //}

        #endregion

        #region Обработка событий
        /// <summary>
        /// Отслеживает изменения свойств
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Захватывает изменения параметров
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));

        }
        #endregion

        #region Свойства
        [CustomLog]
        public string WindowName { get => $"Ведомость элементов {typeof(BillOfElementsViewModel).Assembly.GetName().Version.Major.ToString()}.{typeof(BillOfElementsViewModel).Assembly.GetName().Version.Minor.ToString()}"; }
        public bool FilterByMark { get; set; } = true;

        public bool FilterByProfile { get; set; } = true;

        public bool FilterByMaterial { get; set; } = true;

        [CustomLog]
        public MyObservableCollection<BillOfElements> BillOfElements
        {
            get
            {
                MyObservableCollection<BillOfElements> billOfElements = new MyObservableCollection<BillOfElements>();
                foreach (BillOfElements boe in _billOfElements.Where(t => t.BOE.Equals(_selectedBOE)).OrderBy(t => t.Classificator))
                {
                    billOfElements.Add(boe);
                }
                return billOfElements;
            }
            set
            {
                //_billOfElements = value;
                OnPropertyChanged();
                OnPropertyChanged("BillOfElementsList");
            }
        }


        [CustomLog]
        public List<string> BillOfElementsList
        {
            get
            {
                return _billOfElements.Select(x => x.BOE).Distinct().OrderBy(t => t).ToList();
            }
            set
            {
                _billOfElementsList = _billOfElements.Select(x => x.BOE).Distinct().ToList();
                OnPropertyChanged("BillOfElementsList");
            }
        }

        [CustomLog]
        private Dictionary<string, BillOfElements> BillOfElementsDict 
        { 
            get 
            {
                return _billOfElements.GroupBy(g => g.BOE + "_" + g.Classificator).ToDictionary(k => k.Key, v => v.First());
            } 
        }
    //
        [CustomLog]
        public string SelectedBOE
        {
            get { return _selectedBOE; }
            set
            {
                _selectedBOE = value;
                OnPropertyChanged();
                OnPropertyChanged("BillOfElements");
                OnPropertyChanged("BOELChanger");
            }
        }

        [CustomLog]
        public string BOELChanger
        {
            get { return SelectedBOE; }
            set
            {
                var BOEIterator = _billOfElements.Where(t => t.Selection.Equals(true));
                foreach (BillOfElements boe in BOEIterator)
                {
                    boe.BOE = value;
                    boe.Selection = false;
                }
                OnPropertyChanged("BillOfElementsList");
                SelectedBOE = value;
                OnPropertyChanged();

            }
        }

        [CustomLog]
        public bool InstantUpdate
        {
            get
            {
                return _instantUpdate;
            }
            set
            {
                _instantUpdate = value;
                InstantUpbateForBOECollection(value);
                OnPropertyChanged();
            }
        }

        [CustomLog]
        public bool ModificationBlocked
        {
            get
            {
                return _modificationBlocked;
            }
            set
            {
                _modificationBlocked = value;
                OnPropertyChanged();
                OnPropertyChanged("ButtonIsEnabled");
            }
        }

        [CustomLog]
        public bool WindowOnTop
        {
            get
            {
                return _windowOnTop;
            }
            set
            {
                _windowOnTop = value;
                OnPropertyChanged();
            }
        }

        [CustomLog]
        public bool ButtonIsEnabled
        {
            get
            {
                return _buttonIsEnabled = _billOfElements.Where(x => x.Selection == true).Count() > 0 && ModificationBlocked == false;
            }
            set
            {
                _buttonIsEnabled = _billOfElements.Where(x => x.Selection == true).Count() > 0 && ModificationBlocked == false;
                OnPropertyChanged();
            }
        }

        [CustomLog]
        public BillOfElements SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        #endregion

        #region Конструкторы

        /// <summary>
        /// Создание списка иерархических объектов
        /// </summary>
        public BillOfElementsViewModel()
        {
            try
            {
                List<HierarchicObjectInTekla> hierarchicObjectsInTeklas = TeklaDB.GetHierarchicObjectsWithHierarchicDefinitionName(TeklaDB.hierarchicDefinitionElementListName); //TeklaDB.GetAllHierarchicObjectsInTekla();//
                _billOfElements = BillOfElementsUtils.GetHierarchicObjectsWithHierarchicDefinitionName(hierarchicObjectsInTeklas);
                buildingFragments = BuildingFragmentUtils.GetBuildingFragmentsWithHierarchicDefinitionFatherName(TeklaDB.hierarchicDefinitionFoundationListName);
                SelectedSBOMMaterial = selectedSBOMMaterial;
                AddDrawingInformationToDrawingListTreeView();
                var path = Path.Combine(TeklaDB.model.GetInfo().ModelPath, "#ClassConversion.csv");
                if (!File.Exists(path))
                {
                    var sourcePath = Path.Combine(AssemblyDirectory, "#ClassConversion.csv");
                    File.Copy(sourcePath, path, false);
                }
                RegisterHandlers();
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.StackTrace, ex.Message);
            }
            //RegisterEventHandler();
        }
        #endregion

        #region Методы
        [CustomLog]
        public string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private void InstantUpbateForBOECollection(bool instantUpdateFlag)
        {
            foreach (var boe in _billOfElements)
            {
                boe.InstantUpdate = instantUpdateFlag;
            }
        }

        #endregion

        #region Команды
        [CustomLog]
        public ICommand ReleaseNotes_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    Process myProcess = new Process();
                    myProcess.StartInfo.FileName = "notepad.exe"; //not the full application path
                    string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    string folderPath = Path.GetDirectoryName(path);
                    string relNotesPath = Path.Combine(folderPath, "ReleaseNotes.txt");
                    myProcess.StartInfo.Arguments = $"{relNotesPath}";
                    myProcess.Start();
                }, (obj) => true);
            }
        }

        [CustomLog]
        public ICommand AddModelObjectToHierarchicObject_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var boe = ((DataGrid)obj).SelectedItem as BillOfElements;
                    if (boe.AttachSelectedObjects())
                    {
                        TeklaDB.model.CommitChanges();
                        foreach (var billOfElement in BillOfElements)
                        {
                            billOfElement.OnPropertyChanged("ObjectsCount");
                        }
                        MessageBox.Show("Objects successfully attached to " + boe.Mark + " " + boe.Position);
                    }
                }, (obj) => (obj == null | SelectedItem == null) ? false : (TeklaDB.ModelHasSelectedObjects() && TeklaDB.ProfileIsAllowed(SelectedItem.Profile) && TeklaDB.MaterialIsAllowed(SelectedItem.Material) && ((DataGrid)obj).SelectedIndex != -1));
            }
        }

        [CustomLog]
        public ICommand RemoveModelObjectFromHierarchicObject_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    List<BillOfElements> boes = new List<BillOfElements>();

                    foreach (object row in ((DataGrid)obj).SelectedItems)
                    {
                        boes.Add(row as BillOfElements);
                    }
                    boes = BillOfElements.ToList();
                    if (boes.Count > 0 && TeklaDB.ModelHasSelectedObjects())
                    {
                        foreach (BillOfElements boe in boes)
                        {
                            if (boe.HierarchicObjectInTekla.RemoveSelectedModedlObjectsFromHierarchicObject())
                            {
                                boe.OnPropertyChanged("ObjectsCount");
                                MessageBox.Show("Objects were successfully removed from " + boe.Mark);
                            }
                        }
                        TeklaDB.model.CommitChanges();
                    }
                    else
                    {
                        TeklaDB.RemoveSelectedModedlObjectsFromHierarchicObject();
                    }


                }, (obj) => obj == null ? true : (TeklaDB.ModelHasSelectedObjects())); // && ((DataGrid)obj).SelectedIndex != -1
            }
        }

        [CustomLog]
        public ICommand SetOnTop
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var win = (MainWindow)obj;
                    win.Topmost = _windowOnTop;
                    win.Show();

                }, (obj) => obj == null ? true : true);
            }
        }

        [CustomLog]
        public ICommand AddHierarchicObject_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var boe = ((DataGrid)obj).SelectedItem as BillOfElements;
                    _billOfElements.Add(new BillOfElements(boe, _billOfElements, _selectedBOE));
                    OnPropertyChanged("BillOfElements");
                    OnPropertyChanged("BillOfElementsList");
                }, (obj) => obj == null ? true : (ModificationBlocked == false));
            }
        }

        [CustomLog]
        public ICommand DeleteHierarchicObject_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var boes = ((DataGrid)obj).SelectedItems.Cast<BillOfElements>().ToList();
                    foreach (var boe in boes)
                    {
                        boe.DeleteHierarchicObject();
                        _billOfElements.Remove(boe);
                    }
                    OnPropertyChanged("BillOfElements");
                    OnPropertyChanged("BillOfElementsList");

                }, (obj) => obj == null ? true : ((DataGrid)obj).SelectedIndex != -1 && ModificationBlocked == false);
            }
        }

        [CustomLog]
        public ICommand UpdateModelObjectFromHierarchicObject_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    List<string> upd = new List<string>();
                    bool updated = false;
                    var elementsList = _billOfElements.Where(x => x.Selection == true & TeklaDB.MaterialIsAllowed(x.Material) & TeklaDB.ProfileIsAllowed(x.Profile)).ToArray();
                    foreach (var boe in elementsList)
                    {
                        bool res = boe.UpdateAssociatedObjects();
                        updated = updated | res;
                        if (res)
                            upd.Add(boe.Mark);
                    }
                    if (updated)
                    {
                        TeklaDB.model.CommitChanges();
                        MessageBox.Show($"Properties successfully updated for {string.Join(", ", upd)}");
                    }
                }, (obj) => _billOfElements.Where(x => x.Selection == true & x.Profile.Length > 0).Count() > 0);
            }
        }

        [CustomLog]
        public ICommand ExportELToExcel_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    

                    // Content
                    List<string> upd = new List<string>();
                    bool updated = false;
                    var elementsList = BillOfElements;

                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet(SelectedBOE);
                    IRow row = excelSheet.CreateRow(0);

                    // Styling
                    IFont fontContent= workbook.CreateFont();
                    fontContent.FontName = "Arial";
                    fontContent.FontHeightInPoints = 10;
                    fontContent.Color = HSSFColor.DarkBlue.Index;
                    ICellStyle cellStyleContent= workbook.CreateCellStyle();
                    cellStyleContent.SetFont(fontContent);
                    //cellStyleContent.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
                    //cellStyleContent.FillPattern = FillPattern.SolidForeground;
                    cellStyleContent.BorderLeft = BorderStyle.Thin;
                    cellStyleContent.BorderTop = BorderStyle.Thin;
                    cellStyleContent.BorderRight = BorderStyle.Thin;
                    cellStyleContent.BorderBottom = BorderStyle.Thin;

                    // Content
                    var excelColumns = new[] { 
                        "Префикс сборки",
                        "Профиль", 
                        "Материал", 
                        "Q, кН",
                        "N, кН",
                        "M, кН*м",
                        "Qz_start, кН",
                        "Qz_end, кН",
                        "Nt_start, кН",
                        "Nt_end, кН",
                        "Nc_start, кН",
                        "Nc_end, кН",
                        "My_start, кН*м",
                        "My_end, кН*м",
                        "-My_start, кН*м",
                        "-My_end, кН*м",
                        "Qy_start, кН",
                        "Qy_end, кН",
                        "Mz_start, кН*м",
                        "Mz_end, кН*м"
                    };
                    IRow headerRow = excelSheet.CreateRow(3);
                    var headerColumn = 0;
                    excelColumns.ToList().ForEach(excelColumn =>
                    {
                        var cell = headerRow.CreateCell(headerColumn);
                        cell.SetCellValue(excelColumn);
                        headerColumn++;
                    });
                    var rowCount = 4;
                    elementsList.ToList().ForEach(element => {
                        var row1 = excelSheet.CreateRow(rowCount);
                        var cellCount = 0;
                        var elementList = new List<string>()
                        {
                            element.Mark,
                            element.Profile.Split('_')[0],
                            element.Material,
                            element.GetQzSummary(),
                            element.GetNSummary(),
                            element.GetMSummary(),
                            element.Q,
                            element.Q_end,
                            element.N,
                            element.N_end,
                            element.N_start_min,
                            element.N_end_min,
                            element.M,
                            element.M_end,
                            element.MyStartReverse, 
                            element.MyEndReverse,
                            element.Q_y, 
                            element.Q_end_y,
                            element.M_z,
                            element.M_end_z
                        };                  
                        excelColumns.ToList().ForEach(column => {
                            var cell = row1.CreateCell(cellCount);
                            cell.SetCellValue(elementList[cellCount]);
                            cell.CellStyle = cellStyleContent;
                            cellCount++;
                        });
                        rowCount++;
                    });

                    // Comments below table
                    var lastRow1 = excelSheet.CreateRow(rowCount + 1);
                    var lastCell1 = lastRow1.CreateCell(0);
                    lastCell1.SetCellValue("Формат обозначения для усилий прикрепления");
                    var lastRow2 = excelSheet.CreateRow(rowCount + 2);
                    var lastCell2 = lastRow2.CreateCell(0);
                    lastCell2.SetCellValue("Qz нач/Qz кон");
                    var lastRow3 = excelSheet.CreateRow(rowCount + 3);
                    var lastCell3 = lastRow3.CreateCell(0);
                    lastCell3.SetCellValue("N нач max;N нач min/N кон max;N кон min");
                    var lastRow4 = excelSheet.CreateRow(rowCount + 4);
                    var lastCell4 = lastRow4.CreateCell(0);
                    lastCell4.SetCellValue("My нач;My нач обр/My кон;My кон обр");

                    // Formatting ----------------------------------------------------------

                    //Column widths
                    excelSheet.SetColumnWidth(0, 19 * 256);
                    excelSheet.SetColumnWidth(1, 19 * 256);
                    excelSheet.SetColumnWidth(2, 10 * 256);
                    excelSheet.SetColumnWidth(3, 10 * 256);
                    excelSheet.SetColumnWidth(4, 10 * 256);
                    excelSheet.SetColumnWidth(5, 10 * 256);
                    for (int i = 6; i < 20; i++)
                    {
                        excelSheet.SetColumnWidth(i, 15 * 256);
                    }


                    // Header style
                    IFont font = workbook.CreateFont();
                    font.FontName = "Arial";
                    font.IsBold = true;
                    font.FontHeightInPoints = 10;
                    ICellStyle cellStyleHeader = workbook.CreateCellStyle();
                    cellStyleHeader.SetFont(font);

                    cellStyleHeader.BorderLeft = BorderStyle.Medium;
                    cellStyleHeader.BorderTop = BorderStyle.Medium;
                    cellStyleHeader.BorderRight = BorderStyle.Medium;
                    cellStyleHeader.BorderBottom = BorderStyle.Medium;

                    IRow header = excelSheet.GetRow(3);
                    foreach (var cell in header.Cells) {
                        cell.CellStyle = cellStyleHeader;
                    }

                    // Populate title
                    Model model = new Model();
                    string modelNameFull = model.GetInfo().ModelName;
                    string modelName = modelNameFull.Split('.')[0];
                    var prinfo = model.GetProjectInfo();
                    Hashtable ht = new Hashtable();
                    prinfo.GetStringUserProperties(ref ht);
                    string nameStroit1 = string.Empty;
                    string nameStroit2 = string.Empty;
                    string nameStroit3 = string.Empty;
                    prinfo.GetUserProperty("RU_PTB3_1", ref nameStroit1);
                    prinfo.GetUserProperty("RU_PTB3_2", ref nameStroit2);
                    prinfo.GetUserProperty("RU_PTB3_3", ref nameStroit3);

                    excelSheet.CreateRow(0).CreateCell(0).SetCellValue("Номер проекта");
                    var row2 = excelSheet.CreateRow(1);
                    row2.HeightInPoints= (short)48.8f;
                    row2.CreateCell(0).SetCellValue("Название проекта");
                    excelSheet.GetRow(0).CreateCell(1).SetCellValue(modelName);
                    excelSheet.GetRow(1).CreateCell(1).SetCellValue(nameStroit1 + "\n" + nameStroit2 + "\n" + nameStroit1);

                    // Format title
                    IFont fontTitleBold = workbook.CreateFont();
                    IFont fontTitle = workbook.CreateFont();
                    fontTitleBold.FontName = "Arial";
                    fontTitleBold.FontHeightInPoints = 10;
                    fontTitleBold.Color = HSSFColor.DarkBlue.Index;
                    fontTitleBold.IsBold = false;
                    ICellStyle cellStyleTitle = workbook.CreateCellStyle();
                    cellStyleTitle.SetFont(fontTitleBold);
                    cellStyleTitle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    cellStyleTitle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    fontTitle.FontName = "Arial";
                    fontTitle.FontHeightInPoints = 10;
                    fontTitle.Color = HSSFColor.DarkBlue.Index;
                    fontTitle.IsBold = true;
                    ICellStyle cellStyleTitleBold = workbook.CreateCellStyle();
                    cellStyleTitleBold.SetFont(fontTitle);
                    cellStyleTitleBold.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    cellStyleTitleBold.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    //cellStyleTitleBold.FillForegroundColor = HSSFColor.Grey25Percent.Index;
                    //cellStyleTitleBold.FillPattern = FillPattern.SolidForeground;


                    //cellStyleTitleBold.BorderLeft = BorderStyle.Thin;
                    //cellStyleTitleBold.BorderTop = BorderStyle.Thin;
                    //cellStyleTitleBold.BorderRight = BorderStyle.Thin;
                    //cellStyleTitleBold.BorderBottom = BorderStyle.Thin;
                    //cellStyleTitle.BorderLeft = BorderStyle.Thin;
                    //cellStyleTitle.BorderTop = BorderStyle.Thin;
                    //cellStyleTitle.BorderRight = BorderStyle.Thin;
                    //cellStyleTitle.BorderBottom = BorderStyle.Thin;

                    var row0 = excelSheet.GetRow(0);
                    var cells = excelSheet.GetRow(0).Cells;
                    excelSheet.GetRow(0).Cells[0].CellStyle = cellStyleTitleBold;
                    excelSheet.GetRow(1).Cells[0].CellStyle = cellStyleTitleBold;
                    excelSheet.GetRow(0).Cells[1].CellStyle = cellStyleTitle;
                    excelSheet.GetRow(1).Cells[1].CellStyle = cellStyleTitle;
                    


                    // Merging
                    excelSheet.AddMergedRegion(new CellRangeAddress(0, 0, 1, 5));
                    excelSheet.AddMergedRegion(new CellRangeAddress(1, 1, 1, 5));

                    // Saving ---------------------------------------------------------------
                    Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
                    dialog.FileName = "ElementList"; // Default file name
                    dialog.DefaultExt = ".xlsx"; // Default file extension
                    dialog.Filter = "Excel (.xlsx)|*.xlsx"; // Filter files by extension
                    bool? result = dialog.ShowDialog(); // Show the dialog.
                    if (result == true) // Test result.
                    {
                        string FilePath = dialog.FileName; //path to download
                        try
                        {
                            using (FileStream stream = new FileStream(FilePath, FileMode.OpenOrCreate,
                                               FileAccess.Write))
                            {
                                workbook.Write(stream);
                            }   
                        }
                        catch { }
                    }

                }, (obj) => BillOfElements.Count() > 0);
            }
        }

        [CustomLog]
        public ICommand SelectUnboundParts_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    Array listOfHierarchicObjects = _billOfElements.Select(t => t.HierarchicObjectInTekla).Select(t => t.HierarchicObject).ToArray();
                    TeklaDB.SelectObjectsInModelView(TeklaDB.GetUnboundModelObjects(listOfHierarchicObjects));
                }, (obj) => obj == null ? true : true);
            }
        }

        [CustomLog]
        public ICommand UpdateButtonsAccessCommand
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    ButtonIsEnabled = false;
                }, (obj) => obj == null ? true : true);
            }
        }

        [CustomLog]
        public ICommand UpdateBillOfElementsList
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    OnPropertyChanged("BillOfElementsList");
                }, (obj) => obj == null ? true : true);
            }
        }

        [CustomLog]
        public ICommand AddToHO_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    //var boe = ((DataGrid)obj).SelectedItem as BillOfElements;
                    //_billOfElements.Add(new BillOfElements(boe, _billOfElements, _selectedBOE));
                    var attachedHierarchicObjects = this.BillOfElements.Where(t => t.Selection).ToList();
                    var attachTo = SelectedItem;
                    attachTo.AddAsChildHO(attachedHierarchicObjects);
                    OnPropertyChanged("BillOfElements");
                    OnPropertyChanged("BillOfElementsList");
                }, (obj) => (ModificationBlocked == false & SelectedItem != null & BillOfElements.Where(t => t.Selection).Any(k => k != SelectedItem)));
            }
        }

        [CustomLog]
        public ICommand RemoveFromHO_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var RemoveFromHOChildren = BillOfElements.Where(t => t.Selection).ToList();
                    foreach (var removingHO in RemoveFromHOChildren)
                    {
                        removingHO.RemoveFather();
                        removingHO.OnPropertyChanged("Father");
                    }

                    OnPropertyChanged("BillOfElements");
                    OnPropertyChanged("BillOfElementsList");
                }, (obj) => (ModificationBlocked == false & BillOfElements.Any(t => (t.Selection & t.FatherHierarchicObject.HierarchicObject.Father != null))));
            }
        }

        [CustomLog]
        public ICommand CopyHO_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var new_boe = new BillOfElements(null, _billOfElements, _selectedBOE);
                    foreach (PropertyInfo prop in new_boe.GetType().GetProperties())
                    {
                        if (prop == null)
                            continue;
                        if (!prop.CanWrite)
                            continue;
                        var value = prop.GetValue(SelectedItem, null);
                        prop.SetValue(new_boe, value, null);
                    }
                    _billOfElements.Add(new_boe);
                    OnPropertyChanged("BillOfElements");
                    OnPropertyChanged("BillOfElementsList");
                }, (obj) => obj == null ? (ModificationBlocked == false & SelectedItem != null) : false);

            }
        }

        [CustomLog]
        public ICommand SelectDeselectRows_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    if (BillOfElements.Any(t => !t.Selection))
                    {
                        foreach (var item in BillOfElements)
                        {
                            item.Selection = true;
                        }
                    }
                    else
                    {
                        foreach (var item in BillOfElements)
                        {
                            item.Selection = false;
                        }
                    }

                }, (obj) => obj == null ? (BillOfElements != null & BillOfElements.Count > 0) : false);

            }
        }

        [CustomLog]
        public ICommand SelectSimilarParts_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var selectedBOEPos = ((DataGrid)obj).SelectedItem as BillOfElements;
                    var hoit = _billOfElements.Select(t => t.HierarchicObjectInTekla).ToList();
                    selectedBOEPos.GetSimilardObjects(hoit, FilterByMark, FilterByProfile, FilterByMaterial);
                }, (obj) => SelectedItem != null & TeklaDB.ModelHasSelectedObjects());
            }
        }

        [CustomLog]
        public ICommand BorrowProperties_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var selectedBOEPos = ((DataGrid)obj).SelectedItem as BillOfElements;
                    selectedBOEPos.BorrowPropertiesFromModelObject();
                }, (obj) => SelectedItem != null & TeklaDB.ModelHasSelectedObjects() & !ModificationBlocked);
            }
        }


        //public ICommand SelectMaterialForHierarchicObject_Click
        //{
        //    get
        //    {
        //        return new DelegateCommand((obj) =>
        //        {
        //            var selectedItems =
        //            ((ObservableCollection<BillOfElements>)((DataGrid)obj).Items.SourceCollection)
        //            .Where(x => x.Selection == true)
        //            .ToList();
        //            var elementMaterials = selectedItems.Select(item => item.Material).Distinct().ToList();
        //            mc = new WpfMaterialCatalog();
        //            if (elementMaterials.Count > 1)
        //            {

        //            }
        //            else if (elementMaterials.Count > 0)
        //            {
        //                mc.SelectedMaterial = elementMaterials.First();
        //            }
        //            else
        //            {
        //                MessageBox.Show("Не выбран элемент!");
        //            }

        //            mc.InitializeComponent();


        //        }, (obj) => obj == null ? true : (_billOfElements.Where(x => x.Selection == true).Count() > 0));
        //    }
        //}
        #endregion

        //private void MaterialCatalog_SelectClicked(object sender, EventArgs e)
        //{

        //    //this.materialCatalog.SelectedMaterial;
        //    var dsf = (HODataGrid.SelectedItem);
        //    var erer = (HODataGrid.Columns[1]);
        //    //((HODataGrid.Columns[1]) as Tekla.Structures.Dialog.UIControls.MaterialCatalog).SelectedMaterial = (this.HODataGrid.SelectedItem as BillOfElements).Material;
        //}

        //private void MaterialCatalog_SelectionDone(object sender, EventArgs e)
        //{
        //    //(this.HODataGrid.SelectedItem as BillOfElements).Material = ((sender as Button) as Tekla.Structures.Dialog.UIControls.WpfMaterialCatalog).SelectedMaterial;
        //}
        #endregion

        #region Задания на фундаменты
        #region Параметры        
        private string newBuildingFragmentName = string.Empty;
        private MyObservableCollection<BuildingFragment> buildingFragments;
        private BuildingFragment _selectedBuildingFragment;
        private string _selectedFoundationMark;
        private bool _attaching = false;


        #endregion

        #region Свойства
        public string NewBuildingFragmentName
        {
            get
            {
                return newBuildingFragmentName;
            }
            set
            {
                newBuildingFragmentName = value;
                OnPropertyChanged();
            }
        }

        public BuildingFragment SelectedBuildingFragment
        {
            get => _selectedBuildingFragment;
            set
            {
                _selectedBuildingFragment = value;
                OnPropertyChanged();
                OnPropertyChanged("FoundationGroups");
                OnPropertyChanged("FoundationMarksList");
            }
        }

        public MyObservableCollection<BuildingFragment> BuildingFragments
        {
            get
            {
                return buildingFragments;
            }
            set
            {
                buildingFragments = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFoundationMark
        {
            get
            {
                return _selectedFoundationMark;
            }
            set
            {
                _selectedFoundationMark = value;
                if (_selectedBuildingFragment != null && _selectedBuildingFragment.FoundationGroups.Count > 0)
                    if (_selectedFoundationMark != null)
                        _selectedBuildingFragment.FoundationGroups.Where(t => t.BasementMark.Equals(_selectedFoundationMark)).FirstOrDefault().GetSelectedObjects();
                OnPropertyChanged();
                OnPropertyChanged("FoundationGroups");
            }
        }

        public ObservableCollection<FoundationGroup> FoundationGroups
        {
            get
            {
                if (_selectedBuildingFragment == null) return null;
                if (_selectedFoundationMark == null) return _selectedBuildingFragment.FoundationGroups;
                else
                {
                    return new ObservableCollection<FoundationGroup>(_selectedBuildingFragment.FoundationGroups.Where(t => t.BasementMark.Equals(_selectedFoundationMark)).ToArray());
                }

            }
        }

        public ObservableCollection<string> FoundationMarksList
        {
            get
            {
                if (FoundationGroups == null) return null;
                return new ObservableCollection<string>(FoundationGroups.Select(t => t.BasementMark).Distinct().ToList());
            }
        }

        public bool Attaching
        {
            get
            {
                return _attaching;
            }
            set
            {
                _attaching = value;
                OnPropertyChanged("Attaching");
            }

        }
        #endregion

        #region Команды
        public ICommand AddBuildingFragment
        {
            get
            {
                return new DelegateCommand(
                    (obj) =>
                    {
                        BuildingFragments.Add(new BuildingFragment(NewBuildingFragmentName));
                        OnPropertyChanged("FoundationGroups");
                        OnPropertyChanged("BuildingFragments");
                        OnPropertyChanged("FoundationMarksList");
                    },
                    (obj) =>
                    {
                        if (NewBuildingFragmentName.Length > 0)
                            if (!BuildingFragments.Select(t => t.BuildingFragmentMark).Contains(NewBuildingFragmentName))
                                return true;
                        return false;
                    }
                );
            }
        }

        public ICommand DeleteBuildingFragment
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    SelectedBuildingFragment.RemoveBuildingFragment();
                    buildingFragments.Clear();
                    BuildingFragments = BuildingFragmentUtils.GetBuildingFragmentsWithHierarchicDefinitionFatherName(TeklaDB.hierarchicDefinitionFoundationListName);
                }, (obj) => SelectedBuildingFragment == null ? false : true);
            }
        }

        public ICommand ImportBuildingFragment
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    try
                    {
                        Attaching = true;
                        SelectedBuildingFragment.ImportFoundationGroups();
                        OnPropertyChanged("FoundationMarksList");
                        OnPropertyChanged("FoundationGroups");
                        Attaching = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        Attaching = false;
                    }

                }, (obj) => SelectedBuildingFragment == null ? false : true);
            }
        }

        public ICommand AddObjectsToFoundationGroup
        {
            get
            {
                return new DelegateCommand((obj) =>
                {

                    Thread thread = new Thread(() =>
                    {
                        try
                        {
                            Attaching = true;
                            var selectedComponents = TeklaDB.ModelGetSelectedComponents();
                            SelectedBuildingFragment.RemoveAllBasements();
                            foreach (var currentFoundationGroup in FoundationGroups)
                            {
                                currentFoundationGroup.AddBasements();
                            }
                            TeklaDB.model.CommitChanges();
                            Attaching = false;
                        }
                        catch (Exception ex) { MessageBox.Show(ex.ToString()); }

                    });
                    thread.IsBackground = true;
                    thread.Start();
                }, (obj) => obj == null ? false : (TeklaDB.ModelHasSelectedObjects() && ((ListBox)obj).SelectedIndex != -1) && !Attaching);
            }
        }

        public ICommand RemoveObjectsFromFoundationGroup
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    Thread thread = new Thread(() =>
                    {
                        Attaching = true;
                        foreach (var currentFoundationGroup in FoundationGroups)
                        {
                            currentFoundationGroup.RemoveBasements();
                        }
                        Attaching = false;
                    });
                    thread.IsBackground = true;
                    thread.Start();
                }, (obj) => obj == null ? false : (TeklaDB.ModelHasSelectedObjects() && ((ListBox)obj).SelectedIndex != -1 && !Attaching));
            }
        }
        #endregion
        #endregion

        #region ТСС
        #region Параметры
        private ObservableCollection<SteelBOMPosition> steelBOMPositions;
        bool isLoadingSBOM = false;
        private string selectedAttachmentsToEL;
        private SteelBOMPosition selectedSBOMMaterial;
        private SteelBOMPosition selectedSBOMCategory;
        private SteelBOMPosition selectedSBOMProfile;
        private ObservableCollection<string> selectedSBOMMaterials;
        #endregion

        #region Свойства
        [CustomLog]
        public ObservableCollection<SteelBOMPosition> SteelBOMPositions
        {
            get
            {
                IEnumerable<SteelBOMPosition> visibleSteelBOMPositions = steelBOMPositions;
                if (SBOMMaterials != null && SBOMMaterials.Count > 0)
                    visibleSteelBOMPositions = visibleSteelBOMPositions.Where(s => SBOMMaterials.Contains(s.Material));
                if (SBOMCategories != null && SBOMCategories.Count > 0)
                    visibleSteelBOMPositions = visibleSteelBOMPositions.Where(s => SBOMCategories.Contains(s.Category));
                if (SBOMProfiles != null && SBOMProfiles.Count > 0)
                    visibleSteelBOMPositions = visibleSteelBOMPositions.Where(s => SBOMProfiles.Contains(s.Profile));
                if (SelectedAttachmentToEL != null && visibleSteelBOMPositions != null)
                    visibleSteelBOMPositions = visibleSteelBOMPositions.Where(s => SelectedAttachmentToEL == s.AttachedToEL);
                if (visibleSteelBOMPositions == null)
                    return null;
                return new ObservableCollection<SteelBOMPosition>(visibleSteelBOMPositions);
            }
            set
            {
                steelBOMPositions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SteelBOMPosition> SteelBOMPositionsByMaterial { get; set; }

        public ObservableCollection<SteelBOMPosition> SteelBOMPositionsByProfile { get; set; }

        public ObservableCollection<SteelBOMPosition> SteelBOMPositionsByCategory { get; set; }

        [CustomLog]
        public double SummarySBOMParts
        {
            get
            {
                if (steelBOMPositions == null)
                    return 0;
                return double.Parse(steelBOMPositions.SelectMany(t => t.Parts).Select(w => w.Weight).Sum().ToString("F"));
            }
        }

        [CustomLog]
        public double SummaryGrossSBOMParts
        {
            get
            {
                if (steelBOMPositions == null)
                    return 0;
                return double.Parse(steelBOMPositions.SelectMany(t => t.Parts).Select(w => w.WeightGross).Sum().ToString("F"));
            }
        }

        public List<string> SBOMMaterials { get; set; }

        //public ObservableCollection<string> SelectedSBOMMaterials { get=> selectedSBOMMaterials; set 
        //    {
        //        selectedSBOMMaterials = value;
        //        OnPropertyChanged();
        //    } 
        //}

        public SteelBOMPosition SelectedSBOMMaterial { get => selectedSBOMMaterial; set { selectedSBOMMaterial = value; OnPropertyChanged(); } }

        public List<string> SBOMCategories { get; set; }

        public SteelBOMPosition SelectedSBOMCategory { get => selectedSBOMCategory; set { selectedSBOMCategory = value; OnPropertyChanged(); } }

        public List<string> SBOMProfiles { get; set; }

        public SteelBOMPosition SelectedSBOMProfile { get => selectedSBOMProfile; set { selectedSBOMProfile = value; OnPropertyChanged(); } }

        public ObservableCollection<string> AttachmentsToVE { get => new ObservableCollection<string>() { "Нет", "Частично", "Все" }; }

        public bool SelectPartlyAttachedObjects { get; set; }

        [CustomLog]
        public string SelectedAttachmentToEL
        {
            get => selectedAttachmentsToEL;
            set
            {
                selectedAttachmentsToEL = value;
                OnPropertyChanged();
                OnPropertyChanged("SteelBOMPositions");
            }
        }

        [CustomLog]
        public bool IsLoadingSBOM
        {
            get => isLoadingSBOM;
            set
            {
                isLoadingSBOM = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Команды
        [CustomLog]
        public ICommand SelectedSBOMMaterials
        {
            get
            {
                return new DelegateCommand(
                    (obj) =>
                    {
                        try
                        {

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            IsLoadingSBOM = false;
                        }
                    },
                    (obj) => true
                 );
            }
        }

        [CustomLog]
        public ICommand AddSBOMParts
        {
            get
            {
                return new DelegateCommand(
                    (obj) =>
                    {
                        try
                        {
                            IsLoadingSBOM = true;
                            var task = Task.Run(() => GetSBOMParts());
                            Action unblock = delegate ()
                            {
                                IsLoadingSBOM = false;
                            };
                            task.GetAwaiter().OnCompleted(unblock);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            IsLoadingSBOM = false;
                        }
                    },
                    (obj) => TeklaDB.ModelHasSelectedObjects() & !IsLoadingSBOM
                 );
            }
        }

        [CustomLog]
        public ICommand UnselectAll
        {
            get
            {
                return new DelegateCommand(
                    (obj) =>
                    {
                        try
                        {
                            //(obj as TabItem).;
                            IsLoadingSBOM = true;
                            var task = Task.Run(() =>
                            {
                                SBOMMaterials = null;
                                SBOMCategories = null;
                                SBOMProfiles = null;
                                SelectedAttachmentToEL = null;
                            });
                            Action unblock = delegate ()
                            {
                                IsLoadingSBOM = false;
                            };
                            task.GetAwaiter().OnCompleted(unblock);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            IsLoadingSBOM = false;
                        }
                    },
                    (obj) => TeklaDB.ModelHasSelectedObjects() & !IsLoadingSBOM
                 );
            }
        }
        #endregion

        #region Методы
        private void GetSBOMParts()
        {
            SteelBOMPositions = new ObservableCollection<SteelBOMPosition>(
                TeklaDB.GetSelectedModelObjects()
                .ToArray()
                .Cast<Part>()
                .Select(t => new SteelBOMPart(t))
                .GroupBy(k => k.GroupingCode)
                .Select(t => new SteelBOMPosition(t.ToList()))
                .ToList());
            SteelBOMPositionsByMaterial = new ObservableCollection<SteelBOMPosition>(steelBOMPositions.GroupBy(t => t.Material).Select(k => new SteelBOMPosition(k.SelectMany(m => m.Parts).ToList())).OrderBy(t => t.Material));
            OnPropertyChanged("SteelBOMPositionsByMaterial");
            SteelBOMPositionsByProfile = new ObservableCollection<SteelBOMPosition>(steelBOMPositions.GroupBy(t => t.Profile).Select(k => new SteelBOMPosition(k.SelectMany(m => m.Parts).ToList())).OrderBy(t => t.Profile));
            OnPropertyChanged("SteelBOMPositionsByProfile");
            SteelBOMPositionsByCategory = new ObservableCollection<SteelBOMPosition>(steelBOMPositions.GroupBy(t => t.Category).Select(k => new SteelBOMPosition(k.SelectMany(m => m.Parts).ToList())).OrderBy(t => t.Category));
            OnPropertyChanged("SteelBOMPositionsByCategory");
            OnPropertyChanged("SummarySBOMParts");
            OnPropertyChanged("SummaryGrossSBOMParts");
        }
        #endregion

        #endregion

        #region Чертежи
        #region Поля
        private string selectedDrawingAlbum = null;
        private DrawingManipulator selectedDrawingManipulator;
        #endregion

        #region Свойства

        [CustomLog]
        public ObservableCollection<DrawingManipulator> Drawings { get; set; } = new ObservableCollection<DrawingManipulator>();

        [CustomLog]
        public List<string> Albums
        {
            get => Drawings.Select(t => t.Album).ToList();
        }

        [CustomLog]
        public string SelectedDrawingAlbum
        {
            get => selectedDrawingAlbum;
            set
            {
                selectedDrawingAlbum = value;

                OnPropertyChanged();
                OnPropertyChanged("VisibleDrawingManipulators");
                OnPropertyChanged("AlbumDesigners");
                OnPropertyChanged("PropertyFillersList");
                OnPropertyChanged("ObjectPropertyFillersList");
                OnPropertyChanged("ConstructionObjectPropertyFillersList");
                OnPropertyChanged("DrawingsAlbum");
                OnPropertyChanged("AlbumPhase");
                OnPropertyChanged("ListNumber");
                OnPropertyChanged("ListsInAlbumTotal");
                OnPropertyChanged("TitleFillerList");
            }
        }

        [CustomLog]
        public DrawingManipulator SelectedDrawingManipulator
        {
            get => selectedDrawingManipulator;
            set
            {
                selectedDrawingManipulator = value;
                OnPropertyChanged();
                UpdateProps();
            }
        }

        [CustomLog]
        public ObservableCollection<string> DrawingAlbums
        {
            get { return new ObservableCollection<string>(Drawings.Select(t => t.Album).Distinct()); }
        }

        [CustomLog]
        public ObservableCollection<DrawingManipulator> VisibleDrawingManipulators
        {
            get
            {
                if (SelectedDrawingAlbum == null)
                {
                    DrawingGroup.DrawingManipulators = new List<DrawingManipulator>(Drawings);
                    DrawingGroup.Filler = PropertyRooting.Model;
                    return new ObservableCollection<DrawingManipulator>(Drawings);

                }
                else
                {
                    DrawingGroup.DrawingManipulators = new List<DrawingManipulator>(Drawings.ToList().FindAll(t => t.Album == selectedDrawingAlbum));
                    DrawingGroup.Filler = PropertyRooting.Album;
                    return new ObservableCollection<DrawingManipulator>(Drawings.ToList().FindAll(t => t.Album == selectedDrawingAlbum));
                }
                OnPropertyChanged("PropertyFillersList");
            }
        }

        [CustomLog]
        public ObservableCollection<DrawingManipulator> BorrowedListFromCsv
        {
            get
            {
                return new ObservableCollection<DrawingManipulator>(DrawingGroup.BorrowedListFromCsv);
            }
            set
            {
                DrawingGroup.BorrowedListFromCsv = value.ToList();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PropertyFillers> ObjectPropertyFillersList
        {
            get => DrawingGroup.ObjectPropertyFillersList;
        }

        public ObservableCollection<PropertyFillers> ConstructionObjectPropertyFillersList
        {
            get => DrawingGroup.ConstructionObjectPropertyFillersList;
        }

        public ObservableCollection<PropertyFillers> TitleFillerList
        {
            get => DrawingGroup.TitleFillerList;
        }

        public PropertyFillers ModelCode
        {
            get => DrawingGroup.ModelCode;
        }

        public PropertyFillers DrawingsAlbum
        {
            get => DrawingGroup.DrawingsAlbum;
        }

        public PropertyFillers AlbumPhase
        {
            get => DrawingGroup.AlbumPhase;
        }

        public PropertyFillers ListNumber
        {
            get => DrawingGroup.ListNumber;
        }

        public PropertyFillers ListsInAlbumTotal
        {
            get => DrawingGroup.ListsInAlbumTotal;
        }


        public ObservableCollection<PropertyFillers> ModelPropertyFillers
        {
            get => DrawingGroup.ModelPropertyFillersList;
        }

        public ObservableCollection<PropertyFillers> CompanyNamePropertyFillers
        {
            get => DrawingGroup.CompanyNamePropertyFillers;
        }

        public ObservableCollection<PropertyFillers> AlbumDesigners
        {
            get => DrawingGroup.AlbumDesigners;
        }


        public PropertyFillers DrawingsDates
        {
            get => DrawingGroup.DrawingsDates;
        }
        #endregion

        [CustomLog]
        public void UpdateProps()
        {
            DrawingGroup.Filler = PropertyRooting.Drawing;
            OnPropertyChanged("PropertyFillersList");
            OnPropertyChanged("ObjectPropertyFillersList");
            OnPropertyChanged("ConstructionObjectPropertyFillersList");
            OnPropertyChanged("AlbumDesigners");
            OnPropertyChanged("DrawingsAlbum");
            OnPropertyChanged("ModelCode");
            OnPropertyChanged("AlbumPhase");
            OnPropertyChanged("ListNumber");
            OnPropertyChanged("ListsInAlbumTotal");
            OnPropertyChanged("TitleFillerList");
        }

        #region Команды
        [CustomLog]
        public ICommand BorrowFromCsv
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var borrowedDM = obj as DrawingManipulator;
                    if (borrowedDM != null)
                    {
                        foreach (var dm in DrawingGroup.DrawingManipulators)
                        {
                            dm.BorrowProperties(borrowedDM);
                        }
                        AddDrawingInformationToDrawingListTreeView();
                    }
                }
                , (obj) => true);
            }
        }

        [CustomLog]
        public ICommand LoadFromCsv
        {
            get
            {
                return new DelegateCommand((obj) =>
                {

                        System.Windows.Forms.OpenFileDialog dialogWindow = new System.Windows.Forms.OpenFileDialog();
                        dialogWindow.Filter = "CSV Files (*.csv)|*.csv";
                        dialogWindow.FilterIndex = 1;
                        dialogWindow.Multiselect = false;

                        if (dialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            string sFileName = dialogWindow.FileName;
                            //string[] arrAllFiles = choofdlog.FileNames; //used when Multiselect = true           
                            DrawingGroup.LoadCsv(sFileName);
                            OnPropertyChanged("BorrowedListFromCsv");
                        }

                }
                , (obj) => true);
            }
        }

        [CustomLog]
        public ICommand CreateCsv
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var zz = Drawings.First().Album; 
                    var cc = Drawings.First().Code;
                    DrawingGroup.CsvExport(Drawings);
                }
                , (obj) => true);
            }
        }

        [CustomLog]
        public ICommand UpdateAlbum
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var tb = (obj as TextBox);
                    if (obj != null)
                    {
                        var value = tb.Text;
                        foreach (var item in VisibleDrawingManipulators)
                        {
                            if (value != null)
                                item.Album = value;
                        }
                        AddDrawingInformationToDrawingListTreeView();
                    }
                }, (obj) => true);
            }
        }

        [CustomLog]
        public ICommand UpdateAlbumList
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    OnPropertyChanged("DrawingAlbums");
                }, (obj) => true);
            }
        }

        [CustomLog]
        public ICommand UpdateDrawingList
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    AddDrawingInformationToDrawingListTreeView();
                }, (obj) => true);
            }
        }

        #endregion
        #region Методы
        private void AddDrawingInformationToDrawingListTreeView()
        {
            Drawings.Clear();
            DrawingEnumerator drawingListEnumerator = TeklaDB.DrawingHandler.GetDrawings(); // Get drawing list

            while (drawingListEnumerator.MoveNext())
            {
                Drawing myDrawing = drawingListEnumerator.Current;
                Drawings.Add(new DrawingManipulator() { Drawing = new DrawingManipulator.DrawingEnvelop(myDrawing) });
            }
            OnPropertyChanged("VisibleDrawingManipulators");
            OnPropertyChanged("DrawingAlbums");
        }
        #endregion
        #endregion

        #region Результаты проверки
        #region Параметры
        ObservableCollection<CheckResult> checkResults = new ObservableCollection<CheckResult>();
        private CheckResult selectedCheckResult;
        #endregion

        #region Свойства
        public CheckResult SelectedCheckResult
        {
            get => selectedCheckResult;
            set
            {
                selectedCheckResult = value;
                OnPropertyChanged("CheckResultsDescription");
                OnPropertyChanged("CheckResultsObjects");
            }
        }
        public ObservableCollection<CheckResult> CheckResults
        {
            get => checkResults;
            set
            {
                checkResults = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CheckResultSummary> CheckResultsSummary { get; private set; }

        //public ObservableCollection<CheckResult> CheckedErrorItems
        //{

        //}

        public List<string> CheckResultsDescription
        {
            get
            {
                List<string> result = new List<string>();
                if (SelectedCheckResult != null)
                {
                    result.Add(SelectedCheckResult.GroupError);
                    result.Add(SelectedCheckResult.Error);
                }
                return result;
            }
        }

        public List<string> CheckResultsObjects
        {
            get => SelectedCheckResult == null ? null : SelectedCheckResult.GUIDs;
        }

        #endregion

        public ICommand LoadCheckResults
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    System.Windows.Forms.OpenFileDialog dialogWindow = new System.Windows.Forms.OpenFileDialog();
                    dialogWindow.Filter = "Excel Files (*.xlsx)|*.xlsx";
                    dialogWindow.FilterIndex = 1;
                    dialogWindow.Multiselect = false;

                    if (dialogWindow.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string sFileName = dialogWindow.FileName;
                        //string[] arrAllFiles = choofdlog.FileNames; //used when Multiselect = true
                        var loadedRes = CheckResultExtensions.LoadCheckResults(sFileName);
                        CheckResults = new ObservableCollection<CheckResult>(loadedRes.Item1);
                        CheckResultsSummary = new ObservableCollection<CheckResultSummary>(loadedRes.Item2);
                        OnPropertyChanged("CheckResults");
                        OnPropertyChanged("CheckResultsSummary");
                        OnPropertyChanged("CheckResultsCodes");
                    }
                }
                , (obj) => true);
            }
        }


        public ICommand SelectErrorObjectsByGuids
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var selectors = (obj as ListBox).SelectedItems.Cast<string>().ToList();
                    if (selectors.Any())
                        SelectedCheckResult.SelectErrorObjects(selectors);
                    else
                        SelectedCheckResult.SelectErrorObjects();
                }
                , (obj) => true);
            }
        }
        #endregion

    }


}
