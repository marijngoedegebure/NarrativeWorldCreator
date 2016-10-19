/**************************************************************************
 * 
 * Audio.cs
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
using GameSemantics.Data;
using Semantics.Utilities;

namespace GameSemantics.GameContent
{

    #region Class: Audio
    /// <summary>
    /// Audio.
    /// </summary>
    public class Audio : DynamicContent, IComparable<Audio>
    {

        #region Method Group: Constructors

        #region Constructor: Audio()
        /// <summary>
        /// Creates new audio.
        /// </summary>
        public Audio()
            : base()
        {
        }
        #endregion Constructor: Audio()

        #region Constructor: Audio(uint id)
        /// <summary>
        /// Creates new audio from the given ID.
        /// </summary>
        /// <param name="id">The ID to create audio from.</param>
        protected Audio(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Audio(uint id)

        #region Constructor: Audio(string file)
        /// <summary>
        /// Creates new audio with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the audio.</param>
        public Audio(string file)
            : base(file)
        {
        }
        #endregion Constructor: Audio(string file)

        #region Constructor: Audio(Audio audio)
        /// <summary>
        /// Clones audio.
        /// </summary>
        /// <param name="audio">The audio to clone.</param>
        public Audio(Audio audio)
            : base(audio)
        {
        }
        #endregion Constructor: Audio(Audio audio)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the audio.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Audio other)
        /// <summary>
        /// Compares the audio to the other audio.
        /// </summary>
        /// <param name="other">The audio to compare to this audio.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Audio other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Audio other)

        #endregion Method Group: Other

    }
    #endregion Class: Audio

    #region Class: AudioValued
    /// <summary>
    /// A valued version of audio.
    /// </summary>
    public class AudioValued : DynamicContentValued
    {

        #region Properties and Fields

        #region Property: Audio
        /// <summary>
        /// Gets the audio of which this is valued audio.
        /// </summary>
        public Audio Audio
        {
            get
            {
                return this.Node as Audio;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Audio

        #region Property: Volume
        /// <summary>
        /// Gets or sets the volume (between 0 and 100).
        /// </summary>
        public int Volume
        {
            get
            {
                return GameDatabase.Current.Select<int>(this.ID, GameValueTables.AudioValued, GameColumns.Volume);
            }
            set
            {
                int volume = Math.Max(0, Math.Min(100, (int)value));
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioValued, GameColumns.Volume, volume);
                NotifyPropertyChanged("Volume");
            }
        }
        #endregion Property: Volume

        #region Property: Speed
        /// <summary>
        /// Gets or sets the playback speed (1 = normal).
        /// </summary>
        public float Speed
        {
            get
            {
                return GameDatabase.Current.Select<float>(this.ID, GameValueTables.AudioValued, GameColumns.Speed);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioValued, GameColumns.Speed, value);
                NotifyPropertyChanged("Speed");
            }
        }
        #endregion Property: Speed

        #region Property: Loop
        /// <summary>
        /// Gets or sets the value that indicates whether the audio should loop.
        /// </summary>
        public bool Loop
        {
            get
            {
                return GameDatabase.Current.Select<bool>(this.ID, GameValueTables.AudioValued, GameColumns.Loop);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioValued, GameColumns.Loop, value);
                NotifyPropertyChanged("Loop");
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AudioValued(uint id)
        /// <summary>
        /// Creates new valued audio from the given ID.
        /// </summary>
        /// <param name="id">The ID to create valued audio from.</param>
        protected AudioValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AudioValued(uint id)

        #region Constructor: AudioValued(AudioValued audioValued)
        /// <summary>
        /// Clones valued audio.
        /// </summary>
        /// <param name="audioValued">The valued audio to clone.</param>
        public AudioValued(AudioValued audioValued)
            : base(audioValued)
        {
            if (audioValued != null)
            {
                GameDatabase.Current.StartChange();

                this.Volume = audioValued.Volume;
                this.Speed = audioValued.Speed;
                this.Loop = audioValued.Loop;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: AudioValued(AudioValued audioValued)

        #region Constructor: AudioValued(Audio audio)
        /// <summary>
        /// Creates new valued audio from the given audio.
        /// </summary>
        /// <param name="audio">The audio to create valued audio from.</param>
        public AudioValued(Audio audio)
            : base(audio)
        {
        }
        #endregion Constructor: AudioValued(Audio audio)

        #endregion Method Group: Constructors

    }
    #endregion Class: AudioValued

    #region Class: AudioCondition
    /// <summary>
    /// A condition on audio.
    /// </summary>
    public class AudioCondition : DynamicContentCondition
    {

        #region Properties and Fields

        #region Property: Audio
        /// <summary>
        /// Gets or sets the required audio.
        /// </summary>
        public Audio Audio
        {
            get
            {
                return this.Node as Audio;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Audio

        #region Property: Volume
        /// <summary>
        /// Gets or sets the required volume.
        /// </summary>
        public int? Volume
        {
            get
            {
                return GameDatabase.Current.Select<int?>(this.ID, GameValueTables.AudioCondition, GameColumns.Volume);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioCondition, GameColumns.Volume, value);
                NotifyPropertyChanged("Volume");
            }
        }
        #endregion Property: Volume

        #region Property: VolumeSign
        /// <summary>
        /// Gets or sets the sign for the volume in the condition.
        /// </summary>
        public EqualitySignExtended? VolumeSign
        {
            get
            {
                return GameDatabase.Current.Select<EqualitySignExtended?>(this.ID, GameValueTables.AudioCondition, GameColumns.VolumeSign);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioCondition, GameColumns.VolumeSign, value);
                NotifyPropertyChanged("VolumeSign");
            }
        }
        #endregion Property: VolumeSign

        #region Property: Speed
        /// <summary>
        /// Gets or sets the required speed.
        /// </summary>
        public float? Speed
        {
            get
            {
                return GameDatabase.Current.Select<float?>(this.ID, GameValueTables.AudioCondition, GameColumns.Speed);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioCondition, GameColumns.Speed, value);
                NotifyPropertyChanged("Speed");
            }
        }
        #endregion Property: Speed

        #region Property: SpeedSign
        /// <summary>
        /// Gets or sets the sign for the speed in the condition.
        /// </summary>
        public EqualitySignExtended? SpeedSign
        {
            get
            {
                return GameDatabase.Current.Select<EqualitySignExtended?>(this.ID, GameValueTables.AudioCondition, GameColumns.SpeedSign);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioCondition, GameColumns.SpeedSign, value);
                NotifyPropertyChanged("SpeedSign");
            }
        }
        #endregion Property: SpeedSign

        #region Property: Loop
        /// <summary>
        /// Gets or sets the value that indicates whether the audio should loop.
        /// </summary>
        public bool? Loop
        {
            get
            {
                return GameDatabase.Current.Select<bool?>(this.ID, GameValueTables.AudioCondition, GameColumns.Loop);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioCondition, GameColumns.Loop, value);
                NotifyPropertyChanged("Loop");
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AudioCondition()
        /// <summary>
        /// Creates a new audio condition.
        /// </summary>
        public AudioCondition()
            : base()
        {
            GameDatabase.Current.StartChange();

            this.VolumeSign = EqualitySignExtended.Equal;
            this.SpeedSign = EqualitySignExtended.Equal;

            GameDatabase.Current.StopChange();
        }
        #endregion Constructor: AudioCondition()

        #region Constructor: AudioCondition(uint id)
        /// <summary>
        /// Creates a new audio condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the audio condition from.</param>
        protected AudioCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AudioCondition(uint id)

        #region Constructor: AudioCondition(AudioCondition audioCondition)
        /// <summary>
        /// Clones an audio condition.
        /// </summary>
        /// <param name="audioCondition">The audio condition to clone.</param>
        public AudioCondition(AudioCondition audioCondition)
            : base(audioCondition)
        {
            if (audioCondition != null)
            {
                GameDatabase.Current.StartChange();

                this.Volume = audioCondition.Volume;
                this.VolumeSign = audioCondition.VolumeSign;
                this.Speed = audioCondition.Speed;
                this.SpeedSign = audioCondition.SpeedSign;
                this.Loop = audioCondition.Loop;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: AudioCondition(AudioCondition audioCondition)

        #region Constructor: AudioCondition(Audio audio)
        /// <summary>
        /// Creates a condition for the given audio.
        /// </summary>
        /// <param name="audio">The audio to create a condition for.</param>
        public AudioCondition(Audio audio)
            : base(audio)
        {
        }
        #endregion Constructor: AudioCondition(Audio audio)

        #endregion Method Group: Constructors

    }
    #endregion Class: AudioCondition

    #region Class: AudioChange
    /// <summary>
    /// A change on audio.
    /// </summary>
    public class AudioChange : DynamicContentChange
    {

        #region Properties and Fields

        #region Property: Audio
        /// <summary>
        /// Gets or sets the affected audio.
        /// </summary>
        public Audio Audio
        {
            get
            {
                return this.Node as Audio;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Audio

        #region Property: Volume
        /// <summary>
        /// Gets or sets the volume to change to.
        /// </summary>
        public int? Volume
        {
            get
            {
                return GameDatabase.Current.Select<int?>(this.ID, GameValueTables.AudioChange, GameColumns.Volume);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioChange, GameColumns.Volume, value);
                NotifyPropertyChanged("Volume");
            }
        }
        #endregion Property: Volume

        #region Property: VolumeChange
        /// <summary>
        /// Gets or sets the type of change for the volume.
        /// </summary>
        public ValueChangeType? VolumeChange
        {
            get
            {
                return GameDatabase.Current.Select<ValueChangeType?>(this.ID, GameValueTables.AudioChange, GameColumns.VolumeChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioChange, GameColumns.VolumeChange, value);
                NotifyPropertyChanged("VolumeChange");
            }
        }
        #endregion Property: VolumeChange

        #region Property: Speed
        /// <summary>
        /// Gets or sets the speed to change to.
        /// </summary>
        public float? Speed
        {
            get
            {
                return GameDatabase.Current.Select<float?>(this.ID, GameValueTables.AudioChange, GameColumns.Speed);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioChange, GameColumns.Speed, value);
                NotifyPropertyChanged("Speed");
            }
        }
        #endregion Property: Speed

        #region Property: SpeedChange
        /// <summary>
        /// Gets or sets the type of change for the speed.
        /// </summary>
        public ValueChangeType? SpeedChange
        {
            get
            {
                return GameDatabase.Current.Select<ValueChangeType?>(this.ID, GameValueTables.AudioChange, GameColumns.SpeedChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioChange, GameColumns.SpeedChange, value);
                NotifyPropertyChanged("SpeedChange");
            }
        }
        #endregion Property: SpeedChange

        #region Property: Loop
        /// <summary>
        /// Gets or sets the loop value to change to.
        /// </summary>
        public bool? Loop
        {
            get
            {
                return GameDatabase.Current.Select<bool?>(this.ID, GameValueTables.AudioChange, GameColumns.Loop);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AudioChange, GameColumns.Loop, value);
                NotifyPropertyChanged("Loop");
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AudioChange()
        /// <summary>
        /// Creates an audio change.
        /// </summary>
        public AudioChange()
            : base()
        {
        }
        #endregion Constructor: AudioChange()

        #region Constructor: AudioChange(uint id)
        /// <summary>
        /// Creates a new audio change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an audio change from.</param>
        protected AudioChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AudioChange(uint id)

        #region Constructor: AudioChange(AudioChange audioChange)
        /// <summary>
        /// Clones an audio change.
        /// </summary>
        /// <param name="audioChange">The audio change to clone.</param>
        public AudioChange(AudioChange audioChange)
            : base(audioChange)
        {
            if (audioChange != null)
            {
                GameDatabase.Current.StartChange();

                this.Volume = audioChange.Volume;
                this.VolumeChange = audioChange.VolumeChange;
                this.Speed = audioChange.Speed;
                this.SpeedChange = audioChange.SpeedChange;
                this.Loop = audioChange.Loop;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: AudioChange(AudioChange audioChange)

        #region Constructor: AudioChange(Audio audio)
        /// <summary>
        /// Creates a change for the given audio.
        /// </summary>
        /// <param name="audio">The audio to create a change for.</param>
        public AudioChange(Audio audio)
            : base(audio)
        {
        }
        #endregion Constructor: AudioChange(Audio audio)

        #endregion Method Group: Constructors

    }
    #endregion Class: AudioChange

}