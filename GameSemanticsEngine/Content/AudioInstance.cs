/**************************************************************************
 * 
 * AudioInstance.cs
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
using GameSemantics.Utilities;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;

namespace GameSemanticsEngine.GameContent
{

    #region Class: AudioInstance
    /// <summary>
    /// An instance of audio.
    /// </summary>
    public class AudioInstance : DynamicContentInstance
    {

        #region Properties and Fields

        #region Property: AudioBase
        /// <summary>
        /// Gets the audio base of which this is an audio instance.
        /// </summary>
        public AudioBase AudioBase
        {
            get
            {
                if (this.AudioValuedBase != null)
                    return this.AudioValuedBase.AudioBase;
                return null;
            }
        }
        #endregion Property: AudioBase

        #region Property: AudioValuedBase
        /// <summary>
        /// Gets the valued audio base of which this is an audio instance.
        /// </summary>
        public AudioValuedBase AudioValuedBase
        {
            get
            {
                return this.Base as AudioValuedBase;
            }
        }
        #endregion Property: AudioValuedBase

        #region Property: Volume
        /// <summary>
        /// Gets the volume (between 0 and 100).
        /// </summary>
        public int Volume
        {
            get
            {
                if (this.AudioValuedBase != null)
                    return GetProperty<int>("Volume", this.AudioValuedBase.Volume);

                return GameSemanticsSettings.Values.Volume;
            }
            protected set
            {
                if (this.Volume != value)
                    SetProperty("Volume", Math.Max(0, Math.Min(100, (int)value)));
            }
        }
        #endregion Property: Volume

        #region Property: Speed
        /// <summary>
        /// Gets the playback speed (1 = normal).
        /// </summary>
        public float Speed
        {
            get
            {
                if (this.AudioValuedBase != null)
                    return GetProperty<float>("Speed", this.AudioValuedBase.Speed);

                return GameSemanticsSettings.Values.Speed;
            }
            protected set
            {
                if (this.Speed != value)
                    SetProperty("Speed", value);
            }
        }
        #endregion Property: Speed

        #region Property: Loop
        /// <summary>
        /// Gets the value that indicates whether the audio should loop.
        /// </summary>
        public bool Loop
        {
            get
            {
                if (this.AudioValuedBase != null)
                    return GetProperty<bool>("Loop", this.AudioValuedBase.Loop);

                return GameSemanticsSettings.Values.Loop;
            }
            protected set
            {
                if (this.Loop != value)
                    SetProperty("Loop", value);
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AudioInstance(AudioValuedBase audioValuedBase)
        /// <summary>
        /// Creates a new audio instance from the given valued audio base.
        /// </summary>
        /// <param name="audioValuedBase">The valued audio base to create the audio instance from.</param>
        internal AudioInstance(AudioValuedBase audioValuedBase)
            : base(audioValuedBase)
        {
        }
        #endregion Constructor: AudioInstance(AudioValuedBase audioValuedBase)

        #region Constructor: AudioInstance(AudioInstance audioInstance)
        /// <summary>
        /// Clones an audio instance.
        /// </summary>
        /// <param name="audioInstance">The audio instance to clone.</param>
        protected internal AudioInstance(AudioInstance audioInstance)
            : base(audioInstance)
        {
            if (audioInstance != null)
            {
                this.Volume = audioInstance.Volume;
                this.Speed = audioInstance.Speed;
                this.Loop = audioInstance.Loop;
            }
        }
        #endregion Constructor: AudioInstance(AudioInstance audioInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the audio instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the audio instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the audio instance is satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null && base.Satisfies(conditionBase, iVariableInstanceHolder))
            {
                AudioConditionBase audioConditionBase = conditionBase as AudioConditionBase;
                if (audioConditionBase != null)
                {
                    // Check whether all the properties have the correct values
                    if ((audioConditionBase.VolumeSign == null || audioConditionBase.Volume == null || Toolbox.Compare(this.Volume, (EqualitySignExtended)audioConditionBase.VolumeSign, (int)audioConditionBase.Volume)) &&
                        (audioConditionBase.SpeedSign == null || audioConditionBase.Speed == null || Toolbox.Compare(this.Speed, (EqualitySignExtended)audioConditionBase.SpeedSign, (float)audioConditionBase.Speed)) &&
                        (audioConditionBase.Loop == null || this.Loop == (bool)audioConditionBase.Loop))
                        return true;
                }
                else
                    return true;
            }
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the change to the audio instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the audio instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null && base.Apply(changeBase, iVariableInstanceHolder))
            {
                AudioChangeBase audioChangeBase = changeBase as AudioChangeBase;
                if (audioChangeBase != null)
                {
                    // Apply all changes
                    if (audioChangeBase.VolumeChange != null && audioChangeBase.Volume != null)
                        this.Volume = Toolbox.CalcValue(this.Volume, (ValueChangeType)audioChangeBase.VolumeChange, (int)audioChangeBase.Volume);
                    if (audioChangeBase.SpeedChange != null && audioChangeBase.Speed != null)
                        this.Speed = Toolbox.CalcValue(this.Speed, (ValueChangeType)audioChangeBase.SpeedChange, (float)audioChangeBase.Speed);
                    if (audioChangeBase.Loop != null)
                        this.Loop = (bool)audioChangeBase.Loop;
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: AudioInstance

}