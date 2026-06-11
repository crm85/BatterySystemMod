using System.Collections;
using System.Collections.Generic;
using System.Text;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.Components.BotController.PeacefulActions;

public class BotGatheringController : BotManagerBase, IPeacefulActionController
{
	private const float FIND_GATHERING_FREQ = 10f;

	private const float MAX_GATHER_ENTER_DIST = 40f;

	private const float MAX_GATHER_ENTER_DIST_SQR = 1600f;

	private readonly List<BotComponent> _localList = new List<BotComponent>();

	private readonly List<BotComponent> _selectedBots = new List<BotComponent>();

	private float _nextCheckTime;

	private int _gatherings;

	public EPeacefulAction Action { get; }

	public List<IPeacefulActionExecutor> ActiveActions { get; } = new List<IPeacefulActionExecutor>();

	public bool Active => Count > 0;

	public int Count => ActiveActions.Count;

	public void CheckExecute(BotZoneData data)
	{
		if (_nextCheckTime < Time.time)
		{
			_nextCheckTime = Time.time + 10f;
			findPossibleGatherings(data);
		}
	}

	public BotGatheringController(BotManagerComponent controller, EPeacefulAction action)
		: base(controller)
	{
		Action = action;
	}

	private void findPossibleGatherings(BotZoneData data)
	{
		_selectedBots.Clear();
		if (PeacefulActionHelpers.findBotsForPeacefulAction(data, _localList, _selectedBots, 1600f))
		{
			logConvoStart(data);
		}
	}

	private void selectBotsForConvo(List<BotComponent> list, BotComponent bot)
	{
	}

	private IEnumerator executeConversation(params BotComponent[] bots)
	{
		if (false || !recheckBots(bots))
		{
		}
		yield return null;
	}

	private bool recheckBots(params BotComponent[] bots)
	{
		foreach (BotComponent botComponent in bots)
		{
			if ((Object)(object)botComponent == (Object)null || botComponent.HasEnemy)
			{
				return false;
			}
		}
		return true;
	}

	private void logConvoStart(BotZoneData data)
	{
		if (SAINPlugin.DebugMode || true)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"Gathering [{_gatherings++}]");
			stringBuilder.AppendLine($"Selected [{_selectedBots.Count}] Bots");
			stringBuilder.AppendLine("Name: [" + data.Name + "]");
			stringBuilder.AppendLine($"Time: [{Time.time}]");
			for (int i = 0; i < _selectedBots.Count; i++)
			{
				BotComponent botComponent = _selectedBots[i];
				stringBuilder.AppendLine($"[{i + 1}] : Selected: [{((Object)botComponent).name}]");
			}
			Logger.LogDebug(stringBuilder.ToString());
		}
	}
}
