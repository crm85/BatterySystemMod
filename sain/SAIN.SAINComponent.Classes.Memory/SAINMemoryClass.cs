using EFT;
using SAIN.Components;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Memory;

public class SAINMemoryClass : BotComponentClassBase
{
	private float _nextCheckDeadTime;

	public IPlayer LastUnderFireSource { get; private set; }

	public Enemy LastUnderFireEnemy { get; private set; }

	public Vector3 UnderFireFromPosition { get; set; }

	public SAINExtract Extract { get; } = new SAINExtract();

	public HealthTracker Health { get; private set; }

	public LocationTracker Location { get; private set; }

	public SAINMemoryClass(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
		Health = new HealthTracker(sain);
		Location = new LocationTracker(sain);
	}

	public override void Init()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved += clearEnemy;
		base.Init();
	}

	public override void ManualUpdate()
	{
		Health.ManualUpdate();
		Location.ManualUpdate();
		checkResetUnderFire();
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved -= clearEnemy;
		base.Dispose();
	}

	public void SetUnderFire(Enemy enemy, Vector3 position)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			base.BotOwner.Memory.SetUnderFire(enemy.EnemyIPlayer);
		}
		catch
		{
		}
		LastUnderFireSource = enemy.EnemyIPlayer;
		UnderFireFromPosition = position;
		LastUnderFireEnemy = enemy;
	}

	private void clearEnemy(string profileId, Enemy enemy)
	{
		if (LastUnderFireEnemy == enemy)
		{
			LastUnderFireEnemy = null;
			resetUnderFire();
		}
	}

	private void checkResetUnderFire()
	{
		if (_nextCheckDeadTime < Time.time)
		{
			_nextCheckDeadTime = Time.time + 0.5f;
			resetUnderFire();
		}
	}

	private void resetUnderFire()
	{
		if (base.BotOwner.Memory.IsUnderFire && (LastUnderFireSource == null || !LastUnderFireSource.HealthController.IsAlive))
		{
			base.BotOwner.Memory.float_4 = Time.time;
		}
	}
}
