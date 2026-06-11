using System.Collections.Generic;
using System.Text;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.Components.BotController.PeacefulActions;

public class BotPeacefulActionController : BotManagerBase, IBotControllerClass
{
	private float _nextLogTime;

	public PeacefulBotFinder PeacefulBotFinder { get; }

	public PeacefulActionSet Actions { get; } = new PeacefulActionSet();

	public BotPeacefulActionController(BotManagerComponent controller)
		: base(controller)
	{
	}

	public void Init()
	{
	}

	private void initActions()
	{
		Actions.Add(EPeacefulAction.Gathering, new BotGatheringController(base.BotController, EPeacefulAction.Gathering));
		Actions.Add(EPeacefulAction.Conversation, new BotConversationController(base.BotController, EPeacefulAction.Conversation));
	}

	public void Update()
	{
	}

	public void Dispose()
	{
	}

	private void logDatas()
	{
		if (!(_nextLogTime < Time.time))
		{
			return;
		}
		_nextLogTime = Time.time + 10f;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, BotZoneData> zoneData in PeacefulBotFinder.ZoneDatas)
		{
			stringBuilder.AppendLine(zoneData.Key + " : [" + zoneData.Value.AllContainedBots.Count + "] : [" + zoneData.Value.AllPeacefulBots.Count + "]");
		}
		Logger.LogDebug(stringBuilder.ToString());
	}
}
