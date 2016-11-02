/**************************************************************************
 * 
 * RequirementsBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Entities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: RequirementsBase
    /// <summary>
    /// A base for requirements.
    /// </summary>  
    public sealed class RequirementsBase : Base
    {

        #region Properties and Fields

        #region Property: Requirements
        /// <summary>
        /// Gets the requirements of which this is a requirements base.
        /// </summary>
        internal Requirements Requirements
        {
            get
            {
                return this.IdHolder as Requirements;
            }
        }
        #endregion Property: Requirements

        #region Property: ContextTypes
        /// <summary>
        /// The required context types.
        /// </summary>
        private ContextTypeBase[] contextTypes = null;
        
        /// <summary>
        /// Gets the required context types.
        /// </summary>
        public ReadOnlyCollection<ContextTypeBase> ContextTypes
        {
            get
            {
                if (contextTypes == null)
                {
                    LoadContextTypes();
                    if (contextTypes == null)
                        return new List<ContextTypeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ContextTypeBase>(contextTypes);
            }
        }

        /// <summary>
        /// Loads the required context types.
        /// </summary>
        private void LoadContextTypes()
        {
            if (this.Requirements != null)
            {
                List<ContextTypeBase> contextTypeBases = new List<ContextTypeBase>();
                foreach (ContextType contextType in this.Requirements.ContextTypes)
                    contextTypeBases.Add(BaseManager.Current.GetBase<ContextTypeBase>(contextType));
                contextTypes = contextTypeBases.ToArray();
            }
        }
        #endregion Property: ContextTypes

        #region Property: Conditions
        /// <summary>
        /// The extra conditions, besides the required context types.
        /// </summary>
        private ConditionBase[] conditions = null;

        /// <summary>
        /// Gets the extra conditions, besides the required context types.
        /// </summary>
        public ReadOnlyCollection<ConditionBase> Conditions
        {
            get
            {
                if (conditions == null)
                {
                    LoadConditions();
                    if (conditions == null)
                        return new List<ConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ConditionBase>(conditions);
            }
        }

        /// <summary>
        /// Loads the extra conditions.
        /// </summary>
        private void LoadConditions()
        {
            if (this.Requirements != null)
            {
                List<ConditionBase> conditionBases = new List<ConditionBase>();
                foreach (Condition condition in this.Requirements.Conditions)
                    conditionBases.Add(BaseManager.Current.GetBase<ConditionBase>(condition));
                conditions = conditionBases.ToArray();
            }
        }
        #endregion Property: Conditions

        #region Property: SpatialRequirements
        /// <summary>
        /// The dictionary that defines the spatial requirement between the subject and other entities.
        /// </summary>
        private Dictionary<EntityConditionBase, SpatialRequirementBase> spatialRequirements = null;

        /// <summary>
        /// Gets the dictionary that defines the spatial requirement between the subject and other entities.
        /// </summary>
        public Dictionary<EntityConditionBase, SpatialRequirementBase> SpatialRequirements
        {
            get
            {
                if (spatialRequirements == null)
                {
                    LoadSpatialRequirements();
                    if (spatialRequirements == null)
                        return new Dictionary<EntityConditionBase, SpatialRequirementBase>();
                }
                return spatialRequirements;
            }
        }

        /// <summary>
        /// Loads the dictionary that defines the spatial requirement between the subject and other entities.
        /// </summary>
        private void LoadSpatialRequirements()
        {
            if (this.Requirements != null)
            {
                spatialRequirements = new Dictionary<EntityConditionBase, SpatialRequirementBase>();
                foreach (KeyValuePair<EntityCondition, SpatialRequirement> spatialRequirement in this.Requirements.SpatialRequirements)
                {
                    EntityConditionBase entityConditionBase = BaseManager.Current.GetBase<EntityConditionBase>(spatialRequirement.Key);
                    SpatialRequirementBase spatialRequirementBase = BaseManager.Current.GetBase<SpatialRequirementBase>(spatialRequirement.Value);
                    spatialRequirements.Add(entityConditionBase, spatialRequirementBase);
                }
            }
        }
        #endregion Property: SpatialRequirements

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RequirementsBase(Requirements requirements)
        /// <summary>
        /// Creates a new requirements base from the given requirements.
        /// </summary>
        /// <param name="requirements">The requirements to create a base from.</param>
        internal RequirementsBase(Requirements requirements)
            : base(requirements)
        {
            if (requirements != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadContextTypes();
                    LoadConditions();
                    LoadSpatialRequirements();
                }
            }
        }
        #endregion Constructor: RequirementsBase(Requirements requirements)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: IsSatisfied(EntityInstance subject, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the requirements are satisfied for the given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the requirements are satisfied for the given subject.</returns>
        public bool IsSatisfied(EntityInstance subject, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (subject != null)
            {
                // Check whether the subject has the required context types
                foreach (ContextTypeBase contextTypeBase in this.ContextTypes)
                {
                    if (!subject.ContextTypes.Contains(contextTypeBase))
                        return false;
                }

                // Check the extra conditions
                foreach (ConditionBase extraCondition in this.Conditions)
                {
                    if (!subject.Satisfies(extraCondition, iVariableInstanceHolder))
                        return false;
                }

                // Check the spatial requirements
                PhysicalEntityInstance physicalSubject = subject as PhysicalEntityInstance;
                if (this.SpatialRequirements.Count > 0 && (physicalSubject == null || subject.World == null))
                    return false;
                foreach (KeyValuePair<EntityConditionBase, SpatialRequirementBase> pair in this.SpatialRequirements)
                {
                    bool isSatisfied = false;
                    foreach (PhysicalEntityInstance physicalEntityInstance in subject.World.GetInstances<PhysicalEntityInstance>())
                    {
                        if (physicalEntityInstance.Satisfies(pair.Key, iVariableInstanceHolder) && pair.Value.IsSatisfied(physicalSubject, physicalEntityInstance, iVariableInstanceHolder))
                        {
                            isSatisfied = true;
                            break;
                        }
                    }
                    if (!isSatisfied)
                        return false;
                }
            }
            return true;
        }
        #endregion Method: IsSatisfied(EntityInstance subject, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: RequirementsBase

}