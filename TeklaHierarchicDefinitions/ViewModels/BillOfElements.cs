using NLog;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TeklaHierarchicDefinitions.TeklaAPIUtils;
using TeklaHierarchicDefinitions.Classifiers;
using System;

namespace TeklaHierarchicDefinitions.Models
{
    /// <summary>
    /// Объект, отвечающий за одну строку ВЭ
    /// </summary>
    public class BillOfElements : INotifyPropertyChanged, IDataErrorInfo
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
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

        #region Конструктор
        internal BillOfElements(HierarchicObjectInTekla hierarchicObjectInTekla, MyObservableCollection<BillOfElements> billOfElements)
        {
            _hierarchicObjectInTekla = hierarchicObjectInTekla;
            Collection = billOfElements;
        }

        internal BillOfElements(BillOfElements father, MyObservableCollection<BillOfElements> collection, string boe = "КМ")
        {
            logger.Debug("Collection длиной " + collection.Count);
            if (boe == null)
                boe = "КМ";
            if (father != null)
            {                
                _hierarchicObjectInTekla = new HierarchicObjectInTekla(father.FatherHierarchicObject);
                BOE = father.BOE;
                Collection = collection;
                Mark = father.Mark;
                logger.Debug("father.BOE = "+ father.BOE + ", father.Mark = " + father.Mark);
                father.IsComplexCrossSection = true;
            }
            else
            {
                _hierarchicObjectInTekla = new HierarchicObjectInTekla();
                
                Collection = collection;
                BOE = boe;
                Mark = "New mark";

            }
            Selection = false;
            logger.Debug("Collection присвоено");
            Profile = "I20K1_57837_2017";
            Position = "";
            Q = "";
            N = "";
            M = "";
            Material = "C255";
            Notes = "";
            IsComplexCrossSection = true;
            EmptyRowsNumber = 0;
            OnOtherList = false;
            
            
            _hierarchicObjectInTekla.CommitChanges();
        }

        #endregion

        #region Внутренние параметры объекта

        private HierarchicObjectInTekla _hierarchicObjectInTekla;

        private bool _instantUpdate;

        private int _isComplexCrossSection;

        internal MyObservableCollection<BillOfElements> Collection;

        private int _crossSectionOnOtherList;

        private bool _selection;

        #endregion

        #region Открытые свойства
        public bool InstantUpdate
        {
            get { return _instantUpdate; }
            set 
            { 
                _instantUpdate = value;                
            }
        }

        internal HierarchicObjectInTekla HierarchicObjectInTekla
        {
            get { return _hierarchicObjectInTekla; }
            set { _hierarchicObjectInTekla = value; }
        }

        public bool Selection
        {
            get { return _selection; }
            set 
            {
                _selection = value;
                OnPropertyChanged("Selection");
                OnPropertyChanged("ButtonIsEnabled");
            }
        }      

        public string BOE
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetDependentStrAttribute("BOE"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetDependentAttribute("BOE", value);
                if(_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("BOE", value);
                OnPropertyChanged();
                UpdateChildrenAlbum(value);
            }
        }

        public string Mark
        {
            get { return _hierarchicObjectInTekla.Name; }
            set 
            { 
                _hierarchicObjectInTekla.Name = value;
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartSetPrefix(value);
                Classificator = ClassGenerator.Generate(value);

                OnPropertyChanged("Mark");
                OnPropertyChanged("Father");
                UpdateChildrenMarks(value);
            }
        }

        public string RefList
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetDependentStrAttribute("RefList"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetDependentAttribute("RefList", value);
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("RefList", value);

                OnPropertyChanged();
                UpdateRefList(value);
            }
        }        

        public string Father
        {
            get
            { 
                return _hierarchicObjectInTekla.Father; 
            }
        }

        public string FatherGUID
        {
            get { return _hierarchicObjectInTekla.FatherGUID; }
        }

        internal HierarchicObjectInTekla FatherHierarchicObject
        {
            get
            {
                return _hierarchicObjectInTekla;
            }
        }

        public string Classificator
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Classificator"); }
            set
            {
                bool res = false;
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Classificator", value);
                if (_instantUpdate)
                    res = _hierarchicObjectInTekla.PartSetClass(value);

                if (res)
                {
                    res = TeklaDB.model.CommitChanges();
                }
                OnPropertyChanged();
            }
        }

        public string Sketch
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Sketch"); }
            set 
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Sketch", value);
                OnPropertyChanged("Sketch");
            }
        }

        public string Profile
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Profile"); }
            set 
            { 
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Profile", value);
                var sketch = TeklaDB.GetSketch(value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Sketch", sketch);
                ProfileForSpec = TeklaDB.GetProfileForSpec(value);
                if (_instantUpdate) 
                    _hierarchicObjectInTekla.PartSetProfile(value);
                OnPropertyChanged();
                OnPropertyChanged("Sketch");
            }
        }

        public string ProfileForSpec
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("ProfileForSpec"); }
            set
            {
                var profileForSpec = value;
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("ProfileForSpec", profileForSpec);
                OnPropertyChanged();
            }
        }

        public string Position
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Position"); }
            set 
            { 
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Position", value);
                if (_instantUpdate) 
                    _hierarchicObjectInTekla.PartsSetAttr("PRELIM_MARK", value);
                OnPropertyChanged("Position");
            }
        }

        public string M
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("M"); }
            set 
            { 
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("M", value);
                if (_instantUpdate) 
                    _hierarchicObjectInTekla.PartsSetAttr("moment_M", value);
                OnPropertyChanged("M");
            }
        }

        public string N
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("N"); }
            set 
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N", value);
                if (_instantUpdate) 
                    _hierarchicObjectInTekla.PartsSetAttr("usilie_N", value);
                OnPropertyChanged("N");
            }
        }

        public string Q
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Q"); }
            set 
            { 
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q", value);
                if (_instantUpdate) 
                    _hierarchicObjectInTekla.PartsSetAttr("reakciya_A", value);
                OnPropertyChanged("Q");
            }
        }

        public string Material
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Material"); }
            set
            {

                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Material", value);
                if (_instantUpdate) 
                    _hierarchicObjectInTekla.PartSetMaterial(value);

                OnPropertyChanged("Material");
            }
        }

        public string Notes
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Notes"); }
            set 
            { 
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Notes", value);
                if (_instantUpdate) 
                    _hierarchicObjectInTekla.PartsSetAttr("prim_vedomost", value);
                OnPropertyChanged();
            }
        }

        internal Tekla.Structures.Identifier Identifier
        {
            get
            {
                return _hierarchicObjectInTekla.GetHOID;
            }
        }

        internal Tekla.Structures.Identifier FatherIdentifier
        {
            get
            {
                return _hierarchicObjectInTekla.GetFatherHOID;
            }
        }

        public bool IsComplexCrossSection
        {
            get
            {
                _isComplexCrossSection = BoolToInt(HierarchicObjectInTekla.IsComplex());

                var value = _isComplexCrossSection;
                //_hierarchicObjectInTekla.HierarchicObjectGetIntAttr("IsComplex");
                //_isComplexCrossSection = value;
                if (value == -1)
                    return false;
                return true;
            }
            set
            {
                _isComplexCrossSection = BoolToInt(HierarchicObjectInTekla.IsComplex());

                //_hierarchicObjectInTekla.HierarchicObjectSetAttribute("IsComplex", _isComplexCrossSection);

                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("slozhnoe_sechenie", _isComplexCrossSection);
                OnPropertyChanged();
            }
        }

        public int EmptyRowsNumber
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetIntAttr("EmptyRowsNumber"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("EmptyRowsNumber", value);
                if (_instantUpdate) 
                    _hierarchicObjectInTekla.PartsSetAttr("pustykh_strok", value);
                OnPropertyChanged();
            }
        }

        public bool OnOtherList
        {
            get 
            {
                int value =  _hierarchicObjectInTekla.HierarchicObjectGetDependentIntAttribute("OnOtherList");
                _crossSectionOnOtherList = value;
                return IntToBool(value);
            }
            set
            {
                _crossSectionOnOtherList = BoolToInt(value);
                _hierarchicObjectInTekla.HierarchicObjectSetDependentAttribute("OnOtherList", _crossSectionOnOtherList);

                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("ru_slozh_sech_list", _crossSectionOnOtherList);
                OnPropertyChanged();
                UpdateChildrenOnOtherList(value);
            }
        }

        public string GUID
        {
            get { return _hierarchicObjectInTekla.GetHierarchicObjectGUID().ToString(); }
        }
        #endregion

        #region Открытые методы
        public void GetSelectedObjects()
        {
            _hierarchicObjectInTekla.GetSelectedModedlObjects();
        }

        public bool AttachSelectedObjects()
        {
            bool b = _hierarchicObjectInTekla.AttachSelectedModedlObjects();
            OnPropertyChanged("IsComplexCrossSection");
            bool a = _hierarchicObjectInTekla.SetPropertiesForAttachingParts(
                Mark,
                Profile,
                Position,
                M,
                N,
                Q,
                Material,
                Notes,
                BoolToInt(IsComplexCrossSection),
                EmptyRowsNumber,
                _crossSectionOnOtherList,
                Classificator, 
                BOE);
            
            return a && b;
        }

        internal int BoolToInt(bool boolVal)
        {
            int val;
            if (boolVal)
            {
                val = 0;

            }
            else
            {
                val = -1;
            }
            return val;
        }

        internal bool IntToBool(int intVal)
        {
            bool val;
            if (intVal == -1)
            {
                val = false;

            }
            else
            {
                val = true;
            }
            return val;
        }

        internal void DeleteHierarchicObject()
        {
            _hierarchicObjectInTekla.DeleteHierarchicObject();
        }

        internal bool UpdateAssociatedObjects()
        {
            bool res = _hierarchicObjectInTekla.SetPropertiesForAttachedParts(
                Mark,
                Profile,
                Position,
                M,
                N,
                Q,
                Material,
                Notes,
                _isComplexCrossSection,
                EmptyRowsNumber,
                _crossSectionOnOtherList, 
                Classificator,
                BOE); 
            return res;
        }


        private void UpdateChildrenMarks(string mark)
        {            
            var collectionCut = this.Collection.Where(t => t.HierarchicObjectInTekla.FatherGUID.Equals(this.GUID));
            
            foreach(var boe in collectionCut)
            {
                boe.Mark = mark;
            }
        }

        private void UpdateChildrenAlbum(string album)
        {
            if (Collection != null)
            {
                var collectionCut = this.Collection.Where(t => t.HierarchicObjectInTekla.FatherGUID.Equals(this.GUID));
                foreach (var boe in collectionCut)
                {
                    boe.BOE = album;
                }
            }
        }

        private void UpdateRefList(string refList)
        {
            if (Collection != null)
            {
                var collectionCut = this.Collection.Where(t => t.HierarchicObjectInTekla.FatherGUID.Equals(this.GUID));
                foreach (var boe in collectionCut)
                {
                    boe.RefList = refList;
                }
            }
        }

        private void UpdateChildrenOnOtherList(bool onOtherList)
        {
            if (Collection != null)
            {
                var collectionCut = this.Collection.Where(t => t.HierarchicObjectInTekla.FatherGUID.Equals(this.GUID));

                foreach (var boe in collectionCut)
                {
                    boe.OnOtherList = onOtherList;
                }
            }

            //if (Collection != null)
            //{
            //    var collectionCut = this.Collection.Where(t => t.HierarchicObjectInTekla.GetFatherHOID.Equals(this.HierarchicObjectInTekla.GetHOID)).ToList();
            //    foreach (var boe in collectionCut)
            //    {
            //        boe.OnOtherList = onOtherList;
            //    }
            //}
        }

        #endregion

        #region Валидация
        public string Error
        {
            get
            {
                return null;
            }
        }

        private bool MaterialIsAllowed()
        {
            bool result = TeklaDB.MaterialIsAllowed(Material);
            return result;
        }

        private bool ProfileIsAllowed()
        {
            bool result = TeklaDB.ProfileIsAllowed(Profile);
            return result;
        }

        public string this[string name]
        {
            get
            {
                string result = null;

                if (name == "Material")
                {
                    if ((Material != null) & (!MaterialIsAllowed()))
                    {
                        result = "Check material name";
                    }
                }
                else if(name == "Profile")
                {
                    if ((Profile != null) &(!ProfileIsAllowed()))
                    {
                        result = "Check profile name";
                    }
                }
                else if (name == "Classificator")
                {
                    if ((Classificator != null) & (Classificator == "20000"))
                    {
                        result = "Does csv table is inside model folder?";
                    }
                    else if (Classificator == "10000")
                        result = "Does prefix exist in csv table?";
                }
                else if (name == "ProfileForSpec")
                {
                    if ((ProfileForSpec == null) || (ProfileForSpec == ""))
                    {
                        result = "Empty field is prohibited";
                    }
                    else if (ProfileForSpec == "not found in library")
                    {
                        result = "Please fill the value manually";
                    }
                }
                else if (name == "Sketch")
                {
                    if ((Sketch == null) || (Sketch == ""))
                    {
                        result = "Empty field is prohibited. Please fill the value manually";
                    }
                }

                return result;
            }
        }

        #endregion
    }

    enum IsComplex
    {
        No = -1,
        Yes = 0
    }
    
}