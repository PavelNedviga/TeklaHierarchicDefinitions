using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tekla.Structures.Model;
using TeklaHierarchicDefinitions.Models;
using TeklaHierarchicDefinitions.TeklaAPIUtils;
using Tekla.Structures.Dialog;
using Tekla.Structures.Dialog.UIControls;
using DataGrid = System.Windows.Controls.DataGrid;
using System.Collections;
using System.Threading;
using System.Reflection;

namespace TeklaHierarchicDefinitions.ViewModels
{

    public class BillOfElementsViewModel : INotifyPropertyChanged
    {
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
        public string WindowName { get => $"Ведомость элементов {typeof(BillOfElementsViewModel).Assembly.GetName().Version.Major.ToString()}.{typeof(BillOfElementsViewModel).Assembly.GetName().Version.Minor.ToString()}"; }
        public bool FilterByMark { get; set; } = true;

        public bool FilterByProfile { get; set; } = true;

        public bool FilterByMaterial { get; set; } = true;


        public MyObservableCollection<BillOfElements> BillOfElements
        {
            get 
            {
                MyObservableCollection<BillOfElements> billOfElements = new MyObservableCollection<BillOfElements>();
                foreach (BillOfElements boe in _billOfElements.Where(t => t.BOE.Equals(_selectedBOE)).OrderBy(t=>t.Classificator))
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



        public ObservableCollection<string> BillOfElementsList
        {
            get 
            {
                return _billOfElements.Select(x => x.BOE).Distinct().OrderBy(t=>t).ToList(); 
            }
            set
            {
                _billOfElementsList = _billOfElements.Select(x => x.BOE).Distinct().ToList();
                OnPropertyChanged("BillOfElementsList");
            }
        }

        public string SelectedBOE
        {
            get { return _selectedBOE; }
            set
            {
                _selectedBOE = value;
                OnPropertyChanged();
                OnPropertyChanged("BillOfElements");
            }
        }

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

        public bool ButtonIsEnabled
        {
            get 
            {             
                return _buttonIsEnabled = _billOfElements.Where(x => x.Selection == true).Count() > 0 && ModificationBlocked==false; 
            }
            set
            {
                _buttonIsEnabled = _billOfElements.Where(x => x.Selection == true).Count() > 0 && ModificationBlocked == false;
                OnPropertyChanged();
            }
        }

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
            List<HierarchicObjectInTekla> hierarchicObjectsInTeklas = TeklaDB.GetHierarchicObjectsWithHierarchicDefinitionName(TeklaDB.hierarchicDefinitionElementListName); //TeklaDB.GetAllHierarchicObjectsInTekla();//
            _billOfElements = BillOfElementsUtils.GetHierarchicObjectsWithHierarchicDefinitionName(hierarchicObjectsInTeklas);
            buildingFragments = BuildingFragmentUtils.GetBuildingFragmentsWithHierarchicDefinitionFatherName(TeklaDB.hierarchicDefinitionFoundationListName);
        }
        #endregion

        #region Методы
        private void InstantUpbateForBOECollection(bool instantUpdateFlag)
        {
            foreach(var boe in _billOfElements)
            {
                boe.InstantUpdate = instantUpdateFlag;
            }
        }
        
        #endregion

        #region Команды
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
                }, (obj) => (obj == null | SelectedItem == null) ? false : (TeklaDB.ModelHasSelectedObjects() && TeklaDB.ProfileIsAllowed(SelectedItem.Profile) && TeklaDB.MaterialIsAllowed(SelectedItem.Material) && ((DataGrid)obj).SelectedIndex !=-1));
            }            
        }

        public ICommand RemoveModelObjectFromHierarchicObject_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    List<BillOfElements> boes = new List<BillOfElements>();

                    foreach(object row in ((DataGrid)obj).SelectedItems)
                    {
                        boes.Add(row as BillOfElements);
                    }
                    boes = BillOfElements.ToList();
                    if (boes.Count > 0 && TeklaDB.ModelHasSelectedObjects())
                    {
                        foreach(BillOfElements boe in boes)
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
                }, (obj) => obj == null ? true : (ModificationBlocked==false));
            }
        }

        public ICommand DeleteHierarchicObject_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var boes = ((DataGrid)obj).SelectedItems.Cast<BillOfElements>().ToList();
                    foreach(var boe in boes)
                    {
                        boe.DeleteHierarchicObject();
                        _billOfElements.Remove(boe);
                    }
                    OnPropertyChanged("BillOfElements");
                    OnPropertyChanged("BillOfElementsList");

                }, (obj) => obj == null ? true : ((DataGrid)obj).SelectedIndex != -1 && ModificationBlocked ==false);
            }
        }

        public ICommand UpdateModelObjectFromHierarchicObject_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    List<string> upd = new List<string>();
                    bool updated = false;
                    var elementsList = _billOfElements.Where(x => x.Selection == true & TeklaDB.MaterialIsAllowed(x.Material) & TeklaDB.ProfileIsAllowed(x.Profile)).ToArray();
                    foreach(var boe in elementsList)
                    {
                        bool res = boe.UpdateAssociatedObjects();
                        updated = updated | res;
                        if(res)
                            upd.Add(boe.Mark);                        
                    }
                    if (updated)
                    {
                        TeklaDB.model.CommitChanges();
                        MessageBox.Show($"Properties successfully updated for {string.Join(", ", upd)}");
                    }
                }, (obj) => _billOfElements.Where(x => x.Selection == true & x.Profile.Length>0).Count() > 0);
            }
        }

        public ICommand SelectUnboundParts_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    Array listOfHierarchicObjects = _billOfElements.Select(t => t.HierarchicObjectInTekla).Select(t=>t.HierarchicObject).ToArray();
                    TeklaDB.SelectObjectsInModelView(TeklaDB.GetUnboundModelObjects(listOfHierarchicObjects));
                }, (obj) => obj == null ? true : true);
            }
        }

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
                }, (obj) => (ModificationBlocked == false & SelectedItem != null & BillOfElements.Where(t=>t.Selection).Any(k=>k != SelectedItem)));
            }
        }

        public ICommand RemoveFromHO_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var RemoveFromHOChildren = BillOfElements.Where(t=>t.Selection).ToList();
                    foreach(var removingHO in RemoveFromHOChildren)
                    {
                        removingHO.RemoveFather();
                        removingHO.OnPropertyChanged("Father");
                    }

                    OnPropertyChanged("BillOfElements");
                    OnPropertyChanged("BillOfElementsList");
                }, (obj) => (ModificationBlocked == false & BillOfElements.Any(t => (t.Selection & t.FatherHierarchicObject.HierarchicObject.Father !=null))));
            }
        }

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
                        foreach(var item in BillOfElements)
                        {
                            item.Selection = false;
                        }
                    }

                }, (obj) => obj == null ? (BillOfElements != null & BillOfElements.Count > 0) : false);

            }
        }

        public ICommand SelectSimilarParts_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var selectedBOEPos = ((DataGrid)obj).SelectedItem as BillOfElements;
                    var hoit = _billOfElements.Select(t=>t.HierarchicObjectInTekla).ToList();
                    selectedBOEPos.GetSimilardObjects(hoit, FilterByMark, FilterByProfile, FilterByMaterial);
                }, (obj) => SelectedItem != null & TeklaDB.ModelHasSelectedObjects());
            }
        }
        
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

        #region Задания на фудаменты
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

        public BuildingFragment SelectedBuildingFragment { get => _selectedBuildingFragment; 
            set 
            { 
                _selectedBuildingFragment = value;
                OnPropertyChanged();
                OnPropertyChanged("FoundationGroups");
                OnPropertyChanged("FoundationMarksList");
            } }

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
                if(_selectedBuildingFragment == null) return null;
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
                        catch (Exception ex) { MessageBox.Show(ex.ToString()); }

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
        ObservableCollection<SteelBOMPart> steelBOMPositions;

        public ICommand AddSBOMParts
        {
            get
            {
                return new DelegateCommand(
                    (obj) =>
                    {                        
                        steelBOMPositions = new ObservableCollection<SteelBOMPart>(TeklaDB.GetSelectedModelObjects().ToArray().Cast<Part>().Select(t=> new SteelBOMPart(t)).ToList());
                        
                    },
                    (obj) => TeklaDB.ModelHasSelectedObjects()
                 );
            }
        }

        #endregion

    }


}
