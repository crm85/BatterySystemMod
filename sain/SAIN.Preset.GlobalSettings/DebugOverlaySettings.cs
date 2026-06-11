namespace SAIN.Preset.GlobalSettings;

public class DebugOverlaySettings : SAINSettingsBase<DebugOverlaySettings>, ISAINSettings
{
	public bool Overlay_Info = true;

	public bool Overlay_Info_Expanded = false;

	public bool Overlay_Search = true;

	public bool Overlay_EnemyLists = false;

	public bool Overlay_EnemyInfo = true;

	public bool Overlay_EnemyInfo_Expanded = false;

	public bool Overlay_Decisions = false;

	public bool OverLay_AimInfo = false;

	public bool OverLay_AlwaysShowClosestHumanInfo = false;

	public bool OverLay_AlwaysShowMainPlayerInfo = false;
}
