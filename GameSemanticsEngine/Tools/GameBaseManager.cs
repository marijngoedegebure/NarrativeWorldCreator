/**************************************************************************
 * 
 * GameBaseManager.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using GameSemantics.Components;
using GameSemantics.GameContent;
using GameSemanticsEngine.Components;
using GameSemanticsEngine.GameContent;
using SemanticsEngine.Tools;

namespace GameSemanticsEngine.Tools
{

	#region Class: GameBaseManager
	/// <summary>
	/// The game base manager allows the creation of bases from ID holders.
	/// </summary>
	public class GameBaseManager : BaseManager
	{

		#region Static Constructor: GameBaseManager()
		/// <summary>
		/// Add pairs of non-abstract ID holders and their corresponding base classes.
		/// </summary>
		static GameBaseManager()
		{
			// Components
			AddIdHolderBasePair(typeof(ContentPerView), typeof(ContentPerViewBase));
			AddIdHolderBasePair(typeof(EventContent), typeof(EventContentBase));
			AddIdHolderBasePair(typeof(EventExtension), typeof(EventExtensionBase));
			AddIdHolderBasePair(typeof(GameObject), typeof(GameObjectBase));
			AddIdHolderBasePair(typeof(GameNode), typeof(GameNodeBase));
			AddIdHolderBasePair(typeof(View), typeof(ViewBase));
			AddIdHolderBasePair(typeof(ViewsPerContextType), typeof(ViewsPerContextTypeBase));

			// Content
			AddIdHolderBasePair(typeof(Animation), typeof(AnimationBase));
			AddIdHolderBasePair(typeof(AnimationValued), typeof(AnimationValuedBase));
			AddIdHolderBasePair(typeof(Audio), typeof(AudioBase));
			AddIdHolderBasePair(typeof(AudioValued), typeof(AudioValuedBase));
			AddIdHolderBasePair(typeof(Cinematic), typeof(CinematicBase));
			AddIdHolderBasePair(typeof(CinematicValued), typeof(CinematicValuedBase));
			AddIdHolderBasePair(typeof(GameFilter), typeof(GameFilterBase));
			AddIdHolderBasePair(typeof(GameFilterValued), typeof(GameFilterValuedBase));
			AddIdHolderBasePair(typeof(GameMaterial), typeof(GameMaterialBase));
			AddIdHolderBasePair(typeof(GameMaterialValued), typeof(GameMaterialValuedBase));
			AddIdHolderBasePair(typeof(Icon), typeof(IconBase));
			AddIdHolderBasePair(typeof(IconValued), typeof(IconValuedBase));
			AddIdHolderBasePair(typeof(Model), typeof(ModelBase));
			AddIdHolderBasePair(typeof(ModelValued), typeof(ModelValuedBase));
			AddIdHolderBasePair(typeof(ParticleEmitter), typeof(ParticleEmitterBase));
			AddIdHolderBasePair(typeof(ParticleEmitterValued), typeof(ParticleEmitterValuedBase));
			AddIdHolderBasePair(typeof(ParticleProperties), typeof(ParticlePropertiesBase));
			AddIdHolderBasePair(typeof(ParticlePropertiesValued), typeof(ParticlePropertiesValuedBase));
			AddIdHolderBasePair(typeof(Script), typeof(ScriptBase));
			AddIdHolderBasePair(typeof(ScriptValued), typeof(ScriptValuedBase));
			AddIdHolderBasePair(typeof(Sprite), typeof(SpriteBase));
			AddIdHolderBasePair(typeof(SpriteValued), typeof(SpriteValuedBase));

			// Conditions
			AddIdHolderBasePair(typeof(AnimationCondition), typeof(AnimationConditionBase));
			AddIdHolderBasePair(typeof(AudioCondition), typeof(AudioConditionBase));
			AddIdHolderBasePair(typeof(CinematicCondition), typeof(CinematicConditionBase));
			AddIdHolderBasePair(typeof(GameMaterialCondition), typeof(GameMaterialConditionBase));
			AddIdHolderBasePair(typeof(IconCondition), typeof(IconConditionBase));
			AddIdHolderBasePair(typeof(ModelCondition), typeof(ModelConditionBase));
			AddIdHolderBasePair(typeof(ParticleEmitterCondition), typeof(ParticleEmitterConditionBase));
			AddIdHolderBasePair(typeof(ParticlePropertiesCondition), typeof(ParticlePropertiesConditionBase));
			AddIdHolderBasePair(typeof(ScriptCondition), typeof(ScriptConditionBase));
			AddIdHolderBasePair(typeof(SpriteCondition), typeof(SpriteConditionBase));

			// Changes
			AddIdHolderBasePair(typeof(AnimationChange), typeof(AnimationChangeBase));
			AddIdHolderBasePair(typeof(AudioChange), typeof(AudioChangeBase));
			AddIdHolderBasePair(typeof(CinematicChange), typeof(CinematicChangeBase));
			AddIdHolderBasePair(typeof(GameMaterialChange), typeof(GameMaterialChangeBase));
			AddIdHolderBasePair(typeof(IconChange), typeof(IconChangeBase));
			AddIdHolderBasePair(typeof(ModelChange), typeof(ModelChangeBase));
			AddIdHolderBasePair(typeof(ParticleEmitterChange), typeof(ParticleEmitterChangeBase));
			AddIdHolderBasePair(typeof(ParticlePropertiesChange), typeof(ParticlePropertiesChangeBase));
			AddIdHolderBasePair(typeof(ScriptChange), typeof(ScriptChangeBase));
			AddIdHolderBasePair(typeof(SpriteChange), typeof(SpriteChangeBase));
		}
		#endregion Static Constructor: GameBaseManager()

		#region Method: Initialize()
		/// <summary>
		/// Initialize the game base manager.
		/// </summary>
		public static void Initialize()
		{
			// Make the game base manager the current base manager
			Current = new GameBaseManager();
		}
		#endregion Method: Initialize()
		
	}
	#endregion Class: GameBaseManager

}