/**************************************************************************
 * 
 * PhysicalObjectBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region Class: PhysicalObjectBase
    /// <summary>
    /// A base of a physical object.
    /// </summary>
    public abstract class PhysicalObjectBase : PhysicalEntityBase
    {

        #region Properties and Fields

        #region Property: PhysicalObject
        /// <summary>
        /// Gets the physical object of which this is a physical object base.
        /// </summary>
        protected internal PhysicalObject PhysicalObject
        {
            get
            {
                return this.IdHolder as PhysicalObject;
            }
        }
        #endregion Property: PhysicalObject

        #region Property: Spaces
        /// <summary>
        /// The spaces of the physical object.
        /// </summary>
        private SpaceValuedBase[] spaces = null;

        /// <summary>
        /// Gets the spaces of the physical object.
        /// </summary>
        public ReadOnlyCollection<SpaceValuedBase> Spaces
        {
            get
            {
                if (spaces == null)
                {
                    LoadSpaces();
                    if (spaces == null)
                        return new List<SpaceValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SpaceValuedBase>(spaces);
            }
        }

        /// <summary>
        /// Loads the spaces.
        /// </summary>
        private void LoadSpaces()
        {
            if (this.PhysicalObject != null)
            {
                List<SpaceValuedBase> physicalObjectSpaces = new List<SpaceValuedBase>();
                foreach (SpaceValued spaceValued in this.PhysicalObject.Spaces)
                    physicalObjectSpaces.Add(BaseManager.Current.GetBase<SpaceValuedBase>(spaceValued));
                spaces = physicalObjectSpaces.ToArray();
            }
        }
        #endregion Property: Spaces

        #region Property: DefaultSpace
        /// <summary>
        /// The default space.
        /// </summary>
        private SpaceValuedBase defaultSpace = null;

        /// <summary>
        /// Gets the default space.
        /// </summary>
        public SpaceValuedBase DefaultSpace
        {
            get
            {
                if (defaultSpace == null)
                    LoadDefaultSpace();
                return defaultSpace;
            }
        }

        /// <summary>
        /// Loads the default space.
        /// </summary>
        private void LoadDefaultSpace()
        {
            if (this.PhysicalObject != null)
                defaultSpace = BaseManager.Current.GetBase<SpaceValuedBase>(this.PhysicalObject.DefaultSpace);
        }
        #endregion Property: DefaultSpace

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalObjectBase(PhysicalObject physicalObject)
        /// <summary>
        /// Creates a new physical object base from the given physical object.
        /// </summary>
        /// <param name="physicalObject">The physical object to create a physical object base from.</param>
        protected PhysicalObjectBase(PhysicalObject physicalObject)
            : base(physicalObject)
        {
            if (physicalObject != null)
            {
                if (BaseManager.PreloadProperties)
                    LoadSpaces();
            }
        }
        #endregion Constructor: PhysicalObjectBase(PhysicalObject physicalObject)

        #region Constructor: PhysicalObjectBase(PhysicalObjectValued physicalObjectValued)
        /// <summary>
        /// Creates a new physical object base from the given valued physical object.
        /// </summary>
        /// <param name="physicalObjectValued">The valued physical object to create a physical object base from.</param>
        protected PhysicalObjectBase(PhysicalObjectValued physicalObjectValued)
            : base(physicalObjectValued)
        {
            if (physicalObjectValued != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadSpaces();
                    LoadDefaultSpace();
                }
            }
        }
        #endregion Constructor: PhysicalObjectBase(PhysicalObjectValued physicalObjectValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: PhysicalObjectBase

    #region Class: PhysicalObjectValuedBase
    /// <summary>
    /// A base of a valued physical object.
    /// </summary>
    public abstract class PhysicalObjectValuedBase : PhysicalEntityValuedBase
    {

        #region Properties and Fields

        #region Property: PhysicalObjectValued
        /// <summary>
        /// Gets the valued physical object of which this is a physical object base.
        /// </summary>
        protected internal PhysicalObjectValued PhysicalObjectValued
        {
            get
            {
                return this.NodeValued as PhysicalObjectValued;
            }
        }
        #endregion Property: PhysicalObjectValued

        #region Property: PhysicalObjectBase
        /// <summary>
        /// Gets the physical object of which this is a valued physical object base.
        /// </summary>
        public PhysicalObjectBase PhysicalObjectBase
        {
            get
            {
                return this.NodeBase as PhysicalObjectBase;
            }
        }
        #endregion Property: PhysicalObjectBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalObjectValuedBase(PhysicalObjectValued physicalObjectValued)
        /// <summary>
        /// Creates a new valued physical object base from the given valued physical object.
        /// </summary>
        /// <param name="physicalObjectValued">The valued physical object to create a valued physical object base from.</param>
        protected PhysicalObjectValuedBase(PhysicalObjectValued physicalObjectValued)
            : base(physicalObjectValued)
        {
        }
        #endregion Constructor: PhysicalObjectValuedBase(PhysicalObjectValued physicalObjectValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: PhysicalObjectValuedBase

    #region Class: PhysicalObjectConditionBase
    /// <summary>
    /// A condition on a physical object.
    /// </summary>
    public abstract class PhysicalObjectConditionBase : PhysicalEntityConditionBase
    {

        #region Properties and Fields

        #region Property: PhysicalObjectCondition
        /// <summary>
        /// Gets the physical object condition of which this is a physical object condition base.
        /// </summary>
        protected internal PhysicalObjectCondition PhysicalObjectCondition
        {
            get
            {
                return this.Condition as PhysicalObjectCondition;
            }
        }
        #endregion Property: PhysicalObjectCondition

        #region Property: PhysicalObjectBase
        /// <summary>
        /// Gets the physical object base of which this is a physical object condition base.
        /// </summary>
        public PhysicalObjectBase PhysicalObjectBase
        {
            get
            {
                return this.NodeBase as PhysicalObjectBase;
            }
        }
        #endregion Property: PhysicalObjectBase

        #region Property: HasAllMandatorySpaces
        /// <summary>
        /// The value that indicates whether all mandatory spaces are required.
        /// </summary>
        private bool? hasAllMandatorySpaces = null;

        /// <summary>
        /// Gets the value that indicates whether all mandatory spaces are required.
        /// </summary>
        public bool? HasAllMandatorySpaces
        {
            get
            {
                return hasAllMandatorySpaces;
            }
        }
        #endregion Property: HasAllMandatorySpaces

        #region Property: Spaces
        /// <summary>
        /// The required spaces.
        /// </summary>
        private SpaceConditionBase[] spaces = null;

        /// <summary>
        /// Gets the required spaces.
        /// </summary>
        public ReadOnlyCollection<SpaceConditionBase> Spaces
        {
            get
            {
                if (spaces == null)
                {
                    LoadSpaces();
                    if (spaces == null)
                        return new List<SpaceConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SpaceConditionBase>(spaces);
            }
        }

        /// <summary>
        /// Loads the spaces.
        /// </summary>
        private void LoadSpaces()
        {
            if (this.PhysicalObjectCondition != null)
            {
                List<SpaceConditionBase> spaceConditionBases = new List<SpaceConditionBase>();
                foreach (SpaceCondition spaceCondition in this.PhysicalObjectCondition.Spaces)
                    spaceConditionBases.Add(BaseManager.Current.GetBase<SpaceConditionBase>(spaceCondition));
                spaces = spaceConditionBases.ToArray();
            }
        }
        #endregion Property: Spaces

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalObjectConditionBase(PhysicalObjectCondition physicalObjectCondition)
        /// <summary>
        /// Creates a base of the given physical object condition.
        /// </summary>
        /// <param name="physicalObjectCondition">The physical object condition to create a base of.</param>
        protected PhysicalObjectConditionBase(PhysicalObjectCondition physicalObjectCondition)
            : base(physicalObjectCondition)
        {
            if (physicalObjectCondition != null)
            {
                this.hasAllMandatorySpaces = physicalObjectCondition.HasAllMandatorySpaces;

                if (BaseManager.PreloadProperties)
                    LoadSpaces();
            }
        }
        #endregion Constructor: PhysicalObjectConditionBase(PhysicalObjectCondition physicalObjectCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: PhysicalObjectConditionBase

    #region Class: PhysicalObjectChangeBase
    /// <summary>
    /// A change on a physical object.
    /// </summary>
    public abstract class PhysicalObjectChangeBase : PhysicalEntityChangeBase
    {

        #region Properties and Fields

        #region Property: PhysicalObjectChange
        /// <summary>
        /// Gets the physical object change of which this is a physical object change base.
        /// </summary>
        protected internal PhysicalObjectChange PhysicalObjectChange
        {
            get
            {
                return this.Change as PhysicalObjectChange;
            }
        }
        #endregion Property: PhysicalObjectChange

        #region Property: PhysicalObjectBase
        /// <summary>
        /// Gets the affected physical object base.
        /// </summary>
        public PhysicalObjectBase PhysicalObjectBase
        {
            get
            {
                return this.NodeBase as PhysicalObjectBase;
            }
        }
        #endregion Property: PhysicalObjectBase

        #region Property: Spaces
        /// <summary>
        /// The space changes.
        /// </summary>
        private SpaceChangeBase[] spaces = null;

        /// <summary>
        /// Gets the space changes.
        /// </summary>
        public ReadOnlyCollection<SpaceChangeBase> Spaces
        {
            get
            {
                if (spaces == null)
                {
                    LoadSpaces();
                    if (spaces == null)
                        return new List<SpaceChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SpaceChangeBase>(spaces);
            }
        }

        /// <summary>
        /// Loads the spaces.
        /// </summary>
        private void LoadSpaces()
        {
            if (this.PhysicalObjectChange != null)
            {
                List<SpaceChangeBase> spaceChangeBases = new List<SpaceChangeBase>();
                foreach (SpaceChange spaceChange in this.PhysicalObjectChange.Spaces)
                    spaceChangeBases.Add(BaseManager.Current.GetBase<SpaceChangeBase>(spaceChange));
                spaces = spaceChangeBases.ToArray();
            }
        }
        #endregion Property: Spaces

        #region Property: SpacesToAdd
        /// <summary>
        /// The spaces to add during the change.
        /// </summary>
        private SpaceValuedBase[] spacesToAdd = null;

        /// <summary>
        /// Gets the spaces to add during the change.
        /// </summary>
        public ReadOnlyCollection<SpaceValuedBase> SpacesToAdd
        {
            get
            {
                if (spacesToAdd == null)
                {
                    LoadSpacesToAdd();
                    if (spacesToAdd == null)
                        return new List<SpaceValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SpaceValuedBase>(spacesToAdd);
            }
        }

        /// <summary>
        /// Loads the spaces to add.
        /// </summary>
        private void LoadSpacesToAdd()
        {
            if (this.PhysicalObjectChange != null)
            {
                List<SpaceValuedBase> spaceValuedBases = new List<SpaceValuedBase>();
                foreach (SpaceValued spaceValued in this.PhysicalObjectChange.SpacesToAdd)
                    spaceValuedBases.Add(BaseManager.Current.GetBase<SpaceValuedBase>(spaceValued));
                spacesToAdd = spaceValuedBases.ToArray();
            }
        }
        #endregion Property: SpacesToAdd

        #region Property: SpacesToRemove
        /// <summary>
        /// The spaces to remove during the change.
        /// </summary>
        private SpaceConditionBase[] spacesToRemove = null;

        /// <summary>
        /// Gets the spaces to remove during the change.
        /// </summary>
        public ReadOnlyCollection<SpaceConditionBase> SpacesToRemove
        {
            get
            {
                if (spacesToRemove == null)
                {
                    LoadSpacesToRemove();
                    if (spacesToRemove == null)
                        return new List<SpaceConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SpaceConditionBase>(spacesToRemove);
            }
        }

        /// <summary>
        /// Loads the spaces to remove.
        /// </summary>
        private void LoadSpacesToRemove()
        {
            if (this.PhysicalObjectChange != null)
            {
                List<SpaceConditionBase> spaceConditionBases = new List<SpaceConditionBase>();
                foreach (SpaceCondition spaceCondition in this.PhysicalObjectChange.SpacesToRemove)
                    spaceConditionBases.Add(BaseManager.Current.GetBase<SpaceConditionBase>(spaceCondition));
                spacesToRemove = spaceConditionBases.ToArray();
            }
        }
        #endregion Property: SpacesToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalObjectChangeBase(PhysicalObjectChange physicalObjectChange)
        /// <summary>
        /// Creates a base of the given physical object change.
        /// </summary>
        /// <param name="physicalObjectChange">The physical object change to create a base of.</param>
        protected PhysicalObjectChangeBase(PhysicalObjectChange physicalObjectChange)
            : base(physicalObjectChange)
        {
            if (physicalObjectChange != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadSpaces();
                    LoadSpacesToAdd();
                    LoadSpacesToRemove();
                }
            }
        }
        #endregion Constructor: PhysicalObjectChangeBase(PhysicalObjectChange physicalObjectChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: PhysicalObjectChangeBase

}
