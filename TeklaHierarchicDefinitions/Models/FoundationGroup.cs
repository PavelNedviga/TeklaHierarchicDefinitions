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
        internal FoundationGroup(string foundationGroupName)
        {
            _hierarchicObjectInTekla = new HierarchicObjectInTekla();    
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
        public string Joint
        {
            get => joint;
            internal set
            {
                joint = value;
                OnPropertyChanged();
            }
        }
        public string St 
        { 
            get=>st; 
            internal set 
            { 
                st = value;
                OnPropertyChanged();
            } 
        }
        public string Cr
        {
            get => cr;
            internal set
            {
                cr = value;
                OnPropertyChanged();
            }
        }
        public string Gr
        {
            get => gr;
            internal set
            {
                gr = value;
                OnPropertyChanged();
            }
        }
        public string Sp
        {
            get => sp;
            internal set
            {
                sp = value;
                OnPropertyChanged();
            }
        }
        public string Crit
        {
            get => crit;
            internal set
            {
                crit = value;
                OnPropertyChanged();
            }
        }
        public double Rx
        {
            get => rx;
            internal set
            {
                rx = value;
                OnPropertyChanged();
            }
        }
        public double Ry
        {
            get => ry;
            internal set
            {
                ry = value;
                OnPropertyChanged();
            }
        }
        public double Rz
        {
            get => rz;
            internal set
            {
                rz = value;
                OnPropertyChanged();
            }
        }
        public double Rux
        {
            get => rux;
            internal set
            {
                rux = value;
                OnPropertyChanged();
            }
        }
        public double Ruy
        {
            get => ruy;
            internal set
            {
                ruy = value;
                OnPropertyChanged();
            }
        }
        public double Ruz
        {
            get => ruz;
            internal set
            {
                ruz = value;
                OnPropertyChanged();
            }
        }
        public string ForceMark
        {
            get { return loadName; }
            set
            {
                loadName = value;
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
            input.Add(new KeyValuePair<string, string>("Joint", Joint));
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