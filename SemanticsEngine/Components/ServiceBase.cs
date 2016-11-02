/**************************************************************************
 * 
 * ServiceInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Components;

namespace SemanticsEngine.Components
{

    #region Class: ServiceBase
    /// <summary>
    /// A service.
    /// </summary>
    public abstract class ServiceBase : Base
    {

        #region Properties and Fields

        #region Property: Service
        /// <summary>
        /// Gets the service of which this is a service base.
        /// </summary>
        protected internal Service Service
        {
            get
            {
                return this.IdHolder as Service;
            }
        }
        #endregion Property: Service

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ServiceBase(Service service)
        /// <summary>
        /// Creates a base of the given service.
        /// </summary>
        /// <param name="service">The service to create a base of.</param>
        protected ServiceBase(Service service)
            : base(service)
        {
        }
        #endregion Constructor: ServiceBase(Service service)

        #endregion Method Group: Constructors

    }
    #endregion Class: ServiceBase

}