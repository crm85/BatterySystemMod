using System.Collections.Generic;
using SAIN.Models.Enums;
using SAIN.Preset.GlobalSettings;

namespace SAIN.SAINComponent.Classes.WeaponFunction;

public static class SuppressionHelpers
{
	public static ESuppressionState FindActiveState(float suppNum, out SuppressionConfig suppressionConfig)
	{
		Dictionary<ESuppressionState, SuppressionConfig> sUPPRESSION_STATES = GlobalSettingsClass.Instance.Mind.SUPPRESSION_STATES;
		ESuppressionState eSuppressionState = ESuppressionState.Extreme;
		if (sUPPRESSION_STATES.TryGetValue(eSuppressionState, out suppressionConfig) && suppressionConfig.IsActive(suppNum))
		{
			return eSuppressionState;
		}
		eSuppressionState = ESuppressionState.Heavy;
		if (sUPPRESSION_STATES.TryGetValue(eSuppressionState, out suppressionConfig) && suppressionConfig.IsActive(suppNum))
		{
			return eSuppressionState;
		}
		eSuppressionState = ESuppressionState.Medium;
		if (sUPPRESSION_STATES.TryGetValue(eSuppressionState, out suppressionConfig) && suppressionConfig.IsActive(suppNum))
		{
			return eSuppressionState;
		}
		eSuppressionState = ESuppressionState.Light;
		if (sUPPRESSION_STATES.TryGetValue(eSuppressionState, out suppressionConfig) && suppressionConfig.IsActive(suppNum))
		{
			return eSuppressionState;
		}
		return ESuppressionState.None;
	}
}
