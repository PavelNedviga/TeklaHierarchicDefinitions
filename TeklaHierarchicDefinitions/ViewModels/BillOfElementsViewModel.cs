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


        public MyObservableCollection<BillOfElements> BillOfElements
        {
            get 
            {
                MyObservableCollection<BillOfElements> billOfElements = new MyObservableCollection<BillOfElements>();
                foreach (BillOfElements boe in _billOfElements.Where(t => t.BOE.Equals(_selectedBOE)))
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



        public List<string> BillOfElementsList
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
                    if (boe != null && TeklaDB.ModelHasSelectedObjects())
                    {
                        if (boe.AttachSelectedObjects())
                        {
                            TeklaDB.model.CommitChanges();
                            MessageBox.Show("Objects successfully attached to " + boe.Mark + " " + boe.Position);
                        }
                    }


                }, (obj) => obj == null ? true : (TeklaDB.ModelHasSelectedObjects() && ((DataGrid)obj).SelectedIndex !=-1));
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

                    if (boes.Count > 0 && TeklaDB.ModelHasSelectedObjects())
                    {
                        foreach(BillOfElements boe in boes)
                        {
                            if (boe.HierarchicObjectInTekla.RemoveSelectedModedlObjectsFromHierarchicObject())
                            {
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
                    var boe = ((DataGrid)obj).SelectedItem as BillOfElements;
                    boe.DeleteHierarchicObject();
                    _billOfElements.Remove(boe);
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
                    var elementsList = _billOfElements.Where(x => x.Selection == true).ToArray();
                    foreach(var boe in elementsList)
                    {
                        boe.UpdateAssociatedObjects();
                    }

                    TeklaDB.model.CommitChanges();
                    //MessageBox.Show("Properties successfully updated");
                }, (obj) => _billOfElements.Where(x => x.Selection == true).Count() > 0);
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
                    SelectedBuildingFragment.ImportFoundationGroups();
                    OnPropertyChanged("FoundationMarksList");
                    OnPropertyChanged("FoundationGroups");

                }, (obj) => SelectedBuildingFragment == null ? false : true);
            }
        }

        public ICommand AddObjectsToFoundationGroup
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    var selectedComponents = TeklaDB.ModelGetSelectedComponents();
                    foreach (var currentFoundationGroup in FoundationGroups)
                    {
                        currentFoundationGroup.AddBasements();
                    }
                    TeklaDB.model.CommitChanges();
                }, (obj) => obj == null ? false : (TeklaDB.ModelGetSelectedComponents().Count>0 && ((ListBox)obj).SelectedIndex != -1));
            }
        }

        public ICommand RemoveObjectsFromFoundationGroup
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    foreach (var currentFoundationGroup in FoundationGroups)
                    {
                        currentFoundationGroup.RemoveBasements();
                    }
                }, (obj) => obj == null ? false : (TeklaDB.ModelGetSelectedComponents().Count>0 && ((ListBox)obj).SelectedIndex != -1));
            }
        }

        #endregion
        #endregion
    }


}
