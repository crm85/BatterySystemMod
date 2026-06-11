using System;
using EFT;
using SAIN.Helpers.Events;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public class EnemyControllerEvents : BotSubClass<SAINEnemyController>, IBotClass, IDisposable
{
	public ToggleEventTimeTracked OnPeaceChanged { get; } = new ToggleEventTimeTracked(defaultValue: true);

	public ToggleEvent ActiveHumanEnemyEvent { get; } = new ToggleEvent();

	public ToggleEvent HumanInLineOfSightEvent { get; } = new ToggleEvent();

	public event Action<Enemy> OnEnemyHit;

	public event Action<Enemy> OnEnemyAdded;

	public event Action<string, Enemy> OnEnemyRemoved;

	public event Action<Player> OnEnemyKilled;

	public event Action<Enemy, SAINSoundType, bool, EnemyPlace> OnEnemyHeard;

	public event Action<bool, Enemy> OnEnemyKnownChanged;

	public event Action<Enemy, Enemy> OnEnemyChanged;

	public event Action<ETagStatus, Enemy> OnEnemyHealthChanged;

	public EnemyControllerEvents(SAINEnemyController controller)
		: base(controller)
	{
	}

	public override void Init()
	{
		EnemyList enemyList = base.BaseClass.EnemyLists.GetEnemyList(EEnemyListType.Known);
		enemyList.OnListEmptyOrGetFirst += OnPeaceChanged.CheckToggle;
		enemyList.OnListEmptyOrGetFirstHuman += ActiveHumanEnemyEvent.CheckToggle;
		EnemyList enemyList2 = base.BaseClass.EnemyLists.GetEnemyList(EEnemyListType.InLineOfSight);
		enemyList2.OnListEmptyOrGetFirstHuman += HumanInLineOfSightEvent.CheckToggle;
		base.Init();
	}

	public override void Dispose()
	{
		EnemyListsClass enemyLists = base.BaseClass.EnemyLists;
		if (enemyLists != null)
		{
			EnemyList enemyList = enemyLists.GetEnemyList(EEnemyListType.Known);
			if (enemyList != null)
			{
				enemyList.OnListEmptyOrGetFirst -= OnPeaceChanged.CheckToggle;
				enemyList.OnListEmptyOrGetFirstHuman -= ActiveHumanEnemyEvent.CheckToggle;
			}
			EnemyList enemyList2 = enemyLists.GetEnemyList(EEnemyListType.InLineOfSight);
			if (enemyList2 != null)
			{
				enemyList2.OnListEmptyOrGetFirstHuman -= HumanInLineOfSightEvent.CheckToggle;
			}
		}
		base.Dispose();
	}

	private void enemyHealthChanged(Enemy enemy, ETagStatus health)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		this.OnEnemyHealthChanged?.Invoke(health, enemy);
	}

	public void EnemyChanged(Enemy enemy, Enemy lastEnemy)
	{
		this.OnEnemyChanged?.Invoke(enemy, lastEnemy);
	}

	public void EnemyAdded(Enemy enemy)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		enemy.EnemyPlayer.OnPlayerDead += new GDelegate69(enemyKilled);
		enemy.Events.OnEnemyHeard += enemyHeard;
		EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = enemy.Events.OnEnemyKnownChanged;
		onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Combine(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(enemyKnownChanged));
		enemy.Events.OnEnemyShot += enemyHit;
		enemy.Events.OnHealthStatusChanged += enemyHealthChanged;
		this.OnEnemyAdded?.Invoke(enemy);
	}

	public void EnemyRemoved(string profileID, Enemy enemy)
	{
		if (enemy != null)
		{
			enemy.Events.OnEnemyHeard -= enemyHeard;
			EnemyEvents.EnemyToggleEventTimeTracked onEnemyKnownChanged = enemy.Events.OnEnemyKnownChanged;
			onEnemyKnownChanged.OnToggle = (Action<bool, Enemy>)Delegate.Remove(onEnemyKnownChanged.OnToggle, new Action<bool, Enemy>(enemyKnownChanged));
			enemy.Events.OnEnemyShot -= enemyHit;
			enemy.Events.OnHealthStatusChanged -= enemyHealthChanged;
		}
		this.OnEnemyRemoved?.Invoke(profileID, enemy);
	}

	private void enemyKnownChanged(bool value, Enemy enemy)
	{
		this.OnEnemyKnownChanged?.Invoke(value, enemy);
	}

	private void enemyHit(Enemy enemy)
	{
		this.OnEnemyHit?.Invoke(enemy);
	}

	private void enemyKilled(Player player, IPlayer lastAggressor, DamageInfoStruct lastDamageInfoStruct, EBodyPart lastBodyPart)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		if ((Object)(object)player != (Object)null)
		{
			player.OnPlayerDead -= new GDelegate69(enemyKilled);
			if (lastAggressor != null && lastAggressor.ProfileId == base.Bot.ProfileId)
			{
				this.OnEnemyKilled?.Invoke(player);
			}
		}
	}

	private void enemyHeard(Enemy enemy, SAINSoundType soundType, bool isDanger, EnemyPlace place)
	{
		this.OnEnemyHeard?.Invoke(enemy, soundType, isDanger, place);
	}
}
