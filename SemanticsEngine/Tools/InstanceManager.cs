/**************************************************************************
 * 
 * InstanceManager.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Semantics.Components;
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;

namespace SemanticsEngine.Tools
{

    #region Class: InstanceManager
    /// <summary>
    /// The instance manager allows the creation of instances from ID holders or bases.
    /// </summary>
    public class InstanceManager
    {

        #region Properties and Fields

        #region Property: Current
        /// <summary>
        /// The current instance manager.
        /// </summary>
        private static InstanceManager current = new InstanceManager();

        /// <summary>
        /// Gets the current base manager.
        /// </summary>
        public static InstanceManager Current
        {
            get
            {
                return current;
            }
            protected set
            {
                current = value;
            }
        }
        #endregion Property: Current

        #region Field: baseInstanceType
        /// <summary>
        /// A dictionary containing type pairs of non-abstract base classes and their corresponding instance classes.
        /// </summary>
        private static Dictionary<Type, Type> baseInstanceType = new Dictionary<Type, Type>();
        #endregion Field: baseInstanceType

        #region Field: typeConstructor
        /// <summary>
        /// The constructors of types.
        /// </summary>
        private Dictionary<Type, ConstructorInfo> typeConstructor = new Dictionary<Type, ConstructorInfo>();
        #endregion Field: typeConstructor

        #region Property: IgnoreNecessity
        /// <summary>
        /// Indicates whether the necessity should be ignored when creating instances,
        /// because by default, only Mandatory instances are created, while Optional ones are skipped.
        /// </summary>
        private static bool ignoreNecessity = false;

        /// <summary>
        /// Gets or sets the value that indicates whether the necessity should be ignored when creating instances,
        /// because by default, only Mandatory instances are created, while Optional ones are skipped.
        /// </summary>
        public static bool IgnoreNecessity
        {
            get
            {
                return ignoreNecessity;
            }
            set
            {
                ignoreNecessity = value;
            }
        }
        #endregion Property: IgnoreNecessity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: InstanceManager()
        /// <summary>
        /// Adds pairs of non-abstract base classes and their corresponding instance classes.
        /// </summary>
        static InstanceManager()
        {
            // Abstractions
            AddBaseInstancePair(typeof(AttributeValuedBase), typeof(AttributeInstance));
            AddBaseInstancePair(typeof(GoalBase), typeof(GoalInstance));

            // Entities
            AddBaseInstancePair(typeof(AbstractEntityBase), typeof(AbstractEntityInstance));
            AddBaseInstancePair(typeof(CompoundValuedBase), typeof(CompoundInstance));
            AddBaseInstancePair(typeof(CompoundBase), typeof(CompoundInstance));
            AddBaseInstancePair(typeof(ConnectionItemBase), typeof(TangibleObjectInstance));
            AddBaseInstancePair(typeof(CoverBase), typeof(TangibleObjectInstance));
            AddBaseInstancePair(typeof(ElementBase), typeof(ElementInstance));
            AddBaseInstancePair(typeof(LayerBase), typeof(MatterInstance));
            AddBaseInstancePair(typeof(MaterialValuedBase), typeof(MaterialInstance));
            AddBaseInstancePair(typeof(MaterialBase), typeof(MaterialInstance));
            AddBaseInstancePair(typeof(MixtureValuedBase), typeof(MixtureInstance));
            AddBaseInstancePair(typeof(MixtureBase), typeof(MixtureInstance));
            AddBaseInstancePair(typeof(PartBase), typeof(TangibleObjectInstance));
            AddBaseInstancePair(typeof(SpaceBase), typeof(SpaceInstance));
            AddBaseInstancePair(typeof(SpaceValuedBase), typeof(SpaceInstance));
            AddBaseInstancePair(typeof(SubstanceValuedBase), typeof(SubstanceInstance));
            AddBaseInstancePair(typeof(SubstanceBase), typeof(SubstanceInstance));
            AddBaseInstancePair(typeof(TangibleObjectBase), typeof(TangibleObjectInstance));

            // Components
            AddBaseInstancePair(typeof(EventBase), typeof(EventInstance));
            AddBaseInstancePair(typeof(PredicateBase), typeof(PredicateInstance));
            AddBaseInstancePair(typeof(RelationshipBase), typeof(RelationshipInstance));
            AddBaseInstancePair(typeof(StateGroupBase), typeof(StateGroupInstance));
            AddBaseInstancePair(typeof(BoolValueBase), typeof(BoolValueInstance));
            AddBaseInstancePair(typeof(NumericalValueBase), typeof(NumericalValueInstance));
            AddBaseInstancePair(typeof(StringValueBase), typeof(StringValueInstance));
            AddBaseInstancePair(typeof(TermValueBase), typeof(TermValueInstance));
            AddBaseInstancePair(typeof(VectorValueBase), typeof(VectorValueInstance));
        }
        #endregion Static Constructor: InstanceManager()

        #region Constructor: InstanceManager()
        /// <summary>
        /// Creates a new instance manager.
        /// </summary>
        protected InstanceManager()
        {
        }
        #endregion Constructor: InstanceManager()

        #endregion Method Group: Constructors

        #region Method: AddBaseInstancePair(Type baseType, Type instanceType)
        /// <summary>
        /// Add a pair of a non-abstract base class and its corresponding instance class.
        /// </summary>
        /// <param name="baseType">The type of the base.</param>
        /// <param name="instanceType">The type of the corresponding instance.</param>
        protected static void AddBaseInstancePair(Type baseType, Type instanceType)
        {
            if (baseType != null && instanceType != null)
                baseInstanceType.Add(baseType, instanceType);
        }
        #endregion Method: AddBaseInstancePair(Type baseType, Type instanceType)

        #region Method: Create<T>(Base bas)
        /// <summary>
        /// Create an instance of the given base.
        /// </summary>
        /// <typeparam name="T">The type the instance should get.</typeparam>
        /// <param name="bas">The base to create an instance of.</param>
        /// <returns>A instance of the base.</returns>
        public T Create<T>(Base bas)
            where T : Instance
        {
            if (bas != null)
            {
                try
                {
                    // Create an instance of the base
                    Type type = bas.GetType();
                    ConstructorInfo constructorInfo;
                    if (!typeConstructor.TryGetValue(type, out constructorInfo))
                    {
                        constructorInfo = baseInstanceType[type].GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { type }, null);
                        typeConstructor.Add(type, constructorInfo);
                    }
                    return (T)constructorInfo.Invoke(new object[] { bas });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception in InstanceManager.Create<T>: " + ex.Message);
                }
            }

            return default(T);
        }
        #endregion Method: Create<T>(Base bas)

        #region Method: Create<T>(IdHolder idHolder)
        /// <summary>
        /// Create an instance of the given ID holder.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="node">The ID holder to create an instance of.</param>
        /// <returns>A instance of the ID holder.</returns>
        public T Create<T>(IdHolder idHolder)
            where T : Instance
        {
            if (idHolder != null)
            {
                // Get the base
                Base bas = BaseManager.Current.GetBase(idHolder);

                // Create an instance of the base
                if (bas != null)
                    return Create<T>(bas);
            }

            return default(T);
        }
        #endregion Method: Create<T>(IdHolder idHolder)

        #region Method: Create(TangibleObject tangibleObject, CreateOptions createOptions)
        /// <summary>
        /// Create an instance of the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create the instance from.</param>
        /// <param name="createOptions">The create options.</param>
        /// <returns>A instance of the tangible object.</returns>
        public TangibleObjectInstance Create(TangibleObject tangibleObject, CreateOptions createOptions)
        {
            if (tangibleObject != null)
            {
                // Get the base of the tangible object, and create an instance of it
                TangibleObjectBase tangibleObjectBase = BaseManager.Current.GetBase<TangibleObjectBase>(tangibleObject);
                if (tangibleObjectBase != null)
                    return Create(tangibleObjectBase, createOptions);
            }

            return null;
        }
        #endregion Method: Create(TangibleObject tangibleObject, CreateOptions createOptions)

        #region Method: Create(TangibleObjectBase tangibleObjectBase, CreateOptions createOptions)
        /// <summary>
        /// Create an instance of the given tangible object base.
        /// </summary>
        /// <param name="tangibleObjectBase">The tangible object base to create the instance from.</param>
        /// <param name="createOptions">The create options.</param>
        /// <returns>A instance of the tangible object base.</returns>
        public TangibleObjectInstance Create(TangibleObjectBase tangibleObjectBase, CreateOptions createOptions)
        {
            // Create an instance of the base
            if (tangibleObjectBase != null)
                return new TangibleObjectInstance(tangibleObjectBase, createOptions);

            return null;
        }
        #endregion Method: Create(TangibleObjectBase tangibleObjectBase, CreateOptions createOptions)

        #region Method: Create(PartBase partBase, CreateOptions createOptions)
        /// <summary>
        /// Create an instance of the given part base.
        /// </summary>
        /// <param name="partBase">The part base to create the instance from.</param>
        /// <param name="createOptions">The create options.</param>
        /// <returns>A instance of the part base.</returns>
        internal TangibleObjectInstance Create(PartBase partBase, CreateOptions createOptions)
        {
            // Create an instance of the base
            if (partBase != null)
                return new TangibleObjectInstance(partBase, createOptions);

            return null;
        }
        #endregion Method: Create(PartBase partBase, CreateOptions createOptions)

        #region Method: Create(CoverBase coverBase, CreateOptions createOptions)
        /// <summary>
        /// Create an instance of the given cover base.
        /// </summary>
        /// <param name="coverBase">The cover base to create the instance from.</param>
        /// <param name="createOptions">The create options.</param>
        /// <returns>A instance of the cover base.</returns>
        internal TangibleObjectInstance Create(CoverBase coverBase, CreateOptions createOptions)
        {
            // Create an instance of the base
            if (coverBase != null)
                return new TangibleObjectInstance(coverBase, createOptions);

            return null;
        }
        #endregion Method: Create(CoverBase coverBase, CreateOptions createOptions)

        #region Method: Create(ConnectionItemBase connectionItemBase, CreateOptions createOptions)
        /// <summary>
        /// Create an instance of the given connection item base.
        /// </summary>
        /// <param name="connectionItemBase">The connection item base to create the instance from.</param>
        /// <param name="createOptions">The create options.</param>
        /// <returns>A instance of the cover base.</returns>
        internal TangibleObjectInstance Create(ConnectionItemBase connectionItemBase, CreateOptions createOptions)
        {
            // Create an instance of the base
            if (connectionItemBase != null)
                return new TangibleObjectInstance(connectionItemBase, createOptions);

            return null;
        }
        #endregion Method: Create(ConnectionItemBase connectionItemBase, CreateOptions createOptions)

        #region Method: Create(SpaceValuedBase spaceValuedBase, CreateOptions createOptions)
        /// <summary>
        /// Create an instance of the given valued space base.
        /// </summary>
        /// <param name="spaceValuedBase">The valued space base to create the instance from.</param>
        /// <param name="createOptions">The create options.</param>
        /// <returns>A instance of the valued space base.</returns>
        internal SpaceInstance Create(SpaceValuedBase spaceValuedBase, CreateOptions createOptions)
        {
            // Create an instance of the base
            if (spaceValuedBase != null)
                return new SpaceInstance(spaceValuedBase, createOptions);

            return null;
        }
        #endregion Method: Create(SpaceValuedBase spaceValuedBase, CreateOptions createOptions)

        #region Method: Create(TangibleObjectBase tangibleObjectBase, PhysicalObjectInstance matterSource, bool onlyGetMatterOfSource, PhysicalObjectInstance partSource, bool onlyGetPartsOfSource)
        /// <summary>
        /// Create an instance of the given tangible object base, while getting the required matter and parts from the given source.
        /// </summary>
        /// <param name="tangibleObjectBase">The tangible object base to create the instance from.</param>
        /// <param name="matterSource">The source to retrieve the matter from to create the tangible object instance.</param>
        /// <param name="onlyGetMatterOfSource">Indicates whether the required matter should only be retrieved from the source, in case of a tangible object (true), or whether only tangible matter in the spaces of the source, and its parts, should be retrieved (false).</param>
        /// <param name="partSource">The source to retrieve the parts from to create the tangible object instance.</param>
        /// <param name="onlyGetPartsOfSource">Indicates whether the required parts should only be retrieved from the source, in case of a tangible object (true), or whether only parts in the spaces of the source, and its parts, should be retrieved (false).</param>
        /// <returns>A instance of the tangible object base, if sufficient matter and parts have been found in the source.</returns>
        public TangibleObjectInstance Create(TangibleObjectBase tangibleObjectBase, PhysicalObjectInstance matterSource, bool onlyGetMatterOfSource, PhysicalObjectInstance partSource, bool onlyGetPartsOfSource)
        {
            // Create an instance of the base
            if (tangibleObjectBase != null)
            {
                bool ok = true;

                // Get all matter that should be used
                List<MatterInstance> matterToUse = new List<MatterInstance>();
                TangibleObjectInstance tangibleMatterSource = matterSource as TangibleObjectInstance;
                if (tangibleMatterSource != null)
                {
                    ReadOnlyCollection<MatterValuedBase> requiredMatter = GetRequiredMatter(tangibleObjectBase);
                    
                    // Get all available matter
                    List<MatterInstance> allMatter = new List<MatterInstance>(tangibleMatterSource.Matter);
                    if (!onlyGetMatterOfSource)
                        allMatter.AddRange(tangibleMatterSource.SpaceTangibleMatter);

                    // Look through all matter of the source to check whether all required matter is there
                    foreach (MatterValuedBase matterValuedBase in requiredMatter)
                    {
                        if (matterValuedBase.Necessity == Necessity.Mandatory)
                        {
                            float requiredMatterQuantity = matterValuedBase.Quantity.BaseValue;
                            foreach (MatterInstance matterInstance in allMatter)
                            {
                                if (matterInstance.IsNodeOf(matterValuedBase.MatterBase))
                                {
                                    matterToUse.Add(matterInstance);
                                    allMatter.Remove(matterInstance);
                                    requiredMatterQuantity -= matterInstance.Quantity.BaseValue;
                                    if (requiredMatterQuantity <= 0)
                                        break;
                                }
                            }
                            if (requiredMatterQuantity <= 0)
                            {
                                ok = false;
                                break;
                            }
                        }
                    }
                }

                if (ok)
                {
                    // Get all parts that should be used
                    List<TangibleObjectInstance> partsToUse = new List<TangibleObjectInstance>();
                    TangibleObjectInstance tangiblePartSource = partSource as TangibleObjectInstance;
                    if (tangiblePartSource != null)
                    {
                        // Get all available parts
                        List<TangibleObjectInstance> allParts = new List<TangibleObjectInstance>(tangiblePartSource.Parts);
                        if (!onlyGetPartsOfSource)
                            allParts.AddRange(tangiblePartSource.SpaceItems);

                        // Look through all parts of the source to check whether all required ones are there
                        foreach (PartBase partBase in tangibleObjectBase.Parts)
                        {
                            if (partBase.Necessity == Necessity.Mandatory)
                            {
                                int requiredPartQuantity = (int)partBase.Quantity.Value;
                                foreach (TangibleObjectInstance part in allParts)
                                {
                                    if (part.IsNodeOf(partBase.TangibleObjectBase))
                                    {
                                        partsToUse.Add(part);
                                        allParts.Remove(part);
                                        requiredPartQuantity--;
                                        if (requiredPartQuantity <= 0)
                                            break;
                                    }
                                }
                                if (requiredPartQuantity <= 0)
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }
                    }

                    // Create a new instance and assign it the matter and parts
                    if (ok)
                    {
                        TangibleObjectInstance tangibleObjectInstance = new TangibleObjectInstance(tangibleObjectBase, CreateOptions.Connections | CreateOptions.Items | CreateOptions.TangibleMatter | CreateOptions.Covers | CreateOptions.Spaces);
                        foreach (MatterInstance matterInstance in matterToUse)
                        {
                            if (matterInstance.TangibleObject != null)
                                matterInstance.TangibleObject.RemoveMatter(matterInstance);
                            tangibleObjectInstance.AddMatter(matterInstance);
                        }
                        foreach (TangibleObjectInstance part in partsToUse)
                        {
                            if (part.Whole != null)
                                part.Whole.RemovePart(part);
                            tangibleObjectInstance.AddPart(part);
                        }

                        return tangibleObjectInstance;
                    }
                }
            }

            return null;
        }
        #endregion Method: Create(TangibleObjectBase tangibleObjectBase, PhysicalObjectInstance matterSource, bool onlyGetMatterOfSource, PhysicalObjectInstance partSource, bool onlyGetPartsOfSource)

        #region Method: GetRequiredMatter(TangibleObjectBase tangibleObjectBase)
        /// <summary>
        /// Get the matter required to create an instance of the given tangible object.
        /// </summary>
        /// <param name="tangibleObjectBase">The tangible object base to get the required matter of.</param>
        /// <returns>The required matter to create an instance of the tangible object.</returns>
        protected virtual ReadOnlyCollection<MatterValuedBase> GetRequiredMatter(TangibleObjectBase tangibleObjectBase)
        {
            List<MatterValuedBase> requiredMatter = new List<MatterValuedBase>();

            foreach (MatterValuedBase matterValuedBase in tangibleObjectBase.Matter)
            {
                if (matterValuedBase.Necessity == Necessity.Mandatory)
                    requiredMatter.Add(matterValuedBase);
            }

            return requiredMatter.AsReadOnly();
        }
        #endregion Method: GetRequiredMatter(TangibleObjectBase tangibleObjectBase)
		
    }
    #endregion Class: InstanceManager

}
