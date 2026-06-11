using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Helpers;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Layers.Combat.Solo;

public class StandAndShootAction : CombatAction, ISAINAction
{
	private bool shallMoveShoot = false;

	private bool shallResume = false;

	public StandAndShootAction(BotOwner bot)
		: base(bot, "StandAndShootAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		base.Shoot.ShootAnyVisibleEnemies(base.Bot.Enemy);
		base.Bot.Steering.SteerByPriority(base.Bot.Enemy);
		if (!shallMoveShoot)
		{
			base.Bot.Mover.Pose.SetPoseToCover();
		}
		EndProfilingSample();
	}

	public override void Start()
	{
		Toggle(value: true);
		shallMoveShoot = moveShoot();
		if (!shallMoveShoot)
		{
			base.Bot.Mover.StopMove();
			((CustomLogic)this).BotOwner.Mover.SprintPause(0.5f);
			shallResume = base.Bot.Decision.CurrentCombatDecision == ECombatDecision.ShootDistantEnemy;
		}
		base.Bot.Mover.Lean.HoldLean(0.75f);
	}

	private bool moveShoot()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (base.Bot.Player.IsInPronePose)
		{
			return false;
		}
		if (base.Bot.Enemy != null && base.Bot.Enemy.RealDistance < 50f)
		{
			float num = Random.Range(70, 110);
			if (EFTMath.RandomBool())
			{
				num *= -1f;
			}
			Vector3 enemyDirection = base.Bot.Enemy.EnemyDirection;
			Vector3 normalized = ((Vector3)(ref enemyDirection)).normalized;
			Vector3 val = Vector.Rotate(normalized, 0f, num, 0f);
			val.y = 0f;
			val *= 6f;
			NavMeshHit val2 = default(NavMeshHit);
			NavMeshHit val3 = default(NavMeshHit);
			if (NavMesh.SamplePosition(base.Bot.Position + val, ref val2, 5f, -1) && NavMesh.SamplePosition(base.Bot.Position, ref val3, 0.5f, -1))
			{
				Vector3 position = ((NavMeshHit)(ref val2)).position;
				NavMeshHit val4 = default(NavMeshHit);
				if (NavMesh.Raycast(((NavMeshHit)(ref val3)).position, ((NavMeshHit)(ref val2)).position, ref val4, -1))
				{
					position = ((NavMeshHit)(ref val4)).position;
				}
				bool calculating;
				return base.Bot.Mover.GoToPoint(position, out calculating, -1f, crawl: false, slowAtEnd: false);
			}
		}
		return false;
	}

	public override void Stop()
	{
		Toggle(value: false);
		if (shallResume)
		{
			((CustomLogic)this).BotOwner.Mover.MovementResume();
		}
	}
}
