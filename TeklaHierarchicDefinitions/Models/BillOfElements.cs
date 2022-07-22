using NLog;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TeklaHierarchicDefinitions.TeklaAPIUtils;
using TeklaHierarchicDefinitions.Classifiers;
using System;
using Tekla.Structures.Model;
using System.Collections;

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
            Profile = "D100";
            Position = "";
            Q = "";
            N = "";
            M = "";
            StartFrictionConnection = -1;
            EndFrictionConnection = -1;
            RotNotAllowed = -1;
            StartMomentConnection = 0;
            EndMomentConnection = 0;
            Material = "C100";
            Notes = "";
            IsComplexCrossSection = true;
            EmptyRowsNumber = 0;
            OnOtherList = false;
            
            
            _hierarchicObjectInTekla.CommitChanges();
            TeklaDB.model.CommitChanges();
        }
        #endregion

        #region Внутренние параметры объекта

        private HierarchicObjectInTekla _hierarchicObjectInTekla;

        private bool _instantUpdate;

        private int _isComplexCrossSection;

        internal MyObservableCollection<BillOfElements> Collection;

        private int _crossSectionOnOtherList;

        private bool _selection;

        private static KeyValuePair<int, string>[] momentConnection = {
                new KeyValuePair<int, string>(-1, "No"),
                new KeyValuePair<int, string>(0, "Yes")
            };
        private static KeyValuePair<int, string>[] rotNotAllowed = {
                new KeyValuePair<int, string>(-1, "Yes"),
                new KeyValuePair<int, string>(0, "No")
            };
        private static KeyValuePair<int, string>[] frictionConnection = {
                new KeyValuePair<int, string>(-1, ""),
                new KeyValuePair<int, string>(0, "No"),
                new KeyValuePair<int, string>(1, "Yes")
            };


        #endregion

        #region Открытые свойства
        public KeyValuePair<int, string>[] MomentConnection
        {
            get
            {
                return momentConnection;
            }
            set { }
        }

        public KeyValuePair<int, string>[] RotationOptions
        {
            get
            {
                return rotNotAllowed;
            }
            set { }
        }

        public KeyValuePair<int, string>[] FrictionConnection
        {
            get
            {
                return frictionConnection;
            }
            set { }
        }

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

        public int ObjectsCount
        {
            get { return _hierarchicObjectInTekla.HierarchicObject.GetChildren().GetSize(); }
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
            get 
            { 
                return _hierarchicObjectInTekla.Name; 
            }
            set 
            { 
                
                _hierarchicObjectInTekla.Name = value;
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartSetPrefix(value);
                Classificator = ClassGenerator.Generate(_hierarchicObjectInTekla.Name, Position);
                Category = ClassGenerator.GenerateCategory(_hierarchicObjectInTekla.Name);                
                OnPropertyChanged("Mark");
                OnPropertyChanged("Father");
                UpdateChildrenMarks(value);
            }
        }

        public string Category
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetDependentStrAttribute("RU_BOM_CTG"); }
            set
            {
                bool res = false;
                _hierarchicObjectInTekla.HierarchicObjectSetDependentAttribute("RU_BOM_CTG", value);
                if (_instantUpdate)
                    res = _hierarchicObjectInTekla.AllAssemblyPartsSetAttr("RU_BOM_CTG", value);

                OnPropertyChanged();
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
                Classificator = ClassGenerator.Generate(Mark, value);
                OnPropertyChanged("Position");
            }
        }

        public string M
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("M"); }
            set 
            { 
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("M", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("M_end", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("M_summary", value);
                if (_instantUpdate)                    
                    _hierarchicObjectInTekla.PartsSetAttr("moment_M", value);
                double momentConn;
                if (double.TryParse(value, out momentConn))
                {
                    if (momentConn != 0)
                        StartMomentConnection = 0;
                    else
                        StartMomentConnection = -1;
                }
                OnPropertyChanged("M");
                OnPropertyChanged("M_end");
                OnPropertyChanged("M_summary");
            }
        }

        public string M_end
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("M_end"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("M_end", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("M_summary", M + "/" + value);
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("moment_M", M +"/"+ value);
                double momentConn;
                if (double.TryParse(value, out momentConn))
                {
                    if (momentConn != 0)
                        EndMomentConnection = 0;
                    else
                        EndMomentConnection = -1;
                }                
                OnPropertyChanged("M_end");
                OnPropertyChanged("M_summary");
            }
        }

        internal void AddAsChildHO(List<BillOfElements> fatherItem)
        {
            foreach (var i in fatherItem)
            {
                if(i != this)
                    i.HierarchicObjectInTekla.AddFather(this.HierarchicObjectInTekla);
            }

            OnPropertyChanged("Father");
            OnPropertyChanged("Mark");
            Classificator = ClassGenerator.Generate(Mark, Position);
            Category = ClassGenerator.GenerateCategory(Mark);
        }

        internal void RemoveFather()
        {
            this.HierarchicObjectInTekla.RemoveFather();
            OnPropertyChanged("Father");
            OnPropertyChanged("Mark");
        }

        public string M_summary
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("M_summary"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("M_summary", value);
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("moment_M", value);
                OnPropertyChanged("M_summary");
            }
        }

        public int StartMomentConnection
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetIntAttr("StartMomentConn"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("StartMomentConn", value);
                if (_instantUpdate)     
                    _hierarchicObjectInTekla.PartsSetAttr("START_MOMENT_CONN", value);
                OnPropertyChanged();
            }
        }

        public int EndMomentConnection
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetIntAttr("EndMomentConn"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("EndMomentConn", value);
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("END_MOMENT_CONN", value);
                OnPropertyChanged();
            }
        }

        public int RotNotAllowed
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetIntAttr("RotNotAllowed"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("RotNotAllowed", value);
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("ROT_NOT_ALLOWED", value);
                OnPropertyChanged();
            }
        }

        public int StartFrictionConnection
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetIntAttr("StartFrictionConn"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("StartFrictionConn", value);
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("START_FRICT_CONN", value);
                OnPropertyChanged();
            }
        }

        public int EndFrictionConnection
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetIntAttr("EndFrictionConn"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("EndFrictionConn", value);
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("END_FRICT_CONN", value);
                OnPropertyChanged();
            }
        }

        public string N
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("N"); }
            set 
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_end", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_start_min", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_end_min", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_summary", GetNSummary());
                if (_instantUpdate) 
                    _hierarchicObjectInTekla.PartsSetAttr("usilie_N", N_summary);
                OnPropertyChanged("N");
                OnPropertyChanged("N_end");
                OnPropertyChanged("N_start_min");
                OnPropertyChanged("N_end_min");
                OnPropertyChanged("N_summary");
            }
        }

        public string N_end
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("N_end"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_end", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_end_min", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_summary", GetNSummary());
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("usilie_N", N_summary);
                OnPropertyChanged("N_end");
                OnPropertyChanged("N_end_min");
                OnPropertyChanged("N_summary");
            }
        }

        public string N_start_min
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("N_start_min"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_start_min", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_summary", GetNSummary());
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("usilie_N", N_summary);
                OnPropertyChanged("N_start_min");
                OnPropertyChanged("N_summary");
            }
        }

        public string N_end_min
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("N_end_min"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_end_min", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_summary", GetNSummary()); 
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("usilie_N", N_summary);
                OnPropertyChanged("N_end_min");
                OnPropertyChanged("N_summary");
            }
        }

        public string N_summary
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("N_summary"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("N_summary", value);
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("usilie_N", value);
                OnPropertyChanged("N_summary");
            }
        }

        public string Q
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Q"); }
            set 
            { 
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q_end", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q_summary", GetQSummary());
                if (_instantUpdate) 
                    _hierarchicObjectInTekla.PartsSetAttr("reakciya_A", Q_summary);
                OnPropertyChanged("Q");
                OnPropertyChanged("Q_end");
                OnPropertyChanged("Q_summary");
            }
        }

        public string Q_end
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Q_end"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q_end", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q_summary", GetQSummary());
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("reakciya_A", Q_summary);
                OnPropertyChanged("Q_end");
                OnPropertyChanged("Q_summary");
            }
        }

        public string Q_min
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Q_min"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q_min", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q_end_min", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q_summary", GetQSummary());
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("reakciya_A", Q_summary);
                OnPropertyChanged("Q_min");
                OnPropertyChanged("Q_end_min");
                OnPropertyChanged("Q_summary");
            }
        }

        public string Q_end_min
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Q_end_min"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q_end_min", value);
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q_summary", GetQSummary());
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("reakciya_A", Q_summary);
                OnPropertyChanged("Q_end_min");
                OnPropertyChanged("Q_summary");
            }
        }


        public string Q_summary
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Q_summary"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Q_summary", value);
                if (_instantUpdate)
                    _hierarchicObjectInTekla.PartsSetAttr("reakciya_A", value);
                OnPropertyChanged("Q_summary");
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

        internal void BorrowPropertiesFromModelObject()
        {
            Hashtable ht = TeklaDB.GetInterestedPropertiesFromSelectedObjects();
            this.Mark = ht["ASSEMBLY.PREFIX"].ToString();
            this.Material = ht["MATERIAL"].ToString();
            this.Profile = ht["PROFILE"].ToString();
            var connConvert = FrictionConnection.ToDictionary(t=>t.Value,t=>t.Key);
            if (ht.ContainsKey("END_FRICT_CONN"))
                this.EndFrictionConnection = connConvert[ht["END_FRICT_CONN"].ToString()];
            if (ht.ContainsKey("START_FRICT_CONN"))
                this.StartFrictionConnection = connConvert[ht["START_FRICT_CONN"].ToString()];
            
            var momentConvert = MomentConnection.ToDictionary(t => t.Value, t => t.Key);
            //this.EndMomentConnection = connConvert[ht["END_FRICT_CONN"].ToString()];
            //this.StartMomentConnection
            if (ht.ContainsKey("PRELIM_MARK"))
                this.Position = ht["PRELIM_MARK"].ToString();
            if (ht.ContainsKey("momentY1"))
                this.M = (double.Parse(ht["momentY1"].ToString()) / 1000).ToString();
            if (ht.ContainsKey("momentY2"))
                this.M_end = (double.Parse(ht["momentY2"].ToString()) / 1000).ToString();
            if (ht.ContainsKey("axial1"))
                this.N = (double.Parse(ht["axial1"].ToString()) / 1000).ToString();
            if (ht.ContainsKey("axial2"))
                this.N_end = (double.Parse(ht["axial2"].ToString()) / 1000).ToString();
            if (ht.ContainsKey("axialcomp1"))
                this.N_start_min = (double.Parse(ht["axialcomp1"].ToString()) / 1000).ToString();
            if (ht.ContainsKey("axialcomp1"))
                this.N_end_min = (double.Parse(ht["axialcomp1"].ToString()) / 1000).ToString();
            if (ht.ContainsKey("shearZ1"))
                this.Q = (double.Parse(ht["shearZ1"].ToString()) / 1000).ToString();
            if (ht.ContainsKey("shearZ2"))
                this.Q_end = (double.Parse(ht["shearZ2"].ToString()) / 1000).ToString();

            var rotConvert = RotationOptions.ToDictionary(t => t.Value, t => t.Key);
            if (ht.ContainsKey("ROT_NOT_ALLOWED"))
                this.RotNotAllowed = rotConvert[ht["ROT_NOT_ALLOWED"].ToString()];
            //hashtable["Mark"] = borrowedPart.AssemblyNumber.Prefix;
            //hashtable["Profile"] = borrowedPart.Profile.ProfileString;
            //GetStringPropertyFromObject(borrowedPart, "PRELIM_MARK", "Position", ref hashtable);
            //GetStringPropertyFromObject(borrowedPart, "momentY1", "momentY1", ref hashtable);
            //GetStringPropertyFromObject(borrowedPart, "momentY2", "momentY2", ref hashtable);
            //GetStringPropertyFromObject(borrowedPart, "momentZ1", "momentZ1", ref hashtable);
            //GetStringPropertyFromObject(borrowedPart, "momentZ2", "momentZ2", ref hashtable);
            //GetStringPropertyFromObject(borrowedPart, "axial1", "axial1", ref hashtable);
            //GetStringPropertyFromObject(borrowedPart, "axial2", "axial12", ref hashtable);
            //GetStringPropertyFromObject(borrowedPart, "axialcomp1", "axialcomp1", ref hashtable);
            //GetStringPropertyFromObject(borrowedPart, "axialcomp2", "axialcomp2", ref hashtable);



            //int startMomentConnection,
            //int endMomentConnection,
            //int startFrictionConnection,
            //int endFrictionConnection,

            //string material,
            //string notes,
            //int isSimple,
            //int emptyRowsNumber,
            //int crossSectionOnOtherList,
            //string classificator,
            //string album

            //return exportedHT;
            //throw new NotImplementedException();
        }

        internal void GetSimilardObjects(List<HierarchicObjectInTekla> hoit, bool filterByMark, bool filterByProfile, bool filterByMaterial)
        {
            List<HierarchicObject> HOinTS = hoit.Select(o => o.HierarchicObject).ToList();
            string mark = null;

            if (filterByMark)
                mark = Mark;
            string profile = null;
            if (filterByProfile)
                profile = Profile;
            string material = null;
            if (filterByMaterial)
                material = Material;

            TeklaDB.SelectSimilarModelObjects(mark, profile, material, HOinTS);
        }

        internal string GetNSummary()
        {
            string N_startF;
            string N_endF;
            if (N == N_start_min)
            {
                N_startF = N;
            }
            else
            {
                double d = 0;
                string N_start_res = string.Empty;
                if (double.TryParse(N_start_min, out d))
                {
                    N_start_res = (-d).ToString();
                }
                else
                {
                    N_start_res = N_start_min;
                }

                N_startF = N + ";" + N_start_res;
            }

            if (N_end == N_end_min)
            {
                N_endF = N_end;
            }
            else
            {
                double d = 0;
                string N_end_res = string.Empty;
                if (double.TryParse(N_end_min, out d))
                {
                    N_end_res = (-d).ToString();
                }
                else
                {
                    N_end_res = N_end_min;
                }
                N_endF = N_end + ";" + N_end_res;
            }

            string n_F;
            if (N_startF == N_endF)
            {
                n_F = N_startF;
            }
            else
            {
                n_F = N_startF + "/" + N_endF;
            }
            return n_F;
        }

        internal string GetQSummary()
        {
            string Q_startF;
            string Q_endF;
            if (Q == Q_min)
            {
                Q_startF = Q;
            }
            else
            {
                Q_startF = Q + ";" + Q_min;
            }
            if (Q_end == Q_end_min)
            {
                Q_endF = Q_end;
            }
            else
            {
                Q_endF = Q_end + ";" + Q_end_min;
            }

            string Q_F;
            if (Q_startF == Q_endF)
            {
                Q_F = Q_startF;
            }
            else
            {
                Q_F = Q_startF + "/" + Q_endF;
            }
            return Q_F;
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
                M_end,
                StartMomentConnection,
                EndMomentConnection,
                StartFrictionConnection,
                EndFrictionConnection,
                N,
                N_end,
                N_start_min,
                N_end_min,
                N_summary,
                Q,
                Q_end,
                Q_min,
                Q_end_min,
                Q_summary,
                Material,
                Notes,
                BoolToInt(IsComplexCrossSection),
                EmptyRowsNumber,
                _crossSectionOnOtherList,
                Classificator, 
                BOE,
                Category);
            OnPropertyChanged("ObjectsCount");

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
                M_end,
                StartMomentConnection,
                EndMomentConnection,
                StartFrictionConnection,
                EndFrictionConnection,
                N,
                N_end,
                N_start_min,
                N_end_min,
                N_summary,
                Q,
                Q_end,
                Q_min,
                Q_end_min,
                Q_summary,
                Material,
                Notes,
                _isComplexCrossSection,
                EmptyRowsNumber,
                _crossSectionOnOtherList, 
                Classificator,
                BOE,
                Category); 
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

        private bool IsFilled()
        {
            bool result = Mark.Length > 1;
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
                else if (name == "Mark")
                {
                    if (Mark.Length<2)
                    {
                        result = "Is mark properly set?";
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