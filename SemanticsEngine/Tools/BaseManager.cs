/**************************************************************************
 * 
 * BaseManager.cs
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
using System.ComponentModel;
using System.Reflection;
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Data;
using Semantics.Entities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using Action = Semantics.Abstractions.Action;
using Attribute = Semantics.Abstractions.Attribute;

namespace SemanticsEngine.Tools
{

    #region Class: BaseManager
    /// <summary>
    /// The base manager allows the creation of bases from ID holders.
    /// </summary>
    public class BaseManager
    {

        #region Properties and Fields

        #region Property: Current
        /// <summary>
        /// The current base manager.
        /// </summary>
        private static BaseManager current = new BaseManager();

        /// <summary>
        /// Gets the current base manager.
        /// </summary>
        public static BaseManager Current
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

        #region Field: bases
        /// <summary>
        /// A dictionary containing bases for ID holders.
        /// </summary>
        private Dictionary<IdHolder, Base> bases = new Dictionary<IdHolder, Base>();
        #endregion Field: bases
        
        #region Field: idHolderBaseType
        /// <summary>
        /// A dictionary containing type pairs of non-abstract ID holders and their corresponding base classes.
        /// </summary>
        private static Dictionary<Type, Type> idHolderBaseType = new Dictionary<Type, Type>();
        #endregion Field: idHolderBaseType

        #region Field: typeConstructor
        /// <summary>
        /// The constructors of types.
        /// </summary>
        private Dictionary<Type, ConstructorInfo> typeConstructor = new Dictionary<Type,ConstructorInfo>();
        #endregion Field: typeConstructor

        #region Property: PreloadProperties
        /// <summary>
        /// Indicates whether properties of bases should be preloaded during creation.
        /// </summary>
        private static bool preloadProperties = false;

        /// <summary>
        /// Gets or sets the value that indicates whether properties of bases should be preloaded during creation.
        /// Note that properties of already created bases will not be preloaded anymore!
        /// </summary>
        public static bool PreloadProperties
        {
            get
            {
                return preloadProperties;
            }
            set
            {
                preloadProperties = value;
            }
        }
        #endregion Property: PreloadProperties

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: BaseManager()
        /// <summary>
        /// Adds pairs of non-abstract ID holders and their corresponding base classes.
        /// </summary>
        static BaseManager()
        {
            // Abstractions
            AddIdHolderBasePair(typeof(Action), typeof(ActionBase));
            AddIdHolderBasePair(typeof(Attribute), typeof(AttributeBase));
            AddIdHolderBasePair(typeof(AttributeValued), typeof(AttributeValuedBase));
            AddIdHolderBasePair(typeof(ContextType), typeof(ContextTypeBase));
            AddIdHolderBasePair(typeof(Family), typeof(FamilyBase));
            AddIdHolderBasePair(typeof(FilterType), typeof(FilterTypeBase));
            AddIdHolderBasePair(typeof(Goal), typeof(GoalBase));
            AddIdHolderBasePair(typeof(Group), typeof(GroupBase));
            AddIdHolderBasePair(typeof(PredicateType), typeof(PredicateTypeBase));
            AddIdHolderBasePair(typeof(RelationshipType), typeof(RelationshipTypeBase));
            AddIdHolderBasePair(typeof(Scene), typeof(SceneBase));
            AddIdHolderBasePair(typeof(SceneValued), typeof(SceneValuedBase));
            AddIdHolderBasePair(typeof(State), typeof(StateBase));
            AddIdHolderBasePair(typeof(Unit), typeof(UnitBase));
            AddIdHolderBasePair(typeof(UnitCategory), typeof(UnitCategoryBase));

            // Entities
            AddIdHolderBasePair(typeof(AbstractEntity), typeof(AbstractEntityBase));
            AddIdHolderBasePair(typeof(AbstractEntityValued), typeof(AbstractEntityValuedBase));
            AddIdHolderBasePair(typeof(Compound), typeof(CompoundBase));
            AddIdHolderBasePair(typeof(CompoundValued), typeof(CompoundValuedBase));
            AddIdHolderBasePair(typeof(ConnectionItem), typeof(ConnectionItemBase));
            AddIdHolderBasePair(typeof(Cover), typeof(CoverBase));
            AddIdHolderBasePair(typeof(Element), typeof(ElementBase));
            AddIdHolderBasePair(typeof(ElementValued), typeof(ElementValuedBase));
            AddIdHolderBasePair(typeof(EntityCreation), typeof(EntityCreationBase));
            AddIdHolderBasePair(typeof(Filter), typeof(FilterBase));
            AddIdHolderBasePair(typeof(Layer), typeof(LayerBase));
            AddIdHolderBasePair(typeof(Material), typeof(MaterialBase));
            AddIdHolderBasePair(typeof(MaterialValued), typeof(MaterialValuedBase));
            AddIdHolderBasePair(typeof(Mixture), typeof(MixtureBase));
            AddIdHolderBasePair(typeof(MixtureValued), typeof(MixtureValuedBase));
            AddIdHolderBasePair(typeof(Part), typeof(PartBase));
            AddIdHolderBasePair(typeof(Space), typeof(SpaceBase));
            AddIdHolderBasePair(typeof(SpaceValued), typeof(SpaceValuedBase));
            AddIdHolderBasePair(typeof(Substance), typeof(SubstanceBase));
            AddIdHolderBasePair(typeof(SubstanceValued), typeof(SubstanceValuedBase));
            AddIdHolderBasePair(typeof(TangibleObject), typeof(TangibleObjectBase));
            AddIdHolderBasePair(typeof(TangibleObjectValued), typeof(TangibleObjectValuedBase));

            // Components
            AddIdHolderBasePair(typeof(AddRemoveRequirements), typeof(AddRemoveRequirementsBase));
            AddIdHolderBasePair(typeof(Chance), typeof(ChanceBase));
            AddIdHolderBasePair(typeof(CombinedRelationship), typeof(CombinedRelationshipBase));
            AddIdHolderBasePair(typeof(Context), typeof(ContextBase));
            AddIdHolderBasePair(typeof(Deletion), typeof(DeletionBase));
            AddIdHolderBasePair(typeof(Equation), typeof(EquationBase));
            AddIdHolderBasePair(typeof(RelationshipEstablishment), typeof(RelationshipEstablishmentBase));
            AddIdHolderBasePair(typeof(Event), typeof(EventBase));
            AddIdHolderBasePair(typeof(EventRequirements), typeof(EventRequirementsBase));
            AddIdHolderBasePair(typeof(FilterApplication), typeof(FilterApplicationBase));
            AddIdHolderBasePair(typeof(Range), typeof(RangeBase));
            AddIdHolderBasePair(typeof(Reaction), typeof(ReactionBase));
            AddIdHolderBasePair(typeof(Relationship), typeof(RelationshipBase));
            AddIdHolderBasePair(typeof(Requirements), typeof(RequirementsBase));
            AddIdHolderBasePair(typeof(StateGroup), typeof(StateGroupBase));
            AddIdHolderBasePair(typeof(Transfer), typeof(TransferBase));
            AddIdHolderBasePair(typeof(Transformation), typeof(TransformationBase));
            AddIdHolderBasePair(typeof(Time), typeof(TimeBase));
            AddIdHolderBasePair(typeof(BoolValue), typeof(BoolValueBase));
            AddIdHolderBasePair(typeof(NumericalValue), typeof(NumericalValueBase));
            AddIdHolderBasePair(typeof(StringValue), typeof(StringValueBase));
            AddIdHolderBasePair(typeof(TermValue), typeof(TermValueBase));
            AddIdHolderBasePair(typeof(VectorValue), typeof(VectorValueBase));
            AddIdHolderBasePair(typeof(NumericalValueRange), typeof(NumericalValueRangeBase));
            AddIdHolderBasePair(typeof(BoolVariable), typeof(BoolVariableBase));
            AddIdHolderBasePair(typeof(NumericalVariable), typeof(NumericalVariableBase));
            AddIdHolderBasePair(typeof(StringVariable), typeof(StringVariableBase));
            AddIdHolderBasePair(typeof(VectorVariable), typeof(VectorVariableBase));
            AddIdHolderBasePair(typeof(RandomVariable), typeof(RandomVariableBase));
            AddIdHolderBasePair(typeof(TermVariable), typeof(TermVariableBase));

            // References
            AddIdHolderBasePair(typeof(SpatialReference), typeof(SpatialReferenceBase));
            AddIdHolderBasePair(typeof(SpaceReference), typeof(SpaceReferenceBase));
            AddIdHolderBasePair(typeof(RelationshipReference), typeof(RelationshipReferenceBase));
            AddIdHolderBasePair(typeof(PartReference), typeof(PartReferenceBase));
            AddIdHolderBasePair(typeof(CoverReference), typeof(CoverReferenceBase));
            AddIdHolderBasePair(typeof(ConnectionReference), typeof(ConnectionReferenceBase));
            AddIdHolderBasePair(typeof(ItemReference), typeof(ItemReferenceBase));
            AddIdHolderBasePair(typeof(MatterReference), typeof(MatterReferenceBase));
            AddIdHolderBasePair(typeof(LayerReference), typeof(LayerReferenceBase));
            AddIdHolderBasePair(typeof(SetReference), typeof(SetReferenceBase));
            AddIdHolderBasePair(typeof(OwnerReference), typeof(OwnerReferenceBase));

            // Conditions
            AddIdHolderBasePair(typeof(AttributeCondition), typeof(AttributeConditionBase));
            AddIdHolderBasePair(typeof(RelationshipCondition), typeof(RelationshipConditionBase));
            AddIdHolderBasePair(typeof(SpatialRequirement), typeof(SpatialRequirementBase));
            AddIdHolderBasePair(typeof(StateGroupCondition), typeof(StateGroupConditionBase));
            AddIdHolderBasePair(typeof(BoolValueCondition), typeof(BoolValueConditionBase));
            AddIdHolderBasePair(typeof(NumericalValueCondition), typeof(NumericalValueConditionBase));
            AddIdHolderBasePair(typeof(StringValueCondition), typeof(StringValueConditionBase));
            AddIdHolderBasePair(typeof(VectorValueCondition), typeof(VectorValueConditionBase));
            AddIdHolderBasePair(typeof(AbstractEntityCondition), typeof(AbstractEntityConditionBase));
            AddIdHolderBasePair(typeof(CompoundCondition), typeof(CompoundConditionBase));
            AddIdHolderBasePair(typeof(ConnectionItemCondition), typeof(ConnectionItemConditionBase));
            AddIdHolderBasePair(typeof(CoverCondition), typeof(CoverConditionBase));
            AddIdHolderBasePair(typeof(ElementCondition), typeof(ElementConditionBase));
            AddIdHolderBasePair(typeof(LayerCondition), typeof(LayerConditionBase));
            AddIdHolderBasePair(typeof(MaterialCondition), typeof(MaterialConditionBase));
            AddIdHolderBasePair(typeof(MixtureCondition), typeof(MixtureConditionBase));
            AddIdHolderBasePair(typeof(PartCondition), typeof(PartConditionBase));
            AddIdHolderBasePair(typeof(SpaceCondition), typeof(SpaceConditionBase));
            AddIdHolderBasePair(typeof(SubstanceCondition), typeof(SubstanceConditionBase));
            AddIdHolderBasePair(typeof(TangibleObjectCondition), typeof(TangibleObjectConditionBase));

            // Changes
            AddIdHolderBasePair(typeof(AttributeChange), typeof(AttributeChangeBase));
            AddIdHolderBasePair(typeof(RelationshipChange), typeof(RelationshipChangeBase));
            AddIdHolderBasePair(typeof(StateGroupChange), typeof(StateGroupChangeBase));
            AddIdHolderBasePair(typeof(BoolValueChange), typeof(BoolValueChangeBase));
            AddIdHolderBasePair(typeof(NumericalValueChange), typeof(NumericalValueChangeBase));
            AddIdHolderBasePair(typeof(StringValueChange), typeof(StringValueChangeBase));
            AddIdHolderBasePair(typeof(VectorValueChange), typeof(VectorValueChangeBase));
            AddIdHolderBasePair(typeof(AbstractEntityChange), typeof(AbstractEntityChangeBase));
            AddIdHolderBasePair(typeof(CompoundChange), typeof(CompoundChangeBase));
            AddIdHolderBasePair(typeof(ConnectionItemChange), typeof(ConnectionItemChangeBase));
            AddIdHolderBasePair(typeof(CoverChange), typeof(CoverChangeBase));
            AddIdHolderBasePair(typeof(ElementChange), typeof(ElementChangeBase));
            AddIdHolderBasePair(typeof(LayerChange), typeof(LayerChangeBase));
            AddIdHolderBasePair(typeof(MaterialChange), typeof(MaterialChangeBase));
            AddIdHolderBasePair(typeof(MixtureChange), typeof(MixtureChangeBase));
            AddIdHolderBasePair(typeof(PartChange), typeof(PartChange));
            AddIdHolderBasePair(typeof(SpaceChange), typeof(SpaceChangeBase));
            AddIdHolderBasePair(typeof(SubstanceChange), typeof(SubstanceChangeBase));
            AddIdHolderBasePair(typeof(TangibleObjectChange), typeof(TangibleObjectChangeBase));
        }
        #endregion Static Constructor: BaseManager()

        #region Constructor: BaseManager()
        /// <summary>
        /// Creates a new base manager.
        /// </summary>
        protected BaseManager()
        {
        }
        #endregion Constructor: BaseManager()

        #endregion Method Group: Constructors

        #region Method: AddIdHolderBasePair(Type idHolderType, Type baseType)
        /// <summary>
        /// Add a pair of a non-abstract ID holder and its corresponding base class.
        /// </summary>
        /// <param name="idHolderType">The type of the ID holder.</param>
        /// <param name="baseType">The type of the corresponding base.</param>
        protected static void AddIdHolderBasePair(Type idHolderType, Type baseType)
        {
            if (idHolderType != null && baseType != null)
                idHolderBaseType.Add(idHolderType, baseType);
        }
        #endregion Method: AddIdHolderBasePair(Type idHolderType, Type baseType)

        #region Method: GetBase(IdHolder idHolder)
        /// <summary>
        /// Get the base for the given ID holder, or create one if it does not yet exist.
        /// </summary>
        /// <param name="idHolder">The ID holder to get the base from.</param>
        /// <returns>The base for the ID holder.</returns>
        public Base GetBase(IdHolder idHolder)
        {
            return GetBase<Base>(idHolder);
        }
        #endregion Method: GetBase(IdHolder idHolder)

        #region Method: GetBase<T>(IdHolder idHolder)
        /// <summary>
        /// Get the base for the given ID holder, or create one if it does not yet exist.
        /// </summary>
        /// <typeparam name="T">The type the base should get.</typeparam>
        /// <param name="idHolder">The ID holder to get the base from.</param>
        /// <returns>The base for the ID holder.</returns>
        public T GetBase<T>(IdHolder idHolder)
            where T : Base
        {
            if (idHolder != null)
            {
                try
                {
                    // Check whether the base already exists
                    Base bas = null;
                    if (bases.TryGetValue(idHolder, out bas))
                    {
                        return (T)bas;
                    }
                    else
                    {
                        // Create a new base
                        Type type = idHolder.GetType();
                        ConstructorInfo constructorInfo;
                        if (!typeConstructor.TryGetValue(type, out constructorInfo))
                        {
                            constructorInfo = idHolderBaseType[type].GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { type }, null);
                            typeConstructor.Add(type, constructorInfo);
                        }
                        T createdBase = (T)constructorInfo.Invoke(new object[] { idHolder });

                        // Subscribe for changed properties of the ID holder
                        idHolder.PropertyChanged += new PropertyChangedEventHandler(idHolder_PropertyChanged);

                        return createdBase;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception in BaseManager.GetBase<T>: " + ex.Message);
                }
            }

            return default(T);
        }
        #endregion Method: GetBase<T>(IdHolder idHolder)

        #region Method: RegisterBase(IdHolder idHolder, Base bas)
        /// <summary>
        /// Register the given base for the given ID holder.
        /// </summary>
        /// <param name="idHolder">The ID holder.</param>
        /// <param name="bas">The base for the ID holder.</param>
        internal void RegisterBase(IdHolder idHolder, Base bas)
        {
            if (idHolder != null && !this.bases.ContainsKey(idHolder))
                this.bases.Add(idHolder, bas);
        }
        #endregion Method: RegisterBase(IdHolder idHolder, Base bas)

        #region Method: idHolder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        /// <summary>
        /// Handles the change of a property of the ID holder.
        /// </summary>
        private void idHolder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IdHolder idHolder = sender as IdHolder;
            if (sender != null)
            {
                // Get the old base
                Base oldBase = null;
                bases.TryGetValue(idHolder, out oldBase);

                // Recreate the base
                Type type = idHolder.GetType();
                Base newBase = (Base)idHolderBaseType[type].GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { type }, null).Invoke(new object[] { idHolder });

                // In the table, replace the old base by the new one
                bases[idHolder] = newBase;

                // Let the base know of this change, so it can notify its instances
                if (oldBase != null)
                    oldBase.ReplaceBy(newBase);
            }
        }
        #endregion Method: idHolder_PropertyChanged(object sender, PropertyChangedEventArgs e)

        #region Method: GetIdHolder<T>(Base bas)
        /// <summary>
        /// Get the ID holder for the given base. Be aware that changing the base can have drastic effects!
        /// </summary>
        /// <typeparam name="T">The type of the ID holder.</typeparam>
        /// <param name="bas">The base to get the ID holder of.</param>
        /// <returns>The ID holder of the base.</returns>
        public T GetIdHolder<T>(Base bas)
            where T : IdHolder
        {
            if (bas != null)
                return (T)bas.IdHolder;

            return default(T);
        }
        #endregion Method: GetIdHolder<T>(Base bas)

        #region Method: PreloadBases(IEnumerable<IdHolder> list, bool preloadProperties)
        /// <summary>
        /// Preload all the ID holders in the given list, and, optionally, also preload all their properties.
        /// </summary>
        /// <param name="list">A list with all ID holders that should be preloaded.</param>
        /// <param name="preloadProperties">False when only the bases should be preloaded; true when all properties should be preloaded as well.</param>
        public void PreloadBases(IEnumerable<IdHolder> list, bool preloadProperties)
        {
            // Bases should access this property by themselves to handle the preloading
            PreloadProperties = preloadProperties;

            // Create all bases
            if (list != null)
            {
                foreach (IdHolder idHolder in list)
                    GetBase(idHolder);
            }
        }
        #endregion Method: PreloadBases(IEnumerable<IdHolder> list, bool preloadProperties)

        #region Method: GetBases<T, T2>()
        /// <summary>
        /// Get all bases of the nodes of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the nodes to get the bases of.</typeparam>
        /// <typeparam name="T2">The type of the bases to return.</typeparam>
        /// <returns>Returns all bases of the node type.</returns>
        public List<T2> GetBases<T, T2>()
            where T : Node
            where T2: Base
        {
            List<T2> bases = new List<T2>();

            foreach (T node in DatabaseSearch.GetNodes<T>())
                bases.Add(GetBase<T2>(node));

            return bases;
        }
        #endregion Method: GetBases<T, T2>()
		
        #region Method: Clear()
        /// <summary>
        /// Clear the base manager.
        /// </summary>
        internal void Clear()
        {
            this.bases.Clear();
        }
        #endregion Method: Clear()

    }
    #endregion Class: BaseManager

}