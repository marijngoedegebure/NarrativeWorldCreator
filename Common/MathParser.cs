using System;
using System.Collections.Generic;
using System.Text;

namespace Common.MathParser
{

    public interface ListEvaluationCaller
    {
        int Size { get; }

        void SetIndex(int index);
    }

    public class ListEvaluationAid
    {
        ListEvaluationCaller caller;
        Stack<List<int>> indicesToCheckStack = new Stack<List<int>>();
        List<int>.Enumerator enumerator = new List<int>.Enumerator();
        int baseCount = -1;

        public ListEvaluationAid(ListEvaluationCaller caller)
        {
            this.caller = caller;
        }

        public void PushIndicesToCheckList(List<int> list)
        {
            enumerator = list.GetEnumerator();
            indicesToCheckStack.Push(list);
        }
        public List<int> PopIndicesList()
        {
            List<int> ret = indicesToCheckStack.Pop();
            if (indicesToCheckStack.Count > 0)
                enumerator = indicesToCheckStack.Peek().GetEnumerator();
            return ret;
        }

        public bool SetNextIndex()
        {
            if (indicesToCheckStack.Count == 0)
            {
                ++baseCount;

                if (baseCount == caller.Size)
                {
                    baseCount = -1;
                    return false;
                }
                caller.SetIndex(baseCount);
                return true;
            }
            if (!enumerator.MoveNext())
            {
                enumerator = indicesToCheckStack.Peek().GetEnumerator();
                return false;
            }

            caller.SetIndex(enumerator.Current);
            return true;
        }

        public int Size()
        {
            if (indicesToCheckStack != null && indicesToCheckStack.Count > 0)
                return indicesToCheckStack.Peek().Count;
            return caller.Size;
        }
    }

    public interface CustomTermEvaluater
    {
        List<object> EvaluateFunctionOnList(string function, List<object[]> terms, ListEvaluationAid lea);
        object EvaluateFunction(string function, object[] terms);
        List<object> EvaluateConstantOnList(string constant, CustomTermEvaluater functionsolver, ListEvaluationAid lea);
        object EvaluateConstant(string constant);
        bool Exists(string constant);
        Random GetRandom();
        void SetRandom(Random random);
    };

    public class CombinedEvaluater : CustomTermEvaluater
    {
        CustomTermEvaluater ev1, ev2;
        public CombinedEvaluater(CustomTermEvaluater ev1, CustomTermEvaluater ev2)
        {
            this.ev1 = ev1;
            this.ev2 = ev2;
        }

        #region CustomTermEvaluater Members

        public List<object> EvaluateFunctionOnList(string function, List<object[]> terms, ListEvaluationAid lea)
        {
            throw new NotImplementedException();
        }

        public object EvaluateFunction(string function, object[] terms)
        {
            if (ev1 == null && ev2 == null)
                return null;
            if (ev1 == null)
                return ev2.EvaluateFunction(function, terms);
            else if (ev2 == null)
                return ev1.EvaluateFunction(function, terms);
            Random r1 = ev1.GetRandom();
            Random r2 = ev2.GetRandom();
            if (r1 == null && r2 != null)
                ev1.SetRandom(r2);
            try
            {
                object ret = ev1.EvaluateFunction(function, terms);
                if (ret == null)
                    return ev2.EvaluateFunction(function, terms);
                return ret;

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public List<object> EvaluateConstantOnList(string constant, CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            throw new NotImplementedException();
        }

        public object EvaluateConstant(string constant)
        {
            if (ev1 == null && ev2 == null)
                return null;
            if (ev1 == null)
                return ev2.EvaluateConstant(constant);
            else if (ev2 == null)
                return ev1.EvaluateConstant(constant);
            object ret = ev1.EvaluateConstant(constant);
            if (ret == null)
                return ev2.EvaluateConstant(constant);
            return ret;
        }

        public Random GetRandom()
        {
            if (ev1 == null && ev2 == null)
                return null;
            if (ev1 == null)
                return ev2.GetRandom();
            else if (ev2 == null)
                return ev1.GetRandom();
            Random r = ev1.GetRandom();
            if (r == null)
                return ev2.GetRandom();
            return r;
        }

        public void SetRandom(Random random)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string constant)
        {
            return ev1.Exists(constant) || ev2.Exists(constant);
        }

        #endregion
    }

    public abstract class MathFunctionSolver : CustomTermEvaluater
    {
        protected abstract object EvaluateExtraFunction(string function, object[] terms);

        #region FunctionSolver Members
        object CustomTermEvaluater.EvaluateFunction(string function, object[] terms)
        {
            if (function == "if")
            {
                if (terms.Length != 3)
                    throw new Exception("function 'if' requires 3 terms: condition, if_true, if_false");
                bool cond = (bool)terms[0];
                if (cond)
                    return terms[1];
                else
                    return terms[2];
            }
            if (function == "sin")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'sin' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'sin' requires a double as term");
                else
                    return Math.Sin((double)terms[0]);
            }
            if (function == "cos")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'cos' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'cos' requires a double as term");
                else
                    return Math.Cos((double)terms[0]);
            }
            if (function == "tan")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'tan' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'tan' requires a double as term");
                else
                    return Math.Tan((double)terms[0]);
            }
            if (function == "asin")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'asin' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'asin' requires a double as term");
                else
                    return Math.Asin((double)terms[0]);
            }
            if (function == "acos")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'acos' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'acos' requires a double as term");
                else
                    return Math.Acos((double)terms[0]);
            }
            if (function == "atan")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'atan' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'atan' requires a double as term");
                else
                    return Math.Atan((double)terms[0]);
            }
            if (function == "atan2")
            {
                if (terms.Length != 2)
                    throw new Exception("function 'atan2' requires 2 terms");
                else if (!(terms[0] is double) || !(terms[1] is double))
                    throw new Exception("function 'atan2' requires 2 doubles as term");
                else
                    return Math.Atan2((double)terms[0], (double)terms[1]);
            }
            if (function == "sqrt")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'sqrt' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'sqrt' requires a double as term");
                else
                    return Math.Sqrt((double)terms[0]);
            }
            if (function == "ceil")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'ceil' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'ceil' requires a double as term");
                else
                    return Math.Ceiling((double)terms[0]);
            }
            if (function == "floor")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'floor' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'floor' requires a double as term");
                else
                    return Math.Floor((double)terms[0]);
            }
            if (function == "isinf")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'isinf' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'isinf' requires a double as term");
                else
                    return double.IsInfinity((double)terms[0]);
            }
            if (function == "isnan")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'isnan' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'isnan' requires a double as term");
                else
                    return double.IsNaN((double)terms[0]);
            }
            if (function == "ln")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'ln' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'ln' requires a double as term");
                else
                    return Math.Log((double)terms[0], Math.E);
            }
            if (function == "p")
            {
                //--- Probability function used for CGA
                //--- http://www.procedural.com:8080/help/index.jsp?topic=/com.procedural.cityengine.help/html/manual/cga/basics/toc.html
                if (terms.Length != 1)
                    throw new Exception("function 'p' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'p' requires a double as term");
                else
                {
                    double prob = (double)terms[0];
                    if (prob < 0)
                        return false;
                    if (prob > 1)
                        return true;
                    return GetRandom().NextDouble() < prob;
                }
            }
            if (function == "log10")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'log10' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'log10' requires a double as term");
                else
                    return Math.Log10((double)terms[0]);
            }
            if (function == "exp")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'exp' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'exp' requires a double as term");
                else
                    return Math.Exp((double)terms[0]);
            }
            if (function == "abs")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'abs' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'abs' requires a double as term");
                else
                    return Math.Abs((double)terms[0]);
            }
            if (function == "int")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'int' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'int' requires a double as term");
                else
                    return (double)(int)((double)terms[0]);
            }
            if (function == "rint")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'rint' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'rint' requires a double as term");
                else
                    return Math.Round((double)terms[0]);
            }
            if (function == "round")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'round' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'round' requires a double as term");
                else
                    return Math.Round((double)terms[0]);
            }
            if (function == "bool")
            {
                //--- cga bool function
                //--- http://www.procedural.com:8080/help/index.jsp?topic=/com.procedural.cityengine.help/html/manual/cga/basics/toc.html
                if (terms.Length != 1)
                    throw new Exception("function 'bool' requires 1 term");
                if (terms[0] is string)
                {
                    string s = ((string)terms[0]).ToLower();
                    if (s == "false" || s == "f")
                        return false;
                    double ts;
                    if (double.TryParse(s, out ts))
                        return ts != 0;
                    return true;
                }
                else if (terms[0] is double)
                    return (double)terms[0] != 0;
                else if (terms[0] is bool)
                    return terms[0];
                else
                    throw new NotImplementedException();
            }
            if (function == "float")
            {
                //--- cga float function
                //--- http://www.procedural.com:8080/help/index.jsp?topic=/com.procedural.cityengine.help/html/manual/cga/basics/toc.html
                if (terms.Length != 1)
                    throw new Exception("function 'float' requires 1 term");
                if (terms[0] is string)
                {
                    double ts;
                    if (double.TryParse((string)terms[0], out ts))
                        return ts;
                    else
                        return double.NaN;
                }
                else if (terms[0] is double)
                    return terms[0];
                else if (terms[0] is bool)
                    return (bool)terms[0] ? (double)1 : (double)0;
                else
                    throw new NotImplementedException();
            }
            if (function == "str")
            {
                //--- cga str function
                //--- http://www.procedural.com:8080/help/index.jsp?topic=/com.procedural.cityengine.help/html/manual/cga/basics/toc.html
                if (terms.Length != 1)
                    throw new Exception("function 'str' requires 1 term");
                return terms[0].ToString();
            }
            if (function == "len")
            {
                //--- cga len function
                //--- http://www.procedural.com:8080/help/index.jsp?topic=/com.procedural.cityengine.help/html/manual/cga/basics/toc.html
                if (terms.Length != 1)
                    throw new Exception("function 'len' requires 1 term");
                return ((string)terms[0]).Length;
            }
            if (function == "count")
            {
                //--- cga count function
                //--- http://www.procedural.com:8080/help/index.jsp?topic=/com.procedural.cityengine.help/html/manual/cga/basics/toc.html
                if (terms.Length != 2)
                    throw new Exception("function 'count' requires 2 terms");
                string input = (string)terms[0];
                string find = (string)terms[1];
                int count = 0;
                int index = 0;
                while ((index = input.IndexOf(find)) > -1)
                {
                    ++index;
                    ++count;
                }
                return (double)count;
            }
            if (function == "find")
            {
                //--- cga find function
                //--- http://www.procedural.com:8080/help/index.jsp?topic=/com.procedural.cityengine.help/html/manual/cga/basics/toc.html
                if (terms.Length != 3)
                    throw new Exception("function 'find' requires 3 terms");
                string input = (string)terms[0];
                string find = (string)terms[1];
                int findIndex = (int)Math.Round((double)terms[2]);
                int count = 0;
                int index = 0;
                while ((index = input.IndexOf(find, index)) > -1)
                {
                    if (count == findIndex)
                        return (double)index;
                    ++index;
                    ++count;
                }
                return (double)-1;
            }
            if (function == "subString")
            {
                //--- cga subString function
                //--- http://www.procedural.com:8080/help/index.jsp?topic=/com.procedural.cityengine.help/html/manual/cga/basics/toc.html
                if (terms.Length != 3)
                    throw new Exception("function 'subString' requires 3 terms");
                string input = (string)terms[0];
                int start = (int)Math.Round((double)terms[1]);
                int end = (int)Math.Round((double)terms[2]);
                return input.Substring(start, end - start);
            }
            if (function == "pow")
            {
                if (terms.Length != 2)
                    throw new Exception("function 'pow' needs 2 terms");
                double v1 = (double)terms[0];
                double v2 = (double)terms[1];
                return Math.Pow(v1, v2);
            }
            if (function == "rand")
            {
                if (terms.Length > 3)
                    throw new Exception("function 'rand' needs 0, 1, 2 or 3 terms");
                if (terms.Length < 3)
                {
                    //--- CGA random functions:
                    //--- http://www.procedural.com:8080/help/index.jsp?topic=/com.procedural.cityengine.help/html/manual/cga/basics/toc.html
                    double min = 0;
                    double max = 1;
                    if (terms.Length == 1)
                        if (!(terms[0] is double))
                            throw new Exception("function 'rand' with 1 term needs a double");
                        else
                            max = (double)terms[0];
                    else if (terms.Length == 2)
                    {
                        if (!(terms[0] is double) || !(terms[1] is double))
                            throw new Exception("function 'rand' with 2 terms needs 2 doubles");
                        min = (double)terms[0];
                        max = (double)terms[1];
                    }
                    return min + (max - min) * GetRandom().NextDouble();
                }
                string type = (string)terms[0];
                double val1 = (double)terms[1];
                double val2 = (double)terms[2];
                switch (type)
                {
                    case "const":
                        return val1 + GetRandom().NextDouble() * (val2 - val1);
                    case "dice":
                        return (double)(int)val1 + GetRandom().Next((int)val2 - (int)val1 + 1);
                    case "normal":
                        return (GetRandom().NextDouble() * 2 - 1) * val2 + val1;
                }
            }
            if (function == "clamp")
            {
                if (terms.Length != 3)
                    throw new Exception("function 'clamp' needs at 3 terms");
                double val0 = (double)terms[0];
                double val1 = (double)terms[1];
                double val2 = (double)terms[2];
                double min = Math.Min(val1, val2);
                double max = Math.Max(val1, val2);
                return Math.Min(Math.Max(min, val0), max);
            }
            if (function == "length")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'length' expects one argument: the list of which to calculate the length");
                if (terms[0] is object[])
                    return (double)((object[])terms[0]).Length;
                return (double)1;
            }
            if (function == "exists")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'exists' expects one argument: the name of the constant of which existance should be checked");
                if (terms[0] is string)
                    return Exists((string)terms[0]);
                throw new Exception("function 'exists' expects one string argument: the name of the constant of which existance should be checked");
            }
            if (function == "gate_print")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'gate_print' expects one argument: the variable to print");
                Console.Out.WriteLine(terms[0].ToString());
                return terms[0];
            }
            if ((function == "vec" && terms.Length == 4) || function == "vec4")
            {
                if (terms.Length != 4)
                    throw new Exception("function 'vec4' expects four argument: the components of the vector");
                return new Vec4((float)(double)terms[0], (float)(double)terms[1], (float)(double)terms[2], (float)(double)terms[3]);
            }
            if ((function == "vec" && terms.Length == 3) || function == "vec3")
            {
                if (terms.Length != 3)
                    throw new Exception("function 'vec3' expects three argument: the components of the vector");
                return new Vec3((float)(double)terms[0], (float)(double)terms[1], (float)(double)terms[2]);
            }
            if ((function == "vec" && terms.Length == 2) || function == "vec2")
            {
                if (terms.Length != 2)
                    throw new Exception("function 'vec2' expects two argument: the components of the vector");
                return new Vec2((float)(double)terms[0], (float)(double)terms[1]);
            }
            if (function == "vecCross")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'vecCross' expects one argument: the vector to get the cross vector of");
                if (terms[0] is Vec2)
                    return ((Vec2)terms[0]).Cross();
                Vec3 v3 = (Vec3)terms[0];
                Vec3 cr = (Vec3)((Vec2)v3).Cross();
                cr.Y = v3.Y;
                return cr;
            }
            if (function == "arr")
                return new object[0];
            if (function == "min")
            {
                if (terms.Length != 2)
                    throw new Exception("function 'min' needs 2 terms");
                double v1 = (double)terms[0];
                double v2 = (double)terms[1];
                return Math.Min(v1, v2);
            }
            if (function == "max")
            {
                if (terms.Length != 2)
                    throw new Exception("function 'max' needs 2 terms");
                double v1 = (double)terms[0];
                double v2 = (double)terms[1];
                return Math.Max(v1, v2);
            }
            if (function == "extrude")
            {
                if (terms.Length != 2)
                    throw new Exception("function 'extrude' needs 2 terms");
                Common.Shapes.BaseShape flatShape = (Common.Shapes.BaseShape)terms[0];
                float h = (float)(double)terms[1];
                if (flatShape is Common.Shapes.Point)
                {
                    Common.Shapes.Point p = (Common.Shapes.Point)flatShape;
                    return new Common.Shapes.ExtrudedPoint(p.Pnt, p.PositionY, p.PositionY + h);
                }
                if (flatShape is Common.Shapes.FlatLine)
                {
                    Common.Shapes.FlatLine l = (Common.Shapes.FlatLine)flatShape;
                    return new Common.Shapes.ExtrudedLine(l.Line, l.PositionY, h);
                }
                if (flatShape is Common.Shapes.FlatShape)
                {
                    Common.Shapes.FlatShape s = (Common.Shapes.FlatShape)flatShape;
                    return new Common.Shapes.ExtrudedShape(s.Shape, s.PositionY, h);
                }
                throw new NotImplementedException();
            }
            if (function == "deg")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'deg' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'deg' requires a double as term");
                else
                    return (double)terms[0] / Math.PI * 180;
            }
            if (function == "rad")
            {
                if (terms.Length != 1)
                    throw new Exception("function 'rad' requires 1 term");
                else if (!(terms[0] is double))
                    throw new Exception("function 'rad' requires a double as term");
                else
                    return (double)terms[0] * Math.PI / 180;
            }
            return EvaluateExtraFunction(function, terms);
        }

        object CustomTermEvaluater.EvaluateConstant(string constant)
        {
            if (constant == "true")
                return (bool)true;
            else if (constant == "false")
                return (bool)false;
            return EvaluateConstantValue(constant);
        }
        #endregion

        public abstract object EvaluateConstantValue(string constant);
        public abstract List<object> EvaluateConstantValueOnList(string constant, ListEvaluationAid lea);
        public abstract bool Exists(string constant);

        #region CustomTermEvaluater Members


        public abstract Random GetRandom();

        public List<object> EvaluateFunctionOnList(string function, List<object[]> terms, ListEvaluationAid lea)
        {
            List<object> objList = new List<object>();
            foreach (object[] termList in terms)
                objList.Add(((CustomTermEvaluater)this).EvaluateFunction(function, termList));
            return objList;
        }

        public List<object> EvaluateConstantOnList(string constant, CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            return EvaluateConstantValueOnList(constant, lea);
        }

        #endregion

        public abstract void SetRandom(Random random);
    }

    public class MathFunctionSolverWithCustomConstants : MathFunctionSolver
    {
        Random random = null;
        object thisObject;
        Dictionary<string, object> constants = new Dictionary<string, object>();

        public MathFunctionSolverWithCustomConstants(object thisObject)
        {
            this.thisObject = thisObject;
        }

        public void AddConstant(string name, object value)
        {
            if (constants.ContainsKey(name))
                constants[name] = value;
            else
                constants.Add(name, value);
        }

        public override object EvaluateConstantValue(string constant)
        {
            if (constant == "this")
                return thisObject;
            if (constants.ContainsKey(constant))
                return constants[constant];
            return constant;
        }

        public override List<object> EvaluateConstantValueOnList(string constant, ListEvaluationAid lea)
        {
            List<object> objList = new List<object>();
            if (constant == "this")
            {
                while(lea.SetNextIndex())
                    objList.Add(thisObject);
            }
            else if (constants.ContainsKey(constant))
            {
                object cons = constants[constant];
                while (lea.SetNextIndex())
                    objList.Add(cons);
            }
            else
                while (lea.SetNextIndex())
                    objList.Add(constant);
            return objList;
        }

        public override Random GetRandom()
        {
            return random;
        }

        public override void SetRandom(Random random)
        {
            this.random = random;
        }

        public override bool Exists(string constant)
        {
            return constant == "this" || constants.ContainsKey(constant);
        }

        protected override object EvaluateExtraFunction(string function, object[] terms)
        {
            return (double)0;
        }
    }

    public class BasicMathFunctionSolver : MathFunctionSolver
    {
        Random rand;

        public BasicMathFunctionSolver(Random rand)
        {
            this.rand = rand;
        }

        public override object EvaluateConstantValue(string constant)
        {
            return constant;
        }

        public override Random GetRandom()
        {
            return rand;
        }

        public override List<object> EvaluateConstantValueOnList(string constant, ListEvaluationAid lea)
        {
            throw new Exception("No variables allowed (" + constant + ")");
        }


        public override void SetRandom(Random random)
        {
            rand = random;
        }

        public override bool Exists(string constant)
        {
            return false;
        }

        protected override object EvaluateExtraFunction(string function, object[] terms)
        {
            return (double)0;
        }
    }

    public class NamedTerm
    {
        public readonly string m_name;
        public readonly Term m_term;

        public NamedTerm(string variable)
        {
            string[] param = variable.Split('=');
            m_name = param[0];
            m_term = Term.FromString(param[1]);
        }

        public NamedTerm(System.Xml.XmlNode node)
        {
            m_name = node.Attributes["name"].Value;
            m_term = Term.FromString(node.Attributes["value"].Value);
        }
    }

    public class TermException : Exception
    {
        public readonly Term term, parent;
        public readonly int indexInParent;

        public int GetIndex()
        {
            if (term == null)
                if (parent == null)
                    return indexInParent;
                else
                    return parent.GetFullIndex(indexInParent);
            else
                return term.GetFullIndex(0);
        }

        public TermException(Term term, string message) : base(message)
        {
            this.term = term;
            this.parent = term.parentTerm;
        }

        public TermException(string message, Term parent, int indexInParent)
            : base(message)
        {
            this.term = null;
            this.parent = parent;
            this.indexInParent = indexInParent;
        }
    }

    public abstract class Term
    {
        public static void AddGlobalFunctionalObject(string name, FunctionalObject fo)
        {
            Constant.staticFunctionalObjects.Add(name, fo);
        }

        public static int GetIndexOfCharacterOutsideParenthesis(string str, char character)
        {
            int parent = 0;
            int index = -1;
            foreach (char ch in str)
            {
                ++index;
                if (ch == '(')
                    parent++;
                else if (ch == ')')
                    parent--;
                else if (parent == 0 && ch == character)
                    return index;
            }
            return -1;
        }

        public readonly Term parentTerm;
        public readonly int indexInParent;

        protected Term(Term parentTerm, int indexInParent)
        {
            this.parentTerm = parentTerm;
            this.indexInParent = indexInParent;
        }

        public static Term FromString(string str)
        {
            return FromString(null, 0, str);
        }

        protected static Term FromString(Term parentOfTerm, int indexInParent, string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new TermException("Empty term in formula", parentOfTerm, indexInParent);

            bool isArray = false;
            int arrayElementOpenIndex = -1;
            str = str.TrimStart(new char[] { ' ' });
            str = str.TrimEnd(new char[] { ' ' });
            if ((str.StartsWith("'") && str.EndsWith("'")) || (str.StartsWith("\"") && str.EndsWith("\"")))
                return new StringTerm(parentOfTerm, indexInParent, str.Substring(1, str.Length - 2));
            else
            {
                #region first pass
                int parent = 0;
                int fplu = -1, fmin = -1, ftim = -1, fdiv = -1, fpow = -1, fumin = -1;
                int prevfmin = -1;
                int index = -1;
                char prevChar = '\0';
                char firstNonSpace = ' ';
                char lastNonSpace = ' ';
                int firstNonSpaceIndex = -1;
                int lastNonSpaceIndex = -1;
                int numOfFirstLevelParent = 0;
                int memberFunctionCall = -1;
                int brackets = 0;
                foreach (char ch in str)
                {
                    ++index;
                    if (ch == '(')
                    {
                        if (parent == 0)
                            numOfFirstLevelParent++;
                        parent++;
                    }
                    else if (ch == ')')
                        parent--;
                    else if (ch == '[')
                        ++brackets;
                    else if (ch == ']')
                        --brackets;
                    else if (parent == 0 && brackets == 0)
                    {
                        if (ch == '+' && fplu < 0)
                            fplu = index;
                        if (ch == '-')
                        {
                            if (lastNonSpace == ')' || lastNonSpace == ']' || ((int)lastNonSpace >= '0' && (int)lastNonSpace <= '9')
                                                        || ((int)lastNonSpace >= 'a' && (int)lastNonSpace <= 'z')
                                                        || ((int)lastNonSpace >= 'A' && (int)lastNonSpace <= 'Z'))
                            {
                                prevfmin = fmin;
                                fmin = index;
                            }
                            else
                            {
                                if (fumin < 0)
                                    fumin = index;
                            }
                        }
                        if (ch == '*' && fplu < 0)
                            ftim = index;
                        if (ch == '/' && fplu < 0)
                            fdiv = index;
                        if (ch == '^' && fplu < 0)
                            fpow = index;
                        if (prevChar == '-' && ch == '>' && fmin == index - 1)
                        {
                            memberFunctionCall = index;
                            fmin = prevfmin;
                        }
                    }

                    if (ch != ' ')
                    {
                        lastNonSpace = ch;
                        lastNonSpaceIndex = index;
                        if (firstNonSpace == ' ')
                        {
                            firstNonSpace = ch;
                            firstNonSpaceIndex = index;
                        }
                    }
                    if (ch == ';' && parent == 1)
                        isArray = true;
                    if (ch == '[' && parent == 0)
                        arrayElementOpenIndex = index;
                    prevChar = ch;
                }
            #endregion

                #region handle pass
                if (parent != 0)
                    throw new TermException("Bad number of parenthesis in " + str, parentOfTerm, indexInParent + index);

                if (firstNonSpace == '(' && lastNonSpace == ')' && numOfFirstLevelParent == 1)
                {
                    if (isArray)
                        return new ArrayTerm(parentOfTerm, firstNonSpaceIndex + 1, str.Substring(firstNonSpaceIndex + 1, lastNonSpaceIndex - firstNonSpaceIndex - 1));
                    else
                        return Term.FromString(parentOfTerm, firstNonSpaceIndex + 1, str.Substring(firstNonSpaceIndex + 1, lastNonSpaceIndex - firstNonSpaceIndex - 1));
                }
                else if (fplu >= 0 || fmin >= 0 || ftim >= 0 || fdiv >= 0 || fpow >= 0)
                {
                    int split = 0;
                    BinaryTerm.Type type = BinaryTerm.Type.Divide;
                    if (fplu >= 0)
                    { split = fplu; type = BinaryTerm.Type.Plus; }
                    else if (fmin >= 0)
                    { split = fmin; type = BinaryTerm.Type.Minus; }
                    else if (ftim >= 0)
                    { split = ftim; type = BinaryTerm.Type.Times; }
                    else if (fdiv >= 0)
                    { split = fdiv; type = BinaryTerm.Type.Divide; }
                    else if (fpow >= 0)
                    { split = fpow; type = BinaryTerm.Type.Power; }

                    return new BinaryTerm(parentOfTerm, indexInParent, type, str.Substring(0, split), str.Substring(split + 1));
                }
                else if (fumin >= 0)
                {
                    for (int i = 0; i < fumin; ++i)
                        if (str[i] != ' ')
                            throw new TermException("Unary minus in wrong position in term: " + str, parentOfTerm, indexInParent + i);
                    return new UnaryTerm(parentOfTerm, indexInParent + fumin + 1, UnaryTerm.Type.UnaryMinus, str.Substring(fumin + 1));
                }
                else if (lastNonSpace == ')')
                {
                    if (memberFunctionCall > -1)
                    {
                        string objectName = str.Substring(0, memberFunctionCall - 1);
                        string functioncall = str.Substring(memberFunctionCall + 1);
                        return new MemberFunctionCall(parentOfTerm, indexInParent, objectName, functioncall);
                    }
                    else
                    {
                        int bracketIndex = str.IndexOf('(');
                        string functionname = str.Substring(0, bracketIndex).Substring(firstNonSpaceIndex);
                        string betweenParent = str.Substring(bracketIndex + 1);
                        betweenParent = betweenParent.Substring(0, betweenParent.LastIndexOf(')'));
                        if (functionname == "switch")
                            return new SwitchTerm(parentOfTerm, indexInParent + bracketIndex + 1, betweenParent);
                        else if (functionname == "if")
                            return new ConditionalTerm(parentOfTerm, indexInParent + bracketIndex + 1, betweenParent);
                        else if (functionname == "optional")
                            return new OptionalInputVariableTerm(parentOfTerm, indexInParent + bracketIndex + 1, betweenParent);
                        else
                            return new FunctionCall(parentOfTerm, indexInParent, str);
                    }
                }
                else if (lastNonSpace == ']' && arrayElementOpenIndex >= 0)
                {
                    string array = str.Substring(0, arrayElementOpenIndex);
                    string temp = str.Substring(arrayElementOpenIndex + 1);
                    string element = temp.Substring(0, temp.Length - 1);
                    return new ArrayElementTerm(parentOfTerm, indexInParent, array, element);
                }

                return new Constant(parentOfTerm, indexInParent, str);
                #endregion
            }
        }

        public abstract object GetValue(CustomTermEvaluater functionsolver);

        public abstract List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea);

        public static List<string> ParseCharacterSeparatedListOfStrings(string list, char character)
        {
            List<string> strlist = new List<string>();
            int parent = 0;
            string temp = "";
            foreach (char ch in list)
            {
                if (ch == '(')
                    parent++;
                else if (ch == ')')
                    parent--;

                if (ch == character && parent == 0)
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

        public static List<StringWithLine> ParseCharacterSeparatedListOfStringsWithLines(string list, char character)
        {
            int line = 0;
            List<StringWithLine> strlist = new List<StringWithLine>();
            int parent = 0;
            string temp = "";
            foreach (char ch in list)
            {
                if (ch == '\r')
                    ++line;
                if (ch == '(')
                    parent++;
                else if (ch == ')')
                    parent--;

                if (ch == character && parent == 0)
                {
                    StringWithLine swl;
                    swl.str = temp.Trim();
                    swl.line = line;
                    strlist.Add(swl);
                    temp = "";
                }
                else
                    temp += ch;
            }
            StringWithLine swl2;
            swl2.str = temp.Trim();
            swl2.line = line;
            strlist.Add(swl2);
            return strlist;
        }

        public struct StringWithLine
        {
            public string str;
            public int line;
        }

        public static string BlankCharsBetweenBrackets(string str)
        {
            string ret = "";
            int parent = 0;
            foreach (char ch in str)
            {
                if (ch == '(')
                    parent++;
                else if (ch == ')')
                    parent--;

                if (parent == 0 && ch != ')')
                    ret += ch;
                else
                    ret += " ";
            }
            return ret;
        }

        protected static List<Term> ParseCharacterSeperatedListOfTerms(Term parent, int indexInParent, string list, char character)
        {
            List<string> strlist = ParseCharacterSeparatedListOfStrings(list, character);

            List<Term> terms = new List<Term>();
            foreach (string str in strlist)
            {
                terms.Add(Term.FromString(parent, indexInParent, str));
                indexInParent += str.Length + 1;
            }
            return terms;
        }

        public static object[] GetValuesFromList(List<Term> termlist, CustomTermEvaluater evaluator)
        {
            object[] list = new object[termlist.Count];
            int count = 0;
            foreach (Term t in termlist)
                list[count++] = t.GetValue(evaluator);
            return list;
        }

        public static float[] GetDoubleValuesFromList(List<Term> termlist, CustomTermEvaluater evaluator)
        {
            float[] list = new float[termlist.Count];
            int count = 0;
            foreach (Term t in termlist)
                list[count++] = (float)(double)t.GetValue(evaluator);
            return list;
        }

        protected static List<Term> ParseCommaSeperatedListOfTerms(Term parent, int indexInParent, string list)
        {
            return ParseCharacterSeperatedListOfTerms(parent, indexInParent, list, ',');
        }

        protected static List<Term> ListFromString(Term parent, int indexInParent, string p)
        {
            return ParseCharacterSeperatedListOfTerms(parent, indexInParent, p, ';');
        }

        protected static double[] ParseMathVector(Term parent, int indexInParent, string p)
        {
            List<Term> pars = ParseCharacterSeperatedListOfTerms(parent, indexInParent, p, ';');
            if (pars.Count != 3)
                throw new TermException("A vector should contain exactly three parameters", parent, indexInParent);
            double[] vals = new double[3];
            BasicMathFunctionSolver bmfs = new BasicMathFunctionSolver(null);
            for (int i = 0; i < 3; ++i)
                vals[i] = (double)pars[i].GetValue(bmfs);
            return vals;
        }

        internal int GetFullIndex(int indexInParent)
        {
            if (parentTerm != null)
                indexInParent += parentTerm.GetFullIndex(0);
            return indexInParent;
        }

        public static List<Term> ListFromString(string instruction)
        {
            return ListFromString(null, 0, instruction);
        }

        public static List<Term> ParseCharacterSeperatedListOfTerms(string list, char character)
        {
            return ParseCharacterSeperatedListOfTerms(null, 0, list, character);
        }

        public static List<Term> ParseCommaSeperatedListOfTerms(string list)
        {
            return ParseCommaSeperatedListOfTerms(null, 0, list);
        }

        public static double[] ParseMathVector(string vect)
        {
            return ParseMathVector(null, 0, vect);
        }

        public static object StringValue(string str, CustomTermEvaluater customTermEvaluater)
        {
            return Term.FromString(str).GetValue(customTermEvaluater);
        }

        public static float[] ToDoubleArray(object array)
        {
            object[] arrayo = (object[])array;
            float[] arrayd = new float[arrayo.Length];
            for (int i = 0; i < arrayo.Length; ++i)
                arrayd[i] = (float)(double)arrayo[i];
            return arrayd;
        }

        public abstract bool IsConstant();

        public abstract Term Clone(Term parent);

        public abstract IEnumerable<Constant> GetConstants();
    }

    public class StringTerm : Term
    {
        string term;

        public StringTerm(Term parent, int indexInParent, string term) : base(parent, indexInParent)
        {
            this.term = term;
        }


        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            return term;
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            List<object> terms = new List<object>();
            while (lea.SetNextIndex())
                terms.Add(term);
            return terms;
        }

        public override bool IsConstant()
        {
            return true;
        }

        public override string ToString()
        {
            return "'" + term + "'";
        }

        public override Term Clone(Term parent)
        {
            return new StringTerm(parent, indexInParent, term);
        }

        public override IEnumerable<Constant> GetConstants()
        {
            yield break;
        }
    }

    public class FunctionCall : Term
    {
        string function;
        List<Term> terms = new List<Term>();

        public FunctionCall(Term parent, int indexInParent, string value) : base(parent, indexInParent)
        {
            function = value.Substring(0, value.IndexOf('(')).TrimEnd(new char[] { ' ' });
            string temp = "";
            for(int i = value.IndexOf('(') + 1; i < value.LastIndexOf(')'); i++)
                temp += value[i];
            temp = temp.TrimStart(new char[] { ' ' });
            temp = temp.TrimEnd(new char[] { ' ' });
            if (temp == "")
                return;
            terms = ParseCharacterSeperatedListOfTerms(this, function.Length + 1, temp, ',');
        }

        public FunctionCall(string function, Term parent, int indexInParent)
            : base(parent, indexInParent)
        {
            this.function = function;
        }

        public void AddTerm(Term term)
        {
            this.terms.Add(term);
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            if (terms.Count == 0)
                return functionsolver.EvaluateFunction(function, null);
            object[] dterms = new object[terms.Count];
            for (int i = 0; i < terms.Count; ++i)
                dterms[i] = terms[i].GetValue(functionsolver);
            return functionsolver.EvaluateFunction(function, dterms);
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            if (terms.Count == 0)
                return functionsolver.EvaluateFunctionOnList(function, null, lea);
            List<object[]> dtermList = new List<object[]>();
            while (lea.SetNextIndex())
                dtermList.Add(new object[terms.Count]);

            for (int i = 0; i < terms.Count; ++i)
            {
                List<object> termsi = terms[i].GetValuesForList(functionsolver, lea);
                int count = 0;
                while (lea.SetNextIndex())
                {
                    (dtermList[count])[i] = termsi[count];
                    ++count;
                }
            }

            return functionsolver.EvaluateFunctionOnList(function, dtermList, lea);
        }

        public override bool IsConstant()
        {
            foreach (Term t in terms)
                if (!t.IsConstant())
                    return false;
            return true;
        }

        public override string ToString()
        {
            string termlist = "";
            foreach(Term t in terms)
            {
                if (termlist != "")
                    termlist += ", ";
                termlist += t.ToString();
            }
            return function + "(" + termlist + ")";
        }

        public override Term Clone(Term parent)
        {
            FunctionCall fc = new FunctionCall(function, parent, indexInParent);
            foreach (Term t in terms)
                fc.AddTerm(t.Clone(fc));
            return fc;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach(Term t in terms)
                foreach (Constant c in t.GetConstants())
                    yield return c;
        }
    }

    public class MemberFunctionCall : Term
    {
        Term obj;
        string function;
        List<Term> terms = new List<Term>();

        public MemberFunctionCall(Term parent, int indexInParent, string objectName, string value) : base(parent, indexInParent)
        {
            string temp2 = objectName;
            objectName = objectName.TrimStart(' ');

            int startIndex = temp2.Length - objectName.Length;
            obj = Term.FromString(this, startIndex, objectName);

            int functionIndex = startIndex + value.IndexOf('(') + 1;
            function = value.Substring(0, value.IndexOf('(')).TrimEnd(' ');
            string temp = "";
            for (int i = value.IndexOf('(') + 1; i < value.LastIndexOf(')'); i++)
                temp += value[i];
            temp = temp.TrimStart(' ' );
            temp = temp.TrimEnd(' ');
            if (temp == "")
                return;
            terms = ParseCharacterSeperatedListOfTerms(this, functionIndex, temp, ',');
        }

        private MemberFunctionCall(Term parent, int indexInParent)
            : base(parent, indexInParent)
        {
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            try
            {
                FunctionalObject fo = (FunctionalObject)obj.GetValue(functionsolver);
                if (terms.Count == 0)
                    return fo.EvaluateFunction(function, functionsolver);
                object[] dterms = new object[terms.Count];
                for (int i = 0; i < terms.Count; ++i)
                    dterms[i] = terms[i].GetValue(functionsolver);
                return fo.EvaluateFunction(function, functionsolver, dterms);
            }
            catch (Exception ex)
            {
                
                throw ex;
            } 
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            List<object> objects = obj.GetValuesForList(functionsolver, lea);

            bool allEqual = true;
            for (int i = 1; i < objects.Count && allEqual; ++i)
                allEqual = objects[0] == objects[i];

            if (terms.Count == 0)
            {
                if (allEqual)
                    return ((FunctionalObject)objects[0]).EvaluateFunctionOnList(function, functionsolver, lea, null);
                List<object> ret = new List<object>();
                foreach (object obj2 in objects)
                    ret.Add(((FunctionalObject)obj2).EvaluateFunction(function, functionsolver));
                return ret;
            }
            List<object[]> dtermList = new List<object[]>();
            while (lea.SetNextIndex())
                dtermList.Add(new object[terms.Count]);

            for (int i = 0; i < terms.Count; ++i)
            {
                List<object> termsi = terms[i].GetValuesForList(functionsolver, lea);
                int count = 0;
                while (lea.SetNextIndex())
                {
                    (dtermList[count])[i] = termsi[count];
                    ++count;
                }
            }

            if (allEqual)
                return ((FunctionalObject)objects[0]).EvaluateFunctionOnList(function, functionsolver, lea, dtermList);
            List<object> ret2 = new List<object>();
            int count2 = 0;
            foreach (object obj2 in objects)
                ret2.Add(((FunctionalObject)obj2).EvaluateFunction(function, functionsolver, dtermList[count2++]));
            return ret2;
        }

        public override bool IsConstant()
        {
            return false;
        }

        public override string ToString()
        {
            string termlist = "";
            foreach (Term t in terms)
            {
                if (termlist != "")
                    termlist += ", ";
                termlist += t.ToString();
            }
            return obj.ToString() + CommonSettings.FunctionCallSymbol + function + "(" + termlist + ")";
        }

        public override Term Clone(Term parent)
        {
            MemberFunctionCall mfc = new MemberFunctionCall(parent, indexInParent);
            mfc.function = function;
            mfc.obj = obj.Clone(mfc);
            foreach (Term t in terms)
                mfc.terms.Add(t.Clone(mfc));
            return mfc;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in obj.GetConstants())
                yield return c;
            foreach(Term t in terms)
                foreach (Constant c in t.GetConstants())
                    yield return c;
        }
    }

    public class MemberFunctionCall2 : Term
    {
        Term obj;
        string function;
        List<Term> parameters;

        public MemberFunctionCall2(Term parent, int indexInParent, Term obj, string function, List<Term> parameters)
            : base(parent, indexInParent)
        {
            this.obj = obj;
            this.function = function;
            this.parameters = parameters;
        }

        public MemberFunctionCall2(Term parent, int indexInParent, string function)
            : base(parent, indexInParent)
        {
            this.function = function;
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            try {
                FunctionalObject fo = (FunctionalObject)obj.GetValue(functionsolver);
                object[] pars = new object[parameters.Count];
                for (int i = 0; i < pars.Length; ++i)
                    pars[i] = parameters[i].GetValue(functionsolver);
                return fo.EvaluateFunction(function, functionsolver, pars);
            } catch (Exception e) {
                throw e;
            }
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            throw new NotImplementedException();
        }

        public override bool IsConstant()
        {
            return false;
        }

        public override Term Clone(Term parent)
        {
            MemberFunctionCall2 mfc2 = new MemberFunctionCall2(parent, indexInParent, function);
            mfc2.obj = obj.Clone(mfc2);
            foreach (Term t in this.parameters)
                mfc2.parameters.Add(t.Clone(mfc2));
            return mfc2;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in obj.GetConstants())
                yield return c;
            foreach(Term t in this.parameters)
                foreach (Constant c in t.GetConstants())
                    yield return c;
        }
    }

    public class Constant : Term
    {
        internal static Dictionary<string, FunctionalObject> staticFunctionalObjects = new Dictionary<string, FunctionalObject>();

        private string value;
        protected double val;
        bool isNumericValue = true;

        public bool IsNumericValue { get { return isNumericValue; } }
        public string Value { get { return value; } }

        public void ChangeStringValue(string newValue)
        {
            if (isNumericValue)
                throw new Exception("Only non-numeric constant values can be changed!");
            this.value = newValue;
        }

        public Constant(Term parent, int indexInParent, string value) : base(parent, indexInParent)
        {
            this.value = value;
            isNumericValue = double.TryParse(value, System.Globalization.NumberStyles.Float, CommonSettings.Culture, out val);
        }

        public Constant(Term parent, int indexInParent, double value)
            : base(parent, indexInParent)
        {
            this.val = value;
            this.value = val.ToString();
            isNumericValue = true;
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            if (isNumericValue)
                return val;
            if (value.ToLower() == "pi")
                return Math.PI;
            if (value.ToLower() == "true")
                return true;
            if (value.ToLower() == "false")
                return false;
            if (value.ToLower() == "null")
                return null;
            if (value == "ARRAY")
                return new object[0];
            if (staticFunctionalObjects.ContainsKey(value))
                return staticFunctionalObjects[value];
            return functionsolver.EvaluateConstant(value);
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            List<object> ret = new List<object>();
            while (lea.SetNextIndex())
                ret.Add(GetValue(functionsolver));
            return ret;
        }

        public override bool IsConstant()
        {
            return true;
        }

        public override string ToString()
        {
            return value;
        }

        public override Term Clone(Term parent)
        {
            Constant c = new Constant(parent, indexInParent, value);
            c.isNumericValue = isNumericValue;
            return c;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            yield return this;
        }
    }

    public class UnaryTerm : Term
    {
        public enum Type { UnaryMinus, Single };

        public readonly Type type;
        private Term term;

        public UnaryTerm(Term parent, int indexInParent, Type type, string term) : base(parent, indexInParent)
        {
            this.type = type;
            this.term = Term.FromString(this, 1, term);
        }

        public UnaryTerm(Term parent, int indexInParent, Type type, Term term)
            : base(parent, indexInParent)
        {
            this.type = type;
            this.term = term;
        }

        private UnaryTerm(Term parent, int indexInParent, Type type)
            : base(parent, indexInParent)
        {
            this.type = type;
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            object val = term.GetValue(functionsolver);
            try
            {
                switch (type)
                {
                    case Type.UnaryMinus:
                        if (val is Vec3)
                            return -(Vec3)val;
                        else if (val is Vec2)
                            return -(Vec2)val;
                        return -(double)val;
                }
            }
            catch (Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            try
            {
                switch (type)
                {
                    case Type.UnaryMinus:
                        List<double> values = BasicFunctionality.ObjectListToDoubleList(term.GetValuesForList(functionsolver, lea));
                        List<object> minValues = new List<object>();
                        foreach (double d in values)
                            minValues.Add(-d);
                        return minValues;
                }
            }
            catch (Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public override bool IsConstant()
        {
            return term.IsConstant();
        }

        public override string ToString()
        {
            switch (type)
            {
                case Type.UnaryMinus:
                    return "-" + term.ToString();
            }
            return base.ToString();
        }

        public override Term Clone(Term parent)
        {
            UnaryTerm ut = new UnaryTerm(parent, indexInParent, type);
            ut.term = term.Clone(ut);
            return ut;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in term.GetConstants())
                yield return c;
        }
    }

    public class BinaryTerm : Term
    {
        public enum Type { Plus, Minus, Times, Divide, Power, Modulo };

        public readonly Type type;
        private Term term1 = null, term2 = null;

        public Term Term1 { get { return term1; } }
        public Term Term2 { get { return term2; } }

        public BinaryTerm(Term parent, int indexInParent, Type type, string term1, string term2) : base (parent, indexInParent)
        {
            this.type = type;
            this.term1 = Term.FromString(this, 0, term1);
            this.term2 = Term.FromString(this, term1.Length + 1, term2);
        }

        public BinaryTerm(Term parent, int indexInParent, Type type)
            : base(parent, indexInParent)
        {
            this.type = type;
        }

        public void SetTerms(Term t1, Term t2)
        {
            term1 = t1;
            term2 = t2;
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            object o1 = Term1.GetValue(functionsolver);
            object o2 = Term2.GetValue(functionsolver);

            if (type == Type.Plus && o1 is object[])
            {
                object[] newArray;
                object[] ar1 = (object[])o1;
                if (o2 is object[])
                {
                    object[] ar2 = (object[])o2;
                    newArray = new object[ar1.Length + ar2.Length];
                    int count = 0;
                    foreach (object o in ar1)
                        newArray[count++] = o;
                    foreach (object o in ar2)
                        newArray[count++] = o;
                }
                else
                {
                    newArray = new object[ar1.Length + 1];
                    int count = 0;
                    foreach (object o in ar1)
                        newArray[count++] = o;
                    newArray[count++] = o2;
                }
                return newArray;
            }

            if (type == Type.Plus && (o1 is string || o2 is string))
            {
                if (o1 is string && o2 is Vec2)
                    return (string)o1 + ((Vec2)o2).X + ", " + ((Vec2)o2).Y;
                if (o1 is string && o2 is Vec3)
                    return (string)o1 + ((Vec3)o2).X + ", " + ((Vec3)o2).Y + ", " + ((Vec3)o2).Z;
                string s1 = o1 is string ? (string)o1 : "" + (double)o1;
                string s2 = o2 is string ? (string)o2 : "" + (double)o2;
                return s1 + s2;
            }

            if (!(o1 is double) || !(o2 is double))
                throw new Exception("BinaryTerms only allow doubles as terms, not: [" + o1.ToString() + "] or [" + o2.ToString() + "]");
            double v1 = (double)o1;
            double v2 = (double)o2;
            switch (type)
            {
                case Type.Plus:
                    return v1 + v2;
                case Type.Minus:
                    return v1 - v2;
                case Type.Times:
                    return v1 * v2;
                case Type.Divide:
                    return v1 / v2;
                case Type.Power:
                    return Math.Pow(v1, v2);
                case Type.Modulo:
                    return v1 % v2;
            }
            return 0;
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            List<object> o1List = Term1.GetValuesForList(functionsolver, lea);
            List<object> o2List = Term2.GetValuesForList(functionsolver, lea);
            List<object> ret = new List<object>();
            List<object>.Enumerator o1e = o1List.GetEnumerator(), o2e = o2List.GetEnumerator();
            while (o1e.MoveNext() && o2e.MoveNext())
            {
                object result = null;
                object o1 = o1e.Current;
                object o2 = o2e.Current;
                if (!(o1 is double || o1 is int) || !(o2 is double || o2 is int))
                    throw new Exception("BinaryTerms only allow doubles as terms, not: [" + o1.ToString() + "] or [" + o2.ToString() + "]");
                double v1 = 0;
                if (o1 is double)
                    v1 = (double)o1;
                else if (o1 is int)
                    v1 = (int)o1;
                else
                    throw new Exception("BinaryTerms only allow ints or doubles as terms, not: [" + o1.ToString() + "]");
                double v2 = 0;
                if (o2 is double)
                    v2 = (double)o2;
                else if (o2 is int)
                    v2 = (int)o2;
                else
                    throw new Exception("BinaryTerms only allow ints or doubles as terms, not: [" + o2.ToString() + "]");
                switch (type)
                {
                    case Type.Plus:
                        result = v1 + v2;
                        break;
                    case Type.Minus:
                        result = v1 - v2;
                        break;
                    case Type.Times:
                        result = v1 * v2;
                        break;
                    case Type.Divide:
                        result = v1 / v2;
                        break;
                    case Type.Power:
                        result = Math.Pow(v1, v2);
                        break;
                    case Type.Modulo:
                        result = v1 % v2;
                        break;
                }
                ret.Add(result);
            }
            return ret;
        }

        public override bool IsConstant()
        {
            return Term1.IsConstant() && Term2.IsConstant();
        }

        public override string ToString()
        {
            switch (type)
            {
                case Type.Plus:
                    return term1.ToString() + "+" + term2.ToString();
                case Type.Minus:
                    return term1.ToString() + "-" + term2.ToString();
                case Type.Times:
                    return term1.ToString() + "*" + term2.ToString();
                case Type.Divide:
                    return term1.ToString() + "/" + term2.ToString();
                case Type.Power:
                    return term1.ToString() + "^" + term2.ToString();
                case Type.Modulo:
                    return term1.ToString() + "%" + term2.ToString();
            }
            return base.ToString();
        }

        public override Term Clone(Term parent)
        {
            BinaryTerm bt = new BinaryTerm(parent, indexInParent, type);
            bt.term1 = term1.Clone(bt);
            bt.term2 = term2.Clone(bt);
            return bt;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in term1.GetConstants())
                yield return c;
            foreach (Constant c in term2.GetConstants())
                yield return c;
        }
    }

    public class ArrayTerm : Term
    {
        public readonly List<Term> m_elements;

        public ArrayTerm(Term parent, int indexInParent, string elements) : base(parent, indexInParent)
        {
            m_elements = ListFromString(this, indexInParent, elements);
        }

        public ArrayTerm(Term parent, int indexInParent)
            : base(parent, indexInParent)
        {
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            object[] values = new object[m_elements.Count];
            int count = 0;
            foreach (Term t in m_elements)
            {
                object o = t.GetValue(functionsolver);
                values[count++] = o;
            }
            return values;
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            List<object[]> valuesList = new List<object[]>();
            while (lea.SetNextIndex())
                valuesList.Add(new object[m_elements.Count]);
            int count = 0;
            foreach (Term t in m_elements)
            {
                List<object> oList = t.GetValuesForList(functionsolver, lea);
                int count2 = 0;
                List<object>.Enumerator e = oList.GetEnumerator();
                while (e.MoveNext())
                {
                    valuesList[count2][count] = e.Current;
                    ++count2;
                }
                ++count;
            }
            List<object> ret = new List<object>();
            foreach (object[] dl in valuesList)
                ret.Add(dl);
            return ret;
        }

        public override bool IsConstant()
        {
            foreach (Term t in this.m_elements)
                if (!t.IsConstant())
                    return false;
            return true;
        }

        public override Term Clone(Term parent)
        {
            ArrayTerm at = new ArrayTerm(parent, indexInParent);
            foreach (Term t in this.m_elements)
                at.m_elements.Add(t.Clone(at));
            return at;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach(Term t in m_elements)
                foreach (Constant c in t.GetConstants())
                    yield return c;
        }
    }

    public class ArrayElementTerm : Term
    {
        Term m_array;
        Term m_element;

        public ArrayElementTerm(Term parent, int indexInParent, string array, string element) : base(parent, indexInParent)
        {
            m_array = Term.FromString(this, 0, array);
            m_element = Term.FromString(this, array.Length + 1, element);
        }

        public ArrayElementTerm(Term parent, int indexInParent, Term array, Term element)
            : base(parent, indexInParent)
        {
            m_array = array;
            m_element = element;
        }

        public ArrayElementTerm(Term parent, int indexInParent)
            : base(parent, indexInParent)
        {
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            int element = (int)(double)m_element.GetValue(functionsolver);
            //try
            //{
                object[] list = (object[])m_array.GetValue(functionsolver);
                if (list.Length <= element)
                    throw new Exception("ArrayElementTerm: element '" + element + "' not available in " + list);
                object ret = list[element];
                return ret;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            List<object> elementValues = m_element.GetValuesForList(functionsolver, lea);
            List<object> arrayValues = m_array.GetValuesForList(functionsolver, lea);
            List<object> ret = new List<object>();
            List<object>.Enumerator e1 = elementValues.GetEnumerator(), e2 = arrayValues.GetEnumerator();
            while (e1.MoveNext() && e2.MoveNext())
                ret.Add(((object[])e2.Current)[(int)(double)e1.Current]);
            return ret;
        }

        public override bool IsConstant()
        {
            return (m_array.IsConstant() && m_element.IsConstant());
        }

        public override Term Clone(Term parent)
        {
            ArrayElementTerm aet = new ArrayElementTerm(parent, indexInParent);
            aet.m_array = m_array.Clone(aet);
            aet.m_element = m_element.Clone(aet);
            return aet;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in m_array.GetConstants())
                yield return c;
            foreach (Constant c in m_element.GetConstants())
                yield return c;
        }
    }

    public class SwitchTerm : Term
    {
        Dictionary<double, Term> m_cases = new Dictionary<double, Term>();
        Term m_defaultTerm = null;
        Term m_termToCheck = null;

        public SwitchTerm(Term parent, int indexInParent, string str) : base(parent, indexInParent)
        {
            string startStr = str;
            int comma = Term.GetIndexOfCharacterOutsideParenthesis(str, ',');
            m_termToCheck = Term.FromString(this, 0, str.Substring(0, comma));
            str = str.Substring(comma + 1);
            while ((comma = Term.GetIndexOfCharacterOutsideParenthesis(str, ',')) >= 0)
            {
                AddCase(startStr.Length - str.Length, str.Substring(0, comma));
                str = str.Substring(comma + 1);
            }
            AddCase(startStr.Length - str.Length, str);
        }

        public SwitchTerm(Term parent, int indexInParent)
            : base(parent, indexInParent)
        {
        }

        private void AddCase(int indexInStr, string str)
        {
            int colon = Term.GetIndexOfCharacterOutsideParenthesis(str, ':');
            string key = str.Substring(0, colon);
            Term value = Term.FromString(this, indexInStr, str.Substring(colon + 1));
            if (key == "default")
                m_defaultTerm = value;
            else
                m_cases.Add(double.Parse(key, CommonSettings.Culture), value);
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            double val = (double)m_termToCheck.GetValue(functionsolver);
            Dictionary<double, Term>.Enumerator enumerator = m_cases.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (val == enumerator.Current.Key)
                    return enumerator.Current.Value.GetValue(functionsolver);
            }
            return m_defaultTerm.GetValue(functionsolver);
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            List<double> vals = BasicFunctionality.ObjectListToDoubleList(m_termToCheck.GetValuesForList(functionsolver, lea));
            if (BasicFunctionality.AllEqual(vals))
                return m_cases[vals[0]].GetValuesForList(functionsolver, lea);
            List<object> ret = new List<object>();
            while (lea.SetNextIndex())
                ret.Add(null);
            Dictionary<double, Term>.Enumerator enumerator = m_cases.GetEnumerator();
            while (enumerator.MoveNext())
            {
                List<int> thisVal = new List<int>();
                for (int i = 0; i < vals.Count; ++i)
                    if (vals[i] == enumerator.Current.Key)
                        thisVal.Add(i);
                if (thisVal.Count > 0)
                {
                    lea.PushIndicesToCheckList(thisVal);
                    List<object> tempList = enumerator.Current.Value.GetValuesForList(functionsolver, lea);
                    lea.PopIndicesList();
                    List<object>.Enumerator enumerator2 = tempList.GetEnumerator();
                    foreach(int index in thisVal)
                    {
                        enumerator2.MoveNext();
                        ret[index] = enumerator2.Current;
                    }
                }
            }
            return ret;
        }

        public override bool IsConstant()
        {
            foreach (Term t in m_cases.Values)
                if (!t.IsConstant())
                    return false;
            return this.m_termToCheck.IsConstant() && this.m_defaultTerm.IsConstant();
        }

        public override Term Clone(Term parent)
        {
            SwitchTerm st = new SwitchTerm(parent, indexInParent);
            st.m_termToCheck = m_termToCheck.Clone(st);
            st.m_defaultTerm = m_defaultTerm.Clone(st);
            Dictionary<double, Term>.Enumerator e = m_cases.GetEnumerator();
            while (e.MoveNext())
                st.m_cases.Add(e.Current.Key, e.Current.Value.Clone(st));
            return st;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in m_defaultTerm.GetConstants())
                yield return c;
            foreach (Constant c in m_termToCheck.GetConstants())
                yield return c;
            foreach(Term t in m_cases.Values)
                foreach (Constant c in t.GetConstants())
                    yield return c;
        }
    }

    public class ConditionalTerm : Term
    {
        TermInequality m_condition;
        Term m_true, m_false;

        public ConditionalTerm(Term parent, int indexInParent, string str) : base(parent, indexInParent)
        {
            List<string> terms = ParseCharacterSeparatedListOfStrings(str, ',');
            if (terms.Count != 3)
                throw new TermException("Error in if statement: '" + str + 
                                "': exactly three terms expected: if(condition,true-value,false-value)", 
                                this, str.Length - 1);
            try
            {
                m_condition = new TermInequality(terms[0]);
            }
            catch (Exception ex)
            {
                throw new TermException(this, "Error in if condition " + terms[0] + ": " + ex.Message);
            }
            m_true = Term.FromString(this, terms[0].Length + 1, terms[1]);
            m_false = Term.FromString(this, terms[0].Length + terms[1].Length + 2, terms[2]);
        }

        private ConditionalTerm(Term parent, int indexInParent)
            : base(parent, indexInParent)
        {
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            if (m_condition.evaluate(functionsolver))
                return m_true.GetValue(functionsolver);
            else
                return m_false.GetValue(functionsolver);
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            throw new NotImplementedException();
        }

        public override bool IsConstant()
        {
            return m_condition.IsConstant() && m_true.IsConstant() && m_false.IsConstant();
        }

        public override Term Clone(Term parent)
        {
            ConditionalTerm ct = new ConditionalTerm(parent, indexInParent);
            ct.m_condition = m_condition.Clone();
            ct.m_true = m_true.Clone(ct);
            ct.m_false = m_false.Clone(ct);
            return ct;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in m_condition.GetConstants())
                yield return c;
            foreach (Constant c in m_true.GetConstants())
                yield return c;
            foreach (Constant c in m_false.GetConstants())
                yield return c;
        }
    }

    public class OptionalInputVariableTerm : Term
    {
        string m_inputVariableName;
        Term m_optional;

        public OptionalInputVariableTerm(Term parent, int indexInParent, string str) : base(parent, indexInParent)
        {
            List<string> terms = ParseCharacterSeparatedListOfStrings(str, ',');
            if (terms.Count != 2)
                throw new TermException(this, "Error in optional statement: '" + str + "': exactly two terms expected: optional(inputvariablename,value)");
            m_inputVariableName = terms[0];
            m_optional = Term.FromString(this, terms[0].Length + 1, terms[1]);
        }

        private OptionalInputVariableTerm(Term parent, int indexInParent)
            : base(parent, indexInParent)
        {
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            object cnst = functionsolver.EvaluateConstant(m_inputVariableName);
            if (cnst is string && (string)cnst == m_inputVariableName)
                return m_optional.GetValue(functionsolver);
            return cnst;
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            List<object> cnstList = functionsolver.EvaluateConstantOnList(m_inputVariableName, functionsolver, lea);
            List<int> optionals = new List<int>();
            for (int i = 0; i < cnstList.Count; ++i)
                if (cnstList[i] is string && (string)cnstList[i] == m_inputVariableName)
                    optionals.Add(i);
            if (optionals.Count == 0)
                return cnstList;
            lea.PushIndicesToCheckList(optionals);
            List<object> optionalTerms = m_optional.GetValuesForList(functionsolver, lea);
            lea.PopIndicesList();
            List<object>.Enumerator enumerator = optionalTerms.GetEnumerator();
            foreach (int i in optionals)
            {
                enumerator.MoveNext();
                cnstList[i] = enumerator.Current;
            }
            return cnstList;
        }

        public override bool IsConstant()
        {
            return false;
        }

        public override Term Clone(Term parent)
        {
            OptionalInputVariableTerm oivt = new OptionalInputVariableTerm(parent, indexInParent);
            oivt.m_inputVariableName = m_inputVariableName;
            oivt.m_optional = m_optional.Clone(oivt);
            return oivt;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in m_optional.GetConstants())
                yield return c;
        }
    }

    public class NotTerm : Term
    {
        Term subTerm;

        public NotTerm(Term parent, int indexInParent, Term subTerm)
            : base(parent, indexInParent)
        {
            this.subTerm = subTerm;
        }

        private NotTerm(Term parent, int indexInParent)
            : base(parent, indexInParent)
        {
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            return !(bool)subTerm.GetValue(functionsolver);
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            throw new NotImplementedException();
        }

        public override bool IsConstant()
        {
            return subTerm.IsConstant();
        }

        public override Term Clone(Term parent)
        {
            NotTerm nt = new NotTerm(parent, indexInParent);
            nt.subTerm = subTerm.Clone(nt);
            return nt;
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in subTerm.GetConstants())
                yield return c;
        }
    }
}
