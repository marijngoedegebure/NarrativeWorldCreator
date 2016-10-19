using System;
using System.Collections.Generic;

using System.Text;

namespace Common.Shapes
{
    public class ShapeType
    {
        internal class ListCreator<T> where T : BaseShape
        {
            internal static List<T> Create(object obj)
            {
                List<T> ret;
                if (obj is T)
                {
                    ret = new List<T>();
                    ret.Add((T)obj);
                }
                else
                    ret = (List<T>)obj;
                return ret;
            }

            internal static List<BaseShape> CreateBase(object obj)
            {
                List<T> list = Create(obj);
                List<BaseShape> ret = new List<BaseShape>();
                foreach (T t in list)
                    ret.Add(t);
                return ret;
            }
        }

        static List<ShapeType> allTypes = GetTypes();
        static Dictionary<string, ShapeType> typesPerShorthandName;

        private static List<ShapeType> GetTypes()
        {
            typesPerShorthandName = new Dictionary<string, ShapeType>();
            List<ShapeType> list = new List<ShapeType>();

            list.Add(new ShapeType("Point", "Po", false, false));
            list.Add(new ShapeType("Extruded point", "EPo", false, true));
            list.Add(new ShapeType("Point list", "PoL", true, false));
            list.Add(new ShapeType("Extruded point list", "EPoL", true, true));

            list.Add(new ShapeType("Line", "Li", false, false));
            list.Add(new ShapeType("Extruded line", "ELi", false, true));
            list.Add(new ShapeType("Line list", "LiL", true, false));
            list.Add(new ShapeType("Extruded line list", "ELiL", true, true));

            list.Add(new ShapeType("Shape", "Sh", false, false));
            list.Add(new ShapeType("Extruded shape", "ESh", false, true));
            list.Add(new ShapeType("Shape list", "ShL", true, false));
            list.Add(new ShapeType("Extruded shape list", "EShL", true, true));

            list.Add(new ShapeType("Path", "Pa", false, false));
            list.Add(new ShapeType("Path list", "PaL", true, false));

            return list;
        }

        public static List<BaseShape> GetShapeOrListOfShapesFromObject(object obj)
        {
            List<BaseShape> ret;
            if (obj is Point || obj is List<Point>)
                ret = ListCreator<Point>.CreateBase(obj);
            else if (obj is ExtrudedPoint || obj is List<ExtrudedPoint>)
                ret = ListCreator<ExtrudedPoint>.CreateBase(obj);
            else if (obj is FlatLine || obj is List<FlatLine>)
                ret = ListCreator<FlatLine>.CreateBase(obj);
            else if (obj is ExtrudedLine || obj is List<ExtrudedLine>)
                ret = ListCreator<ExtrudedLine>.CreateBase(obj);
            else if (obj is FlatShape || obj is List<FlatShape>)
                ret = ListCreator<FlatShape>.CreateBase(obj);
            else if (obj is ExtrudedShape || obj is List<ExtrudedShape>)
                ret = ListCreator<ExtrudedShape>.CreateBase(obj);
            else if (obj is Path || obj is List<Path>)
                ret = ListCreator<Path>.CreateBase(obj);
            else
                throw new NotImplementedException();
            return ret;
        }

        string name, shortname;
        bool isList, isExtruded;

        public string Name { get { return name; } }

        private ShapeType(string name, string shortname, bool isList, bool isExtruded)
        {
            this.name = name;
            this.shortname = shortname;
            this.isList = isList;
            this.isExtruded = isExtruded;
            typesPerShorthandName.Add(shortname.ToLower(), this);
        }

        public static ShapeType GetSingleShapeTypeFromShorthandString(string str)
        {
            int i = allTypes.Count;
            return typesPerShorthandName[str.Trim()];
        }

        /// <summary>
        /// String needs to contain shapetype shorthands, separated by a comma ',' !
        /// </summary>
        /// <param name="str">String needs to contain shapetype shorthands, separated by a comma ',' !</param>
        /// <returns></returns>
        public static List<ShapeType> GetListOfShapeTypesFromShorthandString(string str)
        {
            List<ShapeType> list = new List<ShapeType>();
            str = str.Trim().ToLower();
            if (str != "")
            {
                string[] split = str.Split(',');
                foreach (string s in split)
                    list.Add(GetSingleShapeTypeFromShorthandString(s));
            }
            return list;
        }

        public static ShapeType GetFromName(string name)
        {
            foreach (ShapeType st in allTypes)
                if (st.name == name)
                    return st;
            return null;
        }
    }
}
