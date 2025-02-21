﻿using System.Linq;
using Tekla.Structures.Model;
using TeklaHierarchicDefinitions.TeklaAPIUtils;

namespace TeklaHierarchicDefinitions.Models
{
    internal class BuildingFragmentUtils
    {
        /// <summary>
        /// Статический класс для подготовки модели данных и его отображения в интерфейсе.
        /// </summary>
        public static MyObservableCollection<BuildingFragment> GetBuildingFragmentsWithHierarchicDefinitionFatherName(string fatherName)
        {
            MyObservableCollection<BuildingFragment> buildingFragments = new MyObservableCollection<BuildingFragment>();
            var aHD = TeklaDB.GetAllHierarchicDefinitions();
            var allBuildingFragments = aHD.Where(t => t.Name.Equals(TeklaDB.hierarchicDefinitionFoundationListName)).FirstOrDefault();
            if (allBuildingFragments != null)
            {
                foreach (HierarchicDefinition hdit in allBuildingFragments.HierarchicChildren)
                {
                    if (hdit is HierarchicDefinition)
                    {
                        BuildingFragment rowInBuildingFragments = new BuildingFragment(hdit);
                        buildingFragments.Add(rowInBuildingFragments);
                    }
                }
            }
            return buildingFragments;
        }
    }
}
