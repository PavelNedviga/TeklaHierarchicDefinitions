using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TeklaHierarchicDefinitions.TeklaAPIUtils;

namespace TeklaHierarchicDefinitions
{
    public class FoundationGroup : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Внутренние параметры объекта

        private HierarchicObjectInTekla _hierarchicObjectInTekla;        
        #endregion

        #region Конструктор
        internal FoundationGroup(string foundationGroupName)
        {
            _hierarchicObjectInTekla = new HierarchicObjectInTekla();
            
        }
        #endregion

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region Свойства
        public string ForceMark
        {
            get { return _hierarchicObjectInTekla.Name; }
            private set
            {
                _hierarchicObjectInTekla.Name = value;

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