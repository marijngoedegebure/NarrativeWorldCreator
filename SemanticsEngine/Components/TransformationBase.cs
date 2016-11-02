/**************************************************************************
 * 
 * TransformationBase.cs
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
using Semantics.Entities;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: TransformationBase
    /// <summary>
    /// A base of a transformation.
    /// </summary>
    public sealed class TransformationBase : EffectBase
    {

        #region Properties and Fields

        #region Property: Transformation
        /// <summary>
        /// Gets the transformation of which this is a transformation base.
        /// </summary>
        internal Transformation Transformation
        {
            get
            {
                return this.Effect as Transformation;
            }
        }
        #endregion Property: Transformation

        #region Property: Entities
        /// <summary>
        /// All the entities to transform into.
        /// </summary>
        private EntityValuedBase[] entities = null;

        /// <summary>
        /// Gets all the entities to transform into.
        /// </summary>
        public ReadOnlyCollection<EntityValuedBase> Entities
        {
            get
            {
                if (entities == null)
                {
                    LoadEntities();
                    if (entities == null)
                        return new List<EntityValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<EntityValuedBase>(entities);
            }
        }

        /// <summary>
        /// Loads the entities.
        /// </summary>
        private void LoadEntities()
        {
            if (this.Transformation != null)
            {
                List<EntityValuedBase> entityValuedBases = new List<EntityValuedBase>();
                foreach (EntityValued entityValued in this.Transformation.Entities)
                    entityValuedBases.Add(BaseManager.Current.GetBase<EntityValuedBase>(entityValued));
                entities = entityValuedBases.ToArray();
            }
        }
        #endregion Property: Entities

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TransformationBase(Transformation transformation)
        /// <summary>
        /// Creates a base of the given transformation.
        /// </summary>
        /// <param name="transformation">The transformation to create a base of.</param>
        internal TransformationBase(Transformation transformation)
            : base(transformation)
        {
            if (transformation != null)
            {
                if (BaseManager.PreloadProperties)
                    LoadEntities();
            }
        }
        #endregion Constructor: TransformationBase(Transformation transformation)

        #endregion Method Group: Constructors

    }
    #endregion Class: TransformationBase

}