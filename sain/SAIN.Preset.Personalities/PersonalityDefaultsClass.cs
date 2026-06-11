using System.Collections.Generic;
using EFT;

namespace SAIN.Preset.Personalities;

public static class PersonalityDefaultsClass
{
	public static void InitDefaults(PersonalityDictionary Personalities, SAINPresetClass preset)
	{
		if (!Personalities.ContainsKey(EPersonality.Wreckless))
		{
			initWreckless(Personalities, preset);
		}
		if (!Personalities.ContainsKey(EPersonality.SnappingTurtle))
		{
			initSnappingTurtle(Personalities, preset);
		}
		if (!Personalities.ContainsKey(EPersonality.GigaChad))
		{
			initGigaChad(Personalities, preset);
		}
		if (!Personalities.ContainsKey(EPersonality.Chad))
		{
			initChad(Personalities, preset);
		}
		if (!Personalities.ContainsKey(EPersonality.Rat))
		{
			initRat(Personalities, preset);
		}
		if (!Personalities.ContainsKey(EPersonality.Timmy))
		{
			initTimmy(Personalities, preset);
		}
		if (!Personalities.ContainsKey(EPersonality.Coward))
		{
			initCoward(Personalities, preset);
		}
		if (!Personalities.ContainsKey(EPersonality.Normal))
		{
			initNormal(Personalities, preset);
		}
	}

	private static void initGigaChad(PersonalityDictionary Personalities, SAINPresetClass Preset)
	{
		EPersonality ePersonality = EPersonality.GigaChad;
		PersonalitySettingsClass personalitySettingsClass = new PersonalitySettingsClass(ePersonality);
		PersonalityAssignmentSettings assignment = personalitySettingsClass.Assignment;
		assignment.Enabled = true;
		assignment.RandomlyAssignedChance = 3f;
		assignment.CanBeRandomlyAssigned = true;
		assignment.MaxChanceIfMeetRequirements = 80f;
		assignment.MinLevel = 0f;
		assignment.MaxLevel = 100f;
		assignment.PowerLevelMin = 250f;
		assignment.PowerLevelMax = 1000f;
		assignment.PowerLevelScaleStart = 250f;
		assignment.PowerLevelScaleEnd = 500f;
		PersonalityBehaviorSettings behavior = personalitySettingsClass.Behavior;
		behavior.General.KickOpenAllDoors = true;
		behavior.General.AggressionMultiplier = 1f;
		behavior.General.HoldGroundBaseTime = 1.25f;
		behavior.General.HoldGroundMaxRandom = 1.5f;
		behavior.General.HoldGroundMinRandom = 0.65f;
		behavior.Cover.CanShiftCoverPosition = true;
		behavior.Cover.ShiftCoverTimeMultiplier = 0.5f;
		behavior.Cover.MoveToCoverHasEnemySpeed = 1f;
		behavior.Cover.MoveToCoverHasEnemyPose = 1f;
		behavior.Cover.MoveToCoverNoEnemySpeed = 1f;
		behavior.Cover.MoveToCoverNoEnemyPose = 1f;
		behavior.Talk.CanTaunt = true;
		behavior.Talk.CanRespondToEnemyVoice = true;
		behavior.Talk.TauntFrequency = 8f;
		behavior.Talk.TauntChance = 45f;
		behavior.Talk.TauntMaxDistance = 65f;
		behavior.Talk.ConstantTaunt = true;
		behavior.Talk.FrequentTaunt = true;
		behavior.Talk.CanFakeDeathRare = true;
		behavior.Talk.FakeDeathChance = 3f;
		behavior.Search.WillSearchForEnemy = true;
		behavior.Search.WillSearchFromAudio = true;
		behavior.Search.WillChaseDistantGunshots = true;
		behavior.Search.SearchBaseTime = 6f;
		behavior.Search.SprintWhileSearchChance = 75f;
		behavior.Search.SearchHasEnemySpeed = 1f;
		behavior.Search.SearchHasEnemyPose = 1f;
		behavior.Search.SearchNoEnemySpeed = 1f;
		behavior.Search.SearchNoEnemyPose = 1f;
		behavior.Search.SearchWaitMultiplier = 3f;
		behavior.Search.HeardFromPeaceBehavior = EHeardFromPeaceBehavior.SearchNow;
		behavior.Rush.CanRushEnemyReloadHeal = true;
		behavior.Rush.CanJumpCorners = true;
		behavior.Rush.JumpCornerChance = 40f;
		behavior.Rush.CanBunnyHop = true;
		behavior.Rush.BunnyHopChance = 5f;
		addPMCs(personalitySettingsClass.Assignment.AllowedTypes);
		Personalities.Add(ePersonality, personalitySettingsClass);
		if (Preset.Info.IsCustom)
		{
			SAINPresetClass.Export(personalitySettingsClass, Preset.Info.Name, ePersonality.ToString(), "Personalities");
		}
	}

	private static void initWreckless(PersonalityDictionary Personalities, SAINPresetClass Preset)
	{
		EPersonality ePersonality = EPersonality.Wreckless;
		PersonalitySettingsClass personalitySettingsClass = new PersonalitySettingsClass(ePersonality);
		PersonalityAssignmentSettings assignment = personalitySettingsClass.Assignment;
		assignment.Enabled = true;
		assignment.RandomlyAssignedChance = 1f;
		assignment.CanBeRandomlyAssigned = true;
		assignment.MaxChanceIfMeetRequirements = 5f;
		assignment.MinLevel = 0f;
		assignment.MaxLevel = 100f;
		assignment.PowerLevelMin = 250f;
		assignment.PowerLevelMax = 1000f;
		assignment.PowerLevelScaleStart = 250f;
		assignment.PowerLevelScaleEnd = 500f;
		PersonalityBehaviorSettings behavior = personalitySettingsClass.Behavior;
		behavior.General.KickOpenAllDoors = true;
		behavior.General.AggressionMultiplier = 1f;
		behavior.General.HoldGroundBaseTime = 2f;
		behavior.General.HoldGroundMaxRandom = 2.5f;
		behavior.General.HoldGroundMinRandom = 0.75f;
		behavior.Cover.CanShiftCoverPosition = true;
		behavior.Cover.ShiftCoverTimeMultiplier = 0.5f;
		behavior.Cover.MoveToCoverHasEnemySpeed = 1f;
		behavior.Cover.MoveToCoverHasEnemyPose = 1f;
		behavior.Cover.MoveToCoverNoEnemySpeed = 1f;
		behavior.Cover.MoveToCoverNoEnemyPose = 1f;
		behavior.Talk.CanTaunt = true;
		behavior.Talk.CanRespondToEnemyVoice = true;
		behavior.Talk.TauntFrequency = 4f;
		behavior.Talk.TauntChance = 33f;
		behavior.Talk.TauntMaxDistance = 75f;
		behavior.Talk.ConstantTaunt = true;
		behavior.Talk.FrequentTaunt = true;
		behavior.Talk.CanFakeDeathRare = true;
		behavior.Talk.FakeDeathChance = 6f;
		behavior.Search.WillSearchForEnemy = true;
		behavior.Search.WillSearchFromAudio = true;
		behavior.Search.WillChaseDistantGunshots = true;
		behavior.Search.SearchBaseTime = 0.1f;
		behavior.Search.SprintWhileSearchChance = 90f;
		behavior.Search.SearchHasEnemySpeed = 1f;
		behavior.Search.SearchHasEnemyPose = 1f;
		behavior.Search.SearchNoEnemySpeed = 1f;
		behavior.Search.SearchNoEnemyPose = 1f;
		behavior.Search.SearchWaitMultiplier = 1f;
		behavior.Search.HeardFromPeaceBehavior = EHeardFromPeaceBehavior.Charge;
		behavior.Rush.CanRushEnemyReloadHeal = true;
		behavior.Rush.CanJumpCorners = true;
		behavior.Rush.JumpCornerChance = 60f;
		behavior.Rush.CanBunnyHop = true;
		behavior.Rush.BunnyHopChance = 10f;
		addAllTypes(personalitySettingsClass.Assignment.AllowedTypes);
		Personalities.Add(ePersonality, personalitySettingsClass);
		if (Preset.Info.IsCustom)
		{
			SAINPresetClass.Export(personalitySettingsClass, Preset.Info.Name, ePersonality.ToString(), "Personalities");
		}
	}

	private static void initSnappingTurtle(PersonalityDictionary Personalities, SAINPresetClass Preset)
	{
		EPersonality ePersonality = EPersonality.SnappingTurtle;
		PersonalitySettingsClass personalitySettingsClass = new PersonalitySettingsClass(ePersonality);
		PersonalityAssignmentSettings assignment = personalitySettingsClass.Assignment;
		assignment.Enabled = true;
		assignment.RandomlyAssignedChance = 1f;
		assignment.CanBeRandomlyAssigned = true;
		assignment.MaxChanceIfMeetRequirements = 30f;
		assignment.MinLevel = 15f;
		assignment.MaxLevel = 100f;
		assignment.PowerLevelMin = 150f;
		assignment.PowerLevelMax = 1000f;
		assignment.PowerLevelScaleStart = 150f;
		assignment.PowerLevelScaleEnd = 500f;
		PersonalityBehaviorSettings behavior = personalitySettingsClass.Behavior;
		behavior.General.AggressionMultiplier = 1f;
		behavior.General.HoldGroundBaseTime = 1.5f;
		behavior.General.HoldGroundMaxRandom = 1.2f;
		behavior.General.HoldGroundMinRandom = 0.8f;
		behavior.Cover.CanShiftCoverPosition = true;
		behavior.Cover.ShiftCoverTimeMultiplier = 2f;
		behavior.Cover.MoveToCoverHasEnemySpeed = 1f;
		behavior.Cover.MoveToCoverHasEnemyPose = 1f;
		behavior.Cover.MoveToCoverNoEnemySpeed = 1f;
		behavior.Cover.MoveToCoverNoEnemyPose = 1f;
		behavior.Talk.CanTaunt = true;
		behavior.Talk.CanRespondToEnemyVoice = false;
		behavior.Talk.TauntFrequency = 15f;
		behavior.Talk.TauntMaxDistance = 70f;
		behavior.Talk.ConstantTaunt = false;
		behavior.Talk.FrequentTaunt = false;
		behavior.Talk.CanFakeDeathRare = true;
		behavior.Talk.FakeDeathChance = 10f;
		behavior.Search.WillSearchForEnemy = true;
		behavior.Search.WillSearchFromAudio = true;
		behavior.Search.WillChaseDistantGunshots = false;
		behavior.Search.SearchBaseTime = 90f;
		behavior.Search.SprintWhileSearchChance = 40f;
		behavior.Search.Sneaky = true;
		behavior.Search.SneakyPose = 1f;
		behavior.Search.SneakySpeed = 0.33f;
		behavior.Search.SearchHasEnemySpeed = 1f;
		behavior.Search.SearchHasEnemyPose = 1f;
		behavior.Search.SearchNoEnemySpeed = 1f;
		behavior.Search.SearchNoEnemyPose = 1f;
		behavior.Search.SearchWaitMultiplier = 3f;
		behavior.Search.HeardFromPeaceBehavior = EHeardFromPeaceBehavior.Freeze;
		behavior.Rush.CanRushEnemyReloadHeal = true;
		behavior.Rush.CanJumpCorners = true;
		behavior.Rush.JumpCornerChance = 100f;
		behavior.Rush.CanBunnyHop = true;
		behavior.Rush.BunnyHopChance = 20f;
		addPMCs(personalitySettingsClass.Assignment.AllowedTypes);
		Personalities.Add(ePersonality, personalitySettingsClass);
		if (Preset.Info.IsCustom)
		{
			SAINPresetClass.Export(personalitySettingsClass, Preset.Info.Name, ePersonality.ToString(), "Personalities");
		}
	}

	private static void initChad(PersonalityDictionary Personalities, SAINPresetClass Preset)
	{
		EPersonality ePersonality = EPersonality.Chad;
		PersonalitySettingsClass personalitySettingsClass = new PersonalitySettingsClass(ePersonality);
		PersonalityAssignmentSettings assignment = personalitySettingsClass.Assignment;
		assignment.Enabled = true;
		assignment.RandomlyAssignedChance = 8f;
		assignment.CanBeRandomlyAssigned = true;
		assignment.MaxChanceIfMeetRequirements = 80f;
		assignment.MinLevel = 0f;
		assignment.MaxLevel = 100f;
		assignment.PowerLevelMin = 100f;
		assignment.PowerLevelMax = 1000f;
		assignment.PowerLevelScaleStart = 100f;
		assignment.PowerLevelScaleEnd = 400f;
		PersonalityBehaviorSettings behavior = personalitySettingsClass.Behavior;
		behavior.General.AggressionMultiplier = 1f;
		behavior.General.HoldGroundBaseTime = 1.5f;
		behavior.General.HoldGroundMaxRandom = 1.5f;
		behavior.General.HoldGroundMinRandom = 0.75f;
		behavior.Cover.CanShiftCoverPosition = true;
		behavior.Cover.ShiftCoverTimeMultiplier = 1f;
		behavior.Cover.MoveToCoverHasEnemySpeed = 1f;
		behavior.Cover.MoveToCoverHasEnemyPose = 1f;
		behavior.Cover.MoveToCoverNoEnemySpeed = 1f;
		behavior.Cover.MoveToCoverNoEnemyPose = 1f;
		behavior.Talk.CanTaunt = true;
		behavior.Talk.CanRespondToEnemyVoice = false;
		behavior.Talk.TauntFrequency = 20f;
		behavior.Talk.TauntChance = 60f;
		behavior.Talk.TauntMaxDistance = 50f;
		behavior.Talk.FrequentTaunt = true;
		behavior.Talk.ConstantTaunt = false;
		behavior.Talk.CanFakeDeathRare = false;
		behavior.Talk.FakeDeathChance = 0f;
		behavior.Search.WillSearchForEnemy = true;
		behavior.Search.WillSearchFromAudio = true;
		behavior.Search.WillChaseDistantGunshots = true;
		behavior.Search.SearchBaseTime = 16f;
		behavior.Search.SprintWhileSearchChance = 60f;
		behavior.Search.Sneaky = false;
		behavior.Search.SneakyPose = 0f;
		behavior.Search.SneakySpeed = 0f;
		behavior.Search.SearchHasEnemySpeed = 1f;
		behavior.Search.SearchHasEnemyPose = 1f;
		behavior.Search.SearchNoEnemySpeed = 1f;
		behavior.Search.SearchNoEnemyPose = 1f;
		behavior.Search.SearchWaitMultiplier = 1f;
		behavior.Search.HeardFromPeaceBehavior = EHeardFromPeaceBehavior.Freeze;
		behavior.Rush.CanRushEnemyReloadHeal = true;
		behavior.Rush.CanJumpCorners = false;
		behavior.Rush.JumpCornerChance = 0f;
		behavior.Rush.CanBunnyHop = false;
		behavior.Rush.BunnyHopChance = 0f;
		addPMCs(personalitySettingsClass.Assignment.AllowedTypes);
		Personalities.Add(ePersonality, personalitySettingsClass);
		if (Preset.Info.IsCustom)
		{
			SAINPresetClass.Export(personalitySettingsClass, Preset.Info.Name, ePersonality.ToString(), "Personalities");
		}
	}

	private static void initRat(PersonalityDictionary Personalities, SAINPresetClass Preset)
	{
		EPersonality ePersonality = EPersonality.Rat;
		PersonalitySettingsClass personalitySettingsClass = new PersonalitySettingsClass(ePersonality);
		PersonalityAssignmentSettings assignment = personalitySettingsClass.Assignment;
		assignment.Enabled = true;
		assignment.RandomlyAssignedChance = 10f;
		assignment.CanBeRandomlyAssigned = true;
		assignment.MaxChanceIfMeetRequirements = 60f;
		assignment.MinLevel = 0f;
		assignment.MaxLevel = 100f;
		assignment.PowerLevelMin = 0f;
		assignment.PowerLevelMax = 200f;
		assignment.PowerLevelScaleStart = 0f;
		assignment.PowerLevelScaleEnd = 200f;
		assignment.InverseScale = true;
		PersonalityBehaviorSettings behavior = personalitySettingsClass.Behavior;
		behavior.General.AggressionMultiplier = 1f;
		behavior.General.HoldGroundBaseTime = 1f;
		behavior.General.HoldGroundMaxRandom = 1.5f;
		behavior.General.HoldGroundMinRandom = 0.75f;
		behavior.Cover.CanShiftCoverPosition = false;
		behavior.Cover.ShiftCoverTimeMultiplier = 1f;
		behavior.Cover.MoveToCoverHasEnemySpeed = 0.5f;
		behavior.Cover.MoveToCoverHasEnemyPose = 0.5f;
		behavior.Cover.MoveToCoverNoEnemySpeed = 0.5f;
		behavior.Cover.MoveToCoverNoEnemyPose = 1f;
		behavior.Talk.CanTaunt = false;
		behavior.Talk.CanRespondToEnemyVoice = false;
		behavior.Talk.TauntFrequency = 10f;
		behavior.Talk.TauntChance = 0f;
		behavior.Talk.TauntMaxDistance = 70f;
		behavior.Talk.FrequentTaunt = false;
		behavior.Talk.ConstantTaunt = false;
		behavior.Talk.CanFakeDeathRare = false;
		behavior.Talk.FakeDeathChance = 0f;
		behavior.Search.WillSearchForEnemy = true;
		behavior.Search.WillSearchFromAudio = true;
		behavior.Search.WillChaseDistantGunshots = false;
		behavior.Search.SearchBaseTime = 240f;
		behavior.Search.SprintWhileSearchChance = 0f;
		behavior.Search.Sneaky = true;
		behavior.Search.SneakyPose = 0f;
		behavior.Search.SneakySpeed = 0f;
		behavior.Search.SearchHasEnemySpeed = 0f;
		behavior.Search.SearchHasEnemyPose = 0f;
		behavior.Search.SearchNoEnemySpeed = 0f;
		behavior.Search.SearchNoEnemyPose = 1f;
		behavior.Search.SearchWaitMultiplier = 1f;
		behavior.Search.HeardFromPeaceBehavior = EHeardFromPeaceBehavior.Freeze;
		behavior.Rush.CanRushEnemyReloadHeal = false;
		behavior.Rush.CanJumpCorners = false;
		behavior.Rush.JumpCornerChance = 0f;
		behavior.Rush.CanBunnyHop = false;
		behavior.Rush.BunnyHopChance = 0f;
		List<WildSpawnType> allowedTypes = personalitySettingsClass.Assignment.AllowedTypes;
		addAllTypes(allowedTypes);
		allowedTypes.Remove((WildSpawnType)34);
		allowedTypes.Remove((WildSpawnType)24);
		allowedTypes.Remove((WildSpawnType)9);
		Personalities.Add(ePersonality, personalitySettingsClass);
		if (Preset.Info.IsCustom)
		{
			SAINPresetClass.Export(personalitySettingsClass, Preset.Info.Name, ePersonality.ToString(), "Personalities");
		}
	}

	private static void initTimmy(PersonalityDictionary Personalities, SAINPresetClass Preset)
	{
		EPersonality ePersonality = EPersonality.Timmy;
		PersonalitySettingsClass personalitySettingsClass = new PersonalitySettingsClass(ePersonality);
		PersonalityAssignmentSettings assignment = personalitySettingsClass.Assignment;
		assignment.Enabled = true;
		assignment.RandomlyAssignedChance = 0f;
		assignment.CanBeRandomlyAssigned = false;
		assignment.MaxChanceIfMeetRequirements = 60f;
		assignment.MinLevel = 0f;
		assignment.MaxLevel = 15f;
		assignment.PowerLevelMin = 0f;
		assignment.PowerLevelMax = 150f;
		assignment.PowerLevelScaleStart = 0f;
		assignment.PowerLevelScaleEnd = 150f;
		assignment.InverseScale = true;
		PersonalityBehaviorSettings behavior = personalitySettingsClass.Behavior;
		behavior.General.AggressionMultiplier = 1f;
		behavior.General.HoldGroundBaseTime = 0.5f;
		behavior.General.HoldGroundMaxRandom = 1.5f;
		behavior.General.HoldGroundMinRandom = 0.75f;
		behavior.Cover.CanShiftCoverPosition = false;
		behavior.Cover.ShiftCoverTimeMultiplier = 0.5f;
		behavior.Cover.MoveToCoverHasEnemySpeed = 0.5f;
		behavior.Cover.MoveToCoverHasEnemyPose = 0.5f;
		behavior.Cover.MoveToCoverNoEnemySpeed = 0.5f;
		behavior.Cover.MoveToCoverNoEnemyPose = 1f;
		behavior.Talk.CanTaunt = false;
		behavior.Talk.CanRespondToEnemyVoice = false;
		behavior.Talk.TauntFrequency = 10f;
		behavior.Talk.TauntMaxDistance = 70f;
		behavior.Talk.FrequentTaunt = false;
		behavior.Talk.ConstantTaunt = false;
		behavior.Talk.CanFakeDeathRare = false;
		behavior.Talk.FakeDeathChance = 0f;
		behavior.Talk.CanBegForLife = true;
		behavior.Search.WillSearchForEnemy = true;
		behavior.Search.WillSearchFromAudio = false;
		behavior.Search.WillChaseDistantGunshots = false;
		behavior.Search.SearchBaseTime = 90f;
		behavior.Search.SprintWhileSearchChance = 20f;
		behavior.Search.Sneaky = false;
		behavior.Search.SneakyPose = 0f;
		behavior.Search.SneakySpeed = 0f;
		behavior.Search.SearchHasEnemySpeed = 0f;
		behavior.Search.SearchHasEnemyPose = 1f;
		behavior.Search.SearchNoEnemySpeed = 0f;
		behavior.Search.SearchNoEnemyPose = 1f;
		behavior.Search.SearchWaitMultiplier = 0.5f;
		behavior.Search.HeardFromPeaceBehavior = EHeardFromPeaceBehavior.Freeze;
		behavior.Rush.CanRushEnemyReloadHeal = false;
		behavior.Rush.CanJumpCorners = false;
		behavior.Rush.JumpCornerChance = 0f;
		behavior.Rush.CanBunnyHop = false;
		behavior.Rush.BunnyHopChance = 0f;
		List<WildSpawnType> allowedTypes = personalitySettingsClass.Assignment.AllowedTypes;
		addAllTypes(allowedTypes);
		allowedTypes.Remove((WildSpawnType)34);
		allowedTypes.Remove((WildSpawnType)24);
		allowedTypes.Remove((WildSpawnType)9);
		Personalities.Add(ePersonality, personalitySettingsClass);
		if (Preset.Info.IsCustom)
		{
			SAINPresetClass.Export(personalitySettingsClass, Preset.Info.Name, ePersonality.ToString(), "Personalities");
		}
	}

	private static void initCoward(PersonalityDictionary Personalities, SAINPresetClass Preset)
	{
		EPersonality ePersonality = EPersonality.Coward;
		PersonalitySettingsClass personalitySettingsClass = new PersonalitySettingsClass(ePersonality);
		PersonalityAssignmentSettings assignment = personalitySettingsClass.Assignment;
		assignment.Enabled = true;
		assignment.RandomlyAssignedChance = 5f;
		assignment.CanBeRandomlyAssigned = true;
		assignment.MaxChanceIfMeetRequirements = 30f;
		assignment.MinLevel = 0f;
		assignment.MaxLevel = 100f;
		assignment.PowerLevelMin = 0f;
		assignment.PowerLevelMax = 250f;
		assignment.PowerLevelScaleStart = 0f;
		assignment.PowerLevelScaleEnd = 250f;
		assignment.InverseScale = true;
		PersonalityBehaviorSettings behavior = personalitySettingsClass.Behavior;
		behavior.General.AggressionMultiplier = 1f;
		behavior.General.HoldGroundBaseTime = 0.25f;
		behavior.General.HoldGroundMaxRandom = 1.5f;
		behavior.General.HoldGroundMinRandom = 0.75f;
		behavior.Cover.CanShiftCoverPosition = false;
		behavior.Cover.MoveToCoverHasEnemySpeed = 0.5f;
		behavior.Cover.MoveToCoverHasEnemyPose = 0.5f;
		behavior.Cover.MoveToCoverNoEnemySpeed = 0.5f;
		behavior.Cover.MoveToCoverNoEnemyPose = 1f;
		behavior.Talk.CanBegForLife = true;
		behavior.Search.WillSearchForEnemy = false;
		behavior.Search.WillSearchFromAudio = false;
		behavior.Search.WillChaseDistantGunshots = false;
		behavior.Search.HeardFromPeaceBehavior = EHeardFromPeaceBehavior.Freeze;
		List<WildSpawnType> allowedTypes = personalitySettingsClass.Assignment.AllowedTypes;
		addAllTypes(allowedTypes);
		allowedTypes.Remove((WildSpawnType)34);
		allowedTypes.Remove((WildSpawnType)24);
		allowedTypes.Remove((WildSpawnType)9);
		Personalities.Add(ePersonality, personalitySettingsClass);
		if (Preset.Info.IsCustom)
		{
			SAINPresetClass.Export(personalitySettingsClass, Preset.Info.Name, ePersonality.ToString(), "Personalities");
		}
	}

	private static void initNormal(PersonalityDictionary Personalities, SAINPresetClass Preset)
	{
		EPersonality ePersonality = EPersonality.Normal;
		PersonalitySettingsClass personalitySettingsClass = new PersonalitySettingsClass(ePersonality);
		PersonalityAssignmentSettings assignment = personalitySettingsClass.Assignment;
		assignment.Enabled = true;
		assignment.RandomlyAssignedChance = 0f;
		assignment.CanBeRandomlyAssigned = false;
		assignment.MaxChanceIfMeetRequirements = 50f;
		assignment.MinLevel = 0f;
		assignment.MaxLevel = 100f;
		assignment.PowerLevelMin = 0f;
		assignment.PowerLevelMax = 1000f;
		assignment.PowerLevelScaleStart = 0f;
		assignment.PowerLevelScaleEnd = 1000f;
		assignment.InverseScale = true;
		PersonalityBehaviorSettings behavior = personalitySettingsClass.Behavior;
		behavior.General.AggressionMultiplier = 1f;
		behavior.General.HoldGroundBaseTime = 1f;
		behavior.General.HoldGroundMaxRandom = 1.5f;
		behavior.General.HoldGroundMinRandom = 0.5f;
		behavior.Cover.CanShiftCoverPosition = true;
		behavior.Cover.ShiftCoverTimeMultiplier = 1f;
		behavior.Cover.MoveToCoverHasEnemySpeed = 0.75f;
		behavior.Cover.MoveToCoverHasEnemyPose = 1f;
		behavior.Cover.MoveToCoverNoEnemySpeed = 0.75f;
		behavior.Cover.MoveToCoverNoEnemyPose = 1f;
		behavior.Talk.CanRespondToEnemyVoice = true;
		behavior.Talk.TauntFrequency = 10f;
		behavior.Talk.TauntMaxDistance = 50f;
		behavior.Search.WillSearchForEnemy = true;
		behavior.Search.WillSearchFromAudio = true;
		behavior.Search.WillChaseDistantGunshots = false;
		behavior.Search.SearchBaseTime = 60f;
		behavior.Search.SprintWhileSearchChance = 10f;
		behavior.Search.SearchHasEnemySpeed = 1f;
		behavior.Search.SearchHasEnemyPose = 1f;
		behavior.Search.SearchNoEnemySpeed = 1f;
		behavior.Search.SearchNoEnemyPose = 1f;
		behavior.Search.SearchWaitMultiplier = 1f;
		behavior.Search.HeardFromPeaceBehavior = EHeardFromPeaceBehavior.Freeze;
		List<WildSpawnType> allowedTypes = personalitySettingsClass.Assignment.AllowedTypes;
		addAllTypes(allowedTypes);
		Personalities.Add(ePersonality, personalitySettingsClass);
		if (Preset.Info.IsCustom)
		{
			SAINPresetClass.Export(personalitySettingsClass, Preset.Info.Name, ePersonality.ToString(), "Personalities");
		}
	}

	private static void addPMCs(List<WildSpawnType> allowedTypes)
	{
		allowedTypes.Add((WildSpawnType)52);
		allowedTypes.Add((WildSpawnType)51);
	}

	private static void addAllTypes(List<WildSpawnType> allowedTypes)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		allowedTypes.Clear();
		foreach (KeyValuePair<WildSpawnType, BotType> botType in BotTypeDefinitions.BotTypes)
		{
			allowedTypes.Add(botType.Key);
		}
	}
}
