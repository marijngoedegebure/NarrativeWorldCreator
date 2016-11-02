/**************************************************************************
 * 
 * Instance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Semantics.Components;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: Instance
    /// <summary>
    /// An instance, keeping track of modifications, additions, and removals from its base.
    /// </summary>
    public abstract class Instance : PropertyChangedComponent, IDisposable
    {

        #region Properties and Fields

        #region Property: Base
        /// <summary>
        /// The base of this instance.
        /// </summary>
        private Base instanceBase = null;

        /// <summary>
        /// Gets the base of this instance.
        /// </summary>
        protected internal Base Base
        {
            get
            {
                return instanceBase;
            }
        }
        #endregion Property: Base

        #region Property: ID
        /// <summary>
        /// Gets the ID of the ID holder this instance is based on.
        /// </summary>
        public uint ID
        {
            get
            {
                if (instanceBase != null)
                    return instanceBase.ID;
                return 0;
            }
        }
        #endregion Property: Base

        #region Field: modifications
        /// <summary>
        /// The dictionary with modifications from base classes. The key is the property name, the value any change.
        /// </summary>
        private Dictionary<String, object> modifications = new Dictionary<string,object>();
        #endregion Field: modifications

        #region Field: additions
        /// <summary>
        /// The dictionary with additions on base classes. The key is the property name, the value any addition.
        /// </summary>
        private Dictionary<String, object> additions = new Dictionary<string, object>();
        #endregion Field: additions

        #region Field: removals
        /// <summary>
        /// The dictionary with removals from base classes. The key is the property name, the value any removal.
        /// </summary>
        private Dictionary<String, object> removals = new Dictionary<string, object>();
        #endregion Field: removals

        #region Field: customProperties
        /// <summary>
        /// The custom properties.
        /// </summary>
        private Dictionary<string, object> customProperties = new Dictionary<string,object>(0);
        #endregion Field: customProperties

        #region Field: baseHandler
        /// <summary>
        /// A handler for possible base changes.
        /// </summary>
        private Base.BaseHandler baseHandler;
        #endregion Field: baseHandler

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Instance(Base instanceBase)
        /// <summary>
        /// Creates a new instance from the given base.
        /// </summary>
        /// <param name="instanceBase">The base to create the instance from.</param>
        protected Instance(Base instanceBase)
        {
            this.instanceBase = instanceBase;

            // Subscribe for possible base changes
            if (instanceBase != null)
            {
                baseHandler = new Base.BaseHandler(instanceBase_BaseChanged);
                this.instanceBase.BaseChanged += baseHandler;
            }
        }
        #endregion Constructor: Instance(Base instanceBase)

        #region Constructor: Instance(Instance instance)
        /// <summary>
        /// Clones an instance.
        /// </summary>
        /// <param name="instance">The instance to clone.</param>
        protected Instance(Instance instance)
        {
            if (instance != null)
            {
                this.instanceBase = instance.instanceBase;

                this.modifications = CloneDictionary(instance.modifications);
                this.additions = CloneDictionary(instance.additions);
                this.removals = CloneDictionary(instance.removals);

                // Subscribe for possible base changes
                if (instanceBase != null)
                {
                    baseHandler = new Base.BaseHandler(instanceBase_BaseChanged);
                    this.instanceBase.BaseChanged += baseHandler;
                }
            }
        }
        #endregion Constructor: Instance(Instance instance)

        #region Method: CloneDictionary(Dictionary<string, object> dictionary)
        /// <summary>
        /// Clones the given dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to clone.</param>
        /// <returns>The cloned dictionary.</returns>
        private Dictionary<string, object> CloneDictionary(Dictionary<string, object> dictionary)
        {
            Dictionary<string, object> clone = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> pair in dictionary)
            {
                if (pair.Value != null)
                {
                    if (pair.Value is Instance)
                        clone.Add(pair.Key, ((Instance)pair.Value).Clone());

                    else if (pair.Value is IList)
                    {
                        IList list = (IList)pair.Value;
                        IList copy = new List<object>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i] is Instance)
                                copy.Add(((Instance)list[i]).Clone());
                            else
                                copy.Add(list[i]);
                        }
                        clone.Add(pair.Key, copy);
                    }

                    else if (pair.Value.GetType().IsArray)
                    {
                        Array array = (Array)pair.Value;
                        Array copy = Array.CreateInstance(typeof(object), array.Length);
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array.GetValue(i) is Instance)
                                copy.SetValue((((Instance)array.GetValue(i)).Clone()), i);
                            else
                                copy.SetValue(array.GetValue(i), i);
                        }
                        clone.Add(pair.Key, copy);
                    }

                    else
                        clone.Add(pair.Key, pair.Value);
                }
                else
                    clone.Add(pair.Key, pair.Value);
            }

            return clone;
        }
        #endregion Method: CloneDictionary(Dictionary<string, object> dictionary)
		
        #region Method: instanceBase_BaseChanged(Base newBase)
        /// <summary>
        /// Replaces the old base by the new base when it is changed.
        /// </summary>
        /// <param name="newBase">The new base.</param>
        private void instanceBase_BaseChanged(Base newBase)
        {
            if (newBase != null && this.instanceBase != null && this.instanceBase.GetType().Equals(newBase.GetType()))
                this.instanceBase = newBase;
        }
        #endregion Method: instanceBase_BaseChanged(Base newBase)

        #region Method: Dispose()
        /// <summary>
        /// Dispose the instance.
        /// </summary>
        public void Dispose()
        {
            if (this.instanceBase != null && baseHandler != null)
                this.instanceBase.BaseChanged -= baseHandler;
        }
        #endregion Method: Dispose()

        #endregion Method Group: Constructors

        #region Method Group: General

        #region Method: GetProperty<T>(String key)
        /// <summary>
        /// Get the possibly locally modified property with the given key in the modifications table.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key of the property in the modifications table.</param>
        /// <returns>The possibly locally modified property.</returns>
        protected T GetProperty<T>(String key)
        {
            if (key != null)
            {
                object obj = null;
                if (this.modifications.TryGetValue(key, out obj) && obj is T)
                    return (T)obj;
            }
            return default(T);
        }
        #endregion Method: GetProperty<T>(String key)

        #region Method: GetProperty<T>(String key, T baseProperty)
        /// <summary>
        /// Get the possibly locally modified property of the base property, indicated in the modifications table by the given key.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key of the modifications table in which a modification of the base property can be found.</param>
        /// <param name="baseProperty">The base property.</param>
        /// <returns>The base property, or a locally modified one.</returns>
        protected T GetProperty<T>(String key, T baseProperty)
        {
            if (key != null)
            {
                object obj = null;
                if (this.modifications.TryGetValue(key, out obj) && obj is T)
                    return (T)obj;
            }
            return baseProperty;
        }
        #endregion Method: GetProperty<T>(String key, T baseProperty)

        #region Method: TryGetProperty<T>(String key, out T property)
        /// <summary>
        /// Tries to get the possibly locally modified property with the given key in the modifications table.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key of the property in the modifications table.</param>
        /// <param name="property">The possibly locally modified property.</param>
        /// <returns>Returns whether the locally modified property has been found.</returns>
        protected bool TryGetProperty<T>(String key, out T property)
        {
            if (key != null)
            {
                object obj = null;
                if (this.modifications.TryGetValue(key, out obj) && obj is T)
                {
                    property = (T)obj;
                    return true;
                }
            }
            property = default(T);
            return false;
        }
        #endregion Method: TryGetProperty<T>(String key, out T property)

        #region Method: SetProperty(String key, object value)
        /// <summary>
        /// Set the property with the given key with the given value.
        /// </summary>
        /// <param name="key">The key for the modification in the base table.</param>
        /// <param name="value">The value for the property.</param>
        protected void SetProperty(String key, object value)
        {
            if (key != null)
            {
                this.modifications[key] = value;
                NotifyPropertyChanged(key);
            }
        }
        #endregion Method: SetProperty(String key, object value)

        #region Method: AddToModificationsArray<T>(string key, T toAdd)
        /// <summary>
        /// Add the given instance to the array in the modification dictionary found by the given key.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="key">The key for the property in the modifications dictionary.</param>
        /// <param name="toAdd">The instance to add to the array.</param>
        protected void AddToModificationsArray<T>(string key, T toAdd)
            where T : Instance
        {
            if (key != null)
            {
                // Get the modifications array or create one if it does not yet exist
                T[] mods = GetProperty<T[]>(key, null);
                if (mods == null)
                {
                    mods = new T[] { };
                    SetProperty(key, mods);
                }
                else
                {
                    // If the modification is already there, there is no need to add it
                    foreach (T mod in mods)
                    {
                        if (mod.Base != null && mod.Base.Equals(toAdd.Base))
                            return;
                    }
                }

                // Add the modification to the array
                Utils.AddToArray<T>(ref mods, toAdd);
                SetProperty(key, mods);
            }
        }
        #endregion Method: AddToModificationsArray<T>(string key, T toAdd)

        #region Method: ReplaceByModifications<T>(String key, List<T> list, PropertyChangedEventHandler propertyChangedEventHandler)
        /// <summary>
        /// Replace the instances in the list by the possibly modified instances. Also add possible additional instances, or remove instances.
        /// </summary>
        /// <typeparam name="T">The type of instance.</typeparam>
        /// <param name="key">The name of the property in the modifications table.</param>
        /// <param name="list">The original list with instances.</param>
        /// <param name="propertyChangedEventHandler">An event handler for instances from the base of which a property is modified.</param>
        /// <returns>The modified list.</returns>
        protected List<T> ReplaceByModifications<T>(String key, List<T> list, PropertyChangedEventHandler propertyChangedEventHandler)
            where T : Instance
        {
            if (key != null && list != null)
            {
                object obj = null;

                // Add additional entries
                if (this.additions.TryGetValue(key, out obj))
                {
                    T[] additionArray = obj as T[];
                    if (additionArray != null)
                        list.AddRange(additionArray);
                }

                // Remove entries
                if (this.removals.TryGetValue(key, out obj))
                {
                    T[] removalArray = obj as T[];
                    if (removalArray != null)
                    {
                        foreach (T removal in removalArray)
                            list.Remove(removal);
                    }
                }

                // Replace the modified instances
                for (int i = 0; i < list.Count; i++)
                {
                    bool skip = false;
                    T[] mods = GetProperty<T[]>(key, null);
                    if (mods != null)
                    {
                        foreach (T mod in mods)
                        {
                            if (mod.Base.Equals(list[i].Base))
                            {
                                list[i].Dispose();
                                list[i] = mod;
                                skip = true;
                                break;
                            }
                        }
                    }
                    // Subscribe for possible property changes in the instances that are not modified
                    if (!skip)
                        list[i].PropertyChanged += propertyChangedEventHandler;
                }
            }

            return list;
        }
        #endregion Method: ReplaceByModifications<T>(String key, List<T> list, PropertyChangedEventHandler propertyChangedEventHandler)

        #region Method: AddToModificationsArrayBase<T>(string key, T toAdd)
        /// <summary>
        /// Add the given base to the array in the modification dictionary found by the given key.
        /// </summary>
        /// <typeparam name="T">The type of the base.</typeparam>
        /// <param name="key">The key for the property in the modifications dictionary.</param>
        /// <param name="toAdd">The base to add to the array.</param>
        protected void AddToModificationsArrayBase<T>(string key, T toAdd)
            where T : Base
        {
            if (key != null)
            {
                // Get the modifications array or create one if it does not yet exist
                T[] mods = GetProperty<T[]>(key, null);
                if (mods == null)
                {
                    mods = new T[] { };
                    SetProperty(key, mods);
                }
                else
                {
                    // If the modification is already there, there is no need to add it
                    foreach (T mod in mods)
                    {
                        if (mod.Equals(toAdd))
                            return;
                    }
                }

                // Add the modification to the array
                Utils.AddToArray<T>(ref mods, toAdd);
                SetProperty(key, mods);
            }
        }
        #endregion Method: AddToModificationsArrayBase<T>(string key, T toAdd)

        #region Method: ReplaceByModificationsBase<T>(String key, List<T> list, PropertyChangedEventHandler propertyChangedEventHandler)
        /// <summary>
        /// Replace the bases in the list by the possibly modified bases. Also add possible additional bases, or remove bases.
        /// </summary>
        /// <typeparam name="T">The type of base.</typeparam>
        /// <param name="key">The name of the property in the modifications table.</param>
        /// <param name="list">The original list with bases.</param>
        /// <param name="propertyChangedEventHandler">An event handler for bases from the base of which a property is modified.</param>
        /// <returns>The modified list.</returns>
        protected List<T> ReplaceByModificationsBase<T>(String key, List<T> list, PropertyChangedEventHandler propertyChangedEventHandler)
            where T : Base
        {
            if (key != null && list != null)
            {
                object obj = null;

                // Add additional entries
                if (this.additions.TryGetValue(key, out obj))
                {
                    T[] additionArray = obj as T[];
                    if (additionArray != null)
                        list.AddRange(additionArray);
                }

                // Remove entries
                if (this.removals.TryGetValue(key, out obj))
                {
                    T[] removalArray = obj as T[];
                    if (removalArray != null)
                    {
                        foreach (T removal in removalArray)
                            list.Remove(removal);
                    }
                }

                // Replace the modified instances
                for (int i = 0; i < list.Count; i++)
                {
                    bool skip = false;
                    T[] mods = GetProperty<T[]>(key, null);
                    if (mods != null)
                    {
                        foreach (T mod in mods)
                        {
                            if (mod.Equals(list[i]))
                            {
                                list[i] = mod;
                                skip = true;
                                break;
                            }
                        }
                    }
                    // Subscribe for possible property changes in the instances that are not modified
                    if (!skip)
                        list[i].PropertyChanged += propertyChangedEventHandler;
                }
            }

            return list;
        }
        #endregion Method: ReplaceByModificationsBase<T>(String key, List<T> list, PropertyChangedEventHandler propertyChangedEventHandler)

        #region Method: AddToArrayProperty<T>(String key, T addition)
        /// <summary>
        /// Add an entry to the property of the array type with the given key.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key of the array property.</param>
        /// <param name="addition">The item to add to the array of the property.</param>
        protected void AddToArrayProperty<T>(String key, T addition)
        {
            if (key != null)
            {
                object obj = null;
                if (this.additions.TryGetValue(key, out obj))
                {
                    // Add the addition to the existing array
                    T[] array = obj as T[];
                    if (array != null)
                        Utils.AddToArray<T>(ref array, addition);
                    this.additions[key] = array;
                }
                else
                {
                    // Create a new array and add it to the dictionary
                    this.additions[key] = new T[] { addition };
                }

                NotifyPropertyChanged(key);
            }
        }
        #endregion Method: AddToArrayProperty<T>(String key, T addition)

        #region Method: RemoveFromArrayProperty<T>(String key, T removal)
        /// <summary>
        /// Remove an entry from the property of the array type with the given key.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The key of the array property.</param>
        /// <param name="removal">The property to remove from the array of the property.</param>
        protected void RemoveFromArrayProperty<T>(String key, T removal)
        {
            if (key != null && removal != null)
            {
                // Check whether we can remove it from the additions
                object obj = null;
                if (this.additions.TryGetValue(key, out obj))
                {
                    T[] array = obj as T[];
                    if (array != null)
                    {
                        foreach (T addition in array)
                        {
                            if (removal.Equals(addition))
                            {
                                // Remove the removal from the additions
                                Utils.RemoveFromArray<T>(ref array, addition);

                                // Remove the key if there are no more additions
                                if (array.Length == 0)
                                    this.additions.Remove(key);
                                // Otherwise, overwrite the additions in the dictionary
                                else
                                    this.additions[key] = array;

                                NotifyPropertyChanged(key);
                                return;
                            }
                        }
                    }
                }
                
                // If not, add it to the removals
                if (this.removals.TryGetValue(key, out obj))
                {
                    // Add the addition to the existing array
                    T[] array = obj as T[];
                    if (array != null)
                        Utils.AddToArray<T>(ref array, removal);
                    this.removals[key] = array;
                }
                else
                {
                    // Create a new array and add it to the dictionary
                    this.removals[key] = new T[] { removal };
                }

                NotifyPropertyChanged(key);
            }
        }
        #endregion Method: RemoveFromArrayProperty(String key, T removal)

        #region Method: GetCustomProperty(string propertyName)
        /// <summary>
        /// Get the value of the custom property with the given name.
        /// </summary>
        /// <param name="propertyName">The property to get the value of.</param>
        /// <returns>Returns the value of the custom property.</returns>
        public object GetCustomProperty(string propertyName)
        {
            object value = null;
            if (propertyName != null)
                this.customProperties.TryGetValue(propertyName, out value);
            return value;
        }
        #endregion Method: GetCustomProperty(string propertyName)

        #region Method: SetCustomProperty(string propertyName, object value)
        /// <summary>
        /// Set the value of the custom property.
        /// </summary>
        /// <param name="propertyName">The property to set.</param>
        /// <param name="value">The value to assign to the property.</param>
        public void SetCustomProperty(string propertyName, object value)
        {
            if (propertyName != null)
            {
                if (this.customProperties.ContainsKey(propertyName))
                    this.customProperties[propertyName] = value;
                else
                    this.customProperties.Add(propertyName, value);
            }
        }
        #endregion Method: SetCustomProperty(string propertyName, object value)
		
        #endregion Method Group: General

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the instance.
        /// </summary>
        /// <returns>A clone of the instance.</returns>
        public Instance Clone()
        {
            try
            {
                Type type = this.GetType();
                return type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { type }, null).Invoke(new object[] { this }) as Instance;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: Instance
		
}