using EFT;
using SAIN.Components;
using SAIN.SAINComponent.Classes.EnemyClasses;

namespace SAIN.SAINComponent.Classes;

public class BotHitByEnemyClass : BotBase
{
	public Enemy EnemyWhoLastShotMe { get; private set; }

	public BotHitByEnemyClass(BotComponent bot)
		: base(bot)
	{
		base.CanEverTick = false;
	}

	public override void Init()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved += clearEnemy;
		base.Init();
	}

	public void GetHit(DamageInfoStruct DamageInfoStruct, EBodyPart bodyPart, float floatVal)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		IPlayerOwner player = DamageInfoStruct.Player;
		IPlayer val = ((player != null) ? player.iPlayer : null);
		if (val != null)
		{
			Enemy enemy = base.Bot.EnemyController.GetEnemy(val.ProfileId, mustBeActive: true);
			if (enemy != null)
			{
				EnemyWhoLastShotMe = enemy;
				enemy.Status.GetHit(DamageInfoStruct);
			}
		}
	}

	private void clearEnemy(string profileId, Enemy enemy)
	{
		if (enemy == EnemyWhoLastShotMe)
		{
			EnemyWhoLastShotMe = null;
		}
	}

	public override void Dispose()
	{
		base.Bot.EnemyController.Events.OnEnemyRemoved -= clearEnemy;
		base.Dispose();
	}
}
