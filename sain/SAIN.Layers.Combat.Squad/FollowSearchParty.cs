using DrakiaXYZ.BigBrain.Brains;
using EFT;
using SAIN.Components;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Mover;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Layers.Combat.Squad;

internal class FollowSearchParty : CombatAction, ISAINAction
{
	private float _nextUpdatePosTime;

	private Vector3 _LastLeadPos;

	private Enemy _enemy;

	public FollowSearchParty(BotOwner bot)
		: base(bot, "FollowSearchParty")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
		base.Bot.Search.ToggleSearch(value: true, _enemy);
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		if (!base.Bot.Mover.PathFollower.Running)
		{
			base.Shoot.ShootAnyVisibleEnemies(_enemy);
			if (!base.Bot.Steering.SteerByPriority(_enemy, lookRandom: false))
			{
				base.Bot.Steering.LookToLastKnownEnemyPosition(_enemy ?? base.Bot.Enemy);
			}
		}
		if (_nextUpdatePosTime < Time.time)
		{
			MoveToLead(out var nextUpdateTime);
			_nextUpdatePosTime = Time.time + nextUpdateTime;
		}
		EndProfilingSample();
	}

	private void MoveToLead(out float nextUpdateTime)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		BotComponent botComponent = base.Bot.Squad.SquadInfo?.LeaderComponent;
		if ((Object)(object)botComponent == (Object)null)
		{
			nextUpdateTime = 1f;
			return;
		}
		Vector3 val = _LastLeadPos - botComponent.Position;
		if (((Vector3)(ref val)).sqrMagnitude < 1f)
		{
			nextUpdateTime = 1f;
			return;
		}
		Vector3? posNearLead = GetPosNearLead(botComponent.Position);
		if (!posNearLead.HasValue)
		{
			nextUpdateTime = 0.25f;
			return;
		}
		_LastLeadPos = botComponent.Position;
		val = posNearLead.Value - base.Bot.Position;
		float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
		if (sqrMagnitude < 1f)
		{
			nextUpdateTime = 1f;
			return;
		}
		if (sqrMagnitude > 400f && base.Bot.Mover.RunToPoint(posNearLead.Value, ESprintUrgency.Middle, stopSprintEnemyVisible: true))
		{
			nextUpdateTime = 2f;
			return;
		}
		if (base.Bot.Mover.PathFollower.Running)
		{
			nextUpdateTime = 2f;
			return;
		}
		nextUpdateTime = 1f;
		base.Bot.Mover.GoToPoint(posNearLead.Value, out var _);
	}

	private Vector3? GetPosNearLead(Vector3 leadPos)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Vector3? result = null;
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(leadPos, ref val, 3f, -1))
		{
			Vector3 val2 = base.Bot.Position - ((NavMeshHit)(ref val)).position;
			val2.y = 0f;
			val2 = ((Vector3)(ref val2)).normalized * 2f;
			NavMeshHit val3 = default(NavMeshHit);
			result = ((!NavMesh.Raycast(((NavMeshHit)(ref val)).position, val2 + ((NavMeshHit)(ref val)).position, ref val3, -1)) ? new Vector3?(val2 + ((NavMeshHit)(ref val)).position) : new Vector3?(((NavMeshHit)(ref val3)).position));
		}
		return result;
	}

	public override void Start()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		_enemy = base.Bot.Enemy;
		Toggle(value: true);
		_nextUpdatePosTime = 0f;
		_LastLeadPos = Vector3.zero;
	}

	public override void Stop()
	{
		Toggle(value: false);
		_enemy = null;
		base.Bot.Mover.PathFollower.Cancel(0.25f);
	}
}
