using System;
using SAIN.Helpers.Events;
using SAIN.SAINComponent.Classes.EnemyClasses;

namespace SAIN.Components.BotComponentSpace.Classes.EnemyClasses;

public class EnemyKnownChecker : EnemyBase, IBotClass, IDisposable
{
	private const float LAST_KNOWN_TIME_UPDATE_UPPER_LIMIT = 400f;

	public EnemyKnownChecker(Enemy enemy)
		: base(enemy)
	{
	}

	public override void Init()
	{
		ToggleEvent botActiveToggle = base.Bot.BotActivation.BotActiveToggle;
		botActiveToggle.OnToggle = (Action<bool>)Delegate.Combine(botActiveToggle.OnToggle, new Action<bool>(BotStateChanged));
		base.Init();
	}

	public override void ManualUpdate()
	{
		bool enemyKnown = ShallKnowEnemy();
		SetEnemyKnown(enemyKnown);
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		ToggleEvent botActiveToggle = base.Bot.BotActivation.BotActiveToggle;
		botActiveToggle.OnToggle = (Action<bool>)Delegate.Remove(botActiveToggle.OnToggle, new Action<bool>(BotStateChanged));
		base.Dispose();
	}

	private void BotStateChanged(bool botActive)
	{
		if (!botActive)
		{
			SetEnemyKnown(enemyKnown: false);
		}
	}

	public void SetEnemyKnown(bool enemyKnown)
	{
		base.Enemy.Events.OnEnemyKnownChanged.CheckToggle(enemyKnown);
	}

	private bool ShallKnowEnemy()
	{
		if (!base.Enemy.CheckValid())
		{
			return false;
		}
		if (!base.EnemyPlayerComponent.IsActive)
		{
			return false;
		}
		if (!base.Enemy.LastKnownPosition.HasValue)
		{
			return false;
		}
		float timeSinceLastKnownUpdated = base.Enemy.KnownPlaces.TimeSinceLastKnownUpdated;
		if (timeSinceLastKnownUpdated > 400f)
		{
			return false;
		}
		if (timeSinceLastKnownUpdated <= base.Bot.Info.ForgetEnemyTime)
		{
			return true;
		}
		if (BotIsSearchingForMe())
		{
			return true;
		}
		return false;
	}

	public bool BotIsSearchingForMe()
	{
		if (!IsBotSearching())
		{
			return false;
		}
		if (base.Enemy.Events.OnSearch.Value)
		{
			return !base.Enemy.KnownPlaces.SearchedAllKnownLocations;
		}
		return false;
	}

	private bool IsBotSearching()
	{
		if (base.Bot.Decision.CurrentCombatDecision == ECombatDecision.Search)
		{
			return true;
		}
		ESquadDecision currentSquadDecision = base.Bot.Decision.CurrentSquadDecision;
		if (currentSquadDecision == ESquadDecision.Search || currentSquadDecision == ESquadDecision.GroupSearch)
		{
			return true;
		}
		return false;
	}
}
