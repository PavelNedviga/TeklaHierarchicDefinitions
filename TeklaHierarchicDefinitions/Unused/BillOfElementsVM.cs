using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TeklaHierarchicDefinitions.Models;


//namespace TeklaHierarchicDefinitions.ViewsModels
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class ViewModel : BaseViewModel
//    {
//        private MyObservableCollection<BillOfElements> rowDatas = new MyObservableCollection<BillOfElements>();
        
//        /// <summary>
//        /// Свойства в Observerable collection
//        /// </summary>
//        public MyObservableCollection<BillOfElements> RowDatas
//        {
//            get { return rowDatas; }
//            set
//            {
//                rowDatas = value;
//                OnPropertyChanged("RowDatas");
//            }
//        }

//        /// <summary>
//        /// Реализация работы кнопки
//        /// </summary>
//        public DelegateCommand ClickAdd
//        {
//            get
//            {
//                return new DelegateCommand((obj) =>
//                {
//                    BillOfElementsUtils.AddRowData(this.rowDatas, BillOfElementsUtils.CreateRowData()); // реализуемый кнопкой метод
//                }, (obj) =>
//                {
//                    return obj == null ? true : ((DataGrid)obj).Items.Count < 5; // проверка на число строк и запрет добавления при условии ....
//                });
//            }
//        }

//        public DelegateCommand ClickDelete
//        {
//            get
//            {
//                return new DelegateCommand((obj) =>
//                {
//                    BillOfElementsUtils.RemoveRowData(this.rowDatas, ((DataGrid)obj).SelectedIndex);
//                }, (obj) =>   // условие, при котором действие вообще можно выполнить
//                {
//                    return obj == null ? true : ((DataGrid)obj).SelectedIndex != -1; // удалять можно, если что-то выделено
//                });
//            }
//        }

//        /// <summary>
//        /// Просто закрытие окна
//        /// </summary>
//        public DelegateCommand ClickClose
//        {
//            get
//            {
//                return new DelegateCommand((obj) =>
//                {
//                    ((Window)obj).Close();
//                });
//            }
//        }
//    }
//}
