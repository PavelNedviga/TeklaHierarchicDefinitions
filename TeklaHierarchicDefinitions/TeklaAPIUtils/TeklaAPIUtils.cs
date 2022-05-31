using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using Tekla.Structures.Catalogs;
using Tekla.Structures;
using System.Collections.ObjectModel;
using System.Collections;
using ctlg = Tekla.Structures.Catalogs;
using NLog;
using System.Windows;
using System.Text.RegularExpressions;

namespace TeklaHierarchicDefinitions.TeklaAPIUtils
{

    /// <summary>
    /// Статический класс для взаимодействия с Tekla на базе API
    /// </summary>
    static class TeklaDB
    {
        #region Статические поля
        public static Model model = new Model();
        internal static string hierarchicDefinitionElementListName = "Element_list";

        internal static HierarchicDefinition CreateHierarchicDefinitionWithName(string hierarchicDefinitionName, HierarchicDefinition mainHD)
        {
            var hierarchicDefinition = new HierarchicDefinition();
            hierarchicDefinition.Name = hierarchicDefinitionName;
            hierarchicDefinition.HierarchyType = HierarchicDefinitionTypeEnum.DOT_HIERARCHIC_CUSTOM_TYPE;
            hierarchicDefinition.Father = mainHD;
            hierarchicDefinition.Insert();
            return hierarchicDefinition;          
        }

        internal static string hierarchicDefinitionFoundationListName = "Foundation_List";
        internal static ModelObjectSelector objectSelector =  model.GetModelObjectSelector();
        public static Tekla.Structures.Model.UI.ModelObjectSelector modelObjectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        internal static CatalogHandler ch = new CatalogHandler();
        #endregion

        #region Выведение списков иерархических объектов-определений

        private static ObservableCollection<HierarchicObject> GetAllHierarchicObjects()
        {
            ModelObjectEnumerator modelObjectEnumerator = model.GetModelObjectSelector().GetAllObjectsWithType(new Type[] { typeof(HierarchicObject) });

            ObservableCollection<HierarchicObject> hierarchicObjects = new ObservableCollection<HierarchicObject>();

            while (modelObjectEnumerator.MoveNext())
            {
                HierarchicObject hierarchicObject = modelObjectEnumerator.Current as HierarchicObject;
                if (hierarchicObject != null)
                    hierarchicObjects.Add(hierarchicObject);
            }

            return hierarchicObjects;
        }

        public static ObservableCollection<HierarchicDefinition> GetAllHierarchicDefinitions()
        {
            ModelObjectEnumerator modelObjectEnumerator = model.GetModelObjectSelector().GetAllObjectsWithType(new Type[] { typeof(HierarchicDefinition) });

            ObservableCollection<HierarchicDefinition> hierarchicDefinitions = new ObservableCollection<HierarchicDefinition>();

            while (modelObjectEnumerator.MoveNext())
            {
                HierarchicDefinition hierarchicDefinition = modelObjectEnumerator.Current as HierarchicDefinition;
                if (hierarchicDefinition != null)
                    hierarchicDefinitions.Add(hierarchicDefinition);
            }

            return hierarchicDefinitions;
        }

        public static ModelObject GetHierarchicDefinition(Identifier realObjectID)
        {
            var realObject = model.SelectModelObject(realObjectID);
            return realObject;
        }

        public static List<HierarchicObjectInTekla> GetHierarchicObjectsWithHierarchicDefinitionName(string hierarchicDefinitionName)
        {
            
        List<HierarchicObject> ho = new List<HierarchicObject>();
            List<HierarchicDefinition> hd = new List<HierarchicDefinition>();
            ModelObjectEnumerator defHierarchy = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.HIERARCHIC_DEFINITION);
            List<ModelObject> molho = new List<ModelObject>();

            foreach (ModelObject ne in defHierarchy)
            {

                if (ne is HierarchicDefinition)
                {
                    var newhd = (ne as HierarchicDefinition);
                    hd.Add(ne as HierarchicDefinition);
                }
            }

            ModelObjectEnumerator objHierarchy = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.HIERARCHIC_OBJECT);
            foreach (ModelObject ne in objHierarchy)
            {
                if (ne is HierarchicObject)
                {
                    var pos = ne as HierarchicObject;
                    if(pos.Definition != null)
                    {
                        ho.Add(pos);
                        logger.Trace("Добавлен в коллекцию " + pos.Name + " с идентификатором " + pos.Identifier.ToString() + " и иерархическим определением " + pos.Definition.Name + " : " + pos.Definition.Identifier);
                    }

                }
            }

            var hierarchicDefinitions = hd.Where(t => t.Name.Equals(hierarchicDefinitionName)).ToList();
            Identifier hierarchicDefinitionIdentifier;

            if (hierarchicDefinitions.Count == 0)
            {
                HierarchicDefinition hierDef = new HierarchicDefinition();
                hierDef.Name = hierarchicDefinitionName;
                hierDef.Insert();
                hierarchicDefinitionIdentifier = hierDef.Identifier;
            }
            else
            {
                hierarchicDefinitionIdentifier = hierarchicDefinitions.Select(k => k.Identifier).FirstOrDefault();
            }

            
            logger.Trace("Иерархическое определение, которое ищем: " + hierarchicDefinitionIdentifier);
            var hierarchicObjects = ho.Where(t => hierarchicDefinitionIdentifier.Equals(t.Definition.Identifier)).ToList();
            

            List<HierarchicObjectInTekla> array = new List<HierarchicObjectInTekla>();
            foreach (var hobj in hierarchicObjects)
            {
                var hdef = hierarchicDefinitions.Where(t => t.Identifier.Equals(hobj.Definition.Identifier)).FirstOrDefault();
                array.Add(new HierarchicObjectInTekla(hobj, hdef));
            }
            return array;
        }

        public static List<HierarchicObjectInTekla> GetAllHierarchicObjectsInTekla()
        {
            ModelObjectEnumerator modelObjectEnumerator = model.GetModelObjectSelector().GetAllObjectsWithType(new Type[] { typeof(HierarchicObject) });

            List<HierarchicObject> hierarchicObjects = new List<HierarchicObject>();

            while (modelObjectEnumerator.MoveNext())
            {
                HierarchicObject hierarchicObject = modelObjectEnumerator.Current as HierarchicObject;
                if (hierarchicObject != null)
                    hierarchicObjects.Add(hierarchicObject);
            }

            ModelObjectEnumerator defHierarchy = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.HIERARCHIC_DEFINITION);

            List<ModelObject> molho = new List<ModelObject>();
            List<HierarchicDefinition> hd = new List<HierarchicDefinition>();
            foreach (ModelObject ne in defHierarchy)
            {

                if (ne is HierarchicDefinition)
                {
                    var newhd = (ne as HierarchicDefinition);
                    hd.Add(ne as HierarchicDefinition);
                }
            }


            var hierarchicDefinitions = hd;
            List<HierarchicObjectInTekla> array = new List<HierarchicObjectInTekla>();
            foreach (var hobj in hierarchicObjects)
            {
                var hdef = hd.Where(t => t.Identifier.Equals(hobj.Definition.Identifier)).FirstOrDefault();
                array.Add(new HierarchicObjectInTekla(hobj, hdef));
            }
            return array;

        }

        #endregion

        #region Операции с иерархическими объектами

        public static HierarchicObject CreateHierarchicObject(HierarchicDefinition hierarchicDefinition)
        {
            HierarchicObject hierarchicObject = new HierarchicObject();            

            hierarchicObject.Name = "New_HO";
            hierarchicObject.Definition = hierarchicDefinition;
            hierarchicObject.Insert();

            model.CommitChanges();

            return hierarchicObject;
        }

        public static HierarchicObject CreateHierarchicObject(string name, HierarchicDefinition hierarchicDefinition)
        {
            HierarchicObject hierarchicObject = new HierarchicObject();

            hierarchicObject.Name = name;
            hierarchicObject.Definition = hierarchicDefinition;
            return hierarchicObject;
        }


        internal static HierarchicObject CreateHierarchicObject(HierarchicDefinition hierarchicDefinition, HierarchicObject receivedHierarchicObject)
        {
            HierarchicObject hierarchicObject = new HierarchicObject();
            
            hierarchicObject.Name = "New_HO";
            hierarchicObject.Definition = hierarchicDefinition;
            hierarchicObject.Father = receivedHierarchicObject;
            hierarchicObject.Insert();
            //var arr = new ArrayList();
            //arr.AddRange(receivedHierarchicObject.HierarchicChildren);
            //arr.Add(hierarchicObject);
            //receivedHierarchicObject.HierarchicChildren = arr;

            //receivedHierarchicObject.HierarchicChildren.Add(hierarchicObject);
            //receivedHierarchicObject.Modify();

            //var rho = new HierarchicObject();
            //rho.Identifier = receivedHierarchicObject.Identifier;
            //rho.Select();
            //rho.HierarchicChildren.Add(hierarchicObject);
            //rho.Modify();
            
            model.CommitChanges();
            return hierarchicObject;
        }

        public static bool DeleteHierarchicObject(HierarchicObject hierarchicObject)
        {
            bool result = hierarchicObject.Delete();

            return result;
        }

        public static bool AddModelObjectsToHierarchicObject(ArrayList modelObjects, HierarchicObject hierarchicObject)
        {
            bool result = hierarchicObject.AddObjects(modelObjects);

            return result;
        }

        public static bool AddModelObjectsToHierarchicObject(ModelObject modelObject, HierarchicObject hierarchicObject)
        {
            ArrayList modelObjects = new ArrayList() { modelObject };
            bool result = AddModelObjectsToHierarchicObject(modelObjects, hierarchicObject);

            return result;
        }

        public static bool RemoveModelObjectsFromHierarchicObject(ArrayList modelObjects, HierarchicObject hierarchicObject)
        {
            bool result = hierarchicObject.RemoveObjects(modelObjects);

            return result;
        }

        public static bool RemoveModelObjectsFromHierarchicObject(ModelObject modelObject, HierarchicObject hierarchicObject)
        {
            ArrayList modelObjects = new ArrayList() { modelObject };
            bool result = RemoveModelObjectsFromHierarchicObject(modelObjects, hierarchicObject);

            return result;
        }

        internal static bool RemoveSelectedModedlObjectsFromHierarchicObject()
        {
            var selectedObjects = GetSelectedModelObjects();
            var hierarchicObjectList = GetHierarchicObjectsWithHierarchicDefinitionName(hierarchicDefinitionElementListName);
            bool res = false;
            foreach (var ho in hierarchicObjectList)
            {
                res = (res || ho.HierarchicObject.RemoveObjects(selectedObjects));
            }

            return res;

        }

        #endregion

        #region Операции с иерархическими определениями
        public static bool DeleteHierarchicDefinition(HierarchicDefinition hierarchicDefinition)
        {
            string name = hierarchicDefinition.Name;
            if (hierarchicDefinition.Delete())
            {
                model.CommitChanges("removed " + name);
                return true;
            }
            return false;

        }


        /// <summary>
        /// Находит или создает новое определение
        /// </summary>
        /// <returns></returns>
        public static HierarchicDefinition GetHierarchicDefinitionWithName(string name, HierarchicDefinition hierarchicDefinition)
        {
               throw new NotImplementedException();
        }

        /// <summary>
        /// Изменяет данные одного иерархического объекта
        /// </summary>
        /// <returns></returns>
        public static bool WriteChangestoHierarchicDefinition()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Операции с выделением объектов
        public static bool ModelHasSelectedObjects()
        {
            Tekla.Structures.Model.UI.ModelObjectSelector modelObjectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            bool result = modelObjectSelector.GetSelectedObjects().GetSize() > 0;
            return result;
        }


        public static bool SelectObjectsInModelView(ArrayList partList)
        {
            return modelObjectSelector.Select(partList);
        }

        public static ArrayList ModelGetSelectedComponents()
        {
            ArrayList components = new ArrayList();
            foreach (var mo in modelObjectSelector.GetSelectedObjects())
            {                
                if (mo is Detail)
                {
                    components.Add(mo as Detail);
                }
            }            
            return components;
        }

        public static ArrayList GetSelectedModelObjects()
        {
            Tekla.Structures.Model.UI.ModelObjectSelector modelObjectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
            var selectedEnum = modelObjectSelector.GetSelectedObjects();
            ArrayList arrayList = new ArrayList();
            foreach (ModelObject modelObject in selectedEnum)
            {
                if(modelObject is Part)
                    arrayList.Add(modelObject as Part);
            }
            return arrayList;
        }

        /// <summary>
        /// Находит объекты в модели, не принадлежащие к иерархическим.
        /// </summary>
        /// <param name="hierarchicObjects">Собственно список из иерархических объектов для обозначенной цели</param>
        /// <returns></returns>
        public static ArrayList GetUnboundModelObjects(Array hierarchicObjects)
        {
            //List<ModelObjectEnumerator> moes = new List<ModelObjectEnumerator>();
            //moes.Add(objectSelector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM));
            //moes.Add(objectSelector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONTOURPLATE));
            //moes.Add(objectSelector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BENT_PLATE));
            //moes.Add(objectSelector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.BREP));
            //moes.Add(objectSelector.GetAllObjectsWithType(ModelObject.ModelObjectEnum.LOFTED_PLATE));

            // Ищем все прикрепленные объекты
            List<ModelObject> listOfRuledByHierarchicObjects = new List<ModelObject>();
            foreach(var @object in hierarchicObjects)
            {
                var hierarchicObject = @object as HierarchicObject;
                var moe = hierarchicObject.GetChildren();
                foreach(var mObject in moe)
                {
                    listOfRuledByHierarchicObjects.Add(mObject as ModelObject);
                }
            }
            listOfRuledByHierarchicObjects = listOfRuledByHierarchicObjects.Distinct().ToList();

            // Исключаем из всех объектов прикрепленные
            var selectedEnum = objectSelector.GetEnumerator();
            ArrayList arrayList = new ArrayList();
            foreach (ModelObject modelObject in selectedEnum)
            {
                if (modelObject is Part && !listOfRuledByHierarchicObjects.Contains(modelObject))
                    arrayList.Add(modelObject);
            }

            return arrayList;
        }

        /// <summary>
        /// Прикрепляет детали к иерархическому объекту, попутно удаляя их из уже имеющихся прикреплений
        /// </summary>
        /// <param name="hierarchicObject"> Иерархический объект </param>
        /// <returns></returns>
        internal static bool AttachSelectedModedlObjects(HierarchicObject hierarchicObject)
        {
            RemoveSelectedModedlObjectsFromHierarchicObject();
            bool res = hierarchicObject.AddObjects(GetSelectedModelObjects());
            return hierarchicObject.Modify();
        }

        internal static bool AttachModedlObjects(HierarchicObject hierarchicObject, List<ModelObject> modelObjects)
        {
            var mol = new ArrayList(modelObjects);
            bool res = hierarchicObject.AddObjects( mol);
            bool ggg = hierarchicObject.Modify();
            var gg = hierarchicObject.GetChildren();
            return ggg;

        }

        internal static bool AttachSelectedDetails(HierarchicObject hierarchicObject)
        {
            RemoveSelectedModedlObjectsFromHierarchicObject();
            var modelObjects = ModelGetSelectedComponents();
            bool res = hierarchicObject.AddObjects(modelObjects);
            foreach (var modelObject in modelObjects)
            {
                Detail detail = modelObject as Detail;
                if (detail != null)
                {
                    detail.Code = hierarchicObject.Name;
                    hierarchicObject.AddObjects(new ArrayList() { detail.GetPrimaryObject() });
                    detail.Modify();
                }
            }
            return hierarchicObject.Modify();
        }

        internal static bool RemoveSelectedDetails(HierarchicObject hierarchicObject)
        {
            var modelObjects = ModelGetSelectedComponents();
            var mo = new ArrayList();
            foreach (var modelObject in modelObjects)
            {
                Detail detail = modelObject as Detail;
                if (detail != null)
                {
                    detail.Code = "";
                    detail.Modify();
                }
                mo.Add(detail.GetPrimaryObject());
            }
            hierarchicObject.RemoveObjects(mo);
            return hierarchicObject.RemoveObjects(modelObjects);
        }


        internal static bool RemoveSelectedModedlObjects(HierarchicObject hierarchicObject)
        {
            var modelObjects = GetSelectedModelObjects();
            foreach(var modelObject in modelObjects)
            {
                Part part = modelObject as Part;
                if (part != null)
                {
                    part.Class = "1";
                    part.AssemblyNumber.Prefix = "Б";
                    part.SetUserProperty("Album", "");
                    part.SetUserProperty("PRELIM_MARK", "");
                    part.Modify();
                }
            }
            return hierarchicObject.RemoveObjects(modelObjects);
        }
        #endregion

        #region Установка свойств в объектах
        public static string GetPropertyStr(ModelObject @object, string prop)
        {
            string result = string.Empty;
            if (@object.GetUserProperty(prop, ref result))
                return result;
            return "";
        }

        public static bool SetPropertyStr(ModelObject @object, string prop, string val)
        {
            //HierarchicObject ho = @object as HierarchicObject;
            bool res = @object.SetUserProperty(prop, val);

            if (res) {
                @object.Modify();
                model.CommitChanges();
            }

            return res;
        }

        public static bool SetPropertyInt(ModelObject @object, string prop, int val)
        {
            //HierarchicObject ho = @object as HierarchicObject;
            bool res = @object.SetUserProperty(prop, val);

            if (res)
            {
                @object.Modify();
            }

            return res;
        }

        internal static bool SetProfile(Part part, string profile)
        {
            bool result = false;
            part.Profile.ProfileString = profile;
            result = part.Modify();
            return result;
        }

        internal static bool SetMaterial(Part part, string material)
        {
            bool result = false;
            part.Material.MaterialString = material;
            result = part.Modify();
            return result;
        }

        internal static bool SetPrefix(Part part, string profile)
        {
            bool result = false;
            part.AssemblyNumber.Prefix = profile;
            result = part.Modify();
            return result;
        }

        internal static bool InheritPropsFromHierarchicObjectToSelectedParts(
            HierarchicObject hierarchicObject, 
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
            string notes,
            int isSimple,
            int emptyRowsNumber, 
            int crossSectionOnOtherList, 
            string classificator, 
            string album)
        {
            bool res = false;
            ArrayList arrayList = GetSelectedModelObjects();

            foreach (var @object in arrayList)
            {
                Part part = @object as Part;
                if (part != null)
                {
                    part.AssemblyNumber.Prefix = mark;
                    part.Profile.ProfileString = profile;
                    part.Class = classificator;
                    part.SetUserProperty("PRELIM_MARK", position);

                    string moment;
                    if (m == m_end)
                        moment = m;
                    else
                        moment = m + "/" + m_end;
                    part.SetUserProperty("moment_M", moment);
                    double result;
                    Double.TryParse(m, out result);
                    part.SetUserProperty("momentY1", result * 1000);
                    Double.TryParse(m_end, out result);
                    part.SetUserProperty("momentY2", result * 1000);

                    part.SetUserProperty("START_MOMENT_CONN", startMomentConnection);
                    part.SetUserProperty("END_MOMENT_CONN", endMomentConnection);
                    part.SetUserProperty("START_FRICT_CONN", startFrictionConnection);
                    part.SetUserProperty("END_FRICT_CONN", endFrictionConnection);

                    part.SetUserProperty("usilie_N", n_summary);
                    Double.TryParse(n, out result);
                    part.SetUserProperty("axial1", result * 1000);
                    Double.TryParse(n_end, out result);
                    part.SetUserProperty("axial2", result * 1000);
                    Double.TryParse(n_start_min, out result);
                    part.SetUserProperty("axialcomp1", result * 1000);
                    Double.TryParse(n_end_min, out result);
                    part.SetUserProperty("axialcomp2", result * 1000);

                    string shear;
                    if (q == q_end)
                        shear = q;
                    else
                        shear = q + "/" + q_end;
                    part.SetUserProperty("reakciya_A", shear);
                    Double.TryParse(q, out result);
                    part.SetUserProperty("shearZ1", result * 1000);
                    Double.TryParse(q_end, out result);
                    part.SetUserProperty("shearZ2", result * 1000);

                    part.Material.MaterialString = material;
                    part.SetUserProperty("prim_vedomost", notes);

                    part.SetUserProperty("slozhnoe_sechenie", isSimple);
                    part.SetUserProperty("pustykh_strok", emptyRowsNumber);
                    part.SetUserProperty("ru_slozh_sech_list", crossSectionOnOtherList);
                    part.SetUserProperty("Album", album);


                    res = part.Modify();
                }
            }
            model.CommitChanges("Parts updated");
            return res;
        }

        internal static bool InheritPropsFromHierarchicObjectToAssociatedParts(
            HierarchicObject hierarchicObject, 
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
            string notes,
            int isSimple,
            int emptyRowsNumber, 
            int crossSectionOnOtherList, 
            string classificator, 
            string album)
        {
            bool res = false;

            var moe = hierarchicObject.GetChildren();

            while (moe.MoveNext())
            {
                Part part = moe.Current as Part;
                if (part != null)
                {
                    part.AssemblyNumber.Prefix = mark;
                    part.Profile.ProfileString = profile;
                    part.Class = classificator;
                    part.SetUserProperty("PRELIM_MARK", position);

                    string moment;
                    if (m == m_end)
                        moment = m;
                    else
                        moment = m + "/" + m_end;
                    part.SetUserProperty("moment_M", moment);
                    double result;
                    Double.TryParse(m,out result);
                    part.SetUserProperty("momentY1", result * 1000);
                    Double.TryParse(m_end, out result);
                    part.SetUserProperty("momentY2", result * 1000);


                    part.SetUserProperty("START_MOMENT_CONN", startMomentConnection);
                    part.SetUserProperty("END_MOMENT_CONN", endMomentConnection);
                    part.SetUserProperty("START_FRICT_CONN", startFrictionConnection);
                    part.SetUserProperty("END_FRICT_CONN", endFrictionConnection);

                    part.SetUserProperty("usilie_N", n_summary);
                    Double.TryParse(n, out result);
                    part.SetUserProperty("axial1", result * 1000);
                    Double.TryParse(n_end, out result);
                    part.SetUserProperty("axial2", result * 1000);
                    Double.TryParse(n_start_min, out result);
                    part.SetUserProperty("axialcomp1", result * 1000);
                    Double.TryParse(n_end_min, out result);
                    part.SetUserProperty("axialcomp2", result * 1000);


                    string shear;
                    if (q == q_end)
                        shear = q;
                    else
                        shear = q + "/" + q_end;
                    part.SetUserProperty("reakciya_A", shear);

                    Double.TryParse(q, out result);
                    part.SetUserProperty("shearZ1", result * 1000);
                    Double.TryParse(q_end, out result);
                    part.SetUserProperty("shearZ2", result * 1000);

                    part.Material.MaterialString = material;
                    part.SetUserProperty("prim_vedomost", notes);

                    part.SetUserProperty("slozhnoe_sechenie", isSimple);
                    part.SetUserProperty("pustykh_strok", emptyRowsNumber);
                    part.SetUserProperty("ru_slozh_sech_list", crossSectionOnOtherList);
                    part.SetUserProperty("Album", album);

                    res = part.Modify();
                }
            }

            return res;
        }

        #endregion

        #region Работа с каталогами

        internal static bool MaterialIsAllowed(string material)
        {
            MaterialItem materialItem = new MaterialItem();
            return materialItem.Select(material);
        }

        internal static bool ProfileIsAllowed(string profile)
        {
            bool result = false;
            ParametricProfileItem parametricProfileItem = new ParametricProfileItem();
            LibraryProfileItem libraryProfileItem = new LibraryProfileItem();
            Beam beam = new Beam();

            result = libraryProfileItem.Select(profile) || parametricProfileItem.Select(profile);
            return result;
        }

        internal static HierarchicDefinition GetHierarchicDefinitionWithName(string hierarchicDefinitionName)
        {        

            List<HierarchicDefinition> hd = new List<HierarchicDefinition>();
            ModelObjectEnumerator defHierarchy = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.HIERARCHIC_DEFINITION);

            foreach (ModelObject ne in defHierarchy)
            {

                if (ne is HierarchicDefinition)
                {
                    hd.Add(ne as HierarchicDefinition);
                }
            }

            HierarchicDefinition hierarchicDefinition = hd.Where(t => t.Name.Equals(hierarchicDefinitionName)).FirstOrDefault();
            if (hierarchicDefinition != null)
                return hierarchicDefinition;

            hierarchicDefinition = new HierarchicDefinition();
            hierarchicDefinition.Name = hierarchicDefinitionName;
            hierarchicDefinition.HierarchyType = HierarchicDefinitionTypeEnum.DOT_HIERARCHIC_CUSTOM_TYPE;
            hierarchicDefinition.Insert();
            return hierarchicDefinition;
        }

        //internal static HierarchicDefinition GetHierarchicDefinitionWithType(string hierarchicDefinitionName)
        //{

        //    List<HierarchicDefinition> hd = new List<HierarchicDefinition>();
        //    ModelObjectEnumerator defHierarchy = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.HIERARCHIC_DEFINITION);
        //    foreach (ModelObject ne in defHierarchy)
        //    {

        //        if (ne is HierarchicDefinition)
        //        {
        //            hd.Add(ne as HierarchicDefinition);
        //        }
        //    }
        //    HierarchicDefinition hierarchicDefinition = hd.Where(t => t.Name.Equals(hierarchicDefinitionName)).FirstOrDefault();            
        //    if (hierarchicDefinition != null)
        //        return hierarchicDefinition;

        //    hierarchicDefinition = new HierarchicDefinition();
        //    hierarchicDefinition.Name = hierarchicDefinitionName;
        //    hierarchicDefinition.HierarchyType = HierarchicDefinitionTypeEnum.DOT_HIERARCHIC_CUSTOM_TYPE;
        //    hierarchicDefinition.Insert();
        //    return hierarchicDefinition;
        //}

        internal static string GetSketch(string profileString)
        {

            LibraryProfileItem LibraryProfileItem1 = new LibraryProfileItem();
            LibraryProfileItem1.Select(profileString);
            var res = LibraryProfileItem1.aProfileItemUserParameters;
            foreach(var par in res)
            {
                var profileItemParameter = par as ProfileItemParameter;
                if (profileItemParameter.Property == "SYMBOL_NAME")
                {
                    string extractPattern = @"(?i)([A-Z0-9._ %+-]+@[0-9]{1,3})";
                    var extractedValue = Regex.Matches(profileItemParameter.StringValue, extractPattern)
                        .Cast<Match>()
                        .Select(x => x.Groups[1].Value);
                    if(extractedValue.FirstOrDefault() != null)
                    {
                        string pattern = @"@";
                        string[] substrings = Regex.Split(extractedValue.FirstOrDefault(), pattern);    // Split on hyphens
                        var result = substrings[0] + "@" + substrings[1]; //".sym@" + 
                        return result;
                    }
                    return "";
                }

            }
            return "";
        }

        internal static string GetProfileForSpec(string profileString)
        {

            LibraryProfileItem LibraryProfileItem1 = new LibraryProfileItem();
            LibraryProfileItem1.Select(profileString);
            var res = LibraryProfileItem1.aProfileItemUserParameters;
            foreach (var par in res)
            {
                var profileItemParameter = par as ProfileItemParameter;
                if (profileItemParameter.Property == "TPL_NAME")
                    return profileItemParameter.StringValue;
            }
            return "not found in library";
        }

        internal static bool SetClass(Part part, string classficator)
        {
            bool result = false;
            part.Class = classficator;
            result = part.Modify();
            return result;
        }

        #endregion
    }
}
