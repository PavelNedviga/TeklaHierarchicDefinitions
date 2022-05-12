using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tekla.Structures.Model;
using TeklaHierarchicDefinitions.TeklaAPIUtils;

namespace TeklaHierarchicDefinitions.ViewModels
{
    internal class BuildingFragment : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Внутренние параметры объекта

        private HierarchicDefinition _hierarchicDefinitionInTekla;
        #endregion

        #region Конструктор
        internal BuildingFragment(string buildingFragmentMark)
        {
            var mainHDForFragments = TeklaDB.GetHierarchicDefinitionWithName(TeklaDB.hierarchicDefinitionFoundationListName);
            _hierarchicDefinitionInTekla = TeklaDB.CreateHierarchicDefinitionWithName(buildingFragmentMark, mainHDForFragments);
        }
        #endregion

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region Свойства
        public string BuildingFragmentMark
        {
            get { return _hierarchicDefinitionInTekla.Name; }
            private set
            {
                _hierarchicDefinitionInTekla.Name = value;
                OnPropertyChanged();
            }
        }

        public string FoundationMark
        {
            get { return _hierarchicDefinitionInTekla.Name; }
            private set
            {
                _hierarchicDefinitionInTekla.Name = value;
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