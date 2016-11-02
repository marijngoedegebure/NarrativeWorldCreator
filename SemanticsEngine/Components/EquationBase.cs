/**************************************************************************
 * 
 * EquationBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using Common.MathParser;
using Semantics.Components;
using Semantics.Utilities;

namespace SemanticsEngine.Components
{

    #region Class: EquationBase
    /// <summary>
    /// Stores a string representation of an equation, of which the outcome can be calculated by inserting values for particular variables.
    /// </summary>
    public sealed class EquationBase : Base
    {

        #region Properties and Fields

        #region Property: EquationString
        /// <summary>
        /// The equation string.
        /// </summary>
        private String equationString = SemanticsSettings.Values.Equation;
        
        /// <summary>
        /// Gets the equation string.
        /// </summary>
        public String EquationString
        {
            get
            {
                return equationString;
            }
            private set
            {
                if (value != null)
                {
                    // Remove spaces
                    String s = value;
                    String newString = String.Empty;
                    foreach (char c in s)
                    {
                        if (!c.Equals(' '))
                            newString += c;
                    }

                    // Update the equation string and set the new conversion term
                    equationString = newString;
                    SetConversionTerm();
                }
            }
        }
        #endregion Property: EquationString

        #region Property: IsValid
        /// <summary>
        /// Returns whether this equation is valid.
        /// </summary>
        private bool isValid = false;

        /// <summary>
        /// Gets the value that indicates whether this equation is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return isValid;
            }
        }
        #endregion Property: IsValid

        #region Field: conversionTerm
        /// <summary>
        /// The conversion term.
        /// </summary>
        private Common.MathParser.Term conversionTerm = null;
        #endregion Field: conversionTerm
        
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EquationBase(Equation equation)
        /// <summary>
        /// Create a equation base from the given equation.
        /// </summary>
        /// <param name="equation">The equation to create a equation base from.</param>
        internal EquationBase(Equation equation)
            : base(equation)
        {
            if (equation != null)
                this.EquationString = equation.EquationString;
        }
        #endregion Constructor: EquationBase(Equation equation)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Calculate(String variable, float value)
        /// <summary>
        /// Calculates the outcome of the equation when the given value is inserted in the given variable.
        /// </summary>
        /// <param name="variable">The variable in the equation.</param>
        /// <param name="value">The value to insert in the variable.</param>
        /// <returns>The outcome of the equation.</returns>
        internal float Calculate(String variable, float value)
        {
            if (this.conversionTerm == null)
                SetConversionTerm();

            if (this.conversionTerm != null)
            {
                MathFunctionSolverWithCustomConstants evaluator = new MathFunctionSolverWithCustomConstants(null);
                evaluator.AddConstant(variable, (double)value);
                return (float)(double)this.conversionTerm.GetValue(evaluator);
            }

            return 0;
        }
        #endregion Method: Calculate(String variable, float value)

        #region Method: Calculate(String variable1, float value1, String variable2, float value2)
        /// <summary>
        /// Calculates the outcome of the equation when the given values are inserted in the given variables.
        /// </summary>
        /// <param name="variable1">The first variable in the equation.</param>
        /// <param name="value1">The value to insert in the first variable.</param>
        /// <param name="variable2">The second variable in the equation.</param>
        /// <param name="value2">The value to insert in the second variable.</param>
        /// <returns>The outcome of the equation.</returns>
        internal float Calculate(String variable1, float value1, String variable2, float value2)
        {
            if (this.conversionTerm == null)
                SetConversionTerm();

            if (this.conversionTerm != null)
            {
                MathFunctionSolverWithCustomConstants evaluator = new MathFunctionSolverWithCustomConstants(null);
                evaluator.AddConstant(variable1, (double)value1);
                evaluator.AddConstant(variable2, (double)value2);
                return (float)(double)this.conversionTerm.GetValue(evaluator);
            }

            return 0;
        }
        #endregion Method: Calculate(String variable1, float value1, String variable2, float value2)

        #region Method: Calculate(String[] variables, float[] values)
        /// <summary>
        /// Calculates the outcome of the equation when the given values are inserted in the given variables.
        /// </summary>
        /// <param name="variables">The variables in the equation.</param>
        /// <param name="values">The values to insert in the variables.</param>
        /// <returns>The outcome of the equation.</returns>
        internal float Calculate(String[] variables, float[] values)
        {
            if (variables != null && values != null)
            {
                if (this.conversionTerm == null)
                    SetConversionTerm();

                if (this.conversionTerm != null)
                {
                    MathFunctionSolverWithCustomConstants evaluator = new MathFunctionSolverWithCustomConstants(null);

                    if (variables.Length == values.Length)
                    {
                        for (int i = 0; i < variables.Length; i++)
                            evaluator.AddConstant(variables[i], (double)values[i]);
                    }

                    return (float)(double)this.conversionTerm.GetValue(evaluator);
                }
            }

            return 0;
        }
        #endregion Method: Calculate(String[] variables, float[] values)

        #region Method: SetConversionTerm()
        /// <summary>
        /// Try to set the conversion term.
        /// </summary>
        private void SetConversionTerm()
        {
            try
            {
                this.conversionTerm = Common.MathParser.Term.FromString(this.EquationString);
                this.isValid = true;
            }
            catch (Exception)
            {
                this.isValid = false;
            }
        }
        #endregion Method: SetConversionTerm()
		
        #region Method: ToString()
        /// <summary>
        /// Returns the equation string.
        /// </summary>
        /// <returns>The equation string.</returns>
        public override String ToString()
        {
            return this.EquationString;
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: EquationBase

}
