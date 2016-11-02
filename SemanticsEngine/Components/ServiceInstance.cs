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

namespace SemanticsEngine.Components
{

    #region Class: ServiceInstance
    /// <summary>
    /// A service.
    /// </summary>
    public abstract class ServiceInstance : Instance
    {

        #region Properties and Fields

        #region Property: Service
        /// <summary>
        /// Gets the service base of which this is a service instance.
        /// </summary>
        public ServiceBase ServiceBase
        {
            get
            {
                return this.Base as ServiceBase;
            }
        }
        #endregion Property: Service

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ServiceInstance(ServiceBase serviceBase)
        /// <summary>
        /// Creates a new service instance from the given service base.
        /// </summary>
        /// <param name="serviceBase">The service base to create the service instance from.</param>
        protected ServiceInstance(ServiceBase serviceBase)
            : base(serviceBase)
        {
        }
        #endregion Constructor: ServiceInstance(ServiceBase serviceBase)

        #region Constructor: ServiceInstance(ServiceInstance serviceInstance)
        /// <summary>
        /// Clones the service instance.
        /// </summary>
        /// <param name="serviceInstance">The service instance to clone.</param>
        protected ServiceInstance(ServiceInstance serviceInstance)
            : base(serviceInstance)
        {
        }
        #endregion Constructor: ServiceInstance(ServiceInstance serviceInstance)

        #endregion Method Group: Constructors

    }
    #endregion Class: ServiceInstance

}