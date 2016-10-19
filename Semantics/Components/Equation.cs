/**************************************************************************
 * 
 * Equation.cs
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
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: Equation
    /// <summary>
    /// Stores a string representation of an equation, of which the outcome can be calculated by inserting values for particular variables.
    /// </summary>
    public sealed class Equation : IdHolder
    {

        #region Properties and Fields

        #region Property: EquationString
        /// <summary>
        /// Gets or sets the equation string.
        /// </summary>
        public String EquationString
        {
            get
            {
                return Database.Current.Select<String>(this.ID, ValueTables.Equation, Columns.Equation);
            }
            set
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

                    // Update the database and set the new conversion term
                    Database.Current.QueryBegin();
                    Database.Current.Update(this.ID, ValueTables.Equation, Columns.Equation, newString);
                    Database.Current.QueryCommit();
                    SetConversionTerm();
                    NotifyPropertyChanged("EquationString");
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

        #region Constructor: Equation()
        /// <summary>
        /// Creates a default equation.
        /// </summary>
        public Equation()
            : this(SemanticsSettings.Values.Equation)
        {
        }
        #endregion Constructor: Equation(String equation)

        #region Constructor: Equation(uint id)
        /// <summary>
        /// Creates a new equation from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an equation from.</param>
        internal Equation(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Equation(uint id)

        #region Constructor: Equation(Equation equation)
        /// <summary>
        /// Clones an equation.
        /// </summary>
        /// <param name="equation">The equation to clone.</param>
        public Equation(Equation equation)
            : base()
        {
            if (equation != null)
            {
                Database.Current.StartChange();
                
                this.EquationString = equation.EquationString;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Equation(Equation equation)

        #region Constructor: Equation(String equation)
        /// <summary>
        /// Creates a new equation from the given string.
        /// </summary>
        /// <param name="equation">The string to create an equation from.</param>
        public Equation(String equation)
            : base()
        {
            Database.Current.StartChange();

            if (String.IsNullOrEmpty(equation))
                equation = SemanticsSettings.Values.Equation;

            this.EquationString = equation;

            Database.Current.StopChange();
        }
        #endregion Constructor: Equation(String equation)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Calculate(String variable, float value)
        /// <summary>
        /// Calculates the outcome of the equation when the given value is inserted in the given variable.
        /// </summary>
        /// <param name="variable">The variable in the equation.</param>
        /// <param name="value">The value to insert in the variable.</param>
        /// <returns>The outcome of the equation.</returns>
        public float Calculate(String variable, float value)
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
        public float Calculate(String variable1, float value1, String variable2, float value2)
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
        public float Calculate(String[] variables, float[] values)
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
                string equation = this.EquationString;
                if (equation != null)
                {
                    this.conversionTerm = Common.MathParser.Term.FromString(equation);
                    this.isValid = true;
                }
                else
                    this.isValid = false;
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
    #endregion Class: Equation

}
