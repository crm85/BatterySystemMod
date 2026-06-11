using System.Collections.Generic;
using SAIN.Models.Enums;

namespace SAIN.Components.BotController.PeacefulActions;

public interface IPeacefulActionController
{
	bool Active { get; }

	int Count { get; }

	EPeacefulAction Action { get; }

	List<IPeacefulActionExecutor> ActiveActions { get; }

	void CheckExecute(BotZoneData data);
}
