using Newtonsoft.Json;
using SAIN.Attributes;

namespace SAIN.Preset.GlobalSettings;

public class DebugLogSettings : SAINSettingsBase<DebugLogSettings>, ISAINSettings
{
	[Name("Global Debug Mode")]
	public bool GlobalDebugMode;

	[Name("Global Performance Profiling Mode")]
	[Description("Enables function sampling for Unity Profiling.")]
	public bool GlobalProfilingToggle;

	[Name("Test Bot Sprint Pathfinder")]
	public bool ForceBotsToRunAround;

	[Name("Test Bot Crawling")]
	public bool ForceBotsToTryCrawl;

	[Name("Test Grenade Throw")]
	public bool TestGrenadeThrow;

	[Name("Draw Debug Labels")]
	public bool DrawDebugLabels;

	[Name("Debug External")]
	public bool DebugExternal;

	[Name("Debug Recoil Calculations")]
	public bool DebugRecoilCalculations = false;

	[Name("Debug Aim Calculations")]
	public bool DebugAimCalculations = false;

	[Name("Debug Hearing Calc Results")]
	public bool DebugHearing = false;

	[Name("Debug Extracts")]
	public bool DebugExtract = false;

	[Name("Collect and Export Bot Layer and Brain Info")]
	[Hidden]
	[JsonIgnore]
	public bool CollectBotLayerBrainInfo = false;
}
