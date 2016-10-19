/**************************************************************************
 * 
 * PropertyChangedComponent.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.ComponentModel;

namespace Semantics.Components
{

    #region Class: PropertyChangedComponent
    /// <summary>
    /// A component that implements the INotifyPropertyChanged interface.
    /// </summary>
    public abstract class PropertyChangedComponent : INotifyPropertyChanged
    {

        #region Events, Properties and Fields

        #region Event: PropertyChanged
        /// <summary>
        /// An event handler for a changed property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Event: PropertyChanged

        #endregion Events, Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PropertyChangedComponent()
        /// <summary>
        /// Creates a new property changed component.
        /// </summary>
        protected PropertyChangedComponent()
        {
        }
        #endregion Constructor: PropertyChangedComponent() 

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: NotifyPropertyChanged(string propertyName)
        /// <summary>
        /// Notifies that a property has been changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that has been changed.</param>
        protected internal void NotifyPropertyChanged(string propertyName)
        {
            // Send an event to let others know that a property has been modified
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion Method: NotifyPropertyChanged(string propertyName)

        #endregion Method Group: Other

    }
    #endregion Class: PropertyChangedComponent

}
