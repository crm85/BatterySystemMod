using SAIN.Attributes;
using SAIN.Preset.GlobalSettings;

namespace SAIN.Preset.Personalities;

public class PersonalitySearchSettings : SAINSettingsBase<PersonalitySearchSettings>, ISAINSettings
{
	[Advanced]
	public bool WillSearchForEnemy = true;

	[Advanced]
	public bool WillSearchFromAudio = true;

	[Name("Heard From Peace Behavior")]
	[Description("When a bot hears an enemy, and was previously at peace, so had no enemy and was in patrol, what is their reaction?")]
	public EHeardFromPeaceBehavior HeardFromPeaceBehavior = EHeardFromPeaceBehavior.Freeze;

	[Description("Will this personality type hear and chase after distant gunshots if they aren't fired at them?")]
	public bool WillChaseDistantGunshots = true;

	[Description("If a sound is further than this, it will be considered chasing a gunshot sound, and will be ignored if WillChaseDistantGunshots is set to off, unless the gunshot is fired at them.")]
	public float AudioStraightDistanceToIgnore = 125f;

	[Name("Start Search Base Time")]
	[Description("The base time, before modifiers, that a personality will usually start searching for their enemy.")]
	[MinMax(0.1f, 500f, 100f)]
	public float SearchBaseTime = 40f;

	[Name("Search Wait Multiplier")]
	[Description("Linearly increases or decreases the time a bot pauses while searching.")]
	[MinMax(0.01f, 5f, 100f)]
	public float SearchWaitMultiplier = 1f;

	[Percentage(0f, 100f, 1f)]
	public float SprintWhileSearchChance = 25f;

	[Advanced]
	public bool Sneaky = false;

	[Percentage0to1(0f, 1f, 100f)]
	[Advanced]
	public float SneakySpeed = 1f;

	[Percentage0to1(0f, 1f, 100f)]
	[Advanced]
	public float SneakyPose = 1f;

	[Percentage0to1(0f, 1f, 100f)]
	[Advanced]
	public float SearchNoEnemySpeed = 1f;

	[Percentage0to1(0f, 1f, 100f)]
	[Advanced]
	public float SearchNoEnemyPose = 1f;

	[Percentage0to1(0f, 1f, 100f)]
	[Advanced]
	public float SearchHasEnemySpeed = 1f;

	[Percentage0to1(0f, 1f, 100f)]
	[Advanced]
	public float SearchHasEnemyPose = 1f;
}
