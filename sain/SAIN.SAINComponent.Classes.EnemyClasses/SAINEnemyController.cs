using System.Collections.Generic;
using EFT;
using SAIN.Components;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class SAINEnemyController : BotComponentClassBase
{
	private readonly EnemyListController _listController;

	private readonly EnemyChooserClass _enemyChooser;

	private readonly EnemyUpdaterClass _enemyUpdater;

	private GameObject debugLastSeenPosition;

	private GameObject debugLastHeardPosition;

	public Dictionary<string, Enemy> Enemies => _listController.Enemies;

	public HashSet<Enemy> EnemiesArray => _listController.EnemiesArray;

	public EnemyControllerEvents Events { get; }

	public EnemyListsClass EnemyLists { get; }

	public Enemy GoalEnemy => _enemyChooser.GoalEnemy;

	public Enemy LastGoalEnemy => _enemyChooser.LastGoalEnemy;

	public bool AtPeace => Events.OnPeaceChanged.Value && Events.OnPeaceChanged.TimeSinceTrue > 1f;

	public bool ActiveHumanEnemy => Events.ActiveHumanEnemyEvent.Value;

	public bool HumanEnemyInLineofSight => Events.HumanInLineOfSightEvent.Value;

	public SAINEnemyController(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyBotActive;
		_listController = new EnemyListController(this);
		Events = new EnemyControllerEvents(this);
		AddSubClass(Events);
		EnemyLists = new EnemyListsClass(this);
		AddSubClass(EnemyLists);
		_enemyUpdater = new EnemyUpdaterClass(sain);
		AddSubClass(_enemyUpdater);
		_enemyChooser = new EnemyChooserClass(this);
		AddSubClass(_enemyChooser);
	}

	public override void Init()
	{
		_listController.Init();
		base.Init();
	}

	public override void ManualUpdate()
	{
		_listController.ManualUpdate();
		updateDebug();
		base.ManualUpdate();
	}

	public void LateUpdate()
	{
		_enemyUpdater.LateUpdate();
	}

	public override void Dispose()
	{
		_listController.Dispose();
		base.Dispose();
	}

	private void updateDebug()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		Enemy goalEnemy = GoalEnemy;
		if (goalEnemy != null)
		{
			if (SAINPlugin.DebugMode && SAINPlugin.DrawDebugGizmos)
			{
				if (goalEnemy.KnownPlaces.LastHeardPosition.HasValue)
				{
					if ((Object)(object)debugLastHeardPosition == (Object)null)
					{
						debugLastHeardPosition = DebugGizmos.Line(goalEnemy.KnownPlaces.LastHeardPosition.Value, base.Bot.Position, Color.yellow, 0.01f, Time.deltaTime, taperLine: true);
					}
					DebugGizmos.UpdatePositionLine(goalEnemy.KnownPlaces.LastHeardPosition.Value, base.Bot.Position, debugLastHeardPosition);
				}
				if (goalEnemy.KnownPlaces.LastSeenPosition.HasValue)
				{
					if ((Object)(object)debugLastSeenPosition == (Object)null)
					{
						debugLastSeenPosition = DebugGizmos.Line(goalEnemy.KnownPlaces.LastSeenPosition.Value, base.Bot.Position, Color.red, 0.01f, Time.deltaTime, taperLine: true);
					}
					DebugGizmos.UpdatePositionLine(goalEnemy.KnownPlaces.LastSeenPosition.Value, base.Bot.Position, debugLastSeenPosition);
				}
			}
			else if ((Object)(object)debugLastHeardPosition != (Object)null || (Object)(object)debugLastSeenPosition != (Object)null)
			{
				Object.Destroy((Object)(object)debugLastHeardPosition);
				Object.Destroy((Object)(object)debugLastSeenPosition);
			}
		}
		else if ((Object)(object)debugLastHeardPosition != (Object)null || (Object)(object)debugLastSeenPosition != (Object)null)
		{
			Object.Destroy((Object)(object)debugLastHeardPosition);
			Object.Destroy((Object)(object)debugLastSeenPosition);
		}
	}

	public void ClearEnemy()
	{
		_enemyChooser.ClearEnemy();
	}

	public Enemy GetEnemy(string profileID, bool mustBeActive)
	{
		return _listController.GetEnemy(profileID, mustBeActive);
	}

	public Enemy CheckAddEnemy(IPlayer IPlayer)
	{
		return _listController.CheckAddEnemy(IPlayer);
	}

	public void RemoveEnemy(string profileID)
	{
		_listController.RemoveEnemy(profileID);
	}

	public bool IsPlayerAnEnemy(string profileID)
	{
		return _listController.IsPlayerAnEnemy(profileID);
	}

	public bool IsPlayerFriendly(IPlayer iPlayer)
	{
		return _listController.IsPlayerFriendly(iPlayer);
	}

	public bool IsBotInBotsGroup(BotOwner botOwner)
	{
		return _listController.IsBotInBotsGroup(botOwner);
	}
}
