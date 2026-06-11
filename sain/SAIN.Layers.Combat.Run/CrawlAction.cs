using System.Text;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Layers.Combat.Run;

internal class CrawlAction : CombatAction, ISAINAction
{
	private Vector3 _runDestination;

	private float nextRandomRunTime;

	public CrawlAction(BotOwner bot)
		: base(bot, "CrawlAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		base.Bot.Mover.SetTargetMoveSpeed(1f);
		if (nextRandomRunTime > Time.time)
		{
			Vector3 val = _runDestination - base.Bot.Position;
			if (((Vector3)(ref val)).sqrMagnitude < 1f)
			{
				nextRandomRunTime = 0f;
			}
		}
		if (nextRandomRunTime < Time.time && FindRandomPlace(out var _) && base.Bot.Mover.GoToPoint(_runDestination, out var _, -1f, crawl: true))
		{
			nextRandomRunTime = Time.time + 20f;
		}
	}

	public override void Start()
	{
		Toggle(value: true);
	}

	private bool FindRandomPlace(out NavMeshPath path)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		NavMeshHit val2 = default(NavMeshHit);
		for (int i = 0; i < 10; i++)
		{
			Vector3 val = Random.onUnitSphere * 100f;
			if (NavMesh.SamplePosition(val + base.Bot.Position, ref val2, 10f, -1))
			{
				path = new NavMeshPath();
				if (NavMesh.CalculatePath(base.Bot.Position, ((NavMeshHit)(ref val2)).position, -1, path))
				{
					_runDestination = path.corners[path.corners.Length - 1];
					return true;
				}
			}
		}
		path = null;
		return false;
	}

	public override void Stop()
	{
		Toggle(value: false);
	}

	public override void BuildDebugText(StringBuilder stringBuilder)
	{
	}
}
