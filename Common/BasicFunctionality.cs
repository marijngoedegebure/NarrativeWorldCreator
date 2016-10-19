using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Common
{
    public static class BasicFunctionality
    {
        public static List<double> ObjectListToDoubleList(List<object> list)
        {
            List<double> ret = new List<double>();
            foreach (object obj in list)
                ret.Add((double)obj);
            return ret;
        }

        public static bool AllEqual(List<double> list)
        {
            for (int i = 1; i < list.Count; ++i)
                if (list[i] != list[0])
                    return false;
            return true;
        }
        public static void SplitFunctionString(string p, out string name, out List<string> parameters)
        {
            int i1 = p.IndexOf('(');
            int i2 = p.LastIndexOf(')');
            if (i1 == -1 || i2 == -1 || i2 < i1)
                throw new Exception("Locator parameter list corrupt: " + p + "; expected: name(parameters)");
            name = p.Substring(0, i1);
            string pars = p.Substring(i1 + 1, i2 - i1 - 1);
            parameters = Common.MathParser.Term.ParseCharacterSeparatedListOfStrings(pars, ',');
        }

        public static IEnumerable<System.Xml.XmlNode> GetElementsByName(System.Xml.XmlNodeList list, string name)
        {
            foreach (System.Xml.XmlNode subnode in list)
                if (subnode.Name == name)
                    yield return subnode;
        }

        public static List<System.Xml.XmlNode> GetElementsByName(System.Xml.XmlNode list, string name)
        {
            List<System.Xml.XmlNode> nlist = new List<System.Xml.XmlNode>();
            foreach (System.Xml.XmlNode subnode in list.ChildNodes)
                if (subnode.Name == name)
                    nlist.Add(subnode);
            return nlist;
        }

        public static Size Sizing(SizeF size)
        {
            return new Size((int)size.Width, (int)size.Height);
        }

        public static double GetRandomMinus1To1(Random r)
        {
            return r.NextDouble() * 2 - 1;
        }

        static double pi2 = 2 * Math.PI;

        public static double AngleBetween0And2Pi(double d)
        {
            while (d < 0)
                d += pi2;
            while (d >= pi2)
                d -= pi2;
            return d;
        }

        public static double AngleBetweenMinusPiAndPi(double d)
        {
            while (d <= -Math.PI)
                d += pi2;
            while (d > Math.PI)
                d -= pi2;
            return d;
        }

        public static int GetCorrespondingClosingBracketPosition(string text, int index)
        {
            return GetCorrespondingClosingBracketPosition(text, index, '(', ')');
        }

        public static int GetCorrespondingClosingBracketPosition(string text, int index, char openbracket, char closingbracket)
        {
            int i = index;
            int brackets = 0;
            while (i < text.Length)
            {
                char ch = text[i];
                if (ch == openbracket)
                    ++brackets;
                else if (ch == closingbracket)
                {
                    if (brackets == 0)
                        return i;
                    else
                        --brackets;
                }
                ++i;
            }
            return -1;
        }

        public static int GetCorrespondingOpeningBracketPosition(string text, int index)
        {
            return GetCorrespondingOpeningBracketPosition(text, index, '(', ')');
        }

        public static int GetCorrespondingOpeningBracketPosition(string text, int index, char openbracket, char closingbracket)
        {
            int i = index;
            int brackets = 0;
            while (i >= 0)
            {
                char ch = text[i];
                if (ch == closingbracket)
                    ++brackets;
                else if (ch == openbracket)
                {
                    if (brackets == 0)
                        return i;
                    else
                        --brackets;
                }
                --i;
            }
            return -1;
        }

        public static int GetStartLineOfString(string file, string text)
        {
            int count = 1;
            using (StreamReader sr = new StreamReader(file))
            {
                string f = sr.ReadToEnd();
                string t = f.Substring(0, f.IndexOf(text));
                foreach (char ch in t)
                    if (ch == '\r')
                        ++count;
            }
            return count;
        }

        public static string RemoveExtraSpaces(string sentence)
        {
            string[] split = sentence.Split(' ');
            if (split.Length == 0)
                return "";
            string ret = "";
            foreach (string str in split)
                if (ret == "")
                    ret = str;
                else
                    ret += " " + str;
            return ret;
        }

        public static List<string> Clone(List<string> list)
        {
            List<string> clone = new List<string>();
            foreach (string s in list)
                clone.Add(s);
            return clone;
        }

        public static List<string> Merge(List<string> l1, List<string> l2)
        {
            List<string> merged = Clone(l1);
            foreach (string s in l2)
            {
                if (!merged.Contains(s))
                    merged.Add(s);
            }
            return merged;
        }

        public static List<string> SplitListWithCurlyBrackets(string p, char split)
        {
            if (p == "")
                return new List<string>();
            List<string> strlist = new List<string>();
            int parent = 0;
            string temp = "";
            foreach (char ch in p)
            {
                if (ch == '{')
                    parent++;
                else if (ch == '}')
                    parent--;

                if (ch == split && parent == 0)
                {
                    strlist.Add(temp.Trim());
                    temp = "";
                }
                else
                    temp += ch;
            }
            strlist.Add(temp.Trim());
            return strlist;
        }

        public static string GetFilenameEnding(string p)
        {
            int l = p.LastIndexOfAny(new char[] { '/', '\\' });
            if (l == -1)
                return p;
            return p.Substring(l + 1);
        }

        public static string GetDirectory(string p)
        {
            int l = (int)Math.Max(p.LastIndexOf('/'), p.LastIndexOf('\\'));
            return p.Substring(0, l);
        }

        public static List<Vec2> ReadListOfVec2(string p)
        {
            List<float> floats = ReadListOfDoubles(p);
            if (floats.Count % 2 != 0)
                throw new Exception("A list of vec2 elements should always have an even number of elements");
            List<Vec2> vecs = new List<Vec2>(floats.Count / 2);
            for (int i = 0; i < floats.Count; i += 2)
                vecs.Add(new Vec2(floats[i], floats[i + 1]));
            return vecs;
        }

        public static List<float> ReadListOfDoubles(string p)
        {

            string[] split = p.Split(',');
            List<float> list = new List<float>(split.Length);
            foreach (string s in split)
                list.Add((float)double.Parse(s));
            return list;
        }

        public static Dictionary<string, string> ParseParameterList(string list, char separator, char equals)
        {
            string[] parameters = list.Split(separator);
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (string s in parameters)
            {
                if (s != "")
                {
                    string[] temp = s.Split(equals);
                    if (temp.Length == 2)
                    {
                        ret.Add(temp[0], temp[1]);
                    }
                }
            }
            return ret;
        }

        public static string CreateCommaSeparatedList(List<string> list)
        {
            if (list.Count == 0)
                return "";
            string temp = "";
            foreach (string s in list)
                temp += temp == "" ? s : ", " + s;
            return temp;
        }

        public static float[] DoubleArrayToFloatArray(double[] p)
        {
            float[] ret = new float[p.Length];
            for (int i = 0; i < p.Length; ++i)
                ret[i] = (float)p[i];
            return ret;
        }

        public static string GetAppDirectory()
        {
            return Path.GetDirectoryName(Application.ExecutablePath) + "\\";
        }

        #region Method: GetExecutablePath()
        /// <summary>
        /// Gets the path of the executable.
        /// </summary>
        /// <returns>The path of the executable.</returns>
        public static String GetExecutablePath()
        {
            string exePath = Path.GetFullPath(System.Windows.Forms.Application.ExecutablePath);
            int i = exePath.Length - 1;
            while (exePath[i] != '/' && exePath[i] != '\\')
                i--;
            i++;
            return exePath.Substring(0, i);
        }
        #endregion Method: GetExecutablePath()

        #region Method: GetRelativePath(String filePath)
        /// <summary>
        /// Gets the path for the file, relative to the path of the executable. Make sure the file is on the same drive as the executable!
        /// </summary>
        /// <param name="filePath">The full file path.</param>
        /// <returns>The relative path.</returns>
        public static string GetRelativePath(string filePath)
        {
            string relPath = filePath;
            try
            {
                string exePath = Path.GetFullPath(System.Windows.Forms.Application.ExecutablePath);
                int i = 0;
                for (; i < Math.Min(exePath.Length, filePath.Length); ++i)
                    if (exePath[i] != filePath[i])
                        break;
                while (exePath[i] != '/' && exePath[i] != '\\')
                    --i;
                ++i;
                if (i > 0)
                {
                    exePath = exePath.Substring(i);
                    int dirCount = 0;
                    foreach (char ch in exePath)
                        if (ch == '\\')
                            ++dirCount;
                    relPath = "";
                    for (int j = 0; j < dirCount; ++j)
                        relPath += "..\\";
                    if (dirCount == 0)
                        relPath = ".\\";

                    filePath = filePath.Substring(i);
                    if (filePath.StartsWith("\\"))
                        relPath += filePath.Substring(1);
                    else
                        relPath += filePath;
                }
            }
            catch (Exception)
            {
            }
            
            return relPath;
        }
        #endregion Method: GetRelativePath(String filePath)

        #region Method: GetFile(String filePath)
        /// <summary>
        /// Gets the file name without the path.
        /// </summary>
        /// <returns>The file name without the path.</returns>
        public static String GetFile(String filePath)
        {
            if (filePath != null)
            {
                try
                {
                    int index = Math.Max(filePath.LastIndexOf('\\'), filePath.LastIndexOf('/'));
                    return index >= 0 ? filePath.Substring(index + 1) : filePath;
                }
                catch (Exception)
                {
                }
            }
            return filePath;
        }
        #endregion Method: GetFile(String filePath)

        #region Method: GetDerivedTypes(Type baseType, ReadOnlyCollection<Assembly> assemblies)
        /// <summary>
        /// Get the non-abstract derived types of the given type.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <param name="assemblies">A list with the assemblies to look in.</param>
        /// <returns>The non-abstract derived types.</returns>
        public static List<Type> GetDerivedTypes(Type baseType, ReadOnlyCollection<Assembly> assemblies)
        {
            List<Type> returnTypes = new List<Type>();

            if (baseType != null)
            {
                // Get all the types of all assemblies
                List<Type> types = new List<Type>();
                foreach (Assembly assembly in assemblies)
                {
                    foreach (Type type in assembly.GetTypes())
                        types.Add(type);
                }

                // Get the derived types
                foreach (Type type in types)
                {
                    if (!type.IsAbstract)
                    {
                        if (baseType.IsInterface)
                        {
                            Type it = type.GetInterface(baseType.FullName);

                            if (it != null)
                                returnTypes.Add(type);
                        }
                        else if (type.IsSubclassOf(baseType))
                            returnTypes.Add(type);
                    }
                }
            }

            return returnTypes;
        }
        #endregion Method: GetDerivedTypes(Type baseType, ReadOnlyCollection<Assembly> assemblies)


        internal static double clamp(float param, float min, float max)
        {
            return Math.Max(Math.Min(max, param), min);
        }

        #region Method: AreAllValidNumericChars(string str, bool integer)
        /// <summary>
        /// Check if the string consists of valid numeric characters, and if wanted, if it is an integer.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="integer">True if the string should represent an integer, false if it should be a float.</param>
        /// <returns>Returns whether the string consists of valid numeric characters.</returns>
        public static bool AreAllValidNumericChars(string str, bool integer)
        {
            if (str != null)
            {
                // The parse method does not work with the minus sign, so check it for ourselves
                if (str.StartsWith("-", false, CommonSettings.Culture))
                {
                    if (str.Length > 1)
                    {
                        str = str.Substring(1);
                        if (str.StartsWith("-", false, CommonSettings.Culture))
                            return false;
                    }
                    else
                        str = "0";
                }

                if (integer)
                {
                    int result = 0;
                    return int.TryParse(str, NumberStyles.Float, CommonSettings.Culture, out result);
                }
                else
                {
                    float result = 0;
                    return float.TryParse(str, NumberStyles.Float, CommonSettings.Culture, out result);
                }
            }

            return false;
        }
        #endregion Method: AreAllValidNumericChars(string str, bool integer)

        /// <summary>
        /// Check whether the textbox will contain valid numeric characters when new text is inserted.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="newText">The new text.</param>
        /// <returns>Returns whether the text box will contain valid numeric characters.</returns>
        public static bool CheckPreviewTextboxInputForValidNumericChars(System.Windows.Controls.TextBox textBox, string newText)
        {
            return CheckPreviewTextboxInputForValidNumericChars(textBox, newText, false);
        }

        /// <summary>
        /// Check whether the textbox will contain valid numeric characters when new text is inserted.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="newText">The new text.</param>
        /// <param name="integer">True if only integers are allowed, false if floating values are allowed as well.</param>
        /// <returns>Returns whether the text box will contain valid numeric characters.</returns>
        public static bool CheckPreviewTextboxInputForValidNumericChars(System.Windows.Controls.TextBox textBox, string newText, bool integer)
        {
            string text = textBox.Text;
            int selStart = textBox.SelectionStart;
            if (textBox.SelectionLength > 0)
                text = text.Remove(textBox.SelectionStart, textBox.SelectionLength);
            text = text.Insert(selStart, newText);

            return BasicFunctionality.AreAllValidNumericChars(text, integer);
        }

        public static string CreateFullPathFromAppDir(string path)
        {
            if (path.Length > 1 && path[1] == ':')
                return path;
            return GetAppDirectory() + path;
        }

        public static List<T> ShuffleList<T>(List<T> list, Random random)
        {
            List<T> newList = new List<T>(list.Count);
            while (list.Count > 1)
            {
                int index = random.Next(list.Count);
                newList.Add(list[index]);
                list.RemoveAt(index);
            }
            if (list.Count == 1)
            {
                newList.Add(list[0]);
                list.Clear();
            }
            return newList;
        }

        public static float PreferredGridLineDistance(int maxNrOfLines, float width)
        {
            float gridLineDistance = 0.01f;
            while ((int)Math.Round(width / gridLineDistance) > maxNrOfLines)
                gridLineDistance *= 10.0f;
            return gridLineDistance;
        }
    }

    public class BasicGraph
    {
        List<BasicGraphPath> paths = new List<BasicGraphPath>();
        List<BasicGraphNode> nodes = new List<BasicGraphNode>();

        public ReadOnlyCollection<BasicGraphPath> Paths { get { return paths.AsReadOnly(); } }

        public BasicGraph()
        {
        }

        public void AddPath(List<Vec3d> path, int type)
        {
            this.paths.Add(new BasicGraphPath(this, path, type));
        }

        public void CombinePathsWithoutIntersectionOfSameType()
        {
            List<BasicGraphPath> newPaths = new List<BasicGraphPath>();
            while (paths.Count > 0)
            {
                if (paths.Count == 1)
                {
                    newPaths.Add(paths[0]);
                    paths.Clear();
                }
                else
                {
                    BasicGraphPath path = paths[0];
                    bool unchanged = true;
                    if (path.Start.partOfPath.Count == 2)
                    {
                        for (int i = 1; i < paths.Count - 1; ++i)
                        {
                            BasicGraphPath p2 = paths[i];
                            if ((path.Start == p2.Start || path.Start == p2.End) && path.type == p2.type)
                            {
                                if (path.Start == p2.End)
                                    p2.Reverse();
                                path.Reverse();

                                if (!path.HasAngle(p2))
                                {
                                    paths.Remove(path);
                                    path.Delete();
                                    paths.Remove(p2);
                                    p2.Delete();
                                    paths.Insert(0, new BasicGraphPath(path, p2));
                                    unchanged = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (unchanged && path.End.partOfPath.Count == 2)
                    {
                        for (int i = 1; i < paths.Count - 1; ++i)
                        {
                            BasicGraphPath p2 = paths[i];
                            if ((path.End == p2.Start || path.End == p2.End) && path.type == p2.type)
                            {
                                if (path.End == p2.End)
                                    p2.Reverse();

                                if (!path.HasAngle(p2))
                                {
                                    paths.Remove(path);
                                    path.Delete();
                                    paths.Remove(p2);
                                    p2.Delete();
                                    paths.Insert(0, new BasicGraphPath(path, p2));
                                    unchanged = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (unchanged)
                    {
                        paths.Remove(path);
                        newPaths.Add(path);
                    }
                }
            }
            paths = newPaths;
        }

        internal BasicGraphNode GetNode(Vec3d v)
        {
            foreach (BasicGraphNode n in nodes)
            {
                if (Math.Abs(n.position.X - v.X) < 0.1f &&
                    Math.Abs(n.position.Y - v.Y) < 0.1f &&
                    Math.Abs(n.position.Z - v.Z) < 0.1f)
                    return n;
            }
            BasicGraphNode newNode = new BasicGraphNode(v);
            nodes.Add(newNode);
            return newNode;
        }
    }

    public class BasicGraphNode
    {
        public readonly Vec3d position;
        internal List<BasicGraphPath> partOfPath = new List<BasicGraphPath>();

        public int NrOfConnectedPaths { get { return partOfPath.Count; } }

        internal BasicGraphNode(Vec3d pos)
        {
            this.position = pos;
        }
    }

    public class BasicGraphPath
    {
        public readonly List<BasicGraphNode> list;
        public readonly int type;
        bool reversed = false;

        public BasicGraphNode Start { get { return list[0]; } }
        public BasicGraphNode End { get { return list[list.Count - 1]; } }
        public bool Reversed { get { return reversed; } }

        public void Reverse() { list.Reverse(); reversed = !reversed; }

        internal BasicGraphPath(BasicGraphPath p1, BasicGraphPath p2)
        {
            if (p1.End != p2.Start)
                throw new Exception("This cannot be done!");
            this.list = new List<BasicGraphNode>(p1.list.Count + p2.list.Count - 1);
            foreach (BasicGraphNode bn in p1.list)
            {
                this.list.Add(bn);
            }
            this.list.RemoveAt(this.list.Count - 1);
            foreach (BasicGraphNode bn in p2.list)
                this.list.Add(bn);

            foreach (BasicGraphNode bn in this.list)
                bn.partOfPath.Add(this);
            this.reversed = p1.reversed;
        }

        internal BasicGraphPath(BasicGraph graph, List<Vec3d> list, int type)
        {
            this.list = new List<BasicGraphNode>(list.Count);
            foreach (Vec3d v in list)
            {
                BasicGraphNode bn = graph.GetNode(v);
                this.list.Add(bn);
                bn.partOfPath.Add(this);
            }
            this.type = type;
        }

        internal void Delete()
        {
            foreach (BasicGraphNode bn in this.list)
                bn.partOfPath.Remove(this);
        }

        internal bool HasAngle(BasicGraphPath p2)
        {
            Vec2 t1 = new Vec2((float)this.list[this.list.Count - 2].position.X, (float)this.list[this.list.Count - 2].position.Z);
            Vec2 t2 = new Vec2((float)this.list[this.list.Count - 1].position.X, (float)this.list[this.list.Count - 1].position.Z);
            Vec2 v1 = t2 - t1;
            v1 = v1.normalize();
            t1 = new Vec2((float)p2.list[0].position.X, (float)p2.list[0].position.Z);
            t2 = new Vec2((float)p2.list[1].position.X, (float)p2.list[1].position.Z);
            Vec2 v2 = t2 - t1;
            v2 = v2.normalize();

            return Math.Abs(Vec2.AngleBetweenVectors(v1, v2)) > 0.25f;
        }
    }
}
