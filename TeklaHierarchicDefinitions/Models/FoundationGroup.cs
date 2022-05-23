using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Tekla.Structures.Model;
using TeklaHierarchicDefinitions.TeklaAPIUtils;
using Object = System.Object;

namespace TeklaHierarchicDefinitions.Models
{
    public class FoundationGroup : INotifyPropertyChanged, IDataErrorInfo, IEquatable<FoundationGroup>
    {
        #region Внутренние параметры объекта        
        public HierarchicObjectInTekla _hierarchicObjectInTekla;
        private string joint;
        private string st;
        private string cr;
        private string gr;
        private string sp;
        private string crit;
        private double rx;
        private double ry;
        private double rz;
        private double rux;
        private double ruy;
        private double ruz;
        private string forceMark;
        #endregion

        #region Конструктор
        internal FoundationGroup(string foundationGroupName, HierarchicDefinition hierarchicDefinition)
        {
            _hierarchicObjectInTekla = new HierarchicObjectInTekla(foundationGroupName, hierarchicDefinition);    
            _hierarchicObjectInTekla.Name = foundationGroupName;
        }

        internal FoundationGroup(HierarchicObject hierarchicObject, HierarchicDefinition hierarchicDefinition)
        {
            _hierarchicObjectInTekla = new HierarchicObjectInTekla(hierarchicObject, hierarchicDefinition);            
        }

        #endregion

        private static Logger logger = LogManager.GetCurrentClassLogger();


        #region Методы
        /// <summary>
        /// Удаляет группу, вычищая иерархический объект
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        internal void Delete()
        {
            _hierarchicObjectInTekla.DeleteHierarchicObject();
        }

        internal void GetSelectedObjects()
        {
            _hierarchicObjectInTekla.GetSelectedModedlObjects();
        }

        internal bool AddBasements()
        {
            return _hierarchicObjectInTekla.AttachSelectedDetails();
        }

        internal bool RemoveBasements()
        {
            return _hierarchicObjectInTekla.RemoveSelectedDetailsFromHierarchicObject();
        }

        internal void UpdateAndInsert(Dictionary<string, List<ModelObject>> existingGroups)
        {

            ArrayList input = new ArrayList();
            input.Add(new KeyValuePair<string, string>("Joint", joint));
            input.Add(new KeyValuePair<string, string>("St", st));
            input.Add(new KeyValuePair<string, string>("Cr", cr));
            input.Add(new KeyValuePair<string, string>("Gr", gr));
            input.Add(new KeyValuePair<string, string>("Sp", sp));
            input.Add(new KeyValuePair<string, string>("Crit", crit));
            input.Add(new KeyValuePair<string, string>("Rx", rx.ToString()));
            input.Add(new KeyValuePair<string, string>("Ry", ry.ToString()));
            input.Add(new KeyValuePair<string, string>("Rz", rz.ToString()));
            input.Add(new KeyValuePair<string, string>("Rux", rux.ToString()));
            input.Add(new KeyValuePair<string, string>("Ruy", ruy.ToString()));
            input.Add(new KeyValuePair<string, string>("Ruz", ruz.ToString()));
            input.Add(new KeyValuePair<string, string>("ForceMark", forceMark));
            _hierarchicObjectInTekla.HierarchicObjectSetAttributes(input);
            if (existingGroups.ContainsKey(BasementMark))
            {
                TeklaDB.AttachModedlObjects(_hierarchicObjectInTekla.HierarchicObject, existingGroups[BasementMark]);
            }
            OnPropertyChanged("BasementMark");
            OnPropertyChanged("Joint");
            OnPropertyChanged("St");
            OnPropertyChanged("Cr");
            OnPropertyChanged("Gr");
            OnPropertyChanged("Sp");
            OnPropertyChanged("Crit");
            OnPropertyChanged("Rx");
            OnPropertyChanged("Ry");
            OnPropertyChanged("Rz");
            OnPropertyChanged("Rux");
            OnPropertyChanged("Ruy");
            OnPropertyChanged("Ruz");
            OnPropertyChanged("ForceMark");
        }


        public bool Equals(FoundationGroup other)
        {
            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return BasementMark.Equals(other.BasementMark) 
                && Rx.Equals(other.Rx)
                && Ry.Equals(other.Ry)
                && Rz.Equals(other.Rz)
                && Rux.Equals(other.Rux)
                && Ruy.Equals(other.Ruy)
                && Ruz.Equals(other.Ruz);
        }

        #endregion

        #region Свойства
        public string BasementMark
        {
            get { return _hierarchicObjectInTekla.Name; }
            private set
            {
                _hierarchicObjectInTekla.Name = value;
                OnPropertyChanged();
            }
        }
        public string JointNumber
        {
            get 
            {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetIntAttr("Joint").ToString();
                if (res.Length > 0)
                    return res;
                return joint; 
            }
            internal set
            {
                joint = value;
                OnPropertyChanged();
            }
        }
        public string St 
        {
            get 
            {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetAttr("St");
                if (res.Length > 0)
                    return res;
                return st; 
            }
            internal set
            {
                st = value;
                OnPropertyChanged();
            }
        }
        public string Cr
        {
            get
            {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetAttr("Cr");
                if (res.Length > 0)
                    return res;
                return cr; 
            }
            internal set
            {
                cr = value;
                OnPropertyChanged();
            }
        }
        public string Gr
        {
            get
            {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetAttr("Gr");
                if (res.Length > 0)
                    return res; 
                return gr; 
            }
            internal set
            {
                gr = value;
                OnPropertyChanged();
            }
        }
        public string Sp
        {
            get
            {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetAttr("Sp");
                if (res.Length > 0)
                    return res;
                return sp; 
            }
            internal set
            {
                sp = value;
                OnPropertyChanged();
            }
        }
        public string Crit
        {
            get 
            {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetAttr("Crit");
                if (res.Length > 0)
                    return res; 
                return crit; 
            }
            internal set
            {
                crit = value;
                OnPropertyChanged();
            }
        }
        public double Rx
        {
            get 
            {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetDouble("Rx");
                if (res != 0)
                    return res;
                return rx; 
            }
            internal set
            {
                rx = value;
                OnPropertyChanged();
            }
        }
        public double Ry
        {
            get {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetDouble("Ry");
                if (res != 0)
                    return res; 
                return ry; }
            internal set
            {
                ry = value;
                OnPropertyChanged();
            }
        }
        public double Rz
        {
            get {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetDouble("Rz");
                if (res != 0)
                    return res;
                return rz; }
            internal set
            {
                rz = value;
                OnPropertyChanged();
            }
        }
        public double Rux
        {
            get {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetDouble("Rux");
                if (res != 0)
                    return res;
                return rux; }
            internal set
            {
                rux = value;
                OnPropertyChanged();
            }
        }
        public double Ruy
        {
            get {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetDouble("Ruy");
                if (res != 0)
                    return res;
                return ruy; }
            internal set
            {
                ruy = value;
                OnPropertyChanged();
            }
        }
        public double Ruz
        {
            get {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetDouble("Ruz");
                if (res != 0)
                    return res;
                return ruz; }
            internal set
            {
                ruz = value;
                OnPropertyChanged();
            }
        }
        public string ForceMark
        {
            get {
                var res = _hierarchicObjectInTekla.HierarchicObjectGetAttr("ForceMark");
                if (res.Length > 0)
                    return res;
                return forceMark; }
            set
            {
                forceMark = value;
                OnPropertyChanged();
            }
        }

        #endregion

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

        #region Валидация
        public string Error
        {
            get
            {
                return null;
            }
        }


        public string this[string name]
        {
            get
            {
                string result = null;

                //if (name == "Material")
                //{
                //    if ((Material != null) & (!MaterialIsAllowed()))
                //    {
                //        result = "Check material name";
                //    }
                //}


                return result;
            }
        }





        #endregion
    }
}