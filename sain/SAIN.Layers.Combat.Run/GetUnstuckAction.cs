using System.Collections.Generic;
using System.Text;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Classes.Coverfinder;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Layers.Combat.Run;

internal class GetUnstuckAction : CombatAction
{
	public GetUnstuckAction(BotOwner bot)
		: base(bot, "GetUnstuckAction")
	{
	}

	public override void Update(ActionData data)
	{
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		StartProfilingSample("Update");
		base.Bot.Mover.SetTargetPose(1f);
		base.Bot.Mover.SetTargetMoveSpeed(1f);
		base.Bot.Steering.LookToMovingDirection();
		Vector3? val = null;
		List<CoverPoint> coverPoints = base.Bot.Cover.CoverPoints;
		if (coverPoints.Count > 0)
		{
			for (int i = 0; i < coverPoints.Count; i++)
			{
				CoverPoint coverPoint = coverPoints[i];
				NavMeshPath val2 = new NavMeshPath();
				if (NavMesh.CalculatePath(coverPoint.Position, base.Bot.Position, -1, val2))
				{
					val = val2.corners[val2.corners.Length - 1];
					break;
				}
			}
		}
		if (val.HasValue)
		{
			((CustomLogic)this).BotOwner.Mover.GoToByWay((Vector3[])(object)new Vector3[2]
			{
				base.Bot.Position,
				val.Value
			}, -1f);
		}
		EndProfilingSample();
	}

	public override void Start()
	{
		base.Bot.Mover.StopMove();
	}

	public override void Stop()
	{
	}

	public override void BuildDebugText(StringBuilder stringBuilder)
	{
	}
}
