using System.Collections.Generic;
using SAIN.Models.Enums;

namespace SAIN.Components.BotController.PeacefulActions;

public class PeacefulActionSet : Dictionary<EPeacefulAction, IPeacefulActionController>
{
	public void CheckExecute(Dictionary<string, BotZoneData> datas)
	{
		foreach (IPeacefulActionController value in base.Values)
		{
			foreach (BotZoneData value2 in datas.Values)
			{
				value.CheckExecute(value2);
			}
		}
	}
}
