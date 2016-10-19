/**************************************************************************
 * 
 * Toolbox.cs
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
using System.Globalization;
using Common;
using Semantics.Abstractions;

namespace Semantics.Utilities
{

    #region Class: Toolbox
    /// <summary>
    /// Several useful tools, which can be used by several classes.
    /// </summary>
    public static class Toolbox
    {

        #region Method: CalcValue(int first, ChangeType changeType, int second)
        /// <summary>
        /// Calculate which value the first value should get when the given change is applied with the second value.
        /// </summary>
        /// <param name="first">The original value.</param>
        /// <param name="changeType">The change to apply.</param>
        /// <param name="second">The second value.</param>
        /// <returns>The new value for the first value.</returns>
        public static int CalcValue(int first, ValueChangeType changeType, int second)
        {
            switch (changeType)
            {
                case ValueChangeType.Increase:
                    return first + second;
                case ValueChangeType.Decrease:
                    return first - second;
                case ValueChangeType.Equate:
                    return second;
                case ValueChangeType.Multiply:
                    return first * second;
                case ValueChangeType.Divide:
                    if (second != 0)
                        return first / second;
                    break;
                default:
                    return 0;
            }
            return 0;
        }
        #endregion Method: CalcValue(int first, ChangeType changeType, int second)

        #region Method: CalcValue(float first, ChangeType changeType, float second)
        /// <summary>
        /// Calculate which value the first value should get when the given change is applied with the second value.
        /// </summary>
        /// <param name="first">The original value.</param>
        /// <param name="changeType">The change to apply.</param>
        /// <param name="second">The second value.</param>
        /// <returns>The new value for the first value.</returns>
        public static float CalcValue(float first, ValueChangeType changeType, float second)
        {
            switch (changeType)
            {
                case ValueChangeType.Increase:
                    return first + second;
                case ValueChangeType.Decrease:
                    return first - second;
                case ValueChangeType.Equate:
                    return second;
                case ValueChangeType.Multiply:
                    return first * second;
                case ValueChangeType.Divide:
                    if (second != 0)
                        return first / second;
                    break;
                default:
                    return 0;
            }
            return 0;
        }
        #endregion Method: CalcValue(float first, ChangeType changeType, float second)

        #region Method: CalcValue(Vec2 first, ChangeType changeType, Vec2 second)
        /// <summary>
        /// Calculate the vector when the first value is changed by the second one.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="changeType">The type of change.</param>
        /// <param name="second">The second value.</param>
        /// <returns>A new value, in which the first value is changed by the second one.</returns>
        public static Vec2 CalcValue(Vec2 first, ValueChangeType changeType, Vec2 second)
        {
            if (first != null && second != null)
            {
                switch (changeType)
                {
                    case ValueChangeType.Increase:
                        return first + second;
                    case ValueChangeType.Decrease:
                        return first - second;
                    case ValueChangeType.Equate:
                        return second;
                    case ValueChangeType.Multiply:
                        return new Vec2(first.X * second.X, first.Y * second.Y);
                    case ValueChangeType.Divide:
                        if (second.X != 0 && second.Y != 0)
                            return new Vec2(first.X / second.X, first.Y / second.Y);
                        break;
                    default:
                        return Vec2.Zero;
                }
            }
            return null;
        }
        #endregion Method: CalcValue(Vec2 first, ChangeType changeType, Vec2 second)

        #region Method: CalcValue(Vec3 first, ChangeType changeType, Vec3 second)
        /// <summary>
        /// Calculate the vector when the first value is changed by the second one.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="changeType">The type of change.</param>
        /// <param name="second">The second value.</param>
        /// <returns>A new value, in which the first value is changed by the second one.</returns>
        public static Vec3 CalcValue(Vec3 first, ValueChangeType changeType, Vec3 second)
        {
            if (first != null && second != null)
            {
                switch (changeType)
                {
                    case ValueChangeType.Increase:
                        return first + second;
                    case ValueChangeType.Decrease:
                        return first - second;
                    case ValueChangeType.Equate:
                        return second;
                    case ValueChangeType.Multiply:
                        return new Vec3(first.X * second.X, first.Y * second.Y, first.Z * second.Z);
                    case ValueChangeType.Divide:
                        if (second.X != 0 && second.Y != 0 && second.Z != 0)
                            return new Vec3(first.X / second.X, first.Y / second.Y, first.Z / second.Z);
                        break;
                    default:
                        return Vec3.Zero;
                }
            }
            return null;
        }
        #endregion Method: CalcValue(Vec3 first, ChangeType changeType, Vec3 second)

        #region Method: CalcValue(Vec4 first, ChangeType changeType, Vec4 second)
        /// <summary>
        /// Calculate the vector when the first value is changed by the second one.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="changeType">The type of change.</param>
        /// <param name="second">The second value.</param>
        /// <returns>A new value, in which the first value is changed by the second one.</returns>
        public static Vec4 CalcValue(Vec4 first, ValueChangeType changeType, Vec4 second)
        {
            if (first != null && second != null)
            {
                switch (changeType)
                {
                    case ValueChangeType.Increase:
                        return first + second;
                    case ValueChangeType.Decrease:
                        return first - second;
                    case ValueChangeType.Equate:
                        return second;
                    case ValueChangeType.Multiply:
                        return new Vec4(first.X * second.X, first.Y * second.Y, first.Z * second.Z, first.W * second.W);
                    case ValueChangeType.Divide:
                        if (second.X != 0 && second.Y != 0 && second.Z != 0 && second.W != 0)
                            return new Vec4(first.X / second.X, first.Y / second.Y, first.Z / second.Z, first.W / second.W);
                        break;
                    default:
                        return Vec4.Zero;
                }
            }
            return null;
        }
        #endregion Method: CalcValue(Vec4 first, ChangeType changeType, Vec4 second)

        #region Method: Compare(int first, EqualitySignExtended sign, int second)
        /// <summary>
        /// Checks whether the two integers satisfy the condition of the sign.
        /// </summary>
        /// <param name="first">The integer to compare.</param>
        /// <param name="sign">The sign to compare both integers to.</param>
        /// <param name="second">The integer to compare to.</param>
        /// <returns>Returns whether the two integers satisfy the condition of the sign.</returns>
        public static bool Compare(int first, EqualitySignExtended sign, int second)
        {
            return Compare((float)first, sign, (float)second);
        }
        #endregion Method: Compare(int first, EqualitySignExtended sign, int second)

        #region Method: Compare(float first, EqualitySignExtended sign, float second)
        /// <summary>
        /// Checks whether the two floats satisfy the condition of the sign.
        /// </summary>
        /// <param name="first">The float to compare.</param>
        /// <param name="sign">The sign to compare both floats to.</param>
        /// <param name="second">The float to compare to.</param>
        /// <returns>Returns whether the two floats satisfy the condition of the sign.</returns>
        public static bool Compare(float first, EqualitySignExtended sign, float second)
        {
            // Perform the correct check
            switch (sign)
            {
                case EqualitySignExtended.Equal:
                    return first == second;
                case EqualitySignExtended.NotEqual:
                    return first != second;
                case EqualitySignExtended.Greater:
                    return first > second;
                case EqualitySignExtended.GreaterOrEqual:
                    return first >= second;
                case EqualitySignExtended.Lower:
                    return first < second;
                case EqualitySignExtended.LowerOrEqual:
                    return first <= second;
                default:
                    return false;
            }
        }
        #endregion Method: Compare(float first, EqualitySignExtended sign, float second)

        #region Method: Compare(Vec2 first, EqualitySignExtended sign, Vec2 second)
        /// <summary>
        /// Checks whether the two vectors satisfy the condition of the sign.
        /// </summary>
        /// <param name="first">The vector to compare.</param>
        /// <param name="sign">The sign to compare both vectors to.</param>
        /// <param name="second">The vector to compare to.</param>
        /// <returns>Returns whether the two vectors satisfy the condition of the sign.</returns>
        public static bool Compare(Vec2 first, EqualitySignExtended sign, Vec2 second)
        {
            // Perform the correct check
            switch (sign)
            {
                case EqualitySignExtended.Equal:
                    return first == second;
                case EqualitySignExtended.NotEqual:
                    return first != second;
                case EqualitySignExtended.Greater:
                    return first > second;
                case EqualitySignExtended.GreaterOrEqual:
                    return first >= second;
                case EqualitySignExtended.Lower:
                    return first < second;
                case EqualitySignExtended.LowerOrEqual:
                    return first <= second;
                default:
                    return false;
            }
        }
        #endregion Method: Compare(Vec2 first, EqualitySignExtended sign, Vec2 second)

        #region Method: Compare(Vec3 first, EqualitySignExtended sign, Vec3 second)
        /// <summary>
        /// Checks whether the two vectors satisfy the condition of the sign.
        /// </summary>
        /// <param name="first">The vector to compare.</param>
        /// <param name="sign">The sign to compare both vectors to.</param>
        /// <param name="second">The vector to compare to.</param>
        /// <returns>Returns whether the two vectors satisfy the condition of the sign.</returns>
        public static bool Compare(Vec3 first, EqualitySignExtended sign, Vec3 second)
        {
            // Perform the correct check
            switch (sign)
            {
                case EqualitySignExtended.Equal:
                    return first == second;
                case EqualitySignExtended.NotEqual:
                    return first != second;
                case EqualitySignExtended.Greater:
                    return first > second;
                case EqualitySignExtended.GreaterOrEqual:
                    return first >= second;
                case EqualitySignExtended.Lower:
                    return first < second;
                case EqualitySignExtended.LowerOrEqual:
                    return first <= second;
                default:
                    return false;
            }
        }
        #endregion Method: Compare(Vec3 first, EqualitySignExtended sign, Vec3 second)

        #region Method: Compare(Vec4 first, EqualitySignExtended sign, Vec4 second)
        /// <summary>
        /// Checks whether the two vectors satisfy the condition of the sign.
        /// </summary>
        /// <param name="first">The vector to compare.</param>
        /// <param name="sign">The sign to compare both vectors to.</param>
        /// <param name="second">The vector to compare to.</param>
        /// <returns>Returns whether the two vectors satisfy the condition of the sign.</returns>
        public static bool Compare(Vec4 first, EqualitySignExtended sign, Vec4 second)
        {
            // Perform the correct check
            switch (sign)
            {
                case EqualitySignExtended.Equal:
                    return first == second;
                case EqualitySignExtended.NotEqual:
                    return first != second;
                case EqualitySignExtended.Greater:
                    return first > second;
                case EqualitySignExtended.GreaterOrEqual:
                    return first >= second;
                case EqualitySignExtended.Lower:
                    return first < second;
                case EqualitySignExtended.LowerOrEqual:
                    return first <= second;
                default:
                    return false;
            }
        }
        #endregion Method: Compare(Vec4 first, EqualitySignExtended sign, Vec4 second)

        #region Method: Compare(object first, EqualitySign sign, object second)
        /// <summary>
        /// Checks whether the two objects satisfy the condition of the sign.
        /// </summary>
        /// <param name="first">The object to compare.</param>
        /// <param name="sign">The sign to compare both objects to.</param>
        /// <param name="second">The object to compare to.</param>
        /// <returns>Returns whether the two objects satisfy the condition of the sign.</returns>
        public static bool Compare(object first, EqualitySign sign, object second)
        {
            if (first == null && second == null)
                return true;

            if (first != null && second != null)
            {
                switch (sign)
                {
                    case EqualitySign.Equal:
                        return first.Equals(second);
                    case EqualitySign.NotEqual:
                        return !first.Equals(second);
                    default:
                        break;
                }
            }
            return false;
        }
        #endregion Method: Compare(object first, EqualitySign sign, object second)

        #region Method: Compare(float first, EqualitySignExtendedDual sign, float second, float third)
        /// <summary>
        /// Checks whether the first and second float (and possibly a third one in case of the 'Between' sign) satisfy the condition of the sign.
        /// </summary>
        /// <param name="first">The float to compare.</param>
        /// <param name="sign">The sign to compare the floats to.</param>
        /// <param name="second">The float to compare to.</param>
        /// <param name="third">An optional third float, only used for the 'Between' sign.</param>
        /// <returns>Returns whether the first and second float (and possibly the third one in case of the 'Between' sign) satisfy the condition of the sign.</returns>
        public static bool Compare(float first, EqualitySignExtendedDual sign, float second, float third)
        {
            // Perform the correct check
            switch (sign)
            {
                case EqualitySignExtendedDual.Equal:
                    return first == second;
                case EqualitySignExtendedDual.NotEqual:
                    return first != second;
                case EqualitySignExtendedDual.Greater:
                    return first > second;
                case EqualitySignExtendedDual.GreaterOrEqual:
                    return first >= second;
                case EqualitySignExtendedDual.Lower:
                    return first < second;
                case EqualitySignExtendedDual.LowerOrEqual:
                    return first <= second;
                case EqualitySignExtendedDual.Between:
                    return first > second && first < third;
                case EqualitySignExtendedDual.NotBetween:
                    return first <= second || first >= third;
                default:
                    return false;
            }
        }
        #endregion Method: Compare(float first, EqualitySignExtendedDual sign, float second, float third)

        #region Method: PrefixToMultiplier(Prefix prefix)
        /// <summary>
        /// Returns the correct multiplier for the given prefix.
        /// </summary>
        /// <param name="prefix">The prefix to get the multiplier for.</param>
        /// <returns>The multiplier value that the prefix represents.</returns>
        public static float PrefixToMultiplier(Prefix prefix)
        {
            switch (prefix)
            {
                case Prefix.Yocto:
                    return (float)Math.Pow(10, -24);
                case Prefix.Zepto:
                    return (float)Math.Pow(10, -21);
                case Prefix.Atto:
                    return (float)Math.Pow(10, -18);
                case Prefix.Femto:
                    return (float)Math.Pow(10, -15);
                case Prefix.Pico:
                    return (float)Math.Pow(10, -12);
                case Prefix.Nano:
                    return (float)Math.Pow(10, -9);
                case Prefix.Micro:
                    return (float)Math.Pow(10, -6);
                case Prefix.Milli:
                    return (float)Math.Pow(10, -3);
                case Prefix.Centi:
                    return (float)Math.Pow(10, -2);
                case Prefix.Deci:
                    return (float)Math.Pow(10, -1);
                case Prefix.None:
                    return (float)Math.Pow(10, 0);
                case Prefix.Deca:
                    return (float)Math.Pow(10, 1);
                case Prefix.Hecto:
                    return (float)Math.Pow(10, 2);
                case Prefix.Kilo:
                    return (float)Math.Pow(10, 3);
                case Prefix.Mega:
                    return (float)Math.Pow(10, 6);
                case Prefix.Giga:
                    return (float)Math.Pow(10, 9);
                case Prefix.Tera:
                    return (float)Math.Pow(10, 12);
                case Prefix.Peta:
                    return (float)Math.Pow(10, 15);
                case Prefix.Exa:
                    return (float)Math.Pow(10, 18);
                case Prefix.Zetta:
                    return (float)Math.Pow(10, 21);
                case Prefix.Yotta:
                    return (float)Math.Pow(10, 24);
                default:
                    return 0;
            }
        }
        #endregion Method: PrefixToMultiplier(Prefix prefix)

        #region Method: ConvertValue(float value, Prefix prefix1, Prefix prefix2)
        /// <summary>
        /// Convert a value from one prefix to another.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="prefix1">The current prefix.</param>
        /// <param name="prefix2">The new prefix.</param>
        /// <returns>The converted value.</returns>
        public static float ConvertValue(float value, Prefix prefix1, Prefix prefix2)
        {
            return (value * PrefixToMultiplier(prefix1)) / PrefixToMultiplier(prefix2);
        }
        #endregion Method: ConvertValue(float value, Prefix prefix1, Prefix prefix2)

        #region Method: ConvertValue(float value, Unit unit1, Unit unit2)
        /// <summary>
        /// Convert a value from one unit to another. Will only work when both units are of the same category!
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="unit1">The current unit.</param>
        /// <param name="unit2">The new unit.</param>
        /// <returns>The converted value if successful, the original value if unsuccessful.</returns>
        public static float ConvertValue(float value, Unit unit1, Unit unit2)
        {
            if (unit1 != null && unit2 != null)
            {
                UnitCategory unitCategory = unit1.UnitCategory;
                if (unitCategory != null && unitCategory.Equals(unit2.UnitCategory))
                {
                    // Convert to the base unit
                    if (unit1.ConversionBackEquation != null && unit1.ConversionBackEquation.IsValid)
                        value = unit1.ConversionBackEquation.Calculate(SemanticsSettings.General.EquationResultVariable, value);

                    // Convert to the new unit
                    if (unit2.ConversionEquation != null && unit2.ConversionEquation.IsValid)
                        value = unit2.ConversionEquation.Calculate(SemanticsSettings.General.EquationVariable, value);
                }

            }
            return value;
        }
        #endregion Method: ConvertValue(float value, Unit unit1, Unit unit2)

        #region Method: GetBaseValue(float value, Prefix prefix, Unit unit)
        /// <summary>
        /// Get the base value for the given value, prefix, and unit.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="unit">The unit.</param>
        /// <returns>The value that is converted to the base unit, without prefix.</returns>
        public static float GetBaseValue(float value, Prefix prefix, Unit unit)
        {
            float baseValue = value * Toolbox.PrefixToMultiplier(prefix);
            if (unit != null && unit.ConversionBackEquation != null && unit.ConversionBackEquation.IsValid)
                baseValue = unit.ConversionBackEquation.Calculate(SemanticsSettings.General.EquationResultVariable, (float)baseValue);
                
            return value;
        }
        #endregion Method: GetBaseValue(float value, Prefix prefix, Unit unit)
		
        #region Method: Satisfies(ValueChangeType valueChangeType1, float value1, ValueChangeType valueChangeType2, float value2)
        /// <summary>
        /// Checks whether the first value and type satisfy the second value and type.
        /// </summary>
        /// <param name="valueChangeType1">The first value type.</param>
        /// <param name="value1">The first value.</param>
        /// <param name="valueChangeType2">The second value type.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns whether the first value and type satisfy the second value and type.</returns>
        public static bool Satisfies(ValueChangeType valueChangeType1, float value1, ValueChangeType valueChangeType2, float value2)
        {
            if (valueChangeType1 == valueChangeType2 && value1 == value2)
                return true;
            else if (valueChangeType1 == ValueChangeType.Increase && valueChangeType2 == ValueChangeType.Increase && value1 > value2)
                return true;
            else if (valueChangeType1 == ValueChangeType.Decrease && valueChangeType2 == ValueChangeType.Decrease && value1 < value2)
                return true;

            return false;
        }
        #endregion Method: Satisfies(ValueChangeType valueChangeType1, float value1, ValueChangeType valueChangeType2, float value2)

        #region Method: Satisfies(ValueChangeType valueChangeType1, Vec4 value1, ValueChangeType valueChangeType2, Vec4 value2)
        /// <summary>
        /// Checks whether the first value and type satisfy the second value and type.
        /// </summary>
        /// <param name="valueChangeType1">The first value type.</param>
        /// <param name="value1">The first value.</param>
        /// <param name="valueChangeType2">The second value type.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>Returns whether the first value and type satisfy the second value and type.</returns>
        public static bool Satisfies(ValueChangeType valueChangeType1, Vec4 value1, ValueChangeType valueChangeType2, Vec4 value2)
        {
            if (valueChangeType1 == valueChangeType2 && value1 == value2)
                return true;
            else if (valueChangeType1 == ValueChangeType.Increase && valueChangeType2 == ValueChangeType.Increase && value1 > value2)
                return true;
            else if (valueChangeType1 == ValueChangeType.Decrease && valueChangeType2 == ValueChangeType.Decrease && value1 < value2)
                return true;

            return false;
        }
        #endregion Method: Satisfies(ValueChangeType valueChangeType1, Vec4 value1, ValueChangeType valueChangeType2, Vec4 value2)

        #region Method: Calculate(Function function, float value)
        /// <summary>
        /// Calculate the value when a function is applied.
        /// </summary>
        /// <param name="function">The function to apply.</param>
        /// <param name="value">The value.</param>
        /// <returns>The result of the value when the function is applied.</returns>
        public static float Calculate(Function function, float value)
        {
            double val = value;
            switch (function)
            {
                case Function.Sine:
                    val = Math.Sin(val);
                    break;
                case Function.Cosine:
                    val = Math.Cos(val);
                    break;
                case Function.Tangent:
                    val = Math.Tan(val);
                    break;
                case Function.Asine:
                    val = Math.Asin(val);
                    break;
                case Function.Acosine:
                    val = Math.Acos(val);
                    break;
                case Function.Atangent:
                    val = Math.Atan(val);
                    break;
                case Function.SquareRoot:
                    val = Math.Sqrt(val);
                    break;
                case Function.Ceiling:
                    val = Math.Ceiling(val);
                    break;
                case Function.Floor:
                    val = Math.Floor(val);
                    break;
                case Function.NaturalLogarithm:
                    val = Math.Log(val, Math.E);
                    break;
                case Function.Log10:
                    val = Math.Log10(val);
                    break;
                case Function.Exponent:
                    val = Math.Exp(val);
                    break;
                case Function.Absolute:
                    val = Math.Abs(val);
                    break;
                case Function.Round:
                    val = Math.Round(val);
                    break;
                default:
                    break;
            }

            return (float)val;
        }
        #endregion Method: Calculate(Function function, float value)

        #region Method: Calculate(float first, Operator op, float second)
        /// <summary>
        /// Calculate the resulting value when the operator is applied to both values.
        /// </summary>
        /// <param name="first">The first value.</param>
        /// <param name="op">The operator.</param>
        /// <param name="second">The second value.</param>
        /// <returns>The resulting value.</returns>
        public static float Calculate(float first, Operator op, float second)
        {
            switch (op)
            {
                case Operator.Addition:
                    return first + second;
                case Operator.Subtraction:
                    return first - second;
                case Operator.Multiplication:
                    return first * second;
                case Operator.Division:
                     if (second != 0)
                        return first / second;
                    break;
                case Operator.Power:
                    return (int)first ^ (int)second;
                default:
                    break;
            }
            return 0;
        }
        #endregion Method: Calculate(float first, Operator op, float second)
		
        #region Method: ChangeCulture(CultureInfo culture)
        /// <summary>
        /// Changes the culture for enums and new names.
        /// </summary>
        /// <param name="culture">The culture to change to.</param>
        public static void ChangeCulture(CultureInfo culture)
        {
            if (culture != null)
            {
                ResourceEnumConverter.ChangeCulture(culture);
                Enums.Culture = culture;
                Exceptions.Culture = culture;
                NewNames.Culture = culture;
            }
        }
        #endregion Method: ChangeCulture(CultureInfo culture)

    }
    #endregion Class: Toolbox

}