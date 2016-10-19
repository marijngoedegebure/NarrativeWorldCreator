using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Common.MathParser;

namespace Common.Shapes
{
    public enum BaseShapeType
    {
        Undefined = -1, Point = 0, ExtrudedPoint = 1, Line = 2, ExtrudedLine = 3, Box2Points = 4, BoxDimensions = 5, ExtrudedBox2Points = 6,
        ExtrudedBoxDimensions = 7, Path = 8, FlatShape = 9,
    }

    public class BaseShapeDescription : INotifyPropertyChanged, IStringRepresentative
    {

        public static string[][] parameterNames = new string[][] 
                                                    { 
                                                        new string[] { "X", "Y", "Z" },
                                                        new string[] { "X", "Z", "Minimum Y", "Maximum Y" },
                                                        new string[] { "X1", "Z1", "X2", "Z2", "Y" },
                                                        new string[] { "X1", "Z1", "X2", "Z2", "Minimum Y", "Maximum Y" },
                                                        new string[] { "X1", "Z1", "X2", "Z2", "Y" },
                                                        new string[] { "Center X", "Center Z", "Width", "Length", "Y" },
                                                        new string[] { "X1", "Y1", "Z1", "X2", "Y2", "Z2" },
                                                        new string[] { "Center X", "Center Y", "Center Z", "Width", "Length", "Height" },
                                                        new string[] { "Points" },
                                                        new string[] { "Points", "Y" },
                                                    };

        BaseShapeType type = BaseShapeType.Undefined;
        List<CustomTuple<string, string>> parameterValues = new List<CustomTuple<string, string>>();

        public BaseShapeType Type { get { return type; } set { type = value; UpdateParameterValues(); NotifyPropertyChange("Type"); } }

        public BaseShapeDescription()
        {
        }

        /// <summary>
        /// Clones the base shape description.
        /// </summary>
        /// <param name="baseShapeDescription">The base shape description to clone.</param>
        public BaseShapeDescription(BaseShapeDescription baseShapeDescription)
        {
            if (baseShapeDescription != null)
            {
                this.type = baseShapeDescription.Type;
                foreach (CustomTuple<string, string> tuple in baseShapeDescription.parameterValues)
                    parameterValues.Add(new CustomTuple<string, string>(tuple.Item1, tuple.Item2));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterValues">Parameter values: use this form: "parametername:parametervalue"</param>
        public BaseShapeDescription(BaseShapeType type, params string[] parameterValues)
        {
            this.Type = type;
            foreach (string pv in parameterValues)
            {
                string[] split = pv.Split(':');
                SetParameterValue(split[0], split[1]);
            }
        }

        public static List<string> GetParameterNames(BaseShapeType type)
        {
            List<string> ret = new List<string>();
            if (type != BaseShapeType.Undefined)
            {
                foreach (string s in parameterNames[(int)type])
                    ret.Add(s);
            }
            return ret;
        }

        public List<string> GetParameterNames()
        {
            return GetParameterNames(type);
        }

        public string GetParameterValue(string name)
        {
            return GetTuple(name).Item2;
        }

        public void SetParameterValue(string name, string value)
        {
            GetTuple(name).Item2 = value;
            NotifyPropertyChange("ParameterValues");
        }

        private CustomTuple<string, string> GetTuple(string name)
        {
            foreach (CustomTuple<string, string> t in parameterValues)
                if (t.Item1 == name)
                    return t;
            return null;
        }

        private void UpdateParameterValues()
        {
            List<CustomTuple<string, string>> tuplesToDelete = new List<CustomTuple<string, string>>();
            List<string> newNames = GetParameterNames();
            foreach (CustomTuple<string, string> parameterValue in parameterValues)
                if (!newNames.Contains(parameterValue.Item1))
                {
                    tuplesToDelete.Add(parameterValue);
                }
                else
                    newNames.Remove(parameterValue.Item1);
            foreach (CustomTuple<string, string> tuple in tuplesToDelete)
                parameterValues.Remove(tuple);
            foreach (string s in newNames)
                parameterValues.Add(new CustomTuple<string, string>(s, ""));
            NotifyPropertyChange("ParameterValues");
        }

        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public BaseShape Instantiate(CustomTermEvaluater evaluater)
        {
            float ymin;
            switch (type)
            {
                case BaseShapeType.Point:
                    return new Point(CreateVec3(evaluater, "X", "Y", "Z"));
                case BaseShapeType.ExtrudedPoint:
                    return new ExtrudedPoint(CreateVec2(evaluater, "X", "Z"), f("Minimum Y", evaluater), f("Maximum Y", evaluater));
                case BaseShapeType.Line:
                    return new FlatLine(new Line2(CreateVec2(evaluater, "X1", "Z1"), CreateVec2(evaluater, "X2", "Z2")), f("Y", evaluater));
                case BaseShapeType.ExtrudedLine:
                    ymin = f("Minimum Y", evaluater);
                    return new ExtrudedLine(new Line2(CreateVec2(evaluater, "X1", "Z1"), CreateVec2(evaluater, "X2", "Z2")), ymin, f("Maximum Y", evaluater) - ymin);
                case BaseShapeType.Box2Points:
                    return new FlatBox(new Box2(CreateVec2(evaluater, "X1", "Z1"), CreateVec2(evaluater, "X2", "Z2")), f("Y", evaluater));
                case BaseShapeType.BoxDimensions:
                    Vec2 center = CreateVec2(evaluater, "Center X", "Center Z");
                    Vec2 hd = 0.5f * CreateVec2(evaluater, "Width", "Length");
                    return new FlatBox(new Box2(center - hd, center + hd), f("Y", evaluater));
                case BaseShapeType.ExtrudedBox2Points:
                    return new ExtrudedBox(new Box(CreateVec3(evaluater, "X1", "Y1", "Z1"), CreateVec3(evaluater, "X2", "Y2", "Z2")));
                case BaseShapeType.ExtrudedBoxDimensions:
                    Vec3 center3 = CreateVec3(evaluater, "Center X", "Center Y", "Center Z");
                    Vec3 hd3 = 0.5f * CreateVec3(evaluater, "Width", "Height", "Length");
                    return new ExtrudedBox(new Box(center3 - hd3, center3 + hd3));
                case BaseShapeType.Path:
                    List<Vec3> pointList = CreateVec3List(evaluater, "Points");
                    return new Path(pointList);
                case BaseShapeType.FlatShape:
                    List<Vec2> pointList2d = CreateVec2List(evaluater, "Points");
                    ymin = f("Y", evaluater);
                    return new FlatShape(new Common.Geometry.Shape(pointList2d), ymin);
                case BaseShapeType.Undefined:
                    throw new Exception("You cannot instantiate a BaseShape from an undefined description!");
                default:
                    throw new NotImplementedException();
            }
        }

        private List<Vec2> CreateVec2List(CustomTermEvaluater evaluater, string name)
        {
            string val = GetParameterValue(name);
            List<Vec2> ret = new List<Vec2>();
            List<string> points = Term.ParseCharacterSeparatedListOfStrings(val, ';');
            foreach (string point in points)
            {
                List<string> coordinates = Term.ParseCharacterSeparatedListOfStrings(point, ',');
                if (coordinates.Count != 2)
                    throw new Exception("A shape should contain points with 2 parameters!");
                float x = (float)(double)Term.FromString(coordinates[0]).GetValue(evaluater);
                float y = (float)(double)Term.FromString(coordinates[1]).GetValue(evaluater);
                ret.Add(new Vec2(x, y));
            }
            return ret;
        }

        private List<Vec3> CreateVec3List(CustomTermEvaluater evaluater, string name)
        {
            string val = GetParameterValue(name);
            List<Vec3> ret = new List<Vec3>();
            List<string> points = Term.ParseCharacterSeparatedListOfStrings(val, ';');
            foreach (string point in points)
            {
                List<string> coordinates = Term.ParseCharacterSeparatedListOfStrings(point, ',');
                if (coordinates.Count != 3)
                    throw new Exception("A path should contain points with 3 parameters!");
                float x = (float)(double)Term.FromString(coordinates[0]).GetValue(evaluater);
                float y = (float)(double)Term.FromString(coordinates[1]).GetValue(evaluater);
                float z = (float)(double)Term.FromString(coordinates[2]).GetValue(evaluater);
                ret.Add(new Vec3(x, y, z));
            }
            return ret;
        }

        private Vec3 CreateVec3(CustomTermEvaluater evaluater, string x, string y, string z)
        {
            float fx = f(x, evaluater), fy = f(y, evaluater), fz = f(z, evaluater);
            return new Vec3(fx, fy, fz);
        }

        private Vec2 CreateVec2(CustomTermEvaluater evaluater, string x, string y)
        {
            float fx = f(x, evaluater), fy = f(y, evaluater);
            return new Vec2(fx, fy);
        }

        private float f(string name, CustomTermEvaluater evaluater)
        {
            return (float)(double)Term.FromString(GetParameterValue(name)).GetValue(evaluater);
        }

        public static BaseShapeDescription LoadFromString(string description)
        {
            BaseShapeDescription ret = null;
            int firstSemiColonIndex = description.IndexOf('|');
            if (firstSemiColonIndex == -1)
            {
                ret = new BaseShapeDescription(BaseShapeType.Undefined);
            }
            else
            {
                int type = int.Parse(description.Substring(0, firstSemiColonIndex));
                string[] values = description.Substring(firstSemiColonIndex + 1).Split('|');
                ret = new BaseShapeDescription((BaseShapeType)type, values);
            }
            return ret;
        }

        /// <summary>
        /// Create a string with all class data.
        /// </summary>
        /// <returns>A string with all class data.</returns>
        public string CreateString()
        {
            string ret = "" + ((int)type);
            foreach (CustomTuple<string, string> t in parameterValues)
                ret += "|" + t.Item1 + ":" + t.Item2;
            return ret;
        }

        /// <summary>
        /// Load the class data from the given string.
        /// </summary>
        /// <param name="stringToLoad">The string that contains the class data.</param>
        public void LoadString(string stringToload)
        {
            int firstSemiColonIndex = stringToload.IndexOf('|');
            if (firstSemiColonIndex == -1)
                this.Type = BaseShapeType.Undefined;
            else
            {
                int type = int.Parse(stringToload.Substring(0, firstSemiColonIndex));
                string[] values = stringToload.Substring(firstSemiColonIndex + 1).Split('|');
                this.Type = (BaseShapeType)type;
                foreach (string pv in values)
                {
                    string[] split = pv.Split(':');
                    SetParameterValue(split[0], split[1]);
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
