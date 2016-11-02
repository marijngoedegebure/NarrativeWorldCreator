/**************************************************************************
 * 
 * ContextTypeBase.cs
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
using Semantics.Abstractions;
using Semantics.Components;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: ContextTypeBase
    /// <summary>
    /// A base of a context type.
    /// </summary>
    public class ContextTypeBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: ContextType
        /// <summary>
        /// Gets the context type of which this is a context type base.
        /// </summary>
        protected internal ContextType ContextType
        {
            get
            {
                return this.IdHolder as ContextType;
            }
        }
        #endregion Property: ContextType

        #region Property: Contexts
        /// <summary>
        /// The contexts of the context type.
        /// </summary>
        private ContextBase[] contexts = null;
        
        /// <summary>
        /// Gets the contexts of the context type.
        /// </summary>
        public ReadOnlyCollection<ContextBase> Contexts
        {
            get
            {
                if (contexts == null)
                {
                    LoadContexts();
                    if (contexts == null)
                        return new List<ContextBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ContextBase>(contexts);
            }
        }

        /// <summary>
        /// Loads the contexts.
        /// </summary>
        private void LoadContexts()
        {
            if (this.ContextType != null)
            {
                List<ContextBase> contextBases = new List<ContextBase>();
                foreach (Context context in this.ContextType.PersonalContexts)
                    contextBases.Add(BaseManager.Current.GetBase<ContextBase>(context));
                contexts = contextBases.ToArray();
            }
        }
        #endregion Property: Contexts

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ContextTypeBase(ContextType contextType)
        /// <summary>
        /// Create a context type base from the given context type.
        /// </summary>
        /// <param name="contextType">The context type to create a context type base from.</param>
        protected internal ContextTypeBase(ContextType contextType)
            : base(contextType)
        {
            if (contextType != null)
            {
                if (BaseManager.PreloadProperties)
                    LoadContexts();
            }
        }
        #endregion Constructor: ContextTypeBase(ContextType contextType)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: IsSatisfied(EntityInstance subject, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the context type is satisfied for the given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the context type is satisfied for the given subject.</returns>
        public bool IsSatisfied(EntityInstance subject, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (subject != null)
            {
                foreach (ContextBase contextBase in this.Contexts)
                {
                    if (contextBase.IsSatisfied(subject, iVariableInstanceHolder))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: IsSatisfied(EntityInstance subject, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: ContextTypeBase

}
