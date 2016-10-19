/**************************************************************************
 * 
 * PhysicalObject.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Entities
{

    #region Class: PhysicalObject
    /// <summary>
    /// A physical object, containing spaces and relationships.
    /// </summary>
    public abstract class PhysicalObject : PhysicalEntity, IComparable<PhysicalObject>
    {

        #region Properties and Fields

        #region Property: Spaces
        /// <summary>
        /// Gets the spaces of this physical object.
        /// </summary>
        public ReadOnlyCollection<SpaceValued> Spaces
        {
            get
            {
                List<SpaceValued> spaces = new List<SpaceValued>();
                spaces.AddRange(this.PersonalSpaces);
                spaces.AddRange(this.InheritedSpaces);
                spaces.AddRange(this.OverriddenSpaces);
                return spaces.AsReadOnly();
            }
        }
        #endregion Property: Spaces

        #region Property: PersonalSpaces
        /// <summary>
        /// Gets the personal spaces.
        /// </summary>
        public ReadOnlyCollection<SpaceValued> PersonalSpaces
        {
            get
            {
                return Database.Current.SelectAll<SpaceValued>(this.ID, GenericTables.PhysicalObjectSpace, Columns.SpaceValued).AsReadOnly();
            }
        }
        #endregion Property: PersonalSpaces

        #region Property: InheritedSpaces
        /// <summary>
        /// Gets the inherited spaces.
        /// </summary>
        public ReadOnlyCollection<SpaceValued> InheritedSpaces
        {
            get
            {
                List<SpaceValued> inheritedSpaces = new List<SpaceValued>();

                foreach (Node parent in this.PersonalParents)
                {
                    foreach (SpaceValued inheritedSpace in ((PhysicalObject)parent).Spaces)
                    {
                        if (!HasOverriddenSpace(inheritedSpace.Space))
                            inheritedSpaces.Add(inheritedSpace);
                    }
                }

                return inheritedSpaces.AsReadOnly();
            }
        }
        #endregion Property: InheritedSpaces

        #region Property: OverriddenSpaces
        /// <summary>
        /// Gets the overridden spaces.
        /// </summary>
        public ReadOnlyCollection<SpaceValued> OverriddenSpaces
        {
            get
            {
                return Database.Current.SelectAll<SpaceValued>(this.ID, GenericTables.PhysicalObjectOverriddenSpace, Columns.SpaceValued).AsReadOnly();
            }
        }
        #endregion Property: OverriddenSpaces

        #region Property: DefaultSpace
        /// <summary>
        /// Gets or sets the default space. Setting only works when this physical object has the space.
        /// </summary>
        public SpaceValued DefaultSpace
        {
            get
            {
                SpaceValued defaultSpace = Database.Current.Select<SpaceValued>(this.ID, GenericTables.PhysicalObject, Columns.DefaultSpace);

                // Set the default space if this has not been done before
                if (defaultSpace == null)
                {
                    ReadOnlyCollection<SpaceValued> personalSpaces = this.PersonalSpaces;
                    if (personalSpaces.Count > 0)
                        defaultSpace = personalSpaces[0];
                    else
                    {
                        ReadOnlyCollection<SpaceValued> inheritedSpaces = this.InheritedSpaces;
                        if (inheritedSpaces.Count > 0)
                            defaultSpace = inheritedSpaces[0];
                    }
                }

                return defaultSpace;
            }
            set
            {
                if (this.Spaces.Contains(value))
                {
                    Database.Current.Update(this.ID, GenericTables.PhysicalObject, Columns.DefaultSpace, value);
                    NotifyPropertyChanged("DefaultSpace");
                }
            }
        }
        #endregion Property: DefaultSpace

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: PhysicalObject()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static PhysicalObject()
        {
            // Spaces
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SpaceValued, new Tuple<Type, EntryType>(typeof(SpaceValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.PhysicalObjectSpace, typeof(PhysicalObject), dict);

            // Overridden spaces
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SpaceValued, new Tuple<Type, EntryType>(typeof(SpaceValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.PhysicalObjectOverriddenSpace, typeof(PhysicalObject), dict);

            // Default space
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.DefaultSpace, new Tuple<Type, EntryType>(typeof(SpaceValued), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.PhysicalObject, typeof(PhysicalObject), dict);
        }
        #endregion Static Constructor: PhysicalObject()

        #region Constructor: PhysicalObject()
        /// <summary>
        /// Creates a new physical object.
        /// </summary>
        protected PhysicalObject()
            : base()
        {
        }
        #endregion Constructor: PhysicalObject()

        #region Constructor: PhysicalObject(uint id)
        /// <summary>
        /// Creates a new physical object from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a physical object from.</param>
        protected PhysicalObject(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PhysicalObject(uint id)

        #region Constructor: PhysicalObject(string name)
        /// <summary>
        /// Creates a new physical object with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the physical object.</param>
        protected PhysicalObject(string name)
            : base(name)
        {
        }
        #endregion Constructor: PhysicalObject(string name)

        #region Constructor: PhysicalObject(PhysicalObject physicalObject)
        /// <summary>
        /// Clones a physical object.
        /// </summary>
        /// <param name="physicalObject">The physical object to clone.</param>
        protected PhysicalObject(PhysicalObject physicalObject)
            : base(physicalObject)
        {
            if (physicalObject != null)
            {
                Database.Current.StartChange();

                bool defaultSpaceSet = false;
                SpaceValued defaultSpace = physicalObject.DefaultSpace;
                foreach (SpaceValued spaceValued in physicalObject.PersonalSpaces)
                {
                    SpaceValued copiedSpace = new SpaceValued(spaceValued);
                    AddSpace(copiedSpace);

                    if (defaultSpace != null && defaultSpace.Equals(spaceValued))
                    {
                        this.DefaultSpace = copiedSpace;
                        defaultSpaceSet = true;
                    }
                }
                if (!defaultSpaceSet)
                    this.DefaultSpace = defaultSpace;
                foreach (SpaceValued spaceValued in physicalObject.OverriddenSpaces)
                    AddOverriddenSpace(new SpaceValued(spaceValued));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: PhysicalObject(PhysicalObject physicalObject)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddSpace(SpaceValued spaceValued)
        /// <summary>
        /// Adds a valued space.
        /// </summary>
        /// <param name="spaceValued">The valued space to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSpace(SpaceValued spaceValued)
        {
            if (spaceValued != null && spaceValued.Space != null)
            {
                // Make sure to prevent looping
                if (HasPhysicalObject(spaceValued.Space))
                    return Message.RelationFail;

                // Add the space
                Database.Current.Insert(this.ID, GenericTables.PhysicalObjectSpace, new string[] { Columns.Space, Columns.SpaceValued }, new object[] { spaceValued.Space, spaceValued });
                NotifyPropertyChanged("PersonalSpaces");
                NotifyPropertyChanged("Spaces");

                // Set the default space if this has not been done before
                if (this.DefaultSpace == null)
                    this.DefaultSpace = spaceValued;

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSpace(SpaceValued spaceValued)

        #region Method: RemoveSpace(SpaceValued spaceValued)
        /// <summary>
        /// Removes a valued space.
        /// </summary>
        /// <param name="spaceValued">The valued space to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSpace(SpaceValued spaceValued)
        {
            if (spaceValued != null)
            {
                if (this.Spaces.Contains(spaceValued))
                {
                    // Remove the space
                    Database.Current.Remove(this.ID, GenericTables.PhysicalObjectSpace, Columns.SpaceValued, spaceValued);
                    NotifyPropertyChanged("PersonalSpaces");
                    NotifyPropertyChanged("Spaces");

                    // If this space is the default space, try to set another one
                    if (spaceValued.Equals(this.DefaultSpace))
                    {
                        if (this.Spaces.Count > 0)
                            this.DefaultSpace = this.Spaces[0];
                        else
                            this.DefaultSpace = null;
                    }

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveSpace(SpaceValued spaceValued)

        #region Method: OverrideSpace(SpaceValued inheritedSpace)
        /// <summary>
        /// Override the given inherited space.
        /// </summary>
        /// <param name="inheritedSpace">The inherited space that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverrideSpace(SpaceValued inheritedSpace)
        {
            if (inheritedSpace != null && inheritedSpace.Space != null && this.InheritedSpaces.Contains(inheritedSpace))
            {
                // If the space is already available, there is no use to add it
                foreach (SpaceValued personalSpace in this.PersonalSpaces)
                {
                    if (inheritedSpace.Space.Equals(personalSpace.Space))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenSpace(inheritedSpace.Space))
                    return Message.RelationExistsAlready;

                // Copy the valued space and add it
                return AddOverriddenSpace(new SpaceValued(inheritedSpace));
            }
            return Message.RelationFail;
        }
        #endregion Method: OverrideSpace(SpaceValued inheritedSpace)

        #region Method: AddOverriddenSpace(SpaceValued inheritedSpace)
        /// <summary>
        /// Add the given overridden space.
        /// </summary>
        /// <param name="overriddenSpace">The overridden space to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenSpace(SpaceValued overriddenSpace)
        {
            if (overriddenSpace != null)
            {
                Database.Current.Insert(this.ID, GenericTables.PhysicalObjectOverriddenSpace, Columns.SpaceValued, overriddenSpace);
                NotifyPropertyChanged("OverriddenSpaces");
                NotifyPropertyChanged("InheritedSpaces");
                NotifyPropertyChanged("Spaces");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenSpace(SpaceValued inheritedSpace)

        #region Method: RemoveOverriddenSpace(SpaceValued overriddenSpace)
        /// <summary>
        /// Removes an overridden space.
        /// </summary>
        /// <param name="overriddenSpace">The overridden space to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenSpace(SpaceValued overriddenSpace)
        {
            if (overriddenSpace != null)
            {
                if (this.OverriddenSpaces.Contains(overriddenSpace))
                {
                    // Remove the overridden space
                    Database.Current.Remove(this.ID, GenericTables.PhysicalObjectOverriddenSpace, Columns.SpaceValued, overriddenSpace);
                    NotifyPropertyChanged("OverriddenSpaces");
                    NotifyPropertyChanged("InheritedSpaces");
                    NotifyPropertyChanged("Spaces");
                    overriddenSpace.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenSpace(SpaceValued overriddenSpace)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasSpace(Space space)
        /// <summary>
        /// Checks if this physical object has the given space.
        /// </summary>
        /// <param name="space">The space to check.</param>
        /// <returns>Returns true when this physical object has the space.</returns>
        public bool HasSpace(Space space)
        {
            if (space != null)
            {
                foreach (SpaceValued mySpace in this.Spaces)
                {
                    if (space.Equals(mySpace.Space))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSpace(Space space)

        #region Method: HasOverriddenSpace(Space space)
        /// <summary>
        /// Checks if this physical object has the given overridden space.
        /// </summary>
        /// <param name="space">The space to check.</param>
        /// <returns>Returns true when this physical object has the overridden space.</returns>
        private bool HasOverriddenSpace(Space space)
        {
            if (space != null)
            {
                foreach (SpaceValued mySpace in this.OverriddenSpaces)
                {
                    if (space.Equals(mySpace.Space))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasOverriddenSpace(Space space)

        #region Method: HasPhysicalObject(PhysicalObject physicalObject)
        /// <summary>
        /// Checks whether this physical object has the given physical object in any possible way.
        /// </summary>
        /// <param name="physicalObject">The physical object to check.</param>
        /// <returns>Returns whether this physical object has the given physical object in any possible way.</returns>
        protected bool HasPhysicalObject(PhysicalObject physicalObject)
        {
            if (physicalObject != null)
            {
                // Check whether it is the same
                if (this.Equals(physicalObject))
                    return true;

                TangibleObject tangibleObject = this as TangibleObject;
                if (tangibleObject != null)
                {
                    // Check the wholes
                    foreach (TangibleObject whole in tangibleObject.Wholes)
                    {
                        if (whole.HasPhysicalObject(physicalObject))
                            return true;
                    }

                    // Check the covered objects
                    foreach (TangibleObject coveredObject in tangibleObject.CoveredObjects)
                    {
                        if (coveredObject.HasPhysicalObject(physicalObject))
                            return true;
                    }

                    // Check the spaces this is an item of
                    foreach (Space spaceItemOf in tangibleObject.SpacesItemOf)
                    {
                        if (spaceItemOf.HasPhysicalObject(physicalObject))
                            return true;
                    }
                }

                Space space = this as Space;
                if (space != null)
                {
                    // Check the physical objects this is a space of
                    foreach (PhysicalObject physicalObjectSpaceOf in space.PhysicalObjectsSpaceOf)
                    {
                        if (physicalObjectSpaceOf.HasPhysicalObject(physicalObject))
                            return true;
                    }
                }

                // Check the children
                foreach (PhysicalObject child in this.PersonalChildren)
                {
                    if (child.HasPhysicalObject(physicalObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasPhysicalObject(PhysicalObject physicalObject)
		
        #region Method: Remove()
        /// <summary>
        /// Remove the physical object.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the spaces
            foreach (SpaceValued spaceValued in this.PersonalSpaces)
                spaceValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.PhysicalObjectSpace);

            // Remove the overridden spaces
            foreach (SpaceValued spaceValued in this.OverriddenSpaces)
                spaceValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.PhysicalObjectOverriddenSpace);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the physical object.
        /// </summary>
        /// <returns>A clone of the physical object.</returns>
        public new PhysicalObject Clone()
        {
            return base.Clone() as PhysicalObject;
        }
        #endregion Method: Clone()

        #region Method: CompareTo(PhysicalObject other)
        /// <summary>
        /// Compares the physical object to the other physical object.
        /// </summary>
        /// <param name="other">The physical object to compare to this physical object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(PhysicalObject other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(PhysicalObject other)

        #endregion Method Group: Other

    }
    #endregion Class: PhysicalObject

    #region Class: PhysicalObjectValued
    /// <summary>
    /// A valued version of a physical object.
    /// </summary>
    public abstract class PhysicalObjectValued : PhysicalEntityValued
    {

        #region Properties and Fields

        #region Property: PhysicalObject
        /// <summary>
        /// Gets the physical object of which this is a valued physical object.
        /// </summary>
        public PhysicalObject PhysicalObject
        {
            get
            {
                return this.Node as PhysicalObject;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: PhysicalObject

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalObjectValued(uint id)
        /// <summary>
        /// Creates a new valued physical object from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued physical object from.</param>
        protected PhysicalObjectValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PhysicalObjectValued(uint id)

        #region Constructor: PhysicalObjectValued(PhysicalObjectValued physicalObjectValued)
        /// <summary>
        /// Clones a valued physical object.
        /// </summary>
        /// <param name="physicalObjectValued">The valued physical object to clone.</param>
        protected PhysicalObjectValued(PhysicalObjectValued physicalObjectValued)
            : base(physicalObjectValued)
        {
        }
        #endregion Constructor: PhysicalObjectValued(PhysicalObjectValued physicalObjectValued)

        #region Constructor: PhysicalObjectValued(PhysicalObject physicalObject)
        /// <summary>
        /// Creates a new valued physical object from the given physical object.
        /// </summary>
        /// <param name="physicalObject">The physical object to create a valued physical object from.</param>
        protected PhysicalObjectValued(PhysicalObject physicalObject)
            : base(physicalObject)
        {
        }
        #endregion Constructor: PhysicalObjectValued(PhysicalObject physicalObject)

        #region Constructor: PhysicalObjectValued(PhysicalObject physicalObject, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new valued physical object from the given physical object in the given quantity.
        /// </summary>
        /// <param name="physicalObject">The physical object to create a valued physical object from.</param>
        /// <param name="quantity">The quantity of the valued physical object.</param>
        protected PhysicalObjectValued(PhysicalObject physicalObject, NumericalValueRange quantity)
            : base(physicalObject, quantity)
        {
        }
        #endregion Constructor: PhysicalObjectValued(PhysicalObject physicalObject, NumericalValueRange quantity)

        #region Method: Create(PhysicalObject physicalObject)
        /// <summary>
        /// Create a valued physical object of the given physical object.
        /// </summary>
        /// <param name="physicalObject">The physical object to create a valued physical object of.</param>
        /// <returns>A valued physical object of the given physical object.</returns>
        public static PhysicalObjectValued Create(PhysicalObject physicalObject)
        {
            TangibleObject tangibleObject = physicalObject as TangibleObject;
            if (tangibleObject != null)
                return new TangibleObjectValued(tangibleObject);

            Space space = physicalObject as Space;
            if (space != null)
                return new SpaceValued(space);

            return null;
        }
        #endregion Method: Create(PhysicalObject physicalObject)

        #endregion Method Group: Constructors
        
        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the valued physical object.
        /// </summary>
        /// <returns>A clone of the valued physical object.</returns>
        public new PhysicalObjectValued Clone()
        {
            return base.Clone() as PhysicalObjectValued;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: PhysicalObjectValued

    #region Class: PhysicalObjectCondition
    /// <summary>
    /// A condition on a physical object.
    /// </summary>
    public abstract class PhysicalObjectCondition : PhysicalEntityCondition
    {

        #region Properties and Fields

        #region Property: PhysicalObject
        /// <summary>
        /// Gets or sets the required physical object.
        /// </summary>
        public PhysicalObject PhysicalObject
        {
            get
            {
                return this.Node as PhysicalObject;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: PhysicalObject

        #region Property: HasAllMandatorySpaces
        /// <summary>
        /// Gets or sets the value that indicates whether all mandatory spaces are required.
        /// </summary>
        public bool? HasAllMandatorySpaces
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.PhysicalObjectCondition, Columns.HasAllMandatorySpaces);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.PhysicalObjectCondition, Columns.HasAllMandatorySpaces, value);
                NotifyPropertyChanged("HasAllMandatorySpaces");
            }
        }
        #endregion Property: HasAllMandatorySpaces

        #region Property: Spaces
        /// <summary>
        /// Gets the required spaces.
        /// </summary>
        public ReadOnlyCollection<SpaceCondition> Spaces
        {
            get
            {
                return Database.Current.SelectAll<SpaceCondition>(this.ID, ValueTables.PhysicalObjectConditionSpaceCondition, Columns.SpaceCondition).AsReadOnly();
            }
        }
        #endregion Property: Spaces

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: PhysicalObjectCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static PhysicalObjectCondition()
        {
            // Spaces
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SpaceCondition, new Tuple<Type, EntryType>(typeof(SpaceCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.PhysicalObjectConditionSpaceCondition, typeof(PhysicalObjectCondition), dict);
        }
        #endregion Static Constructor: PhysicalObjectCondition()

        #region Constructor: PhysicalObjectCondition()
        /// <summary>
        /// Creates a new physical object condition.
        /// </summary>
        protected PhysicalObjectCondition()
            : base()
        {
        }
        #endregion Constructor: PhysicalObjectCondition()

        #region Constructor: PhysicalObjectCondition(uint id)
        /// <summary>
        /// Creates a new physical object condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the physical object condition from.</param>
        protected PhysicalObjectCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PhysicalObjectCondition(uint id)

        #region Constructor: PhysicalObjectCondition(PhysicalObjectCondition physicalObjectCondition)
        /// <summary>
        /// Clones a physical object condition.
        /// </summary>
        /// <param name="physicalObjectCondition">The physical object condition to clone.</param>
        protected PhysicalObjectCondition(PhysicalObjectCondition physicalObjectCondition)
            : base(physicalObjectCondition)
        {
            if (physicalObjectCondition != null)
            {
                Database.Current.StartChange();

                this.HasAllMandatorySpaces = physicalObjectCondition.HasAllMandatorySpaces;
                foreach (SpaceCondition spaceCondition in physicalObjectCondition.Spaces)
                    AddSpace(new SpaceCondition(spaceCondition));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: PhysicalObjectCondition(PhysicalObjectCondition physicalObjectCondition)

        #region Constructor: PhysicalObjectCondition(PhysicalObject physicalObject)
        /// <summary>
        /// Creates a condition for the given physical object.
        /// </summary>
        /// <param name="physicalObject">The physical object to create a condition for.</param>
        protected PhysicalObjectCondition(PhysicalObject physicalObject)
            : base(physicalObject)
        {
        }
        #endregion Constructor: PhysicalObjectCondition(PhysicalObject physicalObject)

        #region Constructor: PhysicalObjectCondition(PhysicalObject physicalObject, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given physical object in the given quantity.
        /// </summary>
        /// <param name="physicalObject">The physical object to create a condition for.</param>
        /// <param name="quantity">The quantity of the physical object condition.</param>
        protected PhysicalObjectCondition(PhysicalObject physicalObject, NumericalValueCondition quantity)
            : base(physicalObject, quantity)
        {
        }
        #endregion Constructor: PhysicalObjectCondition(PhysicalObject physicalObject, NumericalValueCondition quantity)

        #region Method: Create(PhysicalObject physicalObject)
        /// <summary>
        /// Create a physical object condition of the given physical object.
        /// </summary>
        /// <param name="physicalObject">The physical object to create a physical object condition of.</param>
        /// <returns>A physical object condition of the given physical object.</returns>
        public static PhysicalEntityCondition Create(PhysicalObject physicalObject)
        {
            Space space = physicalObject as Space;
            if (space != null)
                return new SpaceCondition(space);

            TangibleObject tangibleObject = physicalObject as TangibleObject;
            if (tangibleObject != null)
                return new TangibleObjectCondition(tangibleObject);

            return null;
        }
        #endregion Method: Create(PhysicalObject physicalObject)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddSpace(SpaceCondition spaceCondition)
        /// <summary>
        /// Adds a space.
        /// </summary>
        /// <param name="spaceCondition">The space to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSpace(SpaceCondition spaceCondition)
        {
            if (spaceCondition != null)
            {
                // If the space is already present, there's no use to add it again
                if (HasSpace(spaceCondition.Space))
                    return Message.RelationExistsAlready;

                // Add the space
                Database.Current.Insert(this.ID, ValueTables.PhysicalObjectConditionSpaceCondition, Columns.SpaceCondition, spaceCondition);
                NotifyPropertyChanged("Spaces");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSpace(SpaceCondition spaceCondition)

        #region Method: RemoveSpace(SpaceCondition spaceCondition)
        /// <summary>
        /// Removes a space.
        /// </summary>
        /// <param name="spaceCondition">The space to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSpace(SpaceCondition spaceCondition)
        {
            if (spaceCondition != null)
            {
                if (this.Spaces.Contains(spaceCondition))
                {
                    // Remove the space
                    Database.Current.Remove(this.ID, ValueTables.PhysicalObjectConditionSpaceCondition, Columns.SpaceCondition, spaceCondition);
                    NotifyPropertyChanged("Spaces");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveSpace(SpaceCondition spaceCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasSpace(Space space)
        /// <summary>
        /// Checks if this physical object condition has the given space.
        /// </summary>
        /// <param name="space">The space to check.</param>
        /// <returns>Returns true when this physical object condition has the space.</returns>
        public bool HasSpace(Space space)
        {
            if (space != null)
            {
                foreach (SpaceCondition spaceCondition in this.Spaces)
                {
                    if (space.Equals(spaceCondition.Space))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSpace(Space space)

        #region Method: Remove()
        /// <summary>
        /// Remove the physical object condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the spaces
            foreach (SpaceCondition spaceCondition in this.Spaces)
                spaceCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.PhysicalObjectConditionSpaceCondition);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the physical object condition.
        /// </summary>
        /// <returns>A clone of the physical object condition.</returns>
        public new PhysicalObjectCondition Clone()
        {
            return base.Clone() as PhysicalObjectCondition;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: PhysicalObjectCondition

    #region Class: PhysicalObjectChange
    /// <summary>
    /// A change on a physical object.
    /// </summary>
    public abstract class PhysicalObjectChange : PhysicalEntityChange
    {

        #region Properties and Fields

        #region Property: PhysicalObject
        /// <summary>
        /// Gets or sets the affected physical object.
        /// </summary>
        public PhysicalObject PhysicalObject
        {
            get
            {
                return this.Node as PhysicalObject;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: PhysicalObject

        #region Property: Spaces
        /// <summary>
        /// Gets the space changes.
        /// </summary>
        public ReadOnlyCollection<SpaceChange> Spaces
        {
            get
            {
                return Database.Current.SelectAll<SpaceChange>(this.ID, ValueTables.PhysicalObjectChangeSpaceChange, Columns.SpaceChange).AsReadOnly();
            }
        }
        #endregion Property: Spaces

        #region Property: SpacesToAdd
        /// <summary>
        /// Gets the spaces to add during the change.
        /// </summary>
        public ReadOnlyCollection<SpaceValued> SpacesToAdd
        {
            get
            {
                return Database.Current.SelectAll<SpaceValued>(this.ID, ValueTables.PhysicalObjectChangeSpaceToAdd, Columns.SpaceValued).AsReadOnly();
            }
        }
        #endregion Property: SpacesToAdd

        #region Property: SpacesToRemove
        /// <summary>
        /// Gets the spaces to remove during the change.
        /// </summary>
        public ReadOnlyCollection<SpaceCondition> SpacesToRemove
        {
            get
            {
                return Database.Current.SelectAll<SpaceCondition>(this.ID, ValueTables.PhysicalObjectChangeSpaceToRemove, Columns.SpaceCondition).AsReadOnly();
            }
        }
        #endregion Property: SpacesToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: PhysicalObjectChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static PhysicalObjectChange()
        {
            // Spaces
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SpaceChange, new Tuple<Type, EntryType>(typeof(SpaceChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.PhysicalObjectChangeSpaceChange, typeof(PhysicalObjectChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SpaceValued, new Tuple<Type, EntryType>(typeof(SpaceValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.PhysicalObjectChangeSpaceToAdd, typeof(PhysicalObjectChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SpaceCondition, new Tuple<Type, EntryType>(typeof(SpaceCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.PhysicalObjectChangeSpaceToRemove, typeof(PhysicalObjectChange), dict);
        }
        #endregion Static Constructor: PhysicalObjectChange()

        #region Constructor: PhysicalObjectChange()
        /// <summary>
        /// Creates a new physical object change.
        /// </summary>
        protected PhysicalObjectChange()
            : base()
        {
        }
        #endregion Constructor: PhysicalObjectChange()

        #region Constructor: PhysicalObjectChange(uint id)
        /// <summary>
        /// Creates a new physical object change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a physical object change from.</param>
        protected PhysicalObjectChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PhysicalObjectChange(uint id)

        #region Constructor: PhysicalObjectChange(PhysicalObjectChange physicalObjectChange)
        /// <summary>
        /// Clones a physical object change.
        /// </summary>
        /// <param name="physicalObjectChange">The physical object change to clone.</param>
        protected PhysicalObjectChange(PhysicalObjectChange physicalObjectChange)
            : base(physicalObjectChange)
        {
            if (physicalObjectChange != null)
            {
                Database.Current.StartChange();

                foreach (SpaceChange spaceChange in physicalObjectChange.Spaces)
                    AddSpace(new SpaceChange(spaceChange));
                foreach (SpaceValued spaceValued in physicalObjectChange.SpacesToAdd)
                    AddSpaceToAdd(new SpaceValued(spaceValued));
                foreach (SpaceCondition spaceCondition in physicalObjectChange.SpacesToRemove)
                    AddSpaceToRemove(new SpaceCondition(spaceCondition));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: PhysicalObjectChange(PhysicalObjectChange physicalObjectChange)

        #region Constructor: PhysicalObjectChange(PhysicalObject physicalObject)
        /// <summary>
        /// Creates a change for the given physical object.
        /// </summary>
        /// <param name="physicalObject">The physical object to create a change for.</param>
        protected PhysicalObjectChange(PhysicalObject physicalObject)
            : base(physicalObject)
        {
        }
        #endregion Constructor: PhysicalObjectChange(PhysicalObject physicalObject)

        #region Constructor: PhysicalObjectChange(PhysicalObject physicalObject, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given physical object in the form of the given quantity.
        /// </summary>
        /// <param name="physicalObject">The physical object to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        protected PhysicalObjectChange(PhysicalObject physicalObject, NumericalValueChange quantity)
            : base(physicalObject, quantity)
        {
        }
        #endregion Constructor: PhysicalObjectChange(PhysicalObject physicalObject, NumericalValueChange quantity)

        #region Method: Create(PhysicalObject physicalObject)
        /// <summary>
        /// Create a physical object change of the given physical object.
        /// </summary>
        /// <param name="physicalObject">The physical object to create a physical object change of.</param>
        /// <returns>A physical object change of the given physical object.</returns>
        public static PhysicalEntityChange Create(PhysicalObject physicalObject)
        {
            Space space = physicalObject as Space;
            if (space != null)
                return new SpaceChange(space);

            TangibleObject tangibleObject = physicalObject as TangibleObject;
            if (tangibleObject != null)
                return new TangibleObjectChange(tangibleObject);

            return null;
        }
        #endregion Method: Create(PhysicalObject physicalObject)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddSpace(SpaceChange spaceChange)
        /// <summary>
        /// Adds a space.
        /// </summary>
        /// <param name="spaceChange">The space to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSpace(SpaceChange spaceChange)
        {
            if (spaceChange != null)
            {
                // If the space is already present, there's no use to add it again
                if (HasSpace(spaceChange.Space))
                    return Message.RelationExistsAlready;

                // Add the space
                Database.Current.Insert(this.ID, ValueTables.PhysicalObjectChangeSpaceChange, Columns.SpaceChange, spaceChange);
                NotifyPropertyChanged("Spaces");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSpace(SpaceChange spaceChange)

        #region Method: RemoveSpace(SpaceChange spaceChange)
        /// <summary>
        /// Removes a space.
        /// </summary>
        /// <param name="spaceChange">The space to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSpace(SpaceChange spaceChange)
        {
            if (spaceChange != null)
            {
                if (this.Spaces.Contains(spaceChange))
                {
                    // Remove the space
                    Database.Current.Remove(this.ID, ValueTables.PhysicalObjectChangeSpaceChange, Columns.SpaceChange, spaceChange);
                    NotifyPropertyChanged("Spaces");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveSpace(SpaceChange spaceChange)

        #region Method: AddSpaceToAdd(SpaceValued spaceValued)
        /// <summary>
        /// Adds a space to add.
        /// </summary>
        /// <param name="spaceValued">The space to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSpaceToAdd(SpaceValued spaceValued)
        {
            if (spaceValued != null && spaceValued.Space != null)
            {
                // If the space is already present, there's no use to add it again
                if (HasSpaceToAdd(spaceValued.Space))
                    return Message.RelationExistsAlready;

                // Add the space
                Database.Current.Insert(this.ID, ValueTables.PhysicalObjectChangeSpaceToAdd, Columns.SpaceValued, spaceValued);
                NotifyPropertyChanged("SpacesToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSpaceToAdd(SpaceValued spaceValued)

        #region Method: RemoveSpaceToAdd(SpaceValued spaceValued)
        /// <summary>
        /// Removes a space to add.
        /// </summary>
        /// <param name="spaceValued">The space to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSpaceToAdd(SpaceValued spaceValued)
        {
            if (spaceValued != null)
            {
                if (this.SpacesToAdd.Contains(spaceValued))
                {
                    // Remove the space
                    Database.Current.Remove(this.ID, ValueTables.PhysicalObjectChangeSpaceToAdd, Columns.SpaceValued, spaceValued);
                    NotifyPropertyChanged("SpacesToAdd");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveSpaceToAdd(SpaceValued spaceValued)

        #region Method: AddSpaceToRemove(SpaceCondition spaceCondition)
        /// <summary>
        /// Adds a space to remove.
        /// </summary>
        /// <param name="spaceCondition">The space to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSpaceToRemove(SpaceCondition spaceCondition)
        {
            if (spaceCondition != null)
            {
                // If the space is already present, there's no use to add it again
                if (HasSpaceToRemove(spaceCondition.Space))
                    return Message.RelationExistsAlready;

                // Add the space
                Database.Current.Insert(this.ID, ValueTables.PhysicalObjectChangeSpaceToRemove, Columns.SpaceCondition, spaceCondition);
                NotifyPropertyChanged("SpacesToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSpaceToRemove(SpaceCondition spaceCondition)

        #region Method: RemoveSpaceToRemove(SpaceCondition spaceCondition)
        /// <summary>
        /// Removes a space to remove.
        /// </summary>
        /// <param name="spaceCondition">The space to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSpaceToRemove(SpaceCondition spaceCondition)
        {
            if (spaceCondition != null)
            {
                if (this.SpacesToRemove.Contains(spaceCondition))
                {
                    // Remove the space
                    Database.Current.Remove(this.ID, ValueTables.PhysicalObjectChangeSpaceToRemove, Columns.SpaceCondition, spaceCondition);
                    NotifyPropertyChanged("SpacesToRemove");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveSpaceToRemove(SpaceCondition spaceCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasSpace(Space space)
        /// <summary>
        /// Checks if this physical object change has the given space.
        /// </summary>
        /// <param name="space">The space to check.</param>
        /// <returns>Returns true when this physical object change has the space.</returns>
        public bool HasSpace(Space space)
        {
            if (space != null)
            {
                foreach (SpaceChange spaceChange in this.Spaces)
                {
                    if (space.Equals(spaceChange.Space))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSpace(Space space)

        #region Method: HasSpaceToAdd(Space space)
        /// <summary>
        /// Checks if this physical object change has the given space to add.
        /// </summary>
        /// <param name="space">The space to check.</param>
        /// <returns>Returns true when this physical object change has the space to add.</returns>
        public bool HasSpaceToAdd(Space space)
        {
            if (space != null)
            {
                foreach (SpaceValued spaceValued in this.SpacesToAdd)
                {
                    if (space.Equals(spaceValued.Space))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSpaceToAdd(Space space)

        #region Method: HasSpaceToRemove(Space space)
        /// <summary>
        /// Checks if this physical object change has the given space to remove.
        /// </summary>
        /// <param name="space">The space to check.</param>
        /// <returns>Returns true when this physical object change has the space to remove.</returns>
        public bool HasSpaceToRemove(Space space)
        {
            if (space != null)
            {
                foreach (SpaceCondition spaceCondition in this.SpacesToRemove)
                {
                    if (space.Equals(spaceCondition.Space))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSpaceToRemove(Space space)

        #region Method: Remove()
        /// <summary>
        /// Remove the physical object change.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the spaces
            foreach (SpaceChange spaceChange in this.Spaces)
                spaceChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.PhysicalObjectChangeSpaceChange);

            foreach (SpaceValued spaceValued in this.SpacesToAdd)
                spaceValued.Remove();
            Database.Current.Remove(this.ID, ValueTables.PhysicalObjectChangeSpaceToAdd);

            foreach (SpaceCondition spaceCondition in this.SpacesToRemove)
                spaceCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.PhysicalObjectChangeSpaceToRemove);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the physical object change.
        /// </summary>
        /// <returns>A clone of the physical object change.</returns>
        public new PhysicalObjectChange Clone()
        {
            return base.Clone() as PhysicalObjectChange;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: PhysicalObjectChange

}
