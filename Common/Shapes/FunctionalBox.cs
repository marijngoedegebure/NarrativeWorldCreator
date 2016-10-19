using System;
using System.Collections.Generic;

using System.Text;
using Common;


namespace Common.Shapes
{
    public class FunctionalBox : Box, FunctionalObject
    {
        public FunctionalBox(Box box) : base(box)
        {
        }

        #region FunctionalObject Members

        public object EvaluateFunction(string functionName, Common.MathParser.CustomTermEvaluater evaluator, params object[] parameters)
        {
            return base.GetValue(functionName);
        }

        public List<object> EvaluateFunctionOnList(string function, Common.MathParser.CustomTermEvaluater evaluator, Common.MathParser.ListEvaluationAid lea, List<object[]> parameters)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
