using System;
using System.ComponentModel.Composition;
using Tekla.Structures.Model;
using Tekla.Structures.CustomPropertyPlugin;

namespace CustomPropertyHierarchicObject
{
    /// <summary>The test plugin for father component name or number.</summary>
    [Export(typeof(ICustomPropertyPlugin))]
    [ExportMetadata("CustomProperty", "CUSTOM.ASSEMBLY_PREFIX")]
    public class CUSTOM_ASSEMBLY_PREFIX : ICustomPropertyPlugin
    {

        /// <summary>Returns custom property int value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetIntegerProperty(int objectId)
        {
            var part = TeklaHierarchicObject.GetModelObject(objectId);
            if (part != null & part is Part)
            {
                var assembly = (part as Part).GetAssembly();
                if (assembly != null)
                    return 1;
            }
            return 0;
        }

        /// <summary>Returns custom property string value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetStringProperty(int objectId)
        {
            var part = TeklaHierarchicObject.GetModelObject(objectId);
            if (part != null & part is Part)
            {
                var assembly = (part as Part).GetAssembly();
                if (assembly != null)
                    return assembly.AssemblyNumber.Prefix;
                else
                    return "error on getting the assembly";
            }
            else
                return "not a part";
        }

        /// <summary>Returns custom property double value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double GetDoubleProperty(int objectId)
        {
            var part = TeklaHierarchicObject.GetModelObject(objectId);
            if (part != null & part is Part)
            {
                var assembly = (part as Part).GetAssembly();
                if (assembly != null)
                    return 1;
            }
            return 0;
        }
    }

    /// <summary>The test plugin for father component name or number.</summary>
    [Export(typeof(ICustomPropertyPlugin))]
    [ExportMetadata("CustomProperty", "CUSTOM.HO.CLASS_ATTR")]
    public class CUSTOM_HO_CLASS_ATTR : ICustomPropertyPlugin
    {

        /// <summary>Returns custom property int value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetIntegerProperty(int objectId)
        {
            var father = TeklaHierarchicObject.GetElementListHO(objectId);
            //var childrenCount = TeklaHierarchicObject.GetHOChildrenCount(objectId);
            if (father != null)
            {
                string classAttribute = string.Empty;
                if (father.GetUserProperty("Classificator", ref classAttribute))
                    return Int32.Parse(classAttribute);
                else
                    return -1;
            }
            else
                return 0;
        }

        /// <summary>Returns custom property string value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetStringProperty(int objectId)
        {
            return GetIntegerProperty(objectId).ToString();
        }

        /// <summary>Returns custom property double value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double GetDoubleProperty(int objectId)
        {
            return GetIntegerProperty(objectId);
        }
    }

    /// <summary>The test plugin for father component name or number.</summary>
    [Export(typeof(ICustomPropertyPlugin))]
    [ExportMetadata("CustomProperty", "CUSTOM.HO.IS_ROOT")]
    public class CUSTOM_HO_IS_ROOT : ICustomPropertyPlugin
    {

        /// <summary>Returns custom property int value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetIntegerProperty(int objectId)
        {
            var father = TeklaHierarchicObject.GetHOFather(objectId);
            //var childrenCount = TeklaHierarchicObject.GetHOChildrenCount(objectId);

            if (father == null)
                return 1;
            else
                return 0;
        }

        /// <summary>Returns custom property string value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetStringProperty(int objectId)
        {
            return GetIntegerProperty(objectId).ToString();
        }

        /// <summary>Returns custom property double value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double GetDoubleProperty(int objectId)
        {
            return GetIntegerProperty(objectId);
        }
    }

    /// <summary>Hierarchic object has father. True/false</summary>
    [Export(typeof(ICustomPropertyPlugin))]
    [ExportMetadata("CustomProperty", "CUSTOM.HO.HAS_FATHER")]
    public class CUSTOM_HO_HAS_FATHER : ICustomPropertyPlugin
    {

        /// <summary>Returns custom property int value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetIntegerProperty(int objectId)
        {
            var father = TeklaHierarchicObject.GetHOFather(objectId);
            if (father != null)
                return 1;
            else
                return 0;
        }

        /// <summary>Returns custom property string value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetStringProperty(int objectId)
        {
            return GetIntegerProperty(objectId).ToString();
        }

        /// <summary>Returns custom property double value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double GetDoubleProperty(int objectId)
        {
            return GetIntegerProperty(objectId);
        }
    }

    /// <summary>Component is complex. 1 = true, 0 = false </summary>
    [Export(typeof(ICustomPropertyPlugin))]
    [ExportMetadata("CustomProperty", "CUSTOM.HO.IS_COMPLEX")]
    public class CUSTOM_HO_IS_COMPLEX : ICustomPropertyPlugin
    {
        /// <summary>Returns custom property int value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetIntegerProperty(int objectId)
        {
            var father = TeklaHierarchicObject.GetHOFather(objectId);
            var childrenCount = TeklaHierarchicObject.GetHOChildrenCount(objectId);
            var hasInternalPos = TeklaHierarchicObject.GetHOHasInternalPosition(objectId);

            if (father != null || childrenCount > 0 || hasInternalPos)
                return 1;
            else
                return 0;
        }

        /// <summary>Returns custom property string value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetStringProperty(int objectId)
        {
            return GetIntegerProperty(objectId).ToString();
        }

        /// <summary>Returns custom property double value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double GetDoubleProperty(int objectId)
        {
            return GetIntegerProperty(objectId);
        }
    }

    /// <summary>The test if part is attached to element list</summary>
    [Export(typeof(ICustomPropertyPlugin))]
    [ExportMetadata("CustomProperty", "CUSTOM.PART.IS_ATTACHED_TO_EL")]
    public class CUSTOM_PART_IS_ATTACHED_TO_EL : ICustomPropertyPlugin
    {

        /// <summary>Returns custom property int value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetIntegerProperty(int objectId)
        {
            var mo = TeklaHierarchicObject.GetModelObject(objectId);            
            if (mo.GetHierarchicObjects().GetSize() > 0)
                return 1;
            else
                return 0;
        }

        /// <summary>Returns custom property string value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetStringProperty(int objectId)
        {
            return GetIntegerProperty(objectId).ToString();
        }

        /// <summary>Returns custom property double value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double GetDoubleProperty(int objectId)
        {
            return GetIntegerProperty(objectId);
        }
    }

    /// <summary>Is up-to-date to the element list</summary>
    [Export(typeof(ICustomPropertyPlugin))]
    [ExportMetadata("CustomProperty", "CUSTOM.PART.IS_UPTODATE_TO_EL")]
    public class CUSTOM_PART_IS_UPTODATE_TO_EL : ICustomPropertyPlugin
    {

        /// <summary>Returns custom property int value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetIntegerProperty(int objectId)
        {
            var mo = TeklaHierarchicObject.GetModelObject(objectId);
            var part = mo as Part;
            if (part != null & mo.GetHierarchicObjects().GetSize() > 0)
            {
                string res = string.Empty;
                var mobjenum = mo.GetHierarchicObjects();
                while (mobjenum.MoveNext())
                {
                    var mobj = mobjenum.Current as HierarchicObject;
                    if(mobj != null)
                    {
                        string classifier = "", prelim_mark = "", pos = "";
                        
                        mobj.GetUserProperty("Classificator", ref classifier);
                        part.GetUserProperty("PRELIM_MARK", ref prelim_mark);
                        mobj.GetUserProperty("Position", ref pos);
                        
                        if (
                            mobj.Name == part.AssemblyNumber.Prefix 
                            & classifier == part.Class
                            & prelim_mark == pos)
                        {
                            return 1;
                        }
                    }

                }
            }
                
            return 0;
        }

        /// <summary>Returns custom property string value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetStringProperty(int objectId)
        {
            return GetIntegerProperty(objectId).ToString();
        }

        /// <summary>Returns custom property double value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double GetDoubleProperty(int objectId)
        {
            return GetIntegerProperty(objectId);
        }
    }


    /// <summary>The test if part is attached to element list</summary>
    [Export(typeof(ICustomPropertyPlugin))]
    [ExportMetadata("CustomProperty", "CUSTOM.PART.EL")]
    public class CUSTOM_PART_EL : ICustomPropertyPlugin
    {

        /// <summary>Returns custom property int value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetIntegerProperty(int objectId)
        {
            var mo = TeklaHierarchicObject.GetModelObject(objectId);
            if (mo.GetHierarchicObjects().GetSize() > 0)
                return 1;
            else
                return 0;
        }

        /// <summary>Returns custom property string value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetStringProperty(int objectId)
        {
            var mo = TeklaHierarchicObject.GetModelObject(objectId);
            if (mo.GetHierarchicObjects().GetSize() > 0)
            {
                string res = string.Empty;
                var mobjenum = mo.GetHierarchicObjects();
                while (mobjenum.MoveNext())
                {
                    var mobj = mobjenum.Current as HierarchicObject;
                    if (mobj != null)
                    {
                        if (mobj.GetUserProperty("BOE", ref res))
                            return res;
                    }
                    
                }
                return res;
            }                
            else
                return "";
        }

        /// <summary>Returns custom property double value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double GetDoubleProperty(int objectId)
        {
            return GetIntegerProperty(objectId);
        }
    }

    /// <summary>The test if part is attached to element list</summary>
    [Export(typeof(ICustomPropertyPlugin))]
    [ExportMetadata("CustomProperty", "CUSTOM.PART.EL_MARK")]
    public class CUSTOM_PART_EL_MARK : ICustomPropertyPlugin
    {

        /// <summary>Returns custom property int value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetIntegerProperty(int objectId)
        {
            var mo = TeklaHierarchicObject.GetModelObject(objectId);
            if (mo.GetHierarchicObjects().GetSize() > 0)
                return 1;
            else
                return 0;
        }

        /// <summary>Returns custom property string value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetStringProperty(int objectId)
        {
            var mo = TeklaHierarchicObject.GetModelObject(objectId);
            if (mo.GetHierarchicObjects().GetSize() > 0)
            {
                string res = string.Empty;
                var mobjenum = mo.GetHierarchicObjects();
                while (mobjenum.MoveNext())
                {
                    var mobj = mobjenum.Current as HierarchicObject;
                    if (mobj != null)
                    {
                        return mobj.Name;
                    }

                }
                return res;
            }
            else
                return "";
        }

        /// <summary>Returns custom property double value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double GetDoubleProperty(int objectId)
        {
            return GetIntegerProperty(objectId);
        }
    }


    /// <summary>The test if part is attached to foundation list</summary>
    [Export(typeof(ICustomPropertyPlugin))]
    [ExportMetadata("CustomProperty", "CUSTOM.HO.IS_IN_FOUNDATION_LIST")]
    public class CUSTOM_HO_IS_IN_FOUNDATION_LIST : ICustomPropertyPlugin
    {

        /// <summary>Returns custom property int value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetIntegerProperty(int objectId)
        {
            if (TeklaHierarchicObject.GetHORootHierarchicDefinition(objectId) != null)
                return 1;
            else
                return 0;
        }

        /// <summary>Returns custom property string value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetStringProperty(int objectId)
        {
            if (TeklaHierarchicObject.GetHORootHierarchicDefinition(objectId) != null)
                return "1";
            else
                return "0";
        }

        /// <summary>Returns custom property double value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double GetDoubleProperty(int objectId)
        {
            return GetIntegerProperty(objectId);
        }
    }

    /// <summary>The test if part is attached to element list</summary>
    [Export(typeof(ICustomPropertyPlugin))]
    [ExportMetadata("CustomProperty", "CUSTOM.PART.BASE_MARK")]
    public class CUSTOM_PART_BASE_MARK : ICustomPropertyPlugin
    {

        /// <summary>Returns custom property int value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetIntegerProperty(int objectId)
        {
            string baseMark = string.Empty;
            if (TeklaHierarchicObject.GetBaseMark(objectId, ref baseMark))
                return 1;
            else
                return 0;
        }

        /// <summary>Returns custom property string value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetStringProperty(int objectId)
        {
            string baseMark = string.Empty;
            TeklaHierarchicObject.GetBaseMark(objectId, ref baseMark);
            return baseMark;
        }

        /// <summary>Returns custom property double value for object.</summary>
        /// <param name="objectId">The object id.</param>
        /// <returns>The <see cref="double"/>.</returns>
        public double GetDoubleProperty(int objectId)
        {
            return GetIntegerProperty(objectId);
        }
    }


    static class TeklaHierarchicObject
    {
        private static Model model = new Model();

        internal static HierarchicObject GetElementListHO(int id)
        {
            Part modelObject = model.SelectModelObject(new Tekla.Structures.Identifier(id)) as Part;
            if(modelObject != null)
            {
                var enumHO = modelObject.GetHierarchicObjects();
                foreach(var item in enumHO)
                {
                    if (item is HierarchicObject)
                    {
                        var ho = (item as HierarchicObject);
                        if (ho.Select())
                        {
                            HierarchicDefinition hod = model.SelectModelObject(ho.Definition.Identifier) as HierarchicDefinition;
                            if (hod.Name == "Element_list")
                                return ho;
                        }
                        return null;
                    }
                }
            }
            return null;
        }


        internal static HierarchicObject GetHOFather(int id)
        {
            ModelObject modelObject = model.SelectModelObject(new Tekla.Structures.Identifier(id));
            if(modelObject is HierarchicObject)
            {
                var ho = (modelObject as HierarchicObject);
                HierarchicObject fatherHierarchicObject = new HierarchicObject();
                if (ho.Father != null)
                {
                    fatherHierarchicObject.Identifier = ho.Father.Identifier;
                    if (fatherHierarchicObject.Select())
                    {
                        return fatherHierarchicObject;
                    }
                }                
                return null;
            }
            return null;
        }

        internal static int GetHOChildrenCount(int id)
        {
            ModelObject modelObject = model.SelectModelObject(new Tekla.Structures.Identifier(id));
            if (modelObject is HierarchicObject)
            {
                var hierarchicObject = modelObject as HierarchicObject;
                return hierarchicObject.HierarchicChildren.Count;
            }
            return 0;
        }

        internal static bool GetHOHasInternalPosition(int id)
        {
            ModelObject modelObject = model.SelectModelObject(new Tekla.Structures.Identifier(id));
            if (modelObject is HierarchicObject)
            {
                var hierarchicObject = modelObject as HierarchicObject;
                string result = string.Empty;
                hierarchicObject.GetUserProperty("Position", ref result);
                if (result.Length >0)
                    return true;
            }
            return false;
        }

        internal static ModelObject GetModelObject(int id)
        {
            ModelObject modelObject = TeklaHierarchicObject.model.SelectModelObject(new Tekla.Structures.Identifier(id));
            return modelObject;
        }

        internal static HierarchicDefinition GetHORootHierarchicDefinition(int id)
        {
            ModelObject modelObject = model.SelectModelObject(new Tekla.Structures.Identifier(id));
            if (modelObject is HierarchicObject)
            {
                var ho = (modelObject as HierarchicObject);
                HierarchicDefinition fatherHierarchicDefinition = new HierarchicDefinition();
                if (ho.Definition != null)
                {
                    fatherHierarchicDefinition.Identifier = ho.Definition.Identifier;
                    if (fatherHierarchicDefinition.Select())
                    {
                        HierarchicDefinition rootFatherHierarchicDefinition = new HierarchicDefinition();
                        if (fatherHierarchicDefinition.Father != null)
                        {
                            rootFatherHierarchicDefinition.Identifier = fatherHierarchicDefinition.Father.Identifier;
                            if (rootFatherHierarchicDefinition.Select())
                            {
                                if (rootFatherHierarchicDefinition.Name == "Foundation_List")
                                    return rootFatherHierarchicDefinition;
                            }
                        }
                    }
                }
            }
            return null;
        }

        internal static bool GetBaseMark(int objectId, ref string baseMark)
        {
            ModelObject modelObject = model.SelectModelObject(new Tekla.Structures.Identifier(objectId));
            if (modelObject is Part)
            {
                var part = (modelObject as Part);
                foreach (var mo in part.GetHierarchicObjects())
                {
                    var ho = (mo as HierarchicObject);
                    if (TeklaHierarchicObject.GetHORootHierarchicDefinition(ho.Identifier.ID) != null)
                    {
                        baseMark = ho.Name;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
