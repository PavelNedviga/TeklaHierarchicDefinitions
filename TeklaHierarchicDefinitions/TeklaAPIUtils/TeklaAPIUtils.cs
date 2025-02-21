﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls.WebParts;
using Tekla.Structures;
using Tekla.Structures.Catalogs;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using ModelObject = Tekla.Structures.Model.ModelObject;
using Part = Tekla.Structures.Model.Part;

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
        internal static ModelObjectSelector objectSelector = model.GetModelObjectSelector();
        public static Tekla.Structures.Model.UI.ModelObjectSelector modelObjectSelector = new Tekla.Structures.Model.UI.ModelObjectSelector();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        internal static CatalogHandler ch = new CatalogHandler();
        public static DrawingHandler DrawingHandler = new DrawingHandler();
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
                    if (pos.Definition != null)
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

        internal static bool RemoveModedlObjectsFromHierarchicObject(List<Part> parts)
        {
            ArrayList selectedObjects = new ArrayList( parts );
            var hierarchicObjectList = GetHierarchicObjectsWithHierarchicDefinitionName(hierarchicDefinitionElementListName);
            bool res = false;
            foreach (var ho in hierarchicObjectList)
            {
                res = (res || ho.HierarchicObject.RemoveObjects(selectedObjects));
            }

            return res;

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

        /// <summary>
        /// Выделяет похожие объекты
        /// </summary>
        /// <param name="prefix">Префикс сборки</param>
        /// <param name="preliminaryPos">Позиция в составном сечении</param>
        /// <param name="profile">Профиль</param>
        /// <param name="material">Материал</param>
        /// <param name="hierarchicObject">Иерархический объект</param>
        public static void SelectSimilarModelObjects(string prefix, string profile, string material, List<HierarchicObject> hierarchicObjects)
        {
            var modelObjectEnumerator = modelObjectSelector.GetSelectedObjects();
            var selectedArray = new ArrayList();
            var distinctAttachedModelObjects = hierarchicObjects.SelectMany(t =>
            {
                var attObj = new List<ModelObject>();
                var moenum = t.GetChildren();
                foreach (ModelObject modelObj in moenum)
                {
                    attObj.Add(modelObj);
                }
                return attObj;
            }).Select(x => x.Identifier).ToHashSet();
            foreach (ModelObject mo in modelObjectEnumerator)
            {

                if (mo is Part)
                {
                    var part = (Part)mo;
                    //string prelimMark = string.Empty;
                    //part.GetUserProperty("PRELIM_MARK", ref prelimMark);
                    bool result;
                    bool prefixesEquality = false, materialEquality = false, profileEquality = false, checkedIdentifier = false;
                    if (material != null)
                        materialEquality = material == part.Material.MaterialString;
                    else
                        materialEquality = true;
                    if (materialEquality)
                    {
                        if (profile != null)
                            profileEquality = profile == part.Profile.ProfileString;
                        else
                            profileEquality = true;
                        if (profileEquality)
                        {
                            if (prefix != null)
                            {
                                var ass = part.GetAssembly();
                                if (ass != null)
                                    prefixesEquality = ass.AssemblyNumber.Prefix == prefix;
                                else
                                    prefixesEquality = part.AssemblyNumber.Prefix == prefix;
                            }

                            else
                                prefixesEquality = true;
                            if (prefixesEquality)
                            {
                                checkedIdentifier = !distinctAttachedModelObjects.Contains(part.Identifier);
                            }
                        }
                    }
                    result = checkedIdentifier
                    & prefixesEquality
                    //& internalPosEquality
                    & profileEquality
                    & materialEquality;
                    if (result)
                        selectedArray.Add(mo);
                }
            }
            modelObjectSelector.Select(selectedArray);
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
                if (modelObject is Part)
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
            foreach (var @object in hierarchicObjects)
            {
                var hierarchicObject = @object as HierarchicObject;
                var moe = hierarchicObject.GetChildren();
                foreach (var mObject in moe)
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

        public static bool IsModelObjectBoundToEL(ModelObject mo)
        {
            if (mo.GetHierarchicObjects().GetSize() > 0)
            {
                string res = string.Empty;
                var mobjenum = mo.GetHierarchicObjects();
                while (mobjenum.MoveNext())
                {
                    var mobj = mobjenum.Current as HierarchicObject;
                    if (mobj != null)
                    {
                        if (GetHORootHierarchicDefinitionName(mobj.Identifier.ID) == "Element_list")
                            return true;
                    }
                }
            }
            return false;
        }

        internal static string GetHORootHierarchicDefinitionName(int id)
        {
            ModelObject modelObject = model.SelectModelObject(new Tekla.Structures.Identifier(id));
            var ho = (modelObject as HierarchicObject);
            if (ho != null)
            {
                if (ho.Definition != null)
                {
                    HierarchicDefinition fatherHierarchicDefinition = new HierarchicDefinition();
                    fatherHierarchicDefinition.Identifier = ho.Definition.Identifier;
                    if (fatherHierarchicDefinition.Select())
                    {
                        return fatherHierarchicDefinition.Name;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Прикрепляет детали к иерархическому объекту, попутно удаляя их из уже имеющихся прикреплений
        /// </summary>
        /// <param name="hierarchicObject"> Иерархический объект </param>
        /// <returns></returns>
        internal static bool AttachSelectedModelObjects(HierarchicObject hierarchicObject)
        {
            RemoveSelectedModedlObjectsFromHierarchicObject();
            bool res = hierarchicObject.AddObjects(GetSelectedModelObjects());
            return hierarchicObject.Modify();
        }

        internal static bool AttachModelObjects(HierarchicObject hierarchicObject, List<Part> parts)
        {
            RemoveModedlObjectsFromHierarchicObject(parts);
            bool res = hierarchicObject.AddObjects(new ArrayList(parts));
            return hierarchicObject.Modify();
        }

        internal static bool AttachModedlObjects(HierarchicObject hierarchicObject, List<ModelObject> modelObjects)
        {
            var mol = new ArrayList(modelObjects);
            bool res = hierarchicObject.AddObjects(mol);
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

        public static string GetUserProperty(this Part part)
        {
            var profileType = string.Empty;
            part.GetUserProperty("PROFILE_TYPE", ref profileType);
            return profileType;
        }

        internal static bool RemoveSelectedModedlObjects(HierarchicObject hierarchicObject)
        {
            var modelObjects = GetSelectedModelObjects();
            var res = hierarchicObject.RemoveObjects(modelObjects);
            if (res)
            {
                foreach (var modelObject in modelObjects)
                {
                    Part part = modelObject as Part;
                    if (part != null)
                    {
                        part.Class = "1";
                        part.AssemblyNumber.Prefix = "Б";
                        part.SetUserProperty(UDANameConverter.ToTeklaProp("Album"), "");
                        part.SetUserProperty(UDANameConverter.ToTeklaProp("PRELIM_MARK"), "");
                        part.Modify();
                    }
                }
            }
            return res;
        }
        #endregion

        #region Установка свойств в объектах
        public static string GetPropertyStr(ModelObject @object, string prop)
        {
            string result = string.Empty;
            if (@object.GetUserProperty(
                UDANameConverter.ToTeklaProp(prop), 
                ref result))
                return result;
            return "";
        }

        public static bool SetPropertyStr(ModelObject @object, string prop, string val)
        {
            //HierarchicObject ho = @object as HierarchicObject;
            bool res = @object.SetUserProperty(
                UDANameConverter.ToTeklaProp(prop), 
                val);

            if (res)
            {
                @object.Modify();
                //model.CommitChanges();
            }

            return res;
        }

        public static bool SetPropertyInt(ModelObject @object, string prop, int val)
        {
            //HierarchicObject ho = @object as HierarchicObject;

            bool res = @object.SetUserProperty(UDANameConverter.ToTeklaProp(prop), val);

            if (res)
            {
                @object.Modify();
            }

            return res;
        }

        internal static bool SetProfile(Part part, string profile)
        {
            bool result = false;
            string patternPlate = @"^\D*\d+";

            // Instantiate the regular expression object.
            Regex r = new Regex(patternPlate, RegexOptions.IgnoreCase);
            string patternBeam = @"^\D*\d+$";

            // Instantiate the regular expression object.
            Regex rb = new Regex(patternBeam, RegexOptions.IgnoreCase);

            string profileType = string.Empty;
            var rs = part.GetReportProperty("PROFILE_TYPE", ref profileType);
            if (profile.Length > 0)
            {
                if (part is ContourPlate)
                {
                    Match ms = r.Match(profile);
                    if (ms.Success)
                        part.Profile.ProfileString = ms.Groups[0].Value;
                    else
                        part.Profile.ProfileString = profile;
                }
                else
                {
                    Match ms = rb.Match(profile);
                    if (ms.Success & (ms.Groups[0].Value.StartsWith("PL") | ms.Groups[0].Value.StartsWith("-") | ms.Groups[0].Value.StartsWith("FPL")))
                        part.Profile.ProfileString = profile + "*200";
                    else
                        part.Profile.ProfileString = profile;
                }
                result = part.Modify();
            }
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
            string m_start_reverse,
            string m_end,
            string m_end_reverse,
            string m_summary,
            string m_z,
            string m_start_reverse_z,
            string m_end_z,
            string m_end_reverse_z,
            string m_x,
            string m_end_x,
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
            string q_min,
            string q_end_min,
            string q_summary,
            string material,
            string notes,
            int isSimple,
            int emptyRowsNumber,
            int crossSectionOnOtherList,
            string classificator,
            string album,
            int rotNotAllowed,
            List<Part> parts)
        {
            bool res = false;
            ArrayList arrayList;
            if (parts == null)
            {
                arrayList = GetSelectedModelObjects();
            }
            else
            {
                arrayList = new ArrayList(parts);
            }
            

            foreach (var @object in arrayList)
            {
                Part part = @object as Part;
                if (part != null)
                {
                    string patternPlate = @"^\D*\d+";

                    // Instantiate the regular expression object.
                    Regex r = new Regex(patternPlate, RegexOptions.IgnoreCase);
                    string patternBeam = @"^\D*\d+$";

                    // Instantiate the regular expression object.
                    Regex rb = new Regex(patternBeam, RegexOptions.IgnoreCase);

                    part.AssemblyNumber.Prefix = mark;
                    string profileType = string.Empty;
                    var rs = part.GetReportProperty(UDANameConverter.ToTeklaProp("PROFILE_TYPE"), ref profileType);
                    if (part is ContourPlate)
                    {
                        Match ms = r.Match(profile);
                        if (ms.Success)
                            part.Profile.ProfileString = ms.Groups[0].Value;
                        else
                            part.Profile.ProfileString = profile;
                    }
                    else
                    {
                        Match ms = rb.Match(profile);
                        if (ms.Success & (ms.Groups[0].Value.StartsWith("PL") | ms.Groups[0].Value.StartsWith("-") | ms.Groups[0].Value.StartsWith("FPL")))
                            part.Profile.ProfileString = profile + "*200";
                        else
                            part.Profile.ProfileString = profile;
                    }

                    part.Class = classificator;
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("PRELIM_MARK"), position);

                    //string moment;
                    //if (m == m_end)
                    //    moment = m;
                    //else
                    //    moment = m + "/" + m_end;
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("moment_M"), m_summary);

                    double result;
                    Double.TryParse(m, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("momentY1"), result * 1000);
                    Double.TryParse(m_start_reverse, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("momentRY1"), result * 1000);
                    Double.TryParse(m_end, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("momentY2"), result * 1000);
                    Double.TryParse(m_end_reverse, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("momentRY2"), result * 1000);
                    Double.TryParse(m_z, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("moment1"), result * 1000);                  
                    Double.TryParse(m_end_z, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("moment2"), result * 1000);
                    Double.TryParse(m_x, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("torsion1"), result * 1000);
                    Double.TryParse(m_end_x, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("torsion2"), result * 1000);



                    part.SetUserProperty(UDANameConverter.ToTeklaProp("START_MOMENT_CONN"), startMomentConnection);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("END_MOMENT_CONN"), endMomentConnection);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("START_FRICT_CONN"), startFrictionConnection);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("END_FRICT_CONN"), endFrictionConnection);

                    part.SetUserProperty(UDANameConverter.ToTeklaProp("usilie_N"), n_summary);
                    Double.TryParse(n, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("axial1"), result * 1000);
                    Double.TryParse(n_end, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("axial2"), result * 1000);
                    Double.TryParse(n_start_min, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("axialcomp1"), result * 1000);
                    Double.TryParse(n_end_min, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("axialcomp2"), result * 1000);


                    part.SetUserProperty(UDANameConverter.ToTeklaProp("reakciya_A"), q_summary);
                    Double.TryParse(q, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("shearZ1"), result * 1000);
                    Double.TryParse(q_end, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("shearZ2"), result * 1000);
                    Double.TryParse(q_min, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("shear1"), result * 1000);
                    Double.TryParse(q_end_min, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("shear2"), result * 1000);

                    part.Material.MaterialString = material;
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("prim_vedomost"), notes);

                    part.SetUserProperty(UDANameConverter.ToTeklaProp("slozhnoe_sechenie"), isSimple);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("pustykh_strok"), emptyRowsNumber);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("ru_slozh_sech_list"), crossSectionOnOtherList);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("Album"), album);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("ROT_NOT_ALLOWED"), album);

                    res = part.Modify();
                }
            }
            model.CommitChanges("Parts updated");
            return res;
        }

        private static void GetStringPropertyFromObject(Part borrowedPart, string udaName, string dictName, ref Hashtable hashtable)
        {
            string extracted = string.Empty;
            if (borrowedPart.GetUserProperty(udaName, ref extracted))
                hashtable[dictName] = extracted;
            int extractedInt = -1;
            if (borrowedPart.GetUserProperty(udaName, ref extractedInt))
                hashtable[dictName] = extractedInt;
            double extractedDouble = 0;
            if (borrowedPart.GetUserProperty(udaName, ref extractedDouble))
                hashtable[dictName] = extractedDouble;
        }

        internal static Hashtable GetInterestedPropertiesFromSelectedObjects()
        {
            Part borrowedPart = GetSelectedModelObjects().ToArray().SkipWhile(x => !(x is Part)).Cast<Part>().FirstOrDefault();
            Hashtable hashtable = new Hashtable();
            ArrayList stringNames = new ArrayList() { "PRELIM_MARK", "START_FRICT_CONN", "END_FRICT_CONN", "ROT_NOT_ALLOWED", "MATERIAL", "PROFILE", "ASSEMBLY.PREFIX", "prim_vedomost", "RU_BOM_CTG" };
            ArrayList doubleNames = new ArrayList() { "momentY1", "momentY2", "moment1", "moment2", "axial1", "axial2", "axialcomp1", "axialcomp2", "shearZ1", "shearZ2", "shear1", "shear2" };
            ArrayList integerNames = new ArrayList() { };
            borrowedPart.GetAllReportProperties(
                UDANameConverter.ToTeklaProp(stringNames),
                UDANameConverter.ToTeklaProp(doubleNames),
                UDANameConverter.ToTeklaProp(integerNames), 
                ref hashtable);
            return hashtable;
        }

        internal static bool InheritPropsFromHierarchicObjectToAssociatedParts(
            HierarchicObject hierarchicObject,
            string mark,
            string profile,
            string position,
            string m,
            string m_start_reverse,
            string m_end,
            string m_end_reverse,
            string m_summary,
            string m_z,
            string m_start_reverse_z,
            string m_end_z,
            string m_end_reverse_z,
            string m_x,
            string m_end_x,
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
            string q_min,
            string q_end_min,
            string q_summary,
            string material,
            string notes,
            int isSimple,
            int emptyRowsNumber,
            int crossSectionOnOtherList,
            string classificator,
            string album,
            int rotNotAllowed)
        {
            bool res = false;

            var moe = hierarchicObject.GetChildren();

            while (moe.MoveNext())
            {
                Part part = moe.Current as Part;
                if (part != null)
                {
                    part.AssemblyNumber.Prefix = mark;
                    string patternPlate = @"^\D*\d+";

                    // Instantiate the regular expression object.
                    Regex r = new Regex(patternPlate, RegexOptions.IgnoreCase);
                    string patternBeam = @"^\D*\d+$";

                    // Instantiate the regular expression object.
                    Regex rb = new Regex(patternBeam, RegexOptions.IgnoreCase);

                    part.AssemblyNumber.Prefix = mark;
                    part.Profile.ProfileString = profile;
                    part.Modify();
                    part.Material.MaterialString = material;
                    string profileType = string.Empty;
                    var rs = part.GetReportProperty("PROFILE_TYPE", ref profileType);
                    if (part is ContourPlate)
                    {
                        Match ms = r.Match(profile);
                        if (ms.Success)
                            part.Profile.ProfileString = ms.Groups[0].Value;
                        else
                            part.Profile.ProfileString = profile;
                    }
                    else
                    {
                        Match ms = rb.Match(profile);
                        if (ms.Success & (ms.Groups[0].Value.StartsWith("PL") | ms.Groups[0].Value.StartsWith("-") | ms.Groups[0].Value.StartsWith("FPL")))
                            part.Profile.ProfileString = profile + "*200";
                        else
                            part.Profile.ProfileString = profile;
                    }
                    part.Class = classificator;
                    part.Modify();
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("PRELIM_MARK"), position);

                    //string moment;
                    //if (m == m_end)
                    //    moment = m;
                    //else
                    //    moment = m + "/" + m_end;
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("moment_M"), m_summary);

                    double result;
                    Double.TryParse(m, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("momentY1"), result * 1000);
                    Double.TryParse(m_start_reverse, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("momentRY1"), result * 1000);
                    Double.TryParse(m_end, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("momentY2"), result * 1000);
                    Double.TryParse(m_end_reverse, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("momentRY2"), result * 1000);
                    Double.TryParse(m_z, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("moment1"), result * 1000);
                    Double.TryParse(m_end_z, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("moment2"), result * 1000);
                    Double.TryParse(m_x, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("torsion1"), result * 1000);
                    Double.TryParse(m_end_x, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("torsion2"), result * 1000);


                    part.SetUserProperty(UDANameConverter.ToTeklaProp("START_MOMENT_CONN"), startMomentConnection);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("END_MOMENT_CONN"), endMomentConnection);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("START_FRICT_CONN"), startFrictionConnection);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("END_FRICT_CONN"), endFrictionConnection);

                    part.SetUserProperty(UDANameConverter.ToTeklaProp("usilie_N"), n_summary);
                    Double.TryParse(n, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("axial1"), result * 1000);
                    Double.TryParse(n_end, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("axial2"), result * 1000);
                    Double.TryParse(n_start_min, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("axialcomp1"), result * 1000);
                    Double.TryParse(n_end_min, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("axialcomp2"), result * 1000);


                    part.SetUserProperty(UDANameConverter.ToTeklaProp("reakciya_A"), q_summary);
                    Double.TryParse(q, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("shearZ1"), result * 1000);
                    Double.TryParse(q_end, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("shearZ2"), result * 1000);
                    Double.TryParse(q_min, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("shear1"), result * 1000);
                    Double.TryParse(q_end_min, out result);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("shear2"), result * 1000);

                    part.SetUserProperty(UDANameConverter.ToTeklaProp("prim_vedomost"), notes);

                    part.SetUserProperty(UDANameConverter.ToTeklaProp("slozhnoe_sechenie"), isSimple);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("pustykh_strok"), emptyRowsNumber);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("ru_slozh_sech_list"), crossSectionOnOtherList);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("Album"), album);
                    part.SetUserProperty(UDANameConverter.ToTeklaProp("ROT_NOT_ALLOWED"), rotNotAllowed);

                    res = part.Modify();
                }
            }

            return res;
        }

        #endregion

        #region Рисование

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

            result = profile.Length > 0 & (libraryProfileItem.Select(profile) || parametricProfileItem.Select(profile));
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
            var symbolName = UDANameConverter.ToTeklaProp("SYMBOL_NAME");
            foreach (var par in res)
            {
                var profileItemParameter = par as ProfileItemParameter;
                if (profileItemParameter.Property == symbolName)
                {
                    string extractPattern = @"(?i)([A-Z0-9._ %+-]+@[0-9]{1,3})";
                    var extractedValue = Regex.Matches(profileItemParameter.StringValue, extractPattern)
                        .Cast<Match>()
                        .Select(x => x.Groups[1].Value);
                    if (extractedValue.FirstOrDefault() != null)
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
                if (profileItemParameter.Property == UDANameConverter.ToTeklaProp("TPL_NAME"))
                    return profileItemParameter.StringValue;
            }
            return "not found in library";
        }

        internal static string GetMaterialForSpec(string materialString)
        {
            try
            {
                MaterialItem materialItem = new MaterialItem();
                materialItem.Select(UDANameConverter.ToTeklaProp(materialString));
                var res = materialItem.AliasName1.Length > 0;
                return materialItem.AliasName1.Length > 0 ? materialItem.AliasName1 : materialItem.MaterialName;
            }
            catch (Exception ex)
            {
                return UDANameConverter.ToTeklaProp(materialString);
            }
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
