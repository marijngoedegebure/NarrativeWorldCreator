using System;
using System.Collections.Generic;
using System.Text;
using Common.MathParser;
using System.IO;

namespace Common
{
    public abstract class Inequality
    {
        public enum Type { EQUALS, LOWER_EQUALS, LOWER, HIGHER, HIGHER_EQUALS, NOT_EQUAL, UNARY, SPECIAL_CASE };
        protected object element1, element2;
        protected Type m_type;

        protected Inequality()
        {
            m_type = Type.SPECIAL_CASE;
            element1 = null;
            element2 = null;
        }

        protected Inequality(Type type, object term1, object term2)
        {
            this.m_type = type;
            this.element1 = term1;
            this.element2 = term2;
        }

        public Inequality(string inequality)
        {
            inequality = inequality.TrimStart(' ', '\t');
            inequality = inequality.TrimEnd(' ', '\t');
            while (inequality.StartsWith("(") && inequality.EndsWith(")"))
            {
                inequality = inequality.Substring(1, inequality.Length - 2);
                inequality = inequality.TrimStart(' ', '\t');
                inequality = inequality.TrimEnd(' ', '\t');
            }
            string ineqBlank = MathParser.Term.BlankCharsBetweenBrackets(inequality);
            ineqBlank = ineqBlank.Replace("->", "$$");
            string split = "==";
            if (ineqBlank.IndexOf("==") >= 0)
            {
                split = "==";
                m_type = Type.EQUALS;
            }
            else if (ineqBlank.IndexOf("!>=") >= 0)
            {
                split = "!>=";
                m_type = Type.LOWER_EQUALS;
            }
            else if (ineqBlank.IndexOf("!>") >= 0)
            {
                split = "!>";
                m_type = Type.LOWER;
            }
            else if (ineqBlank.IndexOf("<=") >= 0)
            {
                split = "<=";
                m_type = Type.LOWER_EQUALS;
            }
            else if (ineqBlank.IndexOf("<") >= 0)
            {
                split = "<";
                m_type = Type.LOWER;
            }
            else if (ineqBlank.IndexOf(">=") >= 0)
            {
                split = ">=";
                m_type = Type.HIGHER_EQUALS;
            }
            else if (ineqBlank.IndexOf(">") >= 0)
            {
                split = ">";
                m_type = Type.HIGHER;
            }
            else if (ineqBlank.IndexOf("!=") >= 0)
            {
                split = "!=";
                m_type = Type.NOT_EQUAL;
            }
            else
            {
                m_type = Type.UNARY;
            }
            if (m_type == Type.UNARY)
            {
                element1 = ParseElement(inequality);
                element2 = null;
            }
            else
            {
                int spl = ineqBlank.IndexOf(split);
                string s1 = inequality.Substring(0, spl);
                string s2 = inequality.Substring(spl + split.Length);
                element1 = ParseElement(s1);
                element2 = ParseElement(s2);
            }
        }

        protected bool evaluate(double val1, double val2)
        {
            switch (m_type)
            {
                case Type.EQUALS:
                    return val1 == val2;
                case Type.HIGHER:
                    return val1 > val2;
                case Type.HIGHER_EQUALS:
                    return val1 >= val2;
                case Type.LOWER:
                    return val1 < val2;
                case Type.LOWER_EQUALS:
                    return val1 <= val2;
                case Type.NOT_EQUAL:
                    return val1 != val2;
            }
            return false;
        }

        protected bool evaluate(object val1, object val2)
        {
            switch (m_type)
            {
                case Type.EQUALS:
                    return val1 == val2;
                case Type.NOT_EQUAL:
                    return val1 != val2;
                default:
                    throw new Exception("On objects only equals and not equals operations are valid!");
            }
        }

        protected bool evaluate(Common.Vec3 v1, Common.Vec3 v2)
        {
            switch (m_type)
            {
                case Type.EQUALS:
                    return v1 == v2;
                case Type.NOT_EQUAL:
                    return !(v1 != v2);
            }
            throw new Exception("One can only compare vectors with equals or not equals");
        }

        protected abstract object ParseElement(string element);
    }

    public class TermInequality : Inequality
    {
        string original;

        protected TermInequality() { }

        public TermInequality(string inequality) : base(inequality)
        {
            this.original = inequality;
        }

        public TermInequality(Inequality.Type type, Term term1, Term term2)
            : base(type, term1, term2)
        {
        }

        public TermInequality(BinaryReader r)
            : this(r.ReadString())
        {
        }

        public void Save(BinaryWriter w)
        {
            w.Write(original);
        }

        protected override object ParseElement(string element)
        {
            return Term.FromString(element);
        }

        public virtual bool evaluate(CustomTermEvaluater functionsolver)
        {
            if (m_type == Type.UNARY)
            {
                object oo = ((Term)element1).GetValue(functionsolver);
                if (oo is bool)
                    return (bool)oo;
                double val = 0;
                if (oo is double)
                    val = (double)oo;
                else if (oo is int)
                    val = (int)oo;
                else
                    throw new Exception("A unary TermInequality only works with a number or a bool");
                return val != 0;
            }

            object o1 = ((Term)element1).GetValue(functionsolver);
            object o2 = ((Term)element2).GetValue(functionsolver);

            if (o1 is Common.Vec3 || o2 is Common.Vec3)
            {
                if (!(o1 is Common.Vec3 && o2 is Common.Vec3))
                    throw new Exception("An equality can not compare a vector and a non-vector");
                return evaluate((Common.Vec3)o1, (Common.Vec3)o1);
            }

            if (o1 is string || o2 is string)
            {
                if (!(o1 is string && o2 is string))
                    throw new Exception("An equality can not compare a string and a non-string");
                switch (m_type)
                {
                    case Type.EQUALS:
                        return (string)o1 == (string) o2;
                    case Type.NOT_EQUAL:
                        return (string)o1 != (string)o2;
                    default:
                        throw new NotImplementedException();
                }
            }

            double d1 = 0;
            if (o1 is int)
                d1 = (int)o1;
            else if (o1 is bool)
                d1 = (bool)o1 ? 1 : 0;
            else if (o1 is double)
                d1 = (double)o1;
            else
                return evaluate(o1, o2);

            double d2 = 0;
            if (o2 is int)
                d2 = (int)o2;
            else if (o2 is bool)
                d2 = (bool)o2 ? 1 : 0;
            else if (o2 is double)
                d2 = (double)o2;
            else
                return evaluate(o1, o2);
            return evaluate(d1, d2);
        }

        private List<bool> EvaluateList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            if (m_type == Type.UNARY)
            {
                List<object> ooList = ((Term)element1).GetValuesForList(functionsolver, lea);
                List<bool> ret = new List<bool>();
                foreach (object oo in ooList)
                {
                    if (oo is bool)
                        ret.Add((bool)oo);
                    else
                    {
                        double val = 0;
                        if (oo is double)
                            val = (double)oo;
                        else if (oo is int)
                            val = (int)oo;
                        else
                            throw new Exception("A unary TermInequality only works with a number or a bool");
                        ret.Add(val != 0);
                    }
                }
                return ret;
            }

            List<object> o1List = ((Term)element1).GetValuesForList(functionsolver, lea);
            List<object> o2List = ((Term)element2).GetValuesForList(functionsolver, lea);
            List<object>.Enumerator o1e = o1List.GetEnumerator(), o2e = o2List.GetEnumerator();

            try
            {
                List<bool> ret = new List<bool>();
                while (o1e.MoveNext() && o2e.MoveNext())
                {
                    object o1 = o1e.Current, o2 = o2e.Current;
                    double d1 = 0;
                    if (o1 is int)
                        d1 = (int)o1;
                    else
                        d1 = (double)o1;
                    double d2 = 0;
                    if (o2 is int)
                        d2 = (int)o2;
                    else
                        d2 = (double)o2;
                    ret.Add(evaluate(d1, d2));
                }
                return ret;
            }
            catch
            {
                throw new Exception("A TermInequality only works with two numbers");
            }
        }

        public static bool EvaluateAndList(List<TermInequality> inequalities, CustomTermEvaluater evaluator)
        {
            foreach (TermInequality ti in inequalities)
                if (!ti.evaluate(evaluator))
                    return false;
            return true;
        }

        public static bool EvaluateOrList(List<TermInequality> inequalities, CustomTermEvaluater evaluator)
        {
            foreach (TermInequality ti in inequalities)
                if (ti.evaluate(evaluator))
                    return true;
            return false;
        }

        public static List<bool> EvaluateAndListOnList(List<TermInequality> inequalities, 
                                                        CustomTermEvaluater evaluator, ListEvaluationAid lea)
        {
            if (inequalities.Count == 0)
                return null;
            List<bool> ret = inequalities[0].EvaluateList(evaluator, lea);
            
            for (int i = 1; i < inequalities.Count; ++i)
            {
                List<int> trueOnes = new List<int>();
                for (int j = 0; j < ret.Count; ++j)
                    if (ret[j])
                        trueOnes.Add(j);
                if (trueOnes.Count > 0)
                {
                    lea.PushIndicesToCheckList(trueOnes);
                    List<bool> tempRet = inequalities[i].EvaluateList(evaluator, lea);
                    lea.PopIndicesList();
                    List<bool>.Enumerator enumerator = tempRet.GetEnumerator();
                    foreach (int index in trueOnes)
                    {
                        enumerator.MoveNext();
                        ret[index] = enumerator.Current;
                    }
                }
                else
                    return ret;
            }
            return ret;
        }

        internal bool IsConstant()
        {
            if (!(element1 is int) && !(element1 is double))
            {
                if (element1 is Term)
                {
                    if (!((Term)element1).IsConstant())
                        return false;
                }
                else
                    throw new NotImplementedException();
            }
            if (!(element2 is int) && !(element2 is double))
            {
                if (element2 is Term)
                {
                    if (!((Term)element2).IsConstant())
                        return false;
                }
                else
                    throw new NotImplementedException();
            }
            return true;
        }

        internal TermInequality Clone()
        {
            return new TermInequality(this.m_type, ((Term)element1).Clone(null), ((Term)element2).Clone(null));
        }

        internal IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in ((Term)element1).GetConstants())
                yield return c;
            foreach (Constant c in ((Term)element2).GetConstants())
                yield return c;
        }
    }

    public class CombinedTermInequality : TermInequality
    {
        public new enum Type { Unary, And, Or }

        Type type;

        TermInequality eq1, eq2;

        public CombinedTermInequality(string inequality)
        {
            inequality = inequality.TrimStart(' ', '\t');
            inequality = inequality.TrimEnd(' ', '\t');
            while (inequality.StartsWith("(") && inequality.EndsWith(")"))
            {
                inequality = inequality.Substring(1, inequality.Length - 2);
                inequality = inequality.TrimStart(' ', '\t');
                inequality = inequality.TrimEnd(' ', '\t');
            }
            string ineqBlank = MathParser.Term.BlankCharsBetweenBrackets(inequality);
            ineqBlank = ineqBlank.Replace("->", "$$");

            int indexOfOr = ineqBlank.IndexOf("||");
            if (indexOfOr >= 0)
            {
                type = Type.Or;
                eq1 = new CombinedTermInequality(inequality.Substring(0, indexOfOr));
                eq2 = new CombinedTermInequality(inequality.Substring(indexOfOr + 2));
            }
            else
            {
                int indexOfAnd = ineqBlank.IndexOf("&&");
                if (indexOfAnd >= 0)
                {
                    type = Type.And;
                    eq1 = new CombinedTermInequality(inequality.Substring(0, indexOfAnd));
                    eq2 = new CombinedTermInequality(inequality.Substring(indexOfAnd + 2));
                }
                else
                {
                    type = Type.Unary;
                    eq1 = new TermInequality(inequality);
                }
            }
        }

        public override bool evaluate(CustomTermEvaluater functionsolver)
        {
            switch (type)
            {
                case Type.And:
                    return eq1.evaluate(functionsolver) && eq2.evaluate(functionsolver);
                case Type.Or:
                    return eq1.evaluate(functionsolver) || eq2.evaluate(functionsolver);
                case Type.Unary:
                    return ((TermInequality)eq1).evaluate(functionsolver);
            }
            throw new NotImplementedException();
        }
    }

    public class TermInequalityTerm : Term
    {
        TermInequality inequality;

        public TermInequalityTerm(Term parent, int indexInParent, TermInequality inequality)
            : base(parent, indexInParent)
        {
            this.inequality = inequality;
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            return inequality.evaluate(functionsolver);
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            throw new NotImplementedException();
        }

        public override bool IsConstant()
        {
            return inequality.IsConstant();
        }

        public override Term Clone(Term parent)
        {
            return new TermInequalityTerm(parent, indexInParent, inequality.Clone());
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in this.inequality.GetConstants())
                yield return c;
        }
    }

    public class CombinedTermInequalityTerm : Term
    {
        public enum Type { AND, OR };
        Term term1, term2;
        Type type;

        public CombinedTermInequalityTerm(Term parent, int indexInParent, Type type, Term term1, Term term2)
            : base(parent, indexInParent)
        {
            this.type = type;
            this.term1 = term1;
            this.term2 = term2;
        }

        public override object GetValue(CustomTermEvaluater functionsolver)
        {
            try
            {
                object o1 = term1.GetValue(functionsolver);
                bool t1 = o1 is bool ? (bool)o1 : (double)o1 != 0;

                if (type == Type.AND && !t1)
                    return false;
                if (type == Type.OR && t1)
                    return true;

                object o2 = term2.GetValue(functionsolver);

                bool t2 = o2 is bool ? (bool)o2 : (double)o2 != 0;

                switch (type)
                {
                    case Type.AND:
                        return t1 && t2;
                    case Type.OR:
                        return t1 || t2;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public override List<object> GetValuesForList(CustomTermEvaluater functionsolver, ListEvaluationAid lea)
        {
            throw new NotImplementedException();
        }

        public override bool IsConstant()
        {
            return term1.IsConstant() && term2.IsConstant();
        }

        public override Term Clone(Term parent)
        {
            return new CombinedTermInequalityTerm(parent, indexInParent, type, term1.Clone(null), term2.Clone(null));
        }

        public override IEnumerable<Constant> GetConstants()
        {
            foreach (Constant c in term1.GetConstants())
                yield return c;
            foreach (Constant c in term2.GetConstants())
                yield return c;
        }
    }
}
