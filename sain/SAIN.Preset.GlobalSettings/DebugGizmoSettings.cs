using Newtonsoft.Json;
using SAIN.Attributes;

namespace SAIN.Preset.GlobalSettings;

public class DebugGizmoSettings : SAINSettingsBase<DebugGizmoSettings>, ISAINSettings
{
	[Name("Draw Debug Gizmos")]
	public bool DrawDebugGizmos;

	[Name("Draw Transform Gizmos")]
	public bool DrawTransformGizmos;

	[Name("Draw Line of Sight Checks")]
	public bool DrawLineOfSightGizmos;

	[Name("Draw Volumetric Light Gizmos")]
	public bool DrawLightGizmos;

	[Name("Draw Door Links")]
	public bool DrawDoorLinks;

	[Name("Draw Recoil Gizmos")]
	public bool DebugDrawRecoilGizmos = false;

	[Name("Draw Aim Gizmos")]
	public bool DebugDrawAimGizmos = false;

	[Name("Draw Blind Corner Raycasts")]
	public bool DebugDrawBlindCorner = false;

	[Name("Draw Debug Suppression Points")]
	[Hidden]
	public bool DebugDrawProjectionPoints = false;

	[Name("Draw Search Peek Start and End Gizmos")]
	public bool DebugSearchGizmos = false;

	[Name("Draw Debug Path Safety Tester")]
	[Hidden]
	[JsonIgnore]
	public bool DebugDrawSafePaths = false;

	[Name("Path Safety Tester")]
	[Hidden]
	[JsonIgnore]
	public bool DebugEnablePathTester = false;

	[Hidden]
	[JsonIgnore]
	public bool DebugMovementPlan = false;
}
