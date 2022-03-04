using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklaHierarchicDefinitions.TeklaAPIUtils;

namespace TeklaHierarchicDefinitions.Models
{
    /// <summary>
    /// Методы создания - удаления Row data
    /// </summary>
    public static class BillOfElementsUtils
    {
        /// <summary>
        /// Статический класс для подготовки модели данных и его отображения в интерфейсе.
        /// </summary>
        public static MyObservableCollection<BillOfElements> GetHierarchicObjectsWithHierarchicDefinitionName(List<HierarchicObjectInTekla> hierarchicObjectsInTeklas)
        {
            MyObservableCollection<BillOfElements> billOfElements = new MyObservableCollection<BillOfElements>();
            foreach (HierarchicObjectInTekla hoit in hierarchicObjectsInTeklas)
            {
                BillOfElements rowInBillOfElelments = new BillOfElements(hoit, billOfElements);
                billOfElements.Add(rowInBillOfElelments);
            }            
            return billOfElements;
        }

        //public static BillOfElements CreateRowData()
        //{
        //    throw new NotImplementedException();
        //    //BillOfElements data = new BillOfElements();
        //    //return data;
        //}

        //public static void AddRowData(MyObservableCollection<BillOfElements> targetCollection, BillOfElements rowData)
        //{
        //    targetCollection.Add(rowData);
        //    throw new NotImplementedException();
        //}

        //public static void RemoveRowData(MyObservableCollection<BillOfElements> targetCollection, int index)
        //{
        //    targetCollection.RemoveAt(index);
        //    throw new NotImplementedException();
        //}
    }
}
