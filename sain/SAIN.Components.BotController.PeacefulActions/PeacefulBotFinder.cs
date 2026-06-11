using System.Collections.Generic;
using UnityEngine;

namespace SAIN.Components.BotController.PeacefulActions;

public class PeacefulBotFinder : BotManagerBase, IBotControllerClass
{
	public Dictionary<string, BotZoneData> ZoneDatas = new Dictionary<string, BotZoneData>();

	public PeacefulBotFinder(BotManagerComponent controller)
		: base(controller)
	{
	}

	public void Init()
	{
		base.BotController.BotSpawnController.OnBotAdded += botAdded;
		base.BotController.BotSpawnController.OnBotRemoved += botRemoved;
	}

	public void Update()
	{
	}

	public void Dispose()
	{
		base.BotController.BotSpawnController.OnBotAdded -= botAdded;
		base.BotController.BotSpawnController.OnBotRemoved -= botRemoved;
	}

	private void botAdded(BotComponent bot)
	{
		BotZone botZone = bot.BotOwner.BotsGroup.BotZone;
		if ((Object)(object)botZone == (Object)null)
		{
			Logger.LogWarning("Null BotZone for [" + ((Object)bot.BotOwner).name + "]");
			return;
		}
		if (!ZoneDatas.TryGetValue(botZone.NameZone, out var value))
		{
			value = new BotZoneData(botZone);
			ZoneDatas.Add(value.Name, value);
		}
		value.AddBot(bot);
	}

	private void botRemoved(BotComponent bot)
	{
		if ((Object)(object)bot == (Object)null)
		{
			Logger.LogWarning("Null BotComponent");
			return;
		}
		if ((Object)(object)bot.BotOwner == (Object)null)
		{
			Logger.LogWarning("Null BotOwner [" + bot.Info.Profile.Name + "]");
			return;
		}
		if (bot.BotOwner.BotsGroup == null)
		{
			Logger.LogWarning("Null BotGroup [" + bot.Info.Profile.Name + "]");
			return;
		}
		BotZone botZone = bot.BotOwner.BotsGroup.BotZone;
		BotZoneData value;
		if ((Object)(object)botZone == (Object)null)
		{
			Logger.LogWarning("Null BotZone for [" + ((Object)bot.BotOwner).name + "]");
		}
		else if (ZoneDatas.TryGetValue(botZone.NameZone, out value))
		{
			value.RemoveBot(bot);
		}
	}
}
