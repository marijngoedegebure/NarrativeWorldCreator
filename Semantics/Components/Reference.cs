/**************************************************************************
 * 
 * Reference.cs
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
using System.Reflection;
using Common;
using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: Reference
    /// <summary>
    /// A reference.
    /// </summary>
    public abstract class Reference : IdHolder
    {

        #region Properties and Fields

        #region Property: Name
        /// <summary>
        /// Gets or sets the name of the reference.
        /// </summary>
        public String Name
        {
            get
            {
                return Database.Current.Select<String>(this.ID, LocalizationTables.ReferenceName, Columns.Name);
            }
            set
            {
                Database.Current.Update(this.ID, LocalizationTables.ReferenceName, Columns.Name, value);
                NotifyPropertyChanged("Name");
            }
        }
        #endregion Property: Name

        #region Property: Subject
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public ActorTargetArtifactReference Subject
        {
            get
            {
                return Database.Current.Select<ActorTargetArtifactReference>(this.ID, ValueTables.Reference, Columns.Subject);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Reference, Columns.Subject, value);
                NotifyPropertyChanged("Subject");
            }
        }
        #endregion Property: Subject

        #region Property: SubjectReference
        /// <summary>
        /// Gets or sets the reference, in case the Subject has been set to 'Reference'.
        /// </summary>
        public Reference SubjectReference
        {
            get
            {
                return Database.Current.Select<Reference>(this.ID, ValueTables.Reference, Columns.Reference);
            }
            set
            {
                if (!this.Equals(value))
                {
                    Database.Current.Update(this.ID, ValueTables.Reference, Columns.Reference, value);
                    NotifyPropertyChanged("SubjectReference");
                }
            }
        }
        #endregion Property: SubjectReference

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Reference()
        /// <summary>
        /// Creates a new reference.
        /// </summary>
        protected Reference()
            : base()
        {
            Database.Current.StartChange();

            // Insert the name
            Database.Current.Insert(this.ID, LocalizationTables.ReferenceName, Columns.Name, NewNames.Reference);

            Database.Current.StopChange();
        }
        #endregion Constructor: Reference()

        #region Constructor: Reference(uint id)
        /// <summary>
        /// Creates a new reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new reference from.</param>
        protected Reference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Reference(uint id)

        #region Constructor: Reference(Reference reference)
        /// <summary>
        /// Clones the reference.
        /// </summary>
        /// <param name="reference">The reference to clone.</param>
        protected Reference(Reference reference)
            : base()
        {
            if (reference != null)
            {
                Database.Current.StartChange();

                Database.Current.Insert(this.ID, LocalizationTables.ReferenceName, Columns.Name, reference.Name);

                this.Subject = reference.Subject;
                this.SubjectReference = reference.SubjectReference;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Reference(Reference reference)

        #region Constructor: Reference(String name)
        /// <summary>
        /// Creates a new reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new reference from.</param>
        protected Reference(String name)
            : this()
        {
            Database.Current.StartChange();

            this.Name = name;

            Database.Current.StopChange();
        }
        #endregion Constructor: Reference(String name)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the reference.
        /// </summary>
        /// <returns>A clone of the reference.</returns>
        public Reference Clone()
        {
            try
            {
                Type type = this.GetType();
                return type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type }, null).Invoke(new object[] { this }) as Reference;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Method: Clone()

        #region Method: Remove()
        /// <summary>
        /// Remove the reference.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the name
            Database.Current.Remove(this.ID, LocalizationTables.ReferenceName);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation of the reference, which is its name.
        /// </summary>
        /// <returns>The string representation of the reference, which is its name.</returns>
        public override string ToString()
        {
            return this.Name;
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: Reference

    #region Class: SpatialReference
    /// <summary>
    /// A reference to entities in a range of the subject.
    /// </summary>
    public sealed class SpatialReference : Reference
    {

        #region Properties and Fields

        #region Property: SpatialRequirement
        /// <summary>
        /// Gets the spatial requirement.
        /// </summary>
        public SpatialRequirement SpatialRequirement
        {
            get
            {
                return Database.Current.Select<SpatialRequirement>(this.ID, ValueTables.SpatialReference, Columns.SpatialRequirement);
            }
            private set
            {
                Database.Current.Update(this.ID, ValueTables.SpatialReference, Columns.SpatialRequirement, value);
                NotifyPropertyChanged("SpatialRequirement");
            }
        }
        #endregion Property: SpatialRequirement

        #region Property: SelectionType
        /// <summary>
        /// Gets or sets the selection type.
        /// </summary>
        public SelectionType SelectionType
        {
            get
            {
                return Database.Current.Select<SelectionType>(this.ID, ValueTables.SpatialReference, Columns.SelectionType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpatialReference, Columns.SelectionType, value);
                NotifyPropertyChanged("SelectionType");
            }
        }
        #endregion Property: SelectionType

        #region Property: SelectionAmount
        /// <summary>
        /// Gets or sets the amount of entities to select in the range; will not be used for SelectionType 'All'.
        /// </summary>
        public float SelectionAmount
        {
            get
            {
                return Database.Current.Select<float>(this.ID, ValueTables.SpatialReference, Columns.SelectionAmount);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpatialReference, Columns.SelectionAmount, value);
                NotifyPropertyChanged("SelectionAmount");
            }
        }
        #endregion Property: SelectionAmount

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpatialReference()
        /// <summary>
        /// Creates a new spatial reference.
        /// </summary>
        public SpatialReference()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.SpatialReference;
            this.SpatialRequirement = new SpatialRequirement();
            this.SelectionAmount = 1;

            Database.Current.StopChange();
        }
        #endregion Constructor: SpatialReference()

        #region Constructor: SpatialReference(uint id)
        /// <summary>
        /// Creates a new spatial reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new spatial reference from.</param>
        private SpatialReference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SpatialReference(uint id)

        #region Constructor: SpatialReference(SpatialReference spatialReference)
        /// <summary>
        /// Clones the spatial reference.
        /// </summary>
        /// <param name="spatialReference">The spatial reference to clone.</param>
        public SpatialReference(SpatialReference spatialReference)
            : base(spatialReference)
        {
            if (spatialReference != null)
            {
                Database.Current.StartChange();

                if (spatialReference.SpatialRequirement != null)
                    this.SpatialRequirement = new SpatialRequirement(spatialReference.SpatialRequirement);
                this.SelectionType = spatialReference.SelectionType;
                this.SelectionAmount = spatialReference.SelectionAmount;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: SpatialReference(SpatialReference spatialReference)

        #region Constructor: SpatialReference(String name)
        /// <summary>
        /// Creates a new spatial reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new spatial reference from.</param>
        public SpatialReference(String name)
            : base(name)
        {
            Database.Current.StartChange();

            this.SpatialRequirement = new SpatialRequirement();
            this.SelectionAmount = 1;

            Database.Current.StopChange();
        }
        #endregion Constructor: SpatialReference(String name)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpatialReference

    #region Class: SpaceReference
    /// <summary>
    /// A reference to a space of a source.
    /// </summary>
    public sealed class SpaceReference : Reference
    {

        #region Properties and Fields

        #region Property: Space
        /// <summary>
        /// Gets or sets the space.
        /// </summary>
        public Space Space
        {
            get
            {
                return Database.Current.Select<Space>(this.ID, ValueTables.SpaceReference, Columns.Space);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SpaceReference, Columns.Space, value);
                NotifyPropertyChanged("Space");
            }
        }
        #endregion Property: Space

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: SpaceReference()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static SpaceReference()
        {
            // Space
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Space, new Tuple<Type, EntryType>(typeof(Space), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.SpaceReference, typeof(SpaceReference), dict);
        }
        #endregion Static Constructor: SpaceReference()

        #region Constructor: SpaceReference()
        /// <summary>
        /// Creates a new space reference.
        /// </summary>
        public SpaceReference()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.SpaceReference;

            Database.Current.StopChange();
        }
        #endregion Constructor: SpaceReference()

        #region Constructor: SpaceReference(uint id)
        /// <summary>
        /// Creates a new space reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new space reference from.</param>
        private SpaceReference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SpaceReference(uint id)

        #region Constructor: SpaceReference(SpaceReference spaceReference)
        /// <summary>
        /// Clones the space reference.
        /// </summary>
        /// <param name="spaceReference">The space reference to clone.</param>
        public SpaceReference(SpaceReference spaceReference)
            : base(spaceReference)
        {
            if (spaceReference != null)
            {
                Database.Current.StartChange();

                this.Space = spaceReference.Space;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: SpaceReference(SpaceReference spaceReference)

        #region Constructor: SpaceReference(String name)
        /// <summary>
        /// Creates a new space reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new space reference from.</param>
        public SpaceReference(String name)
            : base(name)
        {
        }
        #endregion Constructor: SpaceReference(String name)

        #region Constructor: SpaceReference(String name, Space space)
        /// <summary>
        /// Creates a new space reference for the given space with the given name.
        /// </summary>
        /// <param name="name">The name to create a new space reference from.</param>
        /// <param name="space">The space to create a new space reference from.</param>
        public SpaceReference(String name, Space space)
            : base(name)
        {
            Database.Current.StartChange();

            this.Space = space;

            Database.Current.StopChange();
        }
        #endregion Constructor: SpaceReference(String name, Space space)

        #region Constructor: SpaceReference(Space space)
        /// <summary>
        /// Creates a new space reference for the given space.
        /// </summary>
        /// <param name="space">The space to create a new space reference from.</param>
        public SpaceReference(Space space)
            : this()
        {
            Database.Current.StartChange();

            this.Space = space;

            Database.Current.StopChange();
        }
        #endregion Constructor: SpaceReference(Space space)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpaceReference

    #region Class: RelationshipReference
    /// <summary>
    /// A reference to a relationship.
    /// </summary>
    public sealed class RelationshipReference : Reference
    {

        #region Properties and Fields

        #region Property: RelationshipType
        /// <summary>
        /// Gets or sets the relationship type.
        /// </summary>
        public RelationshipType RelationshipType
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.RelationshipReference, Columns.RelationshipType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipReference, Columns.RelationshipType, value);
                NotifyPropertyChanged("RelationshipType");
            }
        }
        #endregion Property: RelationshipType

        #region Property: SubjectRole
        /// <summary>
        /// Gets or sets the role of the subject in the relationship: source or target; if set to 'null', both are valid.
        /// </summary>
        public SourceTarget? SubjectRole
        {
            get
            {
                return Database.Current.Select<SourceTarget?>(this.ID, ValueTables.RelationshipReference, Columns.SubjectRole);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipReference, Columns.SubjectRole, value);
                NotifyPropertyChanged("SubjectRole");
            }
        }
        #endregion Property: SubjectRole

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: RelationshipReference()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static RelationshipReference()
        {
            // Relationship type
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.RelationshipType, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.RelationshipReference, typeof(RelationshipReference), dict);
        }
        #endregion Static Constructor: RelationshipReference()

        #region Constructor: RelationshipReference()
        /// <summary>
        /// Creates a new relationship reference.
        /// </summary>
        public RelationshipReference()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.RelationshipReference;

            Database.Current.StopChange();
        }
        #endregion Constructor: RelationshipReference()

        #region Constructor: RelationshipReference(uint id)
        /// <summary>
        /// Creates a new relationship reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new relationship reference from.</param>
        private RelationshipReference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: RelationshipReference(uint id)

        #region Constructor: RelationshipReference(RelationshipReference relationshipReference)
        /// <summary>
        /// Clones the relationship reference.
        /// </summary>
        /// <param name="relationshipReference">The relationship reference to clone.</param>
        public RelationshipReference(RelationshipReference relationshipReference)
            : base(relationshipReference)
        {
            if (relationshipReference != null)
            {
                Database.Current.StartChange();

                this.RelationshipType = relationshipReference.RelationshipType;
                this.SubjectRole = relationshipReference.SubjectRole;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: RelationshipReference(RelationshipReference relationshipReference)

        #region Constructor: RelationshipReference(String name)
        /// <summary>
        /// Creates a new relationship reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new relationship reference from.</param>
        public RelationshipReference(String name)
            : base(name)
        {
        }
        #endregion Constructor: RelationshipReference(String name)

        #region Constructor: RelationshipReference(String name, RelationshipType relationshipType)
        /// <summary>
        /// Creates a new relationship reference for the given space with the given name.
        /// </summary>
        /// <param name="name">The name to create a new relationship reference from.</param>
        /// <param name="relationshipType">The relationship type to create a new relationship reference from.</param>
        public RelationshipReference(String name, RelationshipType relationshipType)
            : base(name)
        {
            Database.Current.StartChange();

            this.RelationshipType = relationshipType;

            Database.Current.StopChange();
        }
        #endregion Constructor: RelationshipReference(String name, RelationshipType relationshipType)

        #region Constructor: RelationshipReference(RelationshipType relationshipType)
        /// <summary>
        /// Creates a new relationship reference for the given space.
        /// </summary>
        /// <param name="relationshipType">The relationship type to create a new relationship reference from.</param>
        public RelationshipReference(RelationshipType relationshipType)
            : this()
        {
            Database.Current.StartChange();

            this.RelationshipType = relationshipType;

            Database.Current.StopChange();
        }
        #endregion Constructor: RelationshipReference(RelationshipType relationshipType)

        #endregion Method Group: Constructors

    }
    #endregion Class: RelationshipReference

    #region Class: PartReference
    /// <summary>
    /// A reference to a part.
    /// </summary>
    public sealed class PartReference : Reference
    {

        #region Properties and Fields

        #region Property: Part
        /// <summary>
        /// Gets or sets the part.
        /// </summary>
        public TangibleObject Part
        {
            get
            {
                return Database.Current.Select<TangibleObject>(this.ID, ValueTables.PartReference, Columns.Part);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.PartReference, Columns.Part, value);
                NotifyPropertyChanged("Part");
            }
        }
        #endregion Property: Part

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: PartReference()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static PartReference()
        {
            // Tangible object
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Part, new Tuple<Type, EntryType>(typeof(TangibleObject), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.PartReference, typeof(PartReference), dict);
        }
        #endregion Static Constructor: PartReference()

        #region Constructor: PartReference()
        /// <summary>
        /// Creates a new part reference.
        /// </summary>
        public PartReference()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.PartReference;

            Database.Current.StopChange();
        }
        #endregion Constructor: PartReference()

        #region Constructor: PartReference(uint id)
        /// <summary>
        /// Creates a new part reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new part reference from.</param>
        private PartReference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PartReference(uint id)

        #region Constructor: PartReference(PartReference partReference)
        /// <summary>
        /// Clones the part reference.
        /// </summary>
        /// <param name="partReference">The part reference to clone.</param>
        public PartReference(PartReference partReference)
            : base(partReference)
        {
            if (partReference != null)
            {
                Database.Current.StartChange();

                this.Part = partReference.Part;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: PartReference(PartReference partReference)

        #region Constructor: PartReference(String name)
        /// <summary>
        /// Creates a new part reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new part reference from.</param>
        public PartReference(String name)
            : base(name)
        {
        }
        #endregion Constructor: PartReference(String name)

        #region Constructor: PartReference(String name, TangibleObject part)
        /// <summary>
        /// Creates a new part reference for the given part with the given name.
        /// </summary>
        /// <param name="name">The name to create a new part reference from.</param>
        /// <param name="part">The part to create a new part reference from.</param>
        public PartReference(String name, TangibleObject part)
            : base(name)
        {
            Database.Current.StartChange();

            this.Part = part;

            Database.Current.StopChange();
        }
        #endregion Constructor: PartReference(String name, TangibleObject part)

        #region Constructor: PartReference(TangibleObject part)
        /// <summary>
        /// Creates a new part reference for the given part.
        /// </summary>
        /// <param name="part">The part to create a new part reference from.</param>
        public PartReference(TangibleObject part)
            : this()
        {
            Database.Current.StartChange();

            this.Part = part;

            Database.Current.StopChange();
        }
        #endregion Constructor: PartReference(TangibleObject part)

        #endregion Method Group: Constructors

    }
    #endregion Class: PartReference

    #region Class: CoverReference
    /// <summary>
    /// A reference to a cover.
    /// </summary>
    public sealed class CoverReference : Reference
    {

        #region Properties and Fields

        #region Property: Cover
        /// <summary>
        /// Gets or sets the cover.
        /// </summary>
        public TangibleObject Cover
        {
            get
            {
                return Database.Current.Select<TangibleObject>(this.ID, ValueTables.CoverReference, Columns.Cover);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.CoverReference, Columns.Cover, value);
                NotifyPropertyChanged("Cover");
            }
        }
        #endregion Property: Cover

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: CoverReference()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static CoverReference()
        {
            // Tangible object
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Cover, new Tuple<Type, EntryType>(typeof(TangibleObject), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.CoverReference, typeof(CoverReference), dict);
        }
        #endregion Static Constructor: CoverReference()

        #region Constructor: CoverReference()
        /// <summary>
        /// Creates a new cover reference.
        /// </summary>
        public CoverReference()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.CoverReference;

            Database.Current.StopChange();
        }
        #endregion Constructor: CoverReference()

        #region Constructor: CoverReference(uint id)
        /// <summary>
        /// Creates a new cover reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new cover reference from.</param>
        private CoverReference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: CoverReference(uint id)

        #region Constructor: CoverReference(CoverReference coverReference)
        /// <summary>
        /// Clones the cover reference.
        /// </summary>
        /// <param name="coverReference">The cover reference to clone.</param>
        public CoverReference(CoverReference coverReference)
            : base(coverReference)
        {
            if (coverReference != null)
            {
                Database.Current.StartChange();

                this.Cover = coverReference.Cover;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: CoverReference(CoverReference coverReference)

        #region Constructor: CoverReference(String name)
        /// <summary>
        /// Creates a new cover reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new cover reference from.</param>
        public CoverReference(String name)
            : base(name)
        {
        }
        #endregion Constructor: CoverReference(String name)

        #region Constructor: CoverReference(String name, TangibleObject cover)
        /// <summary>
        /// Creates a new cover reference for the given cover with the given name.
        /// </summary>
        /// <param name="name">The name to create a new cover reference from.</param>
        /// <param name="cover">The cover to create a new cover reference from.</param>
        public CoverReference(String name, TangibleObject cover)
            : base(name)
        {
            Database.Current.StartChange();

            this.Cover = cover;

            Database.Current.StopChange();
        }
        #endregion Constructor: CoverReference(String name, TangibleObject cover)

        #region Constructor: CoverReference(TangibleObject cover)
        /// <summary>
        /// Creates a new cover reference for the given cover.
        /// </summary>
        /// <param name="cover">The cover to create a new cover reference from.</param>
        public CoverReference(TangibleObject cover)
            : this()
        {
            Database.Current.StartChange();

            this.Cover = cover;

            Database.Current.StopChange();
        }
        #endregion Constructor: CoverReference(TangibleObject cover)

        #endregion Method Group: Constructors

    }
    #endregion Class: CoverReference

    #region Class: ConnectionReference
    /// <summary>
    /// A reference to a connection item.
    /// </summary>
    public sealed class ConnectionReference : Reference
    {

        #region Properties and Fields

        #region Property: ConnectionItem
        /// <summary>
        /// Gets or sets the connection item.
        /// </summary>
        public TangibleObject ConnectionItem
        {
            get
            {
                return Database.Current.Select<TangibleObject>(this.ID, ValueTables.ConnectionReference, Columns.ConnectionItem);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ConnectionReference, Columns.ConnectionItem, value);
                NotifyPropertyChanged("ConnectionItem");
            }
        }
        #endregion Property: ConnectionItem

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: ConnectionReference()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static ConnectionReference()
        {
            // Tangible object
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ConnectionItem, new Tuple<Type, EntryType>(typeof(TangibleObject), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.ConnectionReference, typeof(ConnectionReference), dict);
        }
        #endregion Static Constructor: ConnectionReference()

        #region Constructor: ConnectionReference()
        /// <summary>
        /// Creates a new connection reference.
        /// </summary>
        public ConnectionReference()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.ConnectionReference;

            Database.Current.StopChange();
        }
        #endregion Constructor: ConnectionReference()

        #region Constructor: ConnectionReference(uint id)
        /// <summary>
        /// Creates a new connection reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new connection reference from.</param>
        private ConnectionReference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ConnectionReference(uint id)

        #region Constructor: ConnectionReference(ConnectionReference connectionReference)
        /// <summary>
        /// Clones the connection reference.
        /// </summary>
        /// <param name="connectionReference">The connection reference to clone.</param>
        public ConnectionReference(ConnectionReference connectionReference)
            : base(connectionReference)
        {
            if (connectionReference != null)
            {
                Database.Current.StartChange();

                this.ConnectionItem = connectionReference.ConnectionItem;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: ConnectionReference(ConnectionReference connectionReference)

        #region Constructor: ConnectionReference(String name)
        /// <summary>
        /// Creates a new connection reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new connection reference from.</param>
        public ConnectionReference(String name)
            : base(name)
        {
        }
        #endregion Constructor: ConnectionReference(String name)

        #region Constructor: ConnectionReference(String name, TangibleObject connectionItem)
        /// <summary>
        /// Creates a new connection reference for the given connection item with the given name.
        /// </summary>
        /// <param name="name">The name to create a new connection reference from.</param>
        /// <param name="connectionItem">The connection item to create a new connection reference from.</param>
        public ConnectionReference(String name, TangibleObject connectionItem)
            : base(name)
        {
            Database.Current.StartChange();

            this.ConnectionItem = connectionItem;

            Database.Current.StopChange();
        }
        #endregion Constructor: ConnectionReference(String name, TangibleObject connectionItem)

        #region Constructor: ConnectionReference(TangibleObject connectionItem)
        /// <summary>
        /// Creates a new connection reference for the given connection item.
        /// </summary>
        /// <param name="connectionItem">The connection item to create a new connection reference from.</param>
        public ConnectionReference(TangibleObject connectionItem)
            : this()
        {
            Database.Current.StartChange();

            this.ConnectionItem = connectionItem;

            Database.Current.StopChange();
        }
        #endregion Constructor: ConnectionReference(TangibleObject connectionItem)

        #endregion Method Group: Constructors

    }
    #endregion Class: ConnectionReference

    #region Class: ItemReference
    /// <summary>
    /// A reference to an item.
    /// </summary>
    public sealed class ItemReference : Reference
    {

        #region Properties and Fields

        #region Property: Item
        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        public TangibleObject Item
        {
            get
            {
                return Database.Current.Select<TangibleObject>(this.ID, ValueTables.ItemReference, Columns.Item);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ItemReference, Columns.Item, value);
                NotifyPropertyChanged("Item");
            }
        }
        #endregion Property: Item

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: ItemReference()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static ItemReference()
        {
            // Tangible object
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Item, new Tuple<Type, EntryType>(typeof(TangibleObject), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.ItemReference, typeof(ItemReference), dict);
        }
        #endregion Static Constructor: ItemReference()

        #region Constructor: ItemReference()
        /// <summary>
        /// Creates a new item reference.
        /// </summary>
        public ItemReference()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.ItemReference;

            Database.Current.StopChange();
        }
        #endregion Constructor: ItemReference()

        #region Constructor: ItemReference(uint id)
        /// <summary>
        /// Creates a new item reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new item reference from.</param>
        private ItemReference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ItemReference(uint id)

        #region Constructor: ItemReference(ItemReference itemReference)
        /// <summary>
        /// Clones the item reference.
        /// </summary>
        /// <param name="itemReference">The item reference to clone.</param>
        public ItemReference(ItemReference itemReference)
            : base(itemReference)
        {
            if (itemReference != null)
            {
                Database.Current.StartChange();

                this.Item = itemReference.Item;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: ItemReference(ItemReference itemReference)

        #region Constructor: ItemReference(String name)
        /// <summary>
        /// Creates a new item reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new item reference from.</param>
        public ItemReference(String name)
            : base(name)
        {
        }
        #endregion Constructor: ItemReference(String name)

        #region Constructor: ItemReference(String name, TangibleObject item)
        /// <summary>
        /// Creates a new item reference for the given item with the given name.
        /// </summary>
        /// <param name="name">The name to create a new item reference from.</param>
        /// <param name="item">The item to create a new item reference from.</param>
        public ItemReference(String name, TangibleObject item)
            : base(name)
        {
            Database.Current.StartChange();

            this.Item = item;

            Database.Current.StopChange();
        }
        #endregion Constructor: ItemReference(String name, TangibleObject item)

        #region Constructor: ItemReference(TangibleObject item)
        /// <summary>
        /// Creates a new item reference for the given item.
        /// </summary>
        /// <param name="item">The item to create a new item reference from.</param>
        public ItemReference(TangibleObject item)
            : this()
        {
            Database.Current.StartChange();

            this.Item = item;

            Database.Current.StopChange();
        }
        #endregion Constructor: ItemReference(TangibleObject item)

        #endregion Method Group: Constructors

    }
    #endregion Class: ItemReference

    #region Class: MatterReference
    /// <summary>
    /// A reference to a matter.
    /// </summary>
    public sealed class MatterReference : Reference
    {

        #region Properties and Fields

        #region Property: Matter
        /// <summary>
        /// Gets or sets the matter.
        /// </summary>
        public Matter Matter
        {
            get
            {
                return Database.Current.Select<Matter>(this.ID, ValueTables.MatterReference, Columns.Matter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MatterReference, Columns.Matter, value);
                NotifyPropertyChanged("Matter");
            }
        }
        #endregion Property: Matter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: MatterReference()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static MatterReference()
        {
            // Matter
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Matter, new Tuple<Type, EntryType>(typeof(Matter), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.MatterReference, typeof(MatterReference), dict);
        }
        #endregion Static Constructor: MatterReference()

        #region Constructor: MatterReference()
        /// <summary>
        /// Creates a new matter reference.
        /// </summary>
        public MatterReference()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.MatterReference;

            Database.Current.StopChange();
        }
        #endregion Constructor: MatterReference()

        #region Constructor: MatterReference(uint id)
        /// <summary>
        /// Creates a new matter reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new matter reference from.</param>
        private MatterReference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: MatterReference(uint id)

        #region Constructor: MatterReference(MatterReference matterReference)
        /// <summary>
        /// Clones the matter reference.
        /// </summary>
        /// <param name="matterReference">The matter reference to clone.</param>
        public MatterReference(MatterReference matterReference)
            : base(matterReference)
        {
            if (matterReference != null)
            {
                Database.Current.StartChange();

                this.Matter = matterReference.Matter;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: MatterReference(MatterReference matterReference)

        #region Constructor: MatterReference(String name)
        /// <summary>
        /// Creates a new matter reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new matter reference from.</param>
        public MatterReference(String name)
            : base(name)
        {
        }
        #endregion Constructor: MatterReference(String name)

        #region Constructor: MatterReference(String name, Matter matter)
        /// <summary>
        /// Creates a new matter reference for the given matter with the given name.
        /// </summary>
        /// <param name="name">The name to create a new matter reference from.</param>
        /// <param name="matter">The matter to create a new matter reference from.</param>
        public MatterReference(String name, Matter matter)
            : base(name)
        {
            Database.Current.StartChange();

            this.Matter = matter;

            Database.Current.StopChange();
        }
        #endregion Constructor: MatterReference(String name, Matter matter)

        #region Constructor: MatterReference(Matter matter)
        /// <summary>
        /// Creates a new matter reference for the given matter.
        /// </summary>
        /// <param name="matter">The matter to create a new matter reference from.</param>
        public MatterReference(Matter matter)
            : this()
        {
            Database.Current.StartChange();

            this.Matter = matter;

            Database.Current.StopChange();
        }
        #endregion Constructor: MatterReference(Matter matter)

        #endregion Method Group: Constructors

    }
    #endregion Class: MatterReference

    #region Class: LayerReference
    /// <summary>
    /// A reference to a layer.
    /// </summary>
    public sealed class LayerReference : Reference
    {

        #region Properties and Fields

        #region Property: Layer
        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        public Matter Layer
        {
            get
            {
                return Database.Current.Select<Matter>(this.ID, ValueTables.LayerReference, Columns.Layer);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.LayerReference, Columns.Layer, value);
                NotifyPropertyChanged("Layer");
            }
        }
        #endregion Property: Layer

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: LayerReference()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static LayerReference()
        {
            // Matter
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Layer, new Tuple<Type, EntryType>(typeof(Matter), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.LayerReference, typeof(LayerReference), dict);
        }
        #endregion Static Constructor: LayerReference()

        #region Constructor: LayerReference()
        /// <summary>
        /// Creates a new layer reference.
        /// </summary>
        public LayerReference()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.LayerReference;

            Database.Current.StopChange();
        }
        #endregion Constructor: LayerReference()

        #region Constructor: LayerReference(uint id)
        /// <summary>
        /// Creates a new layer reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new layer reference from.</param>
        private LayerReference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: LayerReference(uint id)

        #region Constructor: LayerReference(LayerReference layerReference)
        /// <summary>
        /// Clones the layer reference.
        /// </summary>
        /// <param name="layerReference">The layer reference to clone.</param>
        public LayerReference(LayerReference layerReference)
            : base(layerReference)
        {
            if (layerReference != null)
            {
                Database.Current.StartChange();

                this.Layer = layerReference.Layer;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: LayerReference(LayerReference layerReference)

        #region Constructor: LayerReference(String name)
        /// <summary>
        /// Creates a new layer reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new layer reference from.</param>
        public LayerReference(String name)
            : base(name)
        {
        }
        #endregion Constructor: LayerReference(String name)

        #region Constructor: LayerReference(String name, Matter layer)
        /// <summary>
        /// Creates a new layer reference for the given layer with the given name.
        /// </summary>
        /// <param name="name">The name to create a new layer reference from.</param>
        /// <param name="layer">The layer to create a new layer reference from.</param>
        public LayerReference(String name, Matter layer)
            : base(name)
        {
            Database.Current.StartChange();

            this.Layer = layer;

            Database.Current.StopChange();
        }
        #endregion Constructor: LayerReference(String name, Matter layer)

        #region Constructor: LayerReference(Matter layer)
        /// <summary>
        /// Creates a new layer reference for the given layer.
        /// </summary>
        /// <param name="layer">The layer to create a new layer reference from.</param>
        public LayerReference(Matter layer)
            : this()
        {
            Database.Current.StartChange();

            this.Layer = layer;

            Database.Current.StopChange();
        }
        #endregion Constructor: LayerReference(Matter layer)

        #endregion Method Group: Constructors

    }
    #endregion Class: LayerReference

    #region Class: SetReference
    /// <summary>
    /// A reference to a set of two other references.
    /// </summary>
    public sealed class SetReference : Reference
    {

        #region Properties and Fields

        #region Property: Reference1
        /// <summary>
        /// Gets or sets the first reference.
        /// </summary>
        public Reference Reference1
        {
            get
            {
                return Database.Current.Select<Reference>(this.ID, ValueTables.SetReference, Columns.Reference1);
            }
            set
            {
                if (!this.Equals(value))
                {
                    Database.Current.Update(this.ID, ValueTables.SetReference, Columns.Reference1, value);
                    NotifyPropertyChanged("Reference1");
                }
            }
        }
        #endregion Property: Reference1

        #region Property: BinaryOperator
        /// <summary>
        /// Gets or sets the binary operator.
        /// </summary>
        public BinaryOperator BinaryOperator
        {
            get
            {
                return Database.Current.Select<BinaryOperator>(this.ID, ValueTables.SetReference, Columns.BinaryOperator);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SetReference, Columns.BinaryOperator, value);
                NotifyPropertyChanged("BinaryOperator");
            }
        }
        #endregion Property: BinaryOperator

        #region Property: Reference2
        /// <summary>
        /// Gets or sets the second reference.
        /// </summary>
        public Reference Reference2
        {
            get
            {
                return Database.Current.Select<Reference>(this.ID, ValueTables.SetReference, Columns.Reference2);
            }
            set
            {
                if (!this.Equals(value))
                {
                    Database.Current.Update(this.ID, ValueTables.SetReference, Columns.Reference2, value);
                    NotifyPropertyChanged("Reference2");
                }
            }
        }
        #endregion Property: Reference2

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: SetReference()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static SetReference()
        {
            // References
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Reference1, new Tuple<Type, EntryType>(typeof(Reference), EntryType.Nullable));
            dict.Add(Columns.Reference2, new Tuple<Type, EntryType>(typeof(Reference), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.SetReference, typeof(SetReference), dict);
        }
        #endregion Static Constructor: SetReference()

        #region Constructor: SetReference()
        /// <summary>
        /// Creates a new set reference.
        /// </summary>
        public SetReference()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.SetReference;

            Database.Current.StopChange();
        }
        #endregion Constructor: SetReference()

        #region Constructor: SetReference(uint id)
        /// <summary>
        /// Creates a new set reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new set reference from.</param>
        private SetReference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SetReference(uint id)

        #region Constructor: SetReference(SetReference setReference)
        /// <summary>
        /// Clones the set reference.
        /// </summary>
        /// <param name="setReference">The set reference to clone.</param>
        public SetReference(SetReference setReference)
            : base(setReference)
        {
            if (setReference != null)
            {
                Database.Current.StartChange();

                if (setReference.Reference1 != null)
                    this.Reference1 = setReference.Reference1.Clone();
                this.BinaryOperator = setReference.BinaryOperator;
                if (setReference.Reference2 != null)
                    this.Reference2 = setReference.Reference2.Clone();

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: SetReference(SetReference setReference)

        #region Constructor: SetReference(String name)
        /// <summary>
        /// Creates a new set reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new set reference from.</param>
        public SetReference(String name)
            : base(name)
        {
        }
        #endregion Constructor: SetReference(String name)

        #endregion Method Group: Constructors

    }
    #endregion Class: LayerReference

    #region Class: OwnerReference
    /// <summary>
    /// A reference to the owner of the subject, like the whole of a part, the mixture of a substance, or the space of an item.
    /// </summary>
    public sealed class OwnerReference : Reference
    {

        #region Properties and Fields

        #region Property: OwnerType
        /// <summary>
        /// Gets or sets the type of the owner.
        /// </summary>
        public OwnerType OwnerType
        {
            get
            {
                return Database.Current.Select<OwnerType>(this.ID, ValueTables.OwnerReference, Columns.OwnerType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.OwnerReference, Columns.OwnerType, value);
                NotifyPropertyChanged("OwnerType");
            }
        }
        #endregion Property: OwnerType

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: OwnerReference()
        /// <summary>
        /// Creates a new owner reference.
        /// </summary>
        public OwnerReference()
            : base()
        {
            Database.Current.StartChange();

            this.Name = NewNames.OwnerReference;

            Database.Current.StopChange();
        }
        #endregion Constructor: OwnerReference()

        #region Constructor: OwnerReference(uint id)
        /// <summary>
        /// Creates a new owner reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new owner reference from.</param>
        private OwnerReference(uint id)
            : base(id)
        {
        }
        #endregion Constructor: OwnerReference(uint id)

        #region Constructor: OwnerReference(OwnerReference ownerReference)
        /// <summary>
        /// Clones the owner reference.
        /// </summary>
        /// <param name="ownerReference">The owner reference to clone.</param>
        public OwnerReference(OwnerReference ownerReference)
            : base(ownerReference)
        {
            if (ownerReference != null)
            {
                Database.Current.StartChange();

                this.OwnerType = ownerReference.OwnerType;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: OwnerReference(OwnerReference ownerReference)

        #region Constructor: OwnerReference(String name)
        /// <summary>
        /// Creates a new owner reference with the given name.
        /// </summary>
        /// <param name="name">The name to create a new owner reference from.</param>
        public OwnerReference(String name)
            : base(name)
        {
        }
        #endregion Constructor: OwnerReference(String name)

        #region Constructor: OwnerReference(String name, OwnerType ownerType)
        /// <summary>
        /// Creates a new owner reference for the given owner type with the given name.
        /// </summary>
        /// <param name="name">The name to create a new owner reference from.</param>
        /// <param name="ownerType">The owner type to create a new owner reference from.</param>
        public OwnerReference(String name, OwnerType ownerType)
            : base(name)
        {
            Database.Current.StartChange();

            this.OwnerType = ownerType;

            Database.Current.StopChange();
        }
        #endregion Constructor: OwnerReference(String name, OwnerType ownerType)

        #region Constructor: OwnerReference(OwnerType ownerType)
        /// <summary>
        /// Creates a new owner reference for the given owner type.
        /// </summary>
        /// <param name="ownerType">The owner type to create a new owner reference from.</param>
        public OwnerReference(OwnerType ownerType)
            : this()
        {
            Database.Current.StartChange();

            this.OwnerType = ownerType;

            Database.Current.StopChange();
        }
        #endregion Constructor: OwnerReference(OwnerType ownerType)

        #endregion Method Group: Constructors

    }
    #endregion Class: OwnerReference

}