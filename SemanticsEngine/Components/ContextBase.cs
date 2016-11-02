/**************************************************************************
 * 
 * ContextBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Components;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: ContextBase
    /// <summary>
    /// A base of a context.
    /// </summary>
    public sealed class ContextBase : Base
    {

        #region Properties and Fields

        #region Property: Context
        /// <summary>
        /// Gets the context of which this is a base.
        /// </summary>
        internal Context Context
        {
            get
            {
                return this.IdHolder as Context;
            }
        }
        #endregion Property: Context

        #region Property: ContextType
        /// <summary>
        /// The context type of the context.
        /// </summary>
        private ContextTypeBase contextType = null;

        /// <summary>
        /// Gets the context type of the context.
        /// </summary>
        public ContextTypeBase ContextType
        {
            get
            {
                return contextType;
            }
        }
        #endregion Property: ContextType

        #region Property: Subject
        /// <summary>
        /// The subject of the context.
        /// </summary>
        private EntityBase subject = null;
        
        /// <summary>
        /// Gets the subject of the context.
        /// </summary>
        public EntityBase Subject
        {
            get
            {
                return subject;
            }
        }
        #endregion Property: Subject

        #region Property: Conditions
        /// <summary>
        /// The conditions in the context.
        /// </summary>
        private ConditionBase[] conditions = null;

        /// <summary>
        /// Gets the conditions in the context.
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
        /// Loads the conditions.
        /// </summary>
        private void LoadConditions()
        {
            if (this.Context != null)
            {
                List<ConditionBase> contextConditions = new List<ConditionBase>();
                foreach (Condition condition in this.Context.Conditions)
                    contextConditions.Add(BaseManager.Current.GetBase<ConditionBase>(condition));
                conditions = contextConditions.ToArray();
            }
        }
        #endregion Property: Conditions

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ContextBase(Context context)
        /// <summary>
        /// Creates a new context base from the given context.
        /// </summary>
        /// <param name="context">The context to create a base from.</param>
        internal ContextBase(Context context)
            : base(context)
        {
            if (context != null)
            {
                this.contextType = BaseManager.Current.GetBase<ContextTypeBase>(context.ContextType);
                this.subject = BaseManager.Current.GetBase<EntityBase>(context.Subject);

                if (BaseManager.PreloadProperties)
                    LoadConditions();
            }
        }
        #endregion Constructor: ContextBase(Context context)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: IsSatisfied(EntityInstance entityInstance, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the context is satisfied for the given entity instance.
        /// </summary>
        /// <param name="entityInstance">The entity instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the context is satisfied for the given entity instance.</returns>
        public bool IsSatisfied(EntityInstance entityInstance, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (entityInstance != null && this.subject != null)
            {
                // Check whether the instance is of the correct type
                if (entityInstance.IsNodeOf(this.subject))
                {
                    // Check the conditions
                    foreach (ConditionBase condition in this.Conditions)
                    {
                        if (!entityInstance.Satisfies(condition, iVariableInstanceHolder))
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }
        #endregion Method: IsSatisfied(EntityInstance entityInstance, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            if (this.ContextType != null)
                return this.ContextType.ToString();

            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: ContextBase

}
