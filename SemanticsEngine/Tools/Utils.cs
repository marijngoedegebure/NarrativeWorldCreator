/**************************************************************************
 * 
 * Utils.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Semantics.Data;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;

namespace SemanticsEngine.Tools
{

    #region Class: Utils
    /// <summary>
    /// Several utilities.
    /// </summary>
    public static class Utils
    {

        #region Field: specialAttributes
        /// <summary>
        /// A list with special attributes and their attributes.
        /// </summary>
        private static Dictionary<SpecialAttributes, AttributeBase> specialAttributes = new Dictionary<SpecialAttributes, AttributeBase>();
        #endregion Field: specialAttributes

        #region Field: specialUnitCategories
        /// <summary>
        /// A list with special unit categories and their unit categories.
        /// </summary>
        private static Dictionary<SpecialUnitCategories, UnitCategoryBase> specialUnitCategories = new Dictionary<SpecialUnitCategories, UnitCategoryBase>();
        #endregion Field: specialUnitCategories

        #region Method: GetSpecialAttribute(SpecialAttributes specialAttribute)
        /// <summary>
        /// Get the attribute that has been assigned to the given special attribute type.
        /// </summary>
        /// <param name="specialAttribute">The special attribute.</param>
        /// <returns>The attribute that has been assigned to the special attribute type.</returns>
        public static AttributeBase GetSpecialAttribute(SpecialAttributes specialAttribute)
        {
            // Try to get the attribute from the dictionary; otherwise, get it from the semantics manager.
            AttributeBase attribute = null;
            if (!specialAttributes.TryGetValue(specialAttribute, out attribute))
            {
                if (Database.Current != null)
                {
                    attribute = BaseManager.Current.GetBase<AttributeBase>(SemanticsManager.GetSpecialAttribute(specialAttribute));
                    specialAttributes.Add(specialAttribute, attribute);
                }
            }
            return attribute;
        }
        #endregion Method: GetSpecialAttribute(SpecialAttributes specialAttribute)

        #region Method: GetSpecialUnitCategory(SpecialUnitCategories specialUnitCategory)
        /// <summary>
        /// Get the unit category that has been assigned to the given special unit categories type.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <returns>The unit category that has been assigned to the special unit category type.</returns>
        public static UnitCategoryBase GetSpecialUnitCategory(SpecialUnitCategories specialUnitCategory)
        {
            // Try to get the unit category from the dictionary; otherwise, get it from the semantics manager.
            UnitCategoryBase unitCategory = null;
            if (!specialUnitCategories.TryGetValue(specialUnitCategory, out unitCategory))
            {
                if (Database.Current != null)
                {
                    unitCategory = BaseManager.Current.GetBase<UnitCategoryBase>(SemanticsManager.GetSpecialUnitCategory(specialUnitCategory));
                    specialUnitCategories.Add(specialUnitCategory, unitCategory);
                }
            }
            return unitCategory;
        }
        #endregion Method: GetSpecialUnitCategory(SpecialUnitCategories specialUnitCategory)

        #region Method: Clear()
        /// <summary>
        /// Clears the utilities.
        /// </summary>
        internal static void Clear()
        {
            specialAttributes.Clear();
            specialUnitCategories.Clear();
        }
        #endregion Method: Clear()

        #region Method: AddToArray<T>(ref T[] array, T item)
        /// <summary>
        /// Add the given item to the array.
        /// </summary>
        /// <typeparam name="T">The type of the array and item.</typeparam>
        /// <param name="array">The array to add the item to.</param>
        /// <param name="item">The item to add to the array.</param>
        public static void AddToArray<T>(ref T[] array, T item)
        {
            if (array == null)
                array = new T[] { item };
            else
            {
                Array.Resize(ref array, array.Length + 1);
                array[array.Length - 1] = item;
            }
        }
        #endregion Method: AddToArray<T>(ref T[] array, T item)

        #region Method: RemoveFromArray<T>(ref T[] array, T item)
        /// <summary>
        /// Remove the given item from the array.
        /// </summary>
        /// <typeparam name="T">The type of the array and item.</typeparam>
        /// <param name="array">The array to remove the item from.</param>
        /// <param name="item">The item to remove from the array.</param>
        public static void RemoveFromArray<T>(ref T[] array, T item)
        {
            if (array != null)
            {
                ArrayList arrayList = new ArrayList(array);
                arrayList.Remove(item);
                array = (T[])arrayList.ToArray(typeof(T));
            }
        }
        #endregion Method: RemoveFromArray<T>(ref T[] array, T item)

        #region Method: GetBaseValue(float value, Prefix prefix, UnitBase unit)
        /// <summary>
        /// Get the base value for the given value, prefix, and unit.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="unit">The unit.</param>
        /// <returns>The value that is converted to the base unit, without prefix.</returns>
        public static float GetBaseValue(float value, Prefix prefix, UnitBase unit)
        {
            float baseValue = value * Toolbox.PrefixToMultiplier(prefix);
            if (unit != null && unit.ConversionBackEquation != null && unit.ConversionBackEquation.IsValid)
                baseValue = unit.ConversionBackEquation.Calculate(SemanticsSettings.General.EquationResultVariable, (float)baseValue);

            return baseValue;
        }
        #endregion Method: GetBaseValue(float value, Prefix prefix, UnitBase unit)

        #region Method: ChangeCulture(CultureInfo culture)
        /// <summary>
        /// Changes the culture for enums and new names.
        /// </summary>
        /// <param name="culture">The culture to change to.</param>
        public static void ChangeCulture(CultureInfo culture)
        {
            if (culture != null)
                Enums.Culture = culture;
        }
        #endregion Method: ChangeCulture(CultureInfo culture)

    }
    #endregion Class: Utils

}