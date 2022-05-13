using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklaHierarchicDefinitions.TeklaAPIUtils;
using TeklaHierarchicDefinitions.ViewModels;
using Tekla.Structures.Model;

namespace TeklaHierarchicDefinitions.Models
{
    internal class BuildingFragmentUtils
    {
        /// <summary>
        /// Статический класс для подготовки модели данных и его отображения в интерфейсе.
        /// </summary>
        public static MyObservableCollection<BuildingFragment> GetBuildingFragmentsWithHierarchicDefinitionFatherName(string fatherName)
        {
            MyObservableCollection<BuildingFragment> billOfElements = new MyObservableCollection<BuildingFragment>();
            var aHD = TeklaDB.GetAllHierarchicDefinitions();
            var allBuildingFragments = aHD.Where(t => t.Name.Equals(TeklaDB.hierarchicDefinitionFoundationListName)).FirstOrDefault();
            if (allBuildingFragments != null)
            {
                foreach (HierarchicDefinition hdit in allBuildingFragments.HierarchicChildren)
                {
                    if (hdit is HierarchicDefinition)
                    {
                        BuildingFragment rowInBuildingFragments = new BuildingFragment(hdit);
                        billOfElements.Add(rowInBuildingFragments);
                    }
                }
            }
            return billOfElements;
        }
    }
}
