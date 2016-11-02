/**************************************************************************
 * 
 * AudioBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using GameSemantics.GameContent;
using GameSemantics.Utilities;
using Semantics.Utilities;

namespace GameSemanticsEngine.GameContent
{

    #region Class: AudioBase
    /// <summary>
    /// A base of audio.
    /// </summary>
    public class AudioBase : DynamicContentBase
    {

        #region Properties and Fields

        #region Property: Audio
        /// <summary>
        /// Gets the audio of which this is an audio base.
        /// </summary>
        protected internal Audio Audio
        {
            get
            {
                return this.Node as Audio;
            }
        }
        #endregion Property: Audio

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AudioBase(Audio audio)
        /// <summary>
        /// Creates a new audio base from the given audio.
        /// </summary>
        /// <param name="audio">The audio to create a audio base from.</param>
        protected internal AudioBase(Audio audio)
            : base(audio)
        {
        }
        #endregion Constructor: AudioBase(Audio audio)

        #endregion Method Group: Constructors

    }
    #endregion Class: AudioBase

    #region Class: AudioValuedBase
    /// <summary>
    /// A base of valued audio.
    /// </summary>
    public class AudioValuedBase : DynamicContentValuedBase
    {

        #region Properties and Fields

        #region Property: AudioValued
        /// <summary>
        /// Gets the valued audio of which this is a valued audio base.
        /// </summary>
        protected internal AudioValued AudioValued
        {
            get
            {
                return this.NodeValued as AudioValued;
            }
        }
        #endregion Property: AudioValued

        #region Property: AudioBase
        /// <summary>
        /// Gets the audio base.
        /// </summary>
        public AudioBase AudioBase
        {
            get
            {
                return this.NodeBase as AudioBase;
            }
        }
        #endregion Property: AudioBase

        #region Property: Volume
        /// <summary>
        /// The volume (between 0 and 100).
        /// </summary>
        private int volume = GameSemanticsSettings.Values.Volume;

        /// <summary>
        /// Gets the volume (between 0 and 100).
        /// </summary>
        public int Volume
        {
            get
            {
                return volume;
            }
        }
        #endregion Property: Volume

        #region Property: Speed
        /// <summary>
        /// The playback speed (1 = normal).
        /// </summary>
        private float speed = GameSemanticsSettings.Values.Speed;

        /// <summary>
        /// Gets the playback speed (1 = normal).
        /// </summary>
        public float Speed
        {
            get
            {
                return speed;
            }
        }
        #endregion Property: Speed

        #region Property: Loop
        /// <summary>
        /// The value that indicates whether the audio should loop.
        /// </summary>
        private bool loop = GameSemanticsSettings.Values.Loop;

        /// <summary>
        /// Gets the value that indicates whether the audio should loop.
        /// </summary>
        public bool Loop
        {
            get
            {
                return loop;
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AudioValuedBase(AudioValued audioValued)
        /// <summary>
        /// Create a valued audio base from the given valued audio.
        /// </summary>
        /// <param name="audioValued">The valued audio to create a valued audio base from.</param>
        protected internal AudioValuedBase(AudioValued audioValued)
            : base(audioValued)
        {
            if (audioValued != null)
            {
                this.volume = audioValued.Volume;
                this.speed = audioValued.Speed;
                this.loop = audioValued.Loop;
            }
        }
        #endregion Constructor: AudioValuedBase(AudioValued audioValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: AudioValuedBase

    #region Class: AudioConditionBase
    /// <summary>
    /// A condition on audio.
    /// </summary>
    public class AudioConditionBase : DynamicContentConditionBase
    {

        #region Properties and Fields

        #region Property: AudioCondition
        /// <summary>
        /// Gets the audio condition of which this is an audio condition base.
        /// </summary>
        protected internal AudioCondition AudioCondition
        {
            get
            {
                return this.Condition as AudioCondition;
            }
        }
        #endregion Property: AudioCondition

        #region Property: AudioBase
        /// <summary>
        /// Gets the audio base of which this is an audio condition base.
        /// </summary>
        public AudioBase AudioBase
        {
            get
            {
                return this.NodeBase as AudioBase;
            }
        }
        #endregion Property: AudioBase

        #region Property: Volume
        /// <summary>
        /// The required volume.
        /// </summary>
        private int? volume = null;
        
        /// <summary>
        /// Gets the required volume.
        /// </summary>
        public int? Volume
        {
            get
            {
                return volume;
            }
        }
        #endregion Property: Volume

        #region Property: VolumeSign
        /// <summary>
        /// The sign for the volume in the condition.
        /// </summary>
        private EqualitySignExtended? volumeSign = null;
        
        /// <summary>
        /// Gets the sign for the volume in the condition.
        /// </summary>
        public EqualitySignExtended? VolumeSign
        {
            get
            {
                return volumeSign;
            }
        }
        #endregion Property: VolumeSign

        #region Property: Speed
        /// <summary>
        /// The required speed.
        /// </summary>
        private float? speed = null;
        
        /// <summary>
        /// Gets the required speed.
        /// </summary>
        public float? Speed
        {
            get
            {
                return speed;
            }
        }
        #endregion Property: Speed

        #region Property: SpeedSign
        /// <summary>
        /// The sign for the speed in the condition.
        /// </summary>
        private EqualitySignExtended? speedSign = null;
        
        /// <summary>
        /// Gets the sign for the speed in the condition.
        /// </summary>
        public EqualitySignExtended? SpeedSign
        {
            get
            {
                return speedSign;
            }
        }
        #endregion Property: SpeedSign

        #region Property: Loop
        /// <summary>
        /// The value that indicates whether the audio should loop.
        /// </summary>
        private bool? loop = null;

        /// <summary>
        /// Gets the value that indicates whether the audio should loop.
        /// </summary>
        public bool? Loop
        {
            get
            {
                return loop;
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AudioConditionBase(AudioCondition audioCondition)
        /// <summary>
        /// Creates a base of the given audio condition.
        /// </summary>
        /// <param name="audioCondition">The audio condition to create a base of.</param>
        protected internal AudioConditionBase(AudioCondition audioCondition)
            : base(audioCondition)
        {
            if (audioCondition != null)
            {
                this.volume = audioCondition.Volume;
                this.volumeSign = audioCondition.VolumeSign;
                this.speed = audioCondition.Speed;
                this.speedSign = audioCondition.SpeedSign;
                this.loop = audioCondition.Loop;
            }
        }
        #endregion Constructor: AudioConditionBase(AudioCondition audioCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: AudioConditionBase

    #region Class: AudioChangeBase
    /// <summary>
    /// A change on audio.
    /// </summary>
    public class AudioChangeBase : DynamicContentChangeBase
    {

        #region Properties and Fields

        #region Property: AudioChange
        /// <summary>
        /// Gets the audio change of which this is an audio change base.
        /// </summary>
        protected internal AudioChange AudioChange
        {
            get
            {
                return this.Change as AudioChange;
            }
        }
        #endregion Property: AudioChange

        #region Property: AudioBase
        /// <summary>
        /// Gets the affected audio base.
        /// </summary>
        public AudioBase AudioBase
        {
            get
            {
                return this.NodeBase as AudioBase;
            }
        }
        #endregion Property: AudioBase

        #region Property: Volume
        /// <summary>
        /// The volume to change to.
        /// </summary>
        private int? volume = null;

        /// <summary>
        /// Gets the volume to change to.
        /// </summary>
        public int? Volume
        {
            get
            {
                return volume;
            }
        }
        #endregion Property: Volume

        #region Property: VolumeChange
        /// <summary>
        /// The type of change for the volume.
        /// </summary>
        private ValueChangeType? volumeChange = null;
        
        /// <summary>
        /// Gets the type of change for the volume.
        /// </summary>
        public ValueChangeType? VolumeChange
        {
            get
            {
                return volumeChange;
            }
        }
        #endregion Property: VolumeChange

        #region Property: Speed
        /// <summary>
        /// The speed to change to.
        /// </summary>
        private float? speed = null;
        
        /// <summary>
        /// Gets the speed to change to.
        /// </summary>
        public float? Speed
        {
            get
            {
                return speed;
            }
        }
        #endregion Property: Speed

        #region Property: SpeedChange
        /// <summary>
        /// The type of change for the speed.
        /// </summary>
        private ValueChangeType? speedChange = null;
        
        /// <summary>
        /// Gets the type of change for the speed.
        /// </summary>
        public ValueChangeType? SpeedChange
        {
            get
            {
                return speedChange;
            }
        }
        #endregion Property: SpeedChange

        #region Property: Loop
        /// <summary>
        /// The loop value to change to.
        /// </summary>
        private bool? loop = null;

        /// <summary>
        /// Gets the loop value to change to.
        /// </summary>
        public bool? Loop
        {
            get
            {
                return loop;
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AudioChangeBase(AudioChange audioChange)
        /// <summary>
        /// Creates a base of the given audio change.
        /// </summary>
        /// <param name="audioChange">The audio change to create a base of.</param>
        protected internal AudioChangeBase(AudioChange audioChange)
            : base(audioChange)
        {
            if (audioChange != null)
            {
                this.volume = audioChange.Volume;
                this.volumeChange = audioChange.VolumeChange;
                this.speed = audioChange.Speed;
                this.speedChange = audioChange.SpeedChange;
                this.loop = audioChange.Loop;
            }
        }
        #endregion Constructor: AudioChangeBase(AudioChange audioChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: AudioChangeBase

}