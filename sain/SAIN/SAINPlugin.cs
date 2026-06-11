using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using EFT;
using SAIN.Editor;
using SAIN.Helpers;
using SAIN.Patches.Components;
using SAIN.Patches.Generic;
using SAIN.Patches.Generic.Fixes;
using SAIN.Patches.Generic.SetInHands;
using SAIN.Patches.Hearing;
using SAIN.Patches.Movement;
using SAIN.Patches.Shoot.Aim;
using SAIN.Patches.Shoot.Grenades;
using SAIN.Patches.Shoot.RateOfFire;
using SAIN.Patches.Talk;
using SAIN.Patches.Vision;
using SAIN.Plugin;
using SAIN.Preset;
using SAIN.Preset.GlobalSettings;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN;

[BepInPlugin("me.sol.sain", "SAIN", "4.0.3")]
[BepInDependency("xyz.drakia.bigbrain", "1.0.0")]
[BepInDependency("com.SPT.core", "3.11.0")]
[BepInProcess("EscapeFromTarkov.exe")]
[BepInIncompatibility("com.dvize.BushNoESP")]
[BepInIncompatibility("com.dvize.NoGrenadeESP")]
public class SAINPlugin : BaseUnityPlugin
{
	public static ECombatDecision ForceSoloDecision;

	public static ESquadDecision ForceSquadDecision;

	public static ESelfDecision ForceSelfDecision;

	public static DebugSettings DebugSettings => LoadedPreset.GlobalSettings.General.Debug;

	public static bool DebugMode => DebugSettings.Logs.GlobalDebugMode;

	public static bool ProfilingMode => DebugSettings.Logs.GlobalProfilingToggle;

	public static bool DrawDebugGizmos => DebugSettings.Gizmos.DrawDebugGizmos;

	public static PresetEditorDefaults EditorDefaults => PresetHandler.EditorDefaults;

	public static ConfigEntry<bool> OpenEditorButton { get; private set; }

	public static ConfigEntry<KeyboardShortcut> OpenEditorConfigEntry { get; private set; }

	private List<ModulePatch> SainPatches => new List<ModulePatch>(109)
	{
		(ModulePatch)(object)new WorldTickPatch(),
		(ModulePatch)(object)new PlayerLateUpdatePatch(),
		(ModulePatch)(object)new AddBotComponentPatch(),
		(ModulePatch)(object)new ActivateBotComponentPatch(),
		(ModulePatch)(object)new AddGameWorldPatch(),
		(ModulePatch)(object)new GetBotController(),
		(ModulePatch)(object)new SetEnvironmentPatch(),
		(ModulePatch)(object)new SetPanicPointPatch(),
		(ModulePatch)(object)new AddPointToSearchPatch(),
		(ModulePatch)(object)new TurnDamnLightOffPatch(),
		(ModulePatch)(object)new GrenadeThrownActionPatch(),
		(ModulePatch)(object)new GrenadeExplosionActionPatch(),
		(ModulePatch)(object)new ShallKnowEnemyPatch(),
		(ModulePatch)(object)new ShallKnowEnemyLatePatch(),
		(ModulePatch)(object)new HaveSeenEnemyPatch(),
		(ModulePatch)(object)new AllowRequestPatch(),
		(ModulePatch)(object)new FindRequestForMePatch(),
		(ModulePatch)(object)new SetInHands_Empty(),
		(ModulePatch)(object)new SetInHands_Food_Patch(),
		(ModulePatch)(object)new SetInHands_Grenade_Patch(),
		(ModulePatch)(object)new SetInHands_Knife_Patch(),
		(ModulePatch)(object)new SetInHands_Meds_Patch1(),
		(ModulePatch)(object)new SetInHands_Meds_Patch2(),
		(ModulePatch)(object)new SetInHands_QuickUse_Patch1(),
		(ModulePatch)(object)new SetInHands_QuickUse_Patch2(),
		(ModulePatch)(object)new SetInHands_Weapon_Patch(),
		(ModulePatch)(object)new SetInHands_Weapon_Stationary_Patch(),
		(ModulePatch)(object)new StopSetToNavMeshPatch(),
		(ModulePatch)(object)new StopSetToNavMeshPatch2(),
		(ModulePatch)(object)new FightShallReloadFixPatch(),
		(ModulePatch)(object)new EnableVaultPatch(),
		(ModulePatch)(object)new BotMemoryAddEnemyPatch(),
		(ModulePatch)(object)new BotGroupAddEnemyPatch(),
		(ModulePatch)(object)new FixItemTakerPatch(),
		(ModulePatch)(object)new FixItemTakerPatch2(),
		(ModulePatch)(object)new RotateClampPatch(),
		(ModulePatch)(object)new RunToEnemyUpdatePatch(),
		(ModulePatch)(object)new InfiniteMagFixPatch(),
		(ModulePatch)(object)new EncumberedPatch(),
		(ModulePatch)(object)new DoorOpenerPatch(),
		(ModulePatch)(object)new DoorDisabledPatch(),
		(ModulePatch)(object)new CrawlPatch(),
		(ModulePatch)(object)new PoseStaminaPatch(),
		(ModulePatch)(object)new AimStaminaPatch(),
		(ModulePatch)(object)new GlobalShootSettingsPatch(),
		(ModulePatch)(object)new GlobalLookPatch(),
		(ModulePatch)(object)new MovementContextIsAIPatch(),
		(ModulePatch)(object)new SetDoorCollisionPatch(),
		(ModulePatch)(object)new CanBeSnappedPatch(),
		(ModulePatch)(object)new BotMoverManualUpdatePatch(),
		(ModulePatch)(object)new BotMoverManualFixedUpdatePatch(),
		(ModulePatch)(object)new SprintLookDirPatch(),
		(ModulePatch)(object)new TryPlayShootSoundPatch(),
		(ModulePatch)(object)new OnMakingShotPatch(),
		(ModulePatch)(object)new RegisterShotPatch(),
		(ModulePatch)(object)new OnWeaponModifiedPatch(),
		(ModulePatch)(object)new HearingSensorPatch(),
		(ModulePatch)(object)new GrenadeCollisionPatch(),
		(ModulePatch)(object)new GrenadeCollisionPatch2(),
		(ModulePatch)(object)new ToggleSoundPatch(),
		(ModulePatch)(object)new SpawnInHandsSoundPatch(),
		(ModulePatch)(object)new PlaySwitchHeadlightSoundPatch(),
		(ModulePatch)(object)new BulletImpactPatch(),
		(ModulePatch)(object)new TreeSoundPatch(),
		(ModulePatch)(object)new DoorBreachSoundPatch(),
		(ModulePatch)(object)new DoorOpenSoundPatch(),
		(ModulePatch)(object)new FootstepSoundPatch(),
		(ModulePatch)(object)new SprintSoundPatch(),
		(ModulePatch)(object)new GenericMovementSoundPatch(),
		(ModulePatch)(object)new SpecificStepAudioControllerPatch(),
		(ModulePatch)(object)new JumpSoundPatch(),
		(ModulePatch)(object)new DryShotPatch(),
		(ModulePatch)(object)new ProneSoundPatch(),
		(ModulePatch)(object)new SoundClipNameCheckerPatch(),
		(ModulePatch)(object)new SoundClipNameCheckerPatch2(),
		(ModulePatch)(object)new AimSoundPatch(),
		(ModulePatch)(object)new LootingSoundPatch(),
		(ModulePatch)(object)new JumpPainPatch(),
		(ModulePatch)(object)new PlayerHurtPatch(),
		(ModulePatch)(object)new PlayerTalkPatch(),
		(ModulePatch)(object)new BotTalkPatch(),
		(ModulePatch)(object)new BotTalkManualUpdatePatch(),
		(ModulePatch)(object)new DisableLookUpdatePatch(),
		(ModulePatch)(object)new UpdateLightEnablePatch(),
		(ModulePatch)(object)new UpdateLightEnablePatch2(),
		(ModulePatch)(object)new ToggleNightVisionPatch(),
		(ModulePatch)(object)new SetPartPriorityPatch(),
		(ModulePatch)(object)new GlobalLookSettingsPatch(),
		(ModulePatch)(object)new WeatherTimeVisibleDistancePatch(),
		(ModulePatch)(object)new NoAIESPPatch(),
		(ModulePatch)(object)new BotLightTurnOnPatch(),
		(ModulePatch)(object)new VisionSpeedPatch(),
		(ModulePatch)(object)new WeatherVisionPatch(),
		(ModulePatch)(object)new VisionDistancePatch(),
		(ModulePatch)(object)new CheckFlashlightPatch(),
		(ModulePatch)(object)new DoHitAffectPatch(),
		(ModulePatch)(object)new HitAffectApplyPatch(),
		(ModulePatch)(object)new PlayerHitReactionDisablePatch(),
		(ModulePatch)(object)new SetAimStatusPatch(),
		(ModulePatch)(object)new AimOffsetPatch(),
		(ModulePatch)(object)new AimTimePatch(),
		(ModulePatch)(object)new ForceNoHeadAimPatch(),
		(ModulePatch)(object)new SmoothTurnPatch(),
		(ModulePatch)(object)new ResetGrenadePatch(),
		(ModulePatch)(object)new SetGrenadePatch(),
		(ModulePatch)(object)new FullAutoPatch(),
		(ModulePatch)(object)new SemiAutoPatch(),
		(ModulePatch)(object)new SemiAutoPatch2(),
		(ModulePatch)(object)new SemiAutoPatch3()
	};

	public static SAINPresetClass LoadedPreset => PresetHandler.LoadedPreset;

	public void Awake()
	{
		PresetHandler.Init();
		BindConfigs();
		InitPatches();
		BigBrainHandler.Init();
		Vector.Init();
	}

	private void BindConfigs()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		string text = "SAIN Editor";
		OpenEditorButton = ((BaseUnityPlugin)this).Config.Bind<bool>(text, "Open Editor", false, "Opens the Editor on press");
		OpenEditorConfigEntry = ((BaseUnityPlugin)this).Config.Bind<KeyboardShortcut>(text, "Open Editor Shortcut", new KeyboardShortcut((KeyCode)287, Array.Empty<KeyCode>()), "The keyboard shortcut that toggles editor");
	}

	private void InitPatches()
	{
		foreach (ModulePatch sainPatch in SainPatches)
		{
			sainPatch.Enable();
		}
	}

	public void Update()
	{
		ModDetection.Update();
		SAINEditor.Update();
	}

	public void Start()
	{
		SAINEditor.Init();
	}

	public void LateUpdate()
	{
		SAINEditor.LateUpdate();
	}

	public void OnGUI()
	{
		SAINEditor.OnGUI();
	}

	public static bool IsBotExluded(BotOwner botOwner)
	{
		return SAINEnableClass.IsSAINDisabledForBot(botOwner);
	}
}
