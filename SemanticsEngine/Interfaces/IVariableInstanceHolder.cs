/**************************************************************************
 * 
 * IVariableInstanceHolder.cs
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
using SemanticsEngine.Components;
using SemanticsEngine.Entities;

namespace SemanticsEngine.Interfaces
{

    #region Interface: IVariableInstanceHolder
    /// <summary>
    /// An interface for a class that holds variable and reference instances.
    /// </summary>
    public interface IVariableInstanceHolder
    {

        /// <summary>
        /// Get the variable instance for the given variable base.
        /// </summary>
        /// <param name="variableBase">The variable base to get the variable instance of.</param>
        /// <returns>The variable instance of the variable base.</returns>
        VariableInstance GetVariable(VariableBase variableBase);

        /// <summary>
        /// Get the manual input.
        /// </summary>
        /// <returns>The manual input.</returns>
        Dictionary<string, object> GetManualInput();

        /// <summary>
        /// Get the actor.
        /// </summary>
        /// <returns>The actor.</returns>
        EntityInstance GetActor();

        /// <summary>
        /// Get the target.
        /// </summary>
        /// <returns>The target.</returns>
        EntityInstance GetTarget();

        /// <summary>
        /// Get the artifact.
        /// </summary>
        /// <returns>The artifact.</returns>
        EntityInstance GetArtifact();

    }
    #endregion Interface: IVariableInstanceHolder

    #region Class: VariableReferenceInstanceHolder
    /// <summary>
    /// A holder for variable and reference instances.
    /// </summary>
    internal class VariableReferenceInstanceHolder
    {

        #region Properties and Fields

        #region Field: variables
        /// <summary>
        /// All variables.
        /// </summary>
        private Dictionary<VariableBase, VariableInstance> variables = new Dictionary<VariableBase, VariableInstance>();
        #endregion Field: variables

        #endregion Properties and Fields

        #region Constructor: VariableReferenceInstanceHolder()
        /// <summary>
        /// Creates a new variable and reference holder.
        /// </summary>
        public VariableReferenceInstanceHolder()
        {
        }
        #endregion Constructor: VariableReferenceInstanceHolder() 

        #region Constructor: VariableReferenceInstanceHolder(VariableReferenceInstanceHolder variableReferenceInstanceHolder)
        /// <summary>
        /// Clones the variable and reference instance holder.
        /// </summary>
        /// <param name="variableReferenceInstanceHolder">The variable and reference holder to clone.</param>
        public VariableReferenceInstanceHolder(VariableReferenceInstanceHolder variableReferenceInstanceHolder)
        {
            foreach (KeyValuePair<VariableBase, VariableInstance> variable in variableReferenceInstanceHolder.variables)
                this.variables.Add(variable.Key, variable.Value.Clone());
        }
        #endregion Constructor: VariableReferenceInstanceHolder(VariableReferenceInstanceHolder variableReferenceInstanceHolder) 

        #region Method: GetVariable(VariableBase variableBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the variable instance for the given variable base.
        /// </summary>
        /// <param name="variableBase">The variable base to get the variable instance of.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The variable instance of the variable base.</returns>
        public VariableInstance GetVariable(VariableBase variableBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            VariableInstance variableInstance = null;
            if (!this.variables.TryGetValue(variableBase, out variableInstance))
            {
                if (variableBase is NumericalVariableBase)
                    variableInstance = new NumericalVariableInstance((NumericalVariableBase)variableBase, iVariableInstanceHolder);
                else if (variableBase is VectorVariableBase)
                    variableInstance = new VectorVariableInstance((VectorVariableBase)variableBase, iVariableInstanceHolder);
                else if (variableBase is BoolVariableBase)
                    variableInstance = new BoolVariableInstance((BoolVariableBase)variableBase, iVariableInstanceHolder);
                else if (variableBase is StringVariableBase)
                    variableInstance = new StringVariableInstance((StringVariableBase)variableBase, iVariableInstanceHolder);
                else if (variableBase is TermVariableBase)
                    variableInstance = new TermVariableInstance((TermVariableBase)variableBase, iVariableInstanceHolder);

                if (variableInstance != null)
                    this.variables.Add(variableBase, variableInstance);
            }
            return variableInstance;
        }
        #endregion Method: GetVariable(VariableBase variableBase, IVariableInstanceHolder iVariableInstanceHolder)

    }
    #endregion Class: VariableReferenceInstanceHolder

}
