using System.Collections.Generic;

namespace SAIN.Preset.GlobalSettings;

public class DebugSettings : SAINSettingsBase<DebugSettings>, ISAINSettings
{
	public DebugLogSettings Logs = new DebugLogSettings();

	public DebugGizmoSettings Gizmos = new DebugGizmoSettings();

	public DebugOverlaySettings Overlay = new DebugOverlaySettings();

	public override void Init(List<ISAINSettings> list)
	{
		list.Add(Logs);
		list.Add(Gizmos);
		list.Add(Overlay);
	}
}
