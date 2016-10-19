/**************************************************************************
 * 
 * Metadata.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using Semantics.Data;

namespace Semantics.Components
{

    #region Class: Metadata
    /// <summary>
    /// A class to store metadata for nodes.
    /// </summary>
    public sealed class Metadata : IdHolder
    {

        #region Properties and Fields

        #region Property: Author
        /// <summary>
        /// Gets or sets the author of the node.
        /// </summary>
        public String Author
        {
            get
            {
                return Database.Current.Select<String>(this.ID, GenericTables.Metadata, Columns.Author);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Metadata, Columns.Author, value);
                NotifyPropertyChanged("Author");
            }
        }
        #endregion Property: Author

        #region Property: DateCreated
        /// <summary>
        /// Gets the creation date of the node.
        /// </summary>
        public DateTime DateCreated
        {
            get
            {
                return Database.Current.Select<DateTime>(this.ID, GenericTables.Metadata, Columns.DateCreated);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Metadata, Columns.DateCreated, value);
                NotifyPropertyChanged("DateCreated");
            }
        }
        #endregion Property: DateCreated

        #region Property: Comments
        /// <summary>
        /// Gets or sets the comments on the node.
        /// </summary>
        public String Comments
        {
            get
            {
                return Database.Current.Select<String>(this.ID, LocalizationTables.MetadataComments, Columns.Comments);
            }
            set
            {
                Database.Current.Update(this.ID, LocalizationTables.MetadataComments, Columns.Comments, value);
                NotifyPropertyChanged("Comments");
            }
        }
        #endregion Property: Comments

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Metadata()
        /// <summary>
        /// Creates new metadata.
        /// </summary>
        public Metadata()
            : base()
        {
            Database.Current.StartChange();

            // Set the user and creation date
            Database.Current.QueryBegin();
            this.Author = Environment.UserName;
            this.DateCreated = DateTime.Now;
            Database.Current.QueryCommit();

            // Insert the comments
            Database.Current.Insert(this.ID, LocalizationTables.MetadataComments);

            Database.Current.StopChange();
        }
        #endregion Constructor: Metadata()

        #region Constructor: Metadata(uint id)
        /// <summary>
        /// Creates new metadata from the given ID.
        /// </summary>
        /// <param name="id">The ID to create metadata from.</param>
        private Metadata(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Metadata(uint id)

        #region Constructor: Metadata(Metadata metadata)
        /// <summary>
        /// Clones metadata.
        /// </summary>
        /// <param name="metadata">The metadata to clone.</param>
        public Metadata(Metadata metadata)
            : base()
        {
            if (metadata != null)
            {
                Database.Current.StartChange();

                this.Author = metadata.Author;
                this.DateCreated = metadata.DateCreated;
                Database.Current.Insert(this.ID, LocalizationTables.MetadataComments, Columns.Comments, metadata.Comments);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Metadata(Metadata metadata)

        #endregion Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the metadata.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the comments
            Database.Current.Remove(this.ID, LocalizationTables.MetadataComments);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: Metadata

}