using System.Collections.Generic;
using Comfort.Common;
using EFT;
using SAIN.Components;
using SAIN.Helpers.Events;
using SAIN.Models.Enums;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SAINActivationClass : BotComponentClassBase
{
	private const float ACTIVATE_STANDBY_HUMAN = 150f;

	private const float ACTIVATE_STANDBY_AI = 50f;

	private const float ACTIVATE_STANDBY_CHECK_FREQ = 3f;

	private float _nextCheckEnemiesTime;

	public ESAINLayer ActiveLayer { get; private set; }

	public bool BotActive => BotActiveToggle.Value;

	public ToggleEvent BotActiveToggle { get; } = new ToggleEvent();

	public bool BotInStandBy => BotStandByToggle.Value;

	public ToggleEvent BotStandByToggle { get; } = new ToggleEvent();

	public bool GameEnding => GameEndingToggle.Value;

	public ToggleEvent GameEndingToggle { get; } = new ToggleEvent();

	public bool SAINLayersActive => SAINLayersActiveToggle.Value;

	public ToggleEvent SAINLayersActiveToggle { get; } = new ToggleEvent();

	public bool BotInCombat => BotInCombatToggle.Value;

	public ToggleEvent BotInCombatToggle { get; } = new ToggleEvent();

	public SAINActivationClass(BotComponent botComponent)
		: base(botComponent)
	{
	}

	public void SetActive(bool botActive)
	{
		BotActiveToggle.CheckToggle(botActive);
		if (!botActive)
		{
			BotStandByToggle.CheckToggle(value: true);
			ActiveLayer = ESAINLayer.None;
			SAINLayersActiveToggle.CheckToggle(value: false);
		}
	}

	public void SetInCombat(bool inCombat)
	{
		BotInCombatToggle.CheckToggle(!inCombat);
	}

	public void SetActiveLayer(ESAINLayer layer)
	{
		ActiveLayer = layer;
	}

	public override void ManualUpdate()
	{
		CheckGameEnding();
		CheckBotActive();
		CheckStandBy();
		SAINLayersActiveToggle.CheckToggle(ActiveLayer != ESAINLayer.None);
	}

	private void CheckBotActive()
	{
		if (GameEnding && BotActive)
		{
			SetActive(botActive: false);
		}
		if (!GameEnding && !BotActive && base.Bot.Person.ActivationClass.BotActive)
		{
			Logger.LogWarning("Bot not active but should be!");
			SetActive(botActive: true);
		}
	}

	private void CheckStandBy()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		BotOwner botOwner = base.BotOwner;
		int num;
		if (botOwner == null)
		{
			num = 1;
		}
		else
		{
			BotStandBy standBy = botOwner.StandBy;
			num = (((int)((standBy != null) ? new BotStandByType?(standBy.StandByType) : ((BotStandByType?)null)).GetValueOrDefault() != 3) ? 1 : 0);
		}
		bool flag = (byte)num != 0;
		if (flag && BotActive)
		{
			if (base.Bot.HasEnemy)
			{
				base.BotOwner.StandBy.Activate();
				flag = false;
			}
			else if (CheckAllEnemies())
			{
				Logger.LogDebug("[" + ((Object)base.BotOwner).name + "] disabled standby due to enemies being near.");
				base.BotOwner.StandBy.Activate();
				flag = false;
			}
		}
		BotStandByToggle.CheckToggle(flag);
	}

	private bool CheckAllEnemies()
	{
		if (_nextCheckEnemiesTime > Time.time)
		{
			return false;
		}
		_nextCheckEnemiesTime = Time.time + 3f;
		Dictionary<string, Enemy>.ValueCollection values = base.Bot.EnemyController.Enemies.Values;
		foreach (Enemy item in values)
		{
			if (item != null && (item.InLineOfSight || (item.IsAI && item.RealDistance < 50f) || (!item.IsAI && item.RealDistance < 150f)))
			{
				return true;
			}
		}
		return false;
	}

	private void CheckGameEnding()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		IBotGame instance = Singleton<IBotGame>.Instance;
		bool value = instance == null || (int)instance.Status == 5;
		GameEndingToggle.CheckToggle(value);
	}

	public override void Init()
	{
		SetActive(botActive: true);
		base.Bot.Person.ActivationClass.OnBotActiveChanged += SetActive;
		base.Init();
	}

	public override void Dispose()
	{
		SetActive(botActive: false);
		base.Bot.Person.ActivationClass.OnBotActiveChanged -= SetActive;
		base.Dispose();
	}
}
