using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public interface FunctionalObject
    {
        object EvaluateFunction(string functionName, MathParser.CustomTermEvaluater evaluator, params object[] parameters);

        List<object> EvaluateFunctionOnList(string function, MathParser.CustomTermEvaluater evaluator, 
                                                Common.MathParser.ListEvaluationAid lea, 
                                                List<object[]> parameters);
    }
}
