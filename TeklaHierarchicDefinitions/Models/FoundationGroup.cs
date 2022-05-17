using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Tekla.Structures.Model;
using TeklaHierarchicDefinitions.TeklaAPIUtils;

namespace TeklaHierarchicDefinitions.Models
{
    public class FoundationGroup : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Внутренние параметры объекта        
        private HierarchicObjectInTekla _hierarchicObjectInTekla;
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
        private string loadName;
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
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("JointNumber"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("JointNumber", value);
                OnPropertyChanged();
            }
        }
        public string St 
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("St"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("St", value);
                OnPropertyChanged();
            }
        }
        public string Cr
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Cr"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Cr", value);
                OnPropertyChanged();
            }
        }
        public string Gr
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Gr"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Gr", value);
                OnPropertyChanged();
            }
        }
        public string Sp
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Sp"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Sp", value);
                OnPropertyChanged();
            }
        }
        public string Crit
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("Crit"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Crit", value);
                OnPropertyChanged();
            }
        }
        public double Rx
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetDouble("Rx"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Rx", value);
                OnPropertyChanged();
            }
        }
        public double Ry
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetDouble("Ry"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Ry", value);
                OnPropertyChanged();
            }
        }
        public double Rz
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetDouble("Rz"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Rz", value);
                OnPropertyChanged();
            }
        }
        public double Rux
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetDouble("Rux"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Rux", value);
                OnPropertyChanged();
            }
        }
        public double Ruy
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetDouble("Ruy"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Ruy", value);
                OnPropertyChanged();
            }
        }
        public double Ruz
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetDouble("Ruz"); }
            internal set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("Ruz", value);
                OnPropertyChanged();
            }
        }
        public string ForceMark
        {
            get { return _hierarchicObjectInTekla.HierarchicObjectGetAttr("ForceMark"); }
            set
            {
                _hierarchicObjectInTekla.HierarchicObjectSetAttribute("ForceMark", value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region Методы
        internal void UpdateAndInsert()
        {
            try
            {
                ArrayList input = new ArrayList();
                input.Add(new KeyValuePair<string, string>("BasementMark", BasementMark));
                input.Add(new KeyValuePair<string, string>("Joint", JointNumber));
                input.Add(new KeyValuePair<string, string>("St", St));
                input.Add(new KeyValuePair<string, string>("Cr", Cr));
                input.Add(new KeyValuePair<string, string>("Gr", Gr));
                input.Add(new KeyValuePair<string, string>("Sp", Sp));
                input.Add(new KeyValuePair<string, string>("Crit", Crit));
                input.Add(new KeyValuePair<string, string>("Rx", Rx.ToString()));
                input.Add(new KeyValuePair<string, string>("Ry", Ry.ToString()));
                input.Add(new KeyValuePair<string, string>("Rz", Rz.ToString()));
                input.Add(new KeyValuePair<string, string>("Rux", Rux.ToString()));
                input.Add(new KeyValuePair<string, string>("Ruy", Ruy.ToString()));
                input.Add(new KeyValuePair<string, string>("Ruz", Ruz.ToString()));
                input.Add(new KeyValuePair<string, string>("ForceMark", ForceMark));
                _hierarchicObjectInTekla.HierarchicObjectSetAttributes(input);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error");
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