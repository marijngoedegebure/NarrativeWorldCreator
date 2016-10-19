/**************************************************************************
* 
* INodeExtension.cs
* 
* Jassin Kessing & Ricardo Lopes
* Computer Graphics & CAD/CAM Group
* TU Delft
* 2011
* 
* This program is free software; you can redistribute it and/or modify it.
*
*************************************************************************/

using Semantics.Components;

namespace Semantics.Interfaces
{

    #region Interface: INodeExtension
    /// <summary>
    /// An extension for a node.
    /// </summary>
    public interface INodeExtension
    {

        /// <summary>
        /// Gets or sets the node that is extended.
        /// </summary>
        Node Node { get; set; }

        /// <summary>
        /// Cleans up the NodeExtension.
        /// </summary>
        void Remove();

    }
    #endregion Interface: INodeExtension

}