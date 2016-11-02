/**************************************************************************
 * 
 * EffectInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

namespace SemanticsEngine.Components
{

    #region Class: EffectInstance
    /// <summary>
    /// An instance of an effect.
    /// </summary>
    internal sealed class EffectInstance : Instance
    {

        #region Properties and Fields

        #region Property: EffectBase
        /// <summary>
        /// Gets the effect base of which this is an effect instance.
        /// </summary>
        internal EffectBase EffectBase
        {
            get
            {
                return this.Base as EffectBase;
            }
        }
        #endregion Property: EffectBase

        #region Property: EventInstance
        /// <summary>
        /// The event instance of which this is an effect.
        /// </summary>
        private EventInstance eventInstance = null;

        /// <summary>
        /// Gets the event instance of which this is an effect.
        /// </summary>
        internal EventInstance EventInstance
        {
            get
            {
                return eventInstance;
            }
        }
        #endregion Property: EventInstance

        #region Field: remainingDelay
        /// <summary>
        /// The remaining delay.
        /// </summary>
        internal float remainingDelay = 0;
        #endregion Field: remainingDelay

        #region Field: remainingFrequency
        /// <summary>
        /// The remaining frequency.
        /// </summary>
        internal int remainingFrequency = 0;
        #endregion Field: remainingFrequency

        #region Field: remainingDuration
        /// <summary>
        /// The remaining duration.
        /// </summary>
        internal float remainingDuration = 0;
        #endregion Field: remainingDuration

        #region Field: remainingInterval
        /// <summary>
        /// The remaining interval.
        /// </summary>
        internal float remainingInterval = 0;
        #endregion Field: remainingInterval

        #region Field: totalInterval
        /// <summary>
        /// The total interval.
        /// </summary>
        internal float totalInterval = 0;
        #endregion Field: totalInterval

        #region Field: durationDependentOnRequirements
        /// <summary>
        /// Indicates whether the duration is dependent on the requirements.
        /// </summary>
        internal bool durationDependentOnRequirements = false;
        #endregion Field: durationDependentOnRequirements

        #region Field: infiniteFrequency
        /// <summary>
        /// Indicates whether the frequency is infinite.
        /// </summary>
        internal bool infiniteFrequency = false;
        #endregion Field: infiniteFrequency

        #region Field: firstExecution
        /// <summary>
        /// Indicates whether the effect is executed for the first time.
        /// </summary>
        internal bool firstExecution = true;
        #endregion Field: firstExecution
        
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EffectInstance(EffectBase effectBase, EventInstance eventInstance)
        /// <summary>
        /// Creates a new effect instance from the given effect base for the given event instance.
        /// </summary>
        /// <param name="effectBase">The effect base to create the effect instance from.</param>
        /// <param name="eventInstance">The event instance that started the effect.</param>
        internal EffectInstance(EffectBase effectBase, EventInstance eventInstance)
            : base(effectBase)
        {
            this.eventInstance = eventInstance;
        }
        #endregion Constructor: EffectInstance(EffectBase effectBase, EventInstance eventInstance)

        #endregion Method Group: Constructors

    }
    #endregion Class: EffectInstance
		
}