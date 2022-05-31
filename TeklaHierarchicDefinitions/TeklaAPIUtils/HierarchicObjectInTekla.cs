using System;
using System.Collections.Generic;
using Tekla.Structures.Model;
using System.Collections;

namespace TeklaHierarchicDefinitions.TeklaAPIUtils
{
    /// <summary>
    /// Описывает объект, которым будем управлять в интерфейсе иерархического списка
    /// </summary>
    public class HierarchicObjectInTekla
    {       
        private HierarchicDefinition _hierarchicDefinition;
        private HierarchicObject _hierarchicObject;

        #region Cвойства
        public string Name
        {
            get 
            {
                if (_hierarchicObject.Father != null)
                {
                    HierarchicObject hierarchicObject = new HierarchicObject();
                    hierarchicObject.Identifier = _hierarchicObject.Father.Identifier;
                    hierarchicObject.Select();
                    return hierarchicObject.Name;
                }
                return _hierarchicObject.Name;
            }
            internal set 
            {
                if (_hierarchicObject.Father == null)
                {
                    _hierarchicObject.Name = value;
                    _hierarchicObject.Modify();
                }
                else
                {
                    HierarchicObject fatherHierarchicObject = new HierarchicObject();
                    fatherHierarchicObject.Identifier = _hierarchicObject.Father.Identifier;
                    fatherHierarchicObject.Select();
                    _hierarchicObject.Name = fatherHierarchicObject.Name;
                    _hierarchicObject.Modify();
                }
                TeklaDB.model.CommitChanges("1");
            }
        }

        internal void CommitChanges()
        {
            _hierarchicObject.Modify();
            //TeklaDB.model.CommitChanges();
        }

        private HierarchicDefinition HierarchicDefinition
        {
            get { return _hierarchicDefinition; }
            set { _hierarchicDefinition = value; }
        }

        public HierarchicObject HierarchicObject
        {
            get { return _hierarchicObject; }
            set { _hierarchicObject = value; }
        }

        public string Father 
        {
            get 
            {
                if(_hierarchicObject.Father != null)
                {
                    return GetFather().Name;
                }
                    
                return "";
            }
        }

        public string FatherGUID 
        {
            get
            {
                if (_hierarchicObject.Father != null)
                {
                    var guid = GetFather().Identifier.GUID.ToString();
                    return GetFather().Identifier.GUID.ToString();
                }

                return "";
            } 
        }


        #endregion

        #region Конструкторы
        /// <summary>
        /// Формирует иерархический объект
        /// </summary>
        /// <param name="hierarchicObject"></param>
        internal HierarchicObjectInTekla(HierarchicObject hierarchicObject, HierarchicDefinition hierarchicDefinition)
        {
            _hierarchicDefinition = hierarchicDefinition;
            _hierarchicObject = hierarchicObject;
        }

        internal HierarchicObjectInTekla(string name, HierarchicDefinition hierarchicDefinition)
        {
            _hierarchicDefinition = hierarchicDefinition;
            _hierarchicObject = TeklaDB.CreateHierarchicObject(name , _hierarchicDefinition);
        }

        public HierarchicObjectInTekla()
        {
            _hierarchicDefinition = TeklaDB.GetHierarchicDefinitionWithName(TeklaDB.hierarchicDefinitionElementListName);
            _hierarchicObject = TeklaDB.CreateHierarchicObject(_hierarchicDefinition);
        }

        public HierarchicObjectInTekla(HierarchicObjectInTekla hierarchicObjectInTekla, bool isFoundationList = false)
        {
            if (isFoundationList)
                _hierarchicDefinition = TeklaDB.GetHierarchicDefinitionWithName(TeklaDB.hierarchicDefinitionFoundationListName);
            else
                _hierarchicDefinition = TeklaDB.GetHierarchicDefinitionWithName(TeklaDB.hierarchicDefinitionElementListName);
            _hierarchicObject = TeklaDB.CreateHierarchicObject(_hierarchicDefinition, hierarchicObjectInTekla.HierarchicObject);
        }
        #endregion

        #region Методы HierarchicObjectInTekla
        /// <summary>
        /// Добавить атрибуты
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool HierarchicObjectSetAttributes(Dictionary<string,string> input)
        {
            throw new NotImplementedException();
            //_hierarchicObject.SetUserProperty(name,val)
            //    return true;
        }

        public bool HierarchicObjectSetAttributes(ArrayList input)
        {
            HierarchicObject.Insert();
            foreach (var obj in input)
            {                
                var pair = (KeyValuePair<string, string>)obj;
                string strValue = pair.Value;
                double doubleValue;
                if (double.TryParse(pair.Value, out doubleValue))
                {
                    HierarchicObject.SetUserProperty(pair.Key, doubleValue);
                    continue;
                }
                int intValue;
                if (Int32.TryParse(pair.Value, out intValue))
                {
                    HierarchicObject.SetUserProperty(pair.Key, intValue);
                    continue;
                }                  
                if (!strValue.Equals(null))
                    HierarchicObject.SetUserProperty(pair.Key, strValue);
            }
            var c = HierarchicObject.Modify();
            return c;
        }


        /// <summary>
        /// Вытащить атрибуты
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Dictionary<string,string> HierarchicObjectGetAttributes(List<string> input)
        {
            throw new NotImplementedException();
            //return new Dictionary<string, string>();

        }

        private HierarchicObject GetFather()
        {
            HierarchicObject fatherHierarchicObject = new HierarchicObject();
            fatherHierarchicObject.Identifier = _hierarchicObject.Father.Identifier;
            fatherHierarchicObject.Select();
            return fatherHierarchicObject;
        }

        /// <summary>
        /// Внести атрибут
        /// </summary>
        /// <param name="name">Имя атрибута</param>
        /// <param name="input">Значение атрибута</param>
        /// <returns>Значение внесено</returns>
        public bool HierarchicObjectSetAttribute(string name, string input)
        {            
            bool res = this.HierarchicObject.SetUserProperty(name, input);

            if (res)
            {
                TeklaDB.model.CommitChanges(HierarchicObject.Name + ": property " + name + " is set to " + input);
            }
            return res;
        }

        public string HierarchicObjectGetDependentStrAttribute(string name)
        {
            string res = string.Empty;
            if (_hierarchicObject.Father != null)
            {
                HierarchicObject hierarchicObject = new HierarchicObject();
                hierarchicObject.Identifier = _hierarchicObject.Father.Identifier;
                hierarchicObject.Select();
                
                if (hierarchicObject.GetUserProperty(name, ref res))
                    return res;
                return "";
            }
            else
            {
                if (_hierarchicObject.GetUserProperty(name, ref res))
                    return res;
                return "";
            }
        }

        public int HierarchicObjectGetDependentIntAttribute(string name)
        {
            int res = -100;
            if (_hierarchicObject.Father != null)
            {
                HierarchicObject hierarchicObject = new HierarchicObject();
                hierarchicObject.Identifier = _hierarchicObject.Father.Identifier;
                hierarchicObject.Select();

                if (hierarchicObject.GetUserProperty(name, ref res))
                    return res;
                return -10;
            }
            else
            {
                if (_hierarchicObject.GetUserProperty(name, ref res))
                    return res;
                return -10;
            }
        }

        public bool HierarchicObjectSetDependentAttribute(string name, string input)
        {
            if (_hierarchicObject.Father == null)
            {
                _hierarchicObject.SetUserProperty(name, input);
                _hierarchicObject.Modify();
            }
            else
            {
                HierarchicObject fatherHierarchicObject = new HierarchicObject();
                fatherHierarchicObject.Identifier = _hierarchicObject.Father.Identifier;
                fatherHierarchicObject.Select();
                string res = string.Empty;
                if (fatherHierarchicObject.GetUserProperty(name, ref res))
                {
                    _hierarchicObject.SetUserProperty(name, res);
                    _hierarchicObject.Modify();
                }
            }
            return TeklaDB.model.CommitChanges(HierarchicObject.Name + ": property " + name + " is set to " + input);
        }

        public bool HierarchicObjectSetDependentAttribute(string name, int input)
        {
            if (_hierarchicObject.Father == null)
            {
                _hierarchicObject.SetUserProperty(name, input);
                _hierarchicObject.Modify();
            }
            else
            {
                HierarchicObject fatherHierarchicObject = new HierarchicObject();
                fatherHierarchicObject.Identifier = _hierarchicObject.Father.Identifier;
                fatherHierarchicObject.Select();
                int res = -1;
                if (fatherHierarchicObject.GetUserProperty(name, ref res))
                {
                    _hierarchicObject.SetUserProperty(name, res);
                    _hierarchicObject.Modify();
                }
            }
            return TeklaDB.model.CommitChanges(HierarchicObject.Name + ": property " + name + " is set to " + input);
        }

        public bool HierarchicObjectSetAttribute(string name, int input)
        {
            bool res = this.HierarchicObject.SetUserProperty(name, input);

            if (res)
            {
                TeklaDB.model.CommitChanges(HierarchicObject.Name + ": property "+ name + " is set to " + input);
            }
            return res;
        }

        public bool HierarchicObjectSetAttribute(string name, double input)
        {
            bool res = this.HierarchicObject.SetUserProperty(name, input);

            if (res)
            {
                TeklaDB.model.CommitChanges(HierarchicObject.Name + ": property " + name + " is set to " + input);
            }
            return res;
        }

        public bool PartsSetAttr(string name, string input)
        {
            var moenum = _hierarchicObject.GetChildren();
            foreach (ModelObject mo in moenum)
            {
                Part part = mo as Part;
                if (!part.Equals(null))
                {
                    TeklaDB.SetPropertyStr(part, name, input);
                }
            }

            bool res = TeklaDB.model.CommitChanges(HierarchicObject.Name + ": property " + name + " is set to " + input);
            
            return res;
        }

        public bool AllAssemblyPartsSetAttr(string name, string input)
        {
            var moenum = _hierarchicObject.GetChildren();
            HashSet<Assembly> hs = new HashSet<Assembly>();
            foreach (ModelObject mo in moenum)
            {
                Part part = mo as Part;
                if (!part.Equals(null))
                {
                    hs.Add(part.GetAssembly());
                }
            }
            HashSet<Part> hsParts = new HashSet<Part>();
            foreach (Assembly assembly in hs)
            {
                if (!assembly.Equals(null))
                {
                    GetAllAssemblyParts(assembly, ref hsParts);
                }
            }            
            foreach (Part part in hsParts)
            {
                if (!part.Equals(null))
                    TeklaDB.SetPropertyStr(part, name, input);
            }
            bool res = TeklaDB.model.CommitChanges(HierarchicObject.Name + ": property " + name + " is set to " + input);
            return res;
        }

        void GetAllAssemblyParts(Assembly assembly, ref HashSet<Part> hs)
        {
            var assEnum = assembly.GetSecondaries();
            assEnum.Add(assembly.GetMainPart());
            foreach (ModelObject mo in assEnum)
            {
                if (mo is Part)
                {
                    hs.Add(mo as Part);
                }
                else if (mo is Assembly)
                {
                    GetAllAssemblyParts(mo as Assembly, ref hs);
                }
            }
            
        }

        public bool PartsSetAttr(string name, int input)
        {

            var moenum = _hierarchicObject.GetChildren();
            foreach (ModelObject mo in moenum)
            {
                Part part = mo as Part;
                if (!part.Equals(null))
                {
                    TeklaDB.SetPropertyInt(part, name, input);
                }
            }

            bool res = TeklaDB.model.CommitChanges("Part property " + name + " is set to " + input);

            return res;
        }

        public bool PartSetProfile(string profile)
        {
            var children = HierarchicObject.GetChildren();
            bool res = false;
            
            foreach (var ch in children)
            {
                var part = (Part)ch;
                if (part != null)
                {
                    res = TeklaDB.SetProfile(part,profile);
                }
            }
            
            if (res)
            {
                res = TeklaDB.model.CommitChanges("7");
            }
            return res;
        }

        public bool PartSetMaterial(string material)
        {
            var children = HierarchicObject.GetChildren();
            bool res = false;
            foreach (var ch in children)
            {
                var part = ch as Part;
                if (part != null)
                {
                    res = TeklaDB.SetMaterial(part, material);
                }
            }


            if (res)
            {
                res = TeklaDB.model.CommitChanges("8");
            }
            return res;
        }

        internal bool PartSetClass(string classficator)
        {
            var children = HierarchicObject.GetChildren();
            bool res = false;
            foreach (var ch in children)
            {
                var part = (Part)ch;
                if (part != null)
                {
                    res = TeklaDB.SetClass(part, classficator);
                }
            }

            return res;
        }


        internal void DeleteHierarchicObject()
        {
            if(this._hierarchicObject.Father != null)
            {
                var fatherHO = GetFather();
                fatherHO.HierarchicChildren.Remove(_hierarchicObject);
                fatherHO.Modify();
            }
            TeklaDB.DeleteHierarchicObject(_hierarchicObject);
            TeklaDB.model.CommitChanges("9");
        }

        public bool PartSetPrefix(string prefix)
        {
            var children = HierarchicObject.GetChildren();
            bool res = false;
            foreach (var ch in children)
            {
                var part = (Part)ch;
                if (part != null)
                {
                    res = TeklaDB.SetPrefix(part, prefix);
                }
            }

            if (res)
            {
                res = TeklaDB.model.CommitChanges("10");
            }
            return res;
        }
        /// <summary>
        /// Вытащить атрибут
        /// </summary>
        /// <param name="name">имя атрибута</param>
        /// <returns>Значение</returns>
        public string HierarchicObjectGetAttr(string name)
        {
            string res = string.Empty;
            if (_hierarchicObject.GetUserProperty(name, ref res))
                return res;
            return "";
        }

        public int HierarchicObjectGetIntAttr(string name)
        {
            int res = 0;
            if (_hierarchicObject.GetUserProperty(name, ref res))
                return res;
            return 0;
        }

        public double HierarchicObjectGetDouble(string name)
        {
            double res = 0;
            if (_hierarchicObject.GetUserProperty(name, ref res))
                return res;
            return 0;
        }


        

        /// <summary>
        /// Выдает GUID иерархического объекта
        /// </summary>
        /// <returns></returns>
        public Guid GetHierarchicObjectGUID()
        {
            return _hierarchicObject.Identifier.GUID;
        }

        public Tekla.Structures.Identifier GetHOID
        {
            get
            {
                return _hierarchicObject.Identifier;
            }            
        }
        
        public Tekla.Structures.Identifier GetFatherHOID
        {
            get
            {
                if (_hierarchicObject.Father != null)
                    return _hierarchicObject.Father.Identifier;
                return new Tekla.Structures.Identifier();
            }
        }

        /// <summary>
        /// Выделяет объекты в модели
        /// </summary>
        internal void GetSelectedModedlObjects()
        {
            var moenum = _hierarchicObject.GetChildren();
            ArrayList partList = new ArrayList() {};

            if (moenum != null)
            {
                foreach (ModelObject mo in moenum)
                {
                    partList.Add(mo);
                }
            }

            TeklaDB.SelectObjectsInModelView(partList);
        }

        internal List<ModelObject> GetRuledModedlObjects()
        {
            var moenum = _hierarchicObject.GetChildren();
            List<ModelObject> partList = new List<ModelObject>();
            if (moenum != null)
            {
                foreach (ModelObject mo in moenum)
                {
                    partList.Add(mo);
                }
            }
            return partList;
        }

        internal bool SetPropertiesForAttachingParts(
            string mark, 
            string profile, 
            string position,
            string m, 
            string m_end,
            int startMomentConnection,
            int endMomentConnection,
            int startFrictionConnection,
            int endFrictionConnection,
            string n, 
            string n_end,
            string n_start_min,
            string n_end_min,
            string n_summary,
            string q, 
            string q_end,
            string material, 
            string notes = "",
            int isSimple = -1,
            int emptyRowsNumber = 0,
            int crossSectionOnOtherList = -1,
            string classificator = "40000",
            string album = "unset",
            string category = "")
        {
            AllAssemblyPartsSetAttr("RU_BOM_CTG", category);
            return TeklaDB.InheritPropsFromHierarchicObjectToSelectedParts(
                _hierarchicObject, 
                mark,
                profile,
                position,
                m,
                m_end,
                startMomentConnection,
                endMomentConnection,
                startFrictionConnection,
                endFrictionConnection,
                n,
                n_end,
                n_start_min,
                n_end_min,
                n_summary,
                q,
                q_end,
                material,
                notes,
                isSimple,
                emptyRowsNumber,
                crossSectionOnOtherList,
                classificator,
                album);
        }


        internal bool SetPropertiesForAttachedParts(
            string mark,
            string profile,
            string position,
            string m,
            string m_end,
            int startMomentConnection,
            int endMomentConnection,
            int startFrictionConnection,
            int endFrictionConnection,
            string n,
            string n_end,
            string n_start_min,
            string n_end_min,
            string n_summary,
            string q,
            string q_end,
            string material,
            string notes = "",
            int isSimple = -1,
            int emptyRowsNumber = 0,
            int crossSectionOnOtherList = -1,
            string classificator = "40000",
            string album = "unset",
            string category = "")
        {
            AllAssemblyPartsSetAttr("RU_BOM_CTG", category);
            return TeklaDB.InheritPropsFromHierarchicObjectToAssociatedParts(
                _hierarchicObject,
                mark,
                profile,
                position,
                m,
                m_end,
                startMomentConnection,
                endMomentConnection,
                startFrictionConnection,
                endFrictionConnection,
                n,
                n_end,
                n_start_min,
                n_end_min,
                n_summary,
                q,
                q_end,
                material,
                notes,
                isSimple,
                emptyRowsNumber,
                crossSectionOnOtherList,
                classificator,
                album);
        }

        internal bool AttachSelectedModedlObjects()
        {
            return TeklaDB.AttachSelectedModedlObjects(_hierarchicObject);
        }

        internal bool AttachSelectedDetails()
        {
            return TeklaDB.AttachSelectedDetails(_hierarchicObject);
        }

        internal bool RemoveSelectedModedlObjectsFromHierarchicObject()
        {
            return TeklaDB.RemoveSelectedModedlObjects(_hierarchicObject);
        }

        internal bool RemoveSelectedDetailsFromHierarchicObject()
        {
            return TeklaDB.RemoveSelectedDetails(_hierarchicObject);
        }
        #endregion

        #region Информация о иерархических объектах
        private bool HasFather()
        {
            bool res = false;
            res = _hierarchicObject.Father != null;
            return res;
        }

        private bool HasChildren()
        {
            bool res = false;
            res = _hierarchicObject.HierarchicChildren.Count > 0;
            var vvv = _hierarchicObject.GetHierarchicObjects().GetSize();
            return res;
        }

        internal bool IsComplex()
        {
            return HasFather() || HasChildren();
        }
        #endregion
    }

}
