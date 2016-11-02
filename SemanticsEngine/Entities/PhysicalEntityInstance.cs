/**************************************************************************
 * 
 * PhysicalEntityInstance.cs
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
using System.ComponentModel;
using Common;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region Class: PhysicalEntityInstance
    /// <summary>
    /// An instance of a physical entity.
    /// </summary>
    public abstract class PhysicalEntityInstance : EntityInstance
    {

        #region Events, Properties, and Fields

        #region Event: PositionChanged
        /// <summary>
        /// A handler for a changed position.
        /// </summary>
        /// <param name="sender">The physical entity instance of which the position was changed.</param>
        /// <param name="position">The new position.</param>
        public delegate void PositionHandler(PhysicalEntityInstance sender, Vec3 position);

        /// <summary>
        /// Invoked when the position of the instance changes.
        /// </summary>
        public event PositionHandler PositionChanged;
        #endregion Event: PositionChanged

        #region Property: PhysicalEntityBase
        /// <summary>
        /// Gets the physical entity base of which this is a physical entity instance.
        /// </summary>
        public PhysicalEntityBase PhysicalEntityBase
        {
            get
            {
                return this.NodeBase as PhysicalEntityBase;
            }
        }
        #endregion Property: PhysicalEntityBase

        #region Property: Position
        /// <summary>
        /// The possible attribute instance that stores the position.
        /// </summary>
        private AttributeInstance positionAttribute = null;

        /// <summary>
        /// The position.
        /// </summary>
        private Vec3 position = Vec3.Zero;

        /// <summary>
        /// A handler for changes in the quaternion.
        /// </summary>
        private Vec3.Vec3Handler positionHandler;

        /// <summary>
        /// Indicates whether the position setting should be disabled.
        /// </summary>
        private bool ignorePositionSet = false;

        /// <summary>
        /// Gets the position.
        /// </summary>
        public Vec3 Position
        {
            get
            {
                // If the position is stored as an attribute instance, set their current values
                if (positionAttribute != null)
                {
                    Vec4 attributePosition = ((VectorValueInstance)positionAttribute.Value).Vector;
                    ignorePositionSet = true;
                    position.X = attributePosition.X;
                    position.Y = attributePosition.Y;
                    position.Z = attributePosition.Z;
                    ignorePositionSet = false;
                }

                return position;
            }
            set
            {
                SetPosition(value);
            }
        }

        /// <summary>
        /// Notify of changes in the position vector.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void position_ValueChanged(Vec3 vector)
        {
            if (!ignorePositionSet)
                SetPosition(position);
        }

        /// <summary>
        /// Notify of changes in the position attribute.
        /// </summary>
        private void positionAttribute_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!ignorePositionSet)
            {
                AttributeInstance attributeInstance = sender as AttributeInstance;
                if (attributeInstance != null)
                {
                    VectorValueInstance vectorValueInstance = attributeInstance.Value as VectorValueInstance;
                    if (vectorValueInstance != null)
                        SetPosition(new Vec3(vectorValueInstance.Vector));
                }
            }
        }
        #endregion Property: Position

        #region Property: IsLocked
        /// <summary>
        /// The value that indicates whether this instance is locked, meaning that its position, rotation, and scale cannot be changed.
        /// </summary>
        private bool isLocked = false;

        /// <summary>
        /// Gets or sets the value that indicates whether this instance is locked, meaning that its position, rotation, and scale cannot be changed.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return isLocked;
            }
            set
            {
                SetIsLocked(value);
            }
        }
        #endregion Property: IsLocked

        #endregion Events, Properties, and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalEntityInstance(PhysicalEntityBase physicalEntityBase)
        /// <summary>
        /// Creates a new physical entity instance from the given physical entity base.
        /// </summary>
        /// <param name="physicalEntityBase">The physical entity base to create the physical entity instance from.</param>
        protected PhysicalEntityInstance(PhysicalEntityBase physicalEntityBase)
            : base(physicalEntityBase)
        {
            SetPositionFromAttribute();
        }
        #endregion Constructor: PhysicalEntityInstance(PhysicalEntityBase physicalEntityBase)

        #region Constructor: PhysicalEntityInstance(PhysicalEntityValuedBase physicalEntityValuedBase)
        /// <summary>
        /// Creates a new physical entity instance from the given valued physical entity base.
        /// </summary>
        /// <param name="physicalEntityValuedBase">The valued physical entity base to create the physical entity instance from.</param>
        protected PhysicalEntityInstance(PhysicalEntityValuedBase physicalEntityValuedBase)
            : base(physicalEntityValuedBase)
        {
            SetPositionFromAttribute();
        }
        #endregion Constructor: PhysicalEntityInstance(PhysicalEntityValuedBase physicalEntityValuedBase)

        #region Constructor: PhysicalEntityInstance(PhysicalEntityInstance physicalEntityInstance)
        /// <summary>
        /// Clones a physical entity instance.
        /// </summary>
        /// <param name="physicalEntityInstance">The physical entity instance to clone.</param>
        protected PhysicalEntityInstance(PhysicalEntityInstance physicalEntityInstance)
            : base(physicalEntityInstance)
        {
            SetPositionFromAttribute();

            if (physicalEntityInstance != null)
            {
                this.isLocked = physicalEntityInstance.IsLocked;

                this.Position = new Vec3(physicalEntityInstance.Position);
            }
        }
        #endregion Constructor: PhysicalEntityInstance(PhysicalEntityInstance physicalEntityInstance)

        #region Method: SetPositionFromAttribute()
        /// <summary>
        /// Try to set the position from an attribute.
        /// </summary>
        private void SetPositionFromAttribute()
        {
            if (this.PhysicalEntityBase != null)
            {
                // Set handlers
                this.positionHandler = new Vec3.Vec3Handler(position_ValueChanged);
                this.position.ValueChanged += this.positionHandler;

                // Set the possible position, rotation, and scale attribute instances
                AttributeBase positionAttribute = Utils.GetSpecialAttribute(SpecialAttributes.Position);
                if (positionAttribute != null)
                {
                    foreach (AttributeInstance attributeInstance in this.Attributes)
                    {
                        if (attributeInstance.AttributeBase != null && positionAttribute.Equals(attributeInstance.AttributeBase))
                        {
                            this.positionAttribute = attributeInstance;
                            this.positionAttribute.PropertyChanged += new PropertyChangedEventHandler(positionAttribute_PropertyChanged);
                            break;
                        }
                    }
                }
            }
        }
        #endregion Method: SetPositionFromAttribute()

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: SetPosition(Vec3 newPosition)
        /// <summary>
        /// Set the new position.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        protected virtual void SetPosition(Vec3 newPosition)
        {
            if (newPosition != null && !this.IsLocked)
            {
                if (this.position != null)
                    this.position.ValueChanged -= new Vec3.Vec3Handler(position_ValueChanged);

                Vec3 oldPosition = new Vec3(this.Position);

                // If the position is stored as an attribute instance, store the value there
                if (this.positionAttribute != null)
                {
                    Vec4 vector = ((VectorValueInstance)this.positionAttribute.Value).Vector;
                    this.ignorePositionSet = true;
                    vector.X = newPosition.X;
                    vector.Y = newPosition.Y;
                    vector.Z = newPosition.Z;
                    vector.W = 1;
                    this.position.X = newPosition.X;
                    this.position.Y = newPosition.Y;
                    this.position.Z = newPosition.Z;
                    this.ignorePositionSet = false;
                }
                // Otherwise, just set the default position
                else
                    this.position = newPosition;

                // Send notifications
                NotifyPropertyChanged("Position");
                if (PositionChanged != null)
                    PositionChanged(this, this.position);

                if (this.position != null)
                    this.position.ValueChanged += new Vec3.Vec3Handler(position_ValueChanged);
            }
        }
        #endregion Method: SetPosition(Vec3 newPosition)

        #region Method: SetIsLocked(bool isLockedValue)
        /// <summary>
        /// Set the new value for IsLocked.
        /// </summary>
        /// <param name="isLockedValue">The new value for IsLocked.</param>
        protected virtual void SetIsLocked(bool isLockedValue)
        {
            if (isLocked != isLockedValue)
            {
                isLocked = isLockedValue;
                NotifyPropertyChanged("IsLocked");
            }
        }
        #endregion Method: SetIsLocked(bool isLockedValue)

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the physical entity instance.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the physical entity instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the physical entity instance.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
                return base.Satisfies(conditionBase, iVariableInstanceHolder);

            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the physical entity instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the physical entity instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
                return base.Apply(changeBase, iVariableInstanceHolder);

            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetDistance(PhysicalEntityInstance otherInstance, bool threeD, bool squared)
        /// <summary>
        /// Get the distance between this physical entity instance and the other one.
        /// </summary>
        /// <param name="otherInstance">The other physical entity instance.</param>
        /// <param name="threeD">When true, X, Y, and Z are taken into account. When false, only X and Z.</param>
        /// <param name="squared">When true, the squared distance will be returned. When false, the exact distance.</param>
        /// <returns>The distance between this physical entity instance and the other one.</returns>
        public float GetDistance(PhysicalEntityInstance otherInstance, bool threeD, bool squared)
        {
            if (otherInstance != null)
            {
                Vec3 pos1 = this.Position;
                Vec3 pos2 = otherInstance.Position;

                if (pos1 != null && pos2 != null)
                {
                    if (threeD)
                    {
                        if (squared)
                            return (pos1.X - pos2.X) * (pos1.X - pos2.X) + (pos1.Y - pos2.Y) * (pos1.Y - pos2.Y) + (pos1.Z - pos2.Z) * (pos1.Z - pos2.Z);
                        else
                            return (float)Math.Sqrt((pos1.X - pos2.X) * (pos1.X - pos2.X) + (pos1.Y - pos2.Y) * (pos1.Y - pos2.Y) + (pos1.Z - pos2.Z) * (pos1.Z - pos2.Z));
                    }
                    else
                    {
                        if (squared)
                            return (pos1.X - pos2.X) * (pos1.X - pos2.X) + (pos1.Z - pos2.Z) * (pos1.Z - pos2.Z);
                        else
                            return (float)Math.Sqrt((pos1.X - pos2.X) * (pos1.X - pos2.X) + (pos1.Z - pos2.Z) * (pos1.Z - pos2.Z));
                    }
                }
            }
            return 0;
        }
        #endregion Method: GetDistance(PhysicalEntityInstance otherInstance, bool threeD, bool squared)

        #endregion Method Group: Other

    }
    #endregion Class: PhysicalEntityInstance

}