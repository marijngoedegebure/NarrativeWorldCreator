/**************************************************************************
 * 
 * Service.cs
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
using System.Reflection;

namespace Semantics.Components
{

    #region Class: Service
    /// <summary>
    /// A service.
    /// </summary>
    public abstract class Service : IdHolder
    {

        #region Method Group: Constructors

        #region Constructor: Service()
        /// <summary>
        /// Creates a new service.
        /// </summary>
        protected Service()
            : base()
        {
        }
        #endregion Constructor: Service()

        #region Constructor: Service(uint id)
        /// <summary>
        /// Creates a new service from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new service from.</param>
        protected Service(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Service(uint id)

        #region Constructor: Service(Service service)
        /// <summary>
        /// Clones a service.
        /// </summary>
        /// <param name="service">The service to clone.</param>
        protected Service(Service service)
            : base()
        {
        }
        #endregion Constructor: Service(Service service)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the service.
        /// </summary>
        /// <returns>A clone of the service.</returns>
        public Service Clone()
        {
            try
            {
                Type type = this.GetType();
                return type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type }, null).Invoke(new object[] { this }) as Service;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: Service

}