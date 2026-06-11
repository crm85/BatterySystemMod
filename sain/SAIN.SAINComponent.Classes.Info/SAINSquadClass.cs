using System.Collections.Generic;
using SAIN.BotController.Classes;
using SAIN.Components;
using SAIN.Components.PlayerComponentSpace;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Info;

public class SAINSquadClass : BotComponentClassBase
{
	public readonly List<BotComponent> VisibleMembers = new List<BotComponent>();

	private float _updateMemberTime = 0f;

	private bool _humanFriendclose;

	private float _nextCheckhumantime;

	public Squad SquadInfo { get; private set; }

	public float DistanceToSquadLeader { get; private set; }

	public bool IAmLeader => SquadInfo.LeaderId == base.Bot.ProfileId;

	public BotComponent LeaderComponent => SquadInfo?.LeaderComponent;

	public bool BotInGroup => base.BotOwner.BotsGroup.MembersCount > 1 || HumanFriendClose;

	public Dictionary<string, BotComponent> Members => SquadInfo?.Members;

	public bool HumanFriendClose
	{
		get
		{
			if (_nextCheckhumantime < Time.time)
			{
				_nextCheckhumantime = Time.time + 3f;
				_humanFriendclose = humanFriendClose(2500f);
			}
			return _humanFriendclose;
		}
	}

	public SAINSquadClass(BotComponent bot)
		: base(bot)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
		getSquad();
	}

	public void RemoveFromSquad()
	{
		SquadInfo = null;
		getSquad();
	}

	private void getSquad()
	{
		SquadInfo = BotManagerComponent.Instance.BotSquads.GetSquad(base.Bot.Person.AIInfo.BotOwner);
	}

	private bool humanFriendClose(float distToCheck)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		foreach (PlayerComponent value in GameWorldComponent.Instance.PlayerTracker.AlivePlayersDictionary.Values)
		{
			if (!((Object)(object)value != (Object)null) || value.IsAI)
			{
				continue;
			}
			BotComponent bot = base.Bot;
			if (bot != null && bot.EnemyController?.IsPlayerAnEnemy(value.ProfileId) == false)
			{
				Vector3 val = value.Position - base.Bot.Position;
				if (((Vector3)(ref val)).sqrMagnitude < distToCheck)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void ManualUpdate()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (base.BotOwner.BotsGroup.MembersCount > 1 && SquadInfo != null && _updateMemberTime < Time.time)
		{
			_updateMemberTime = Time.time + 0.5f;
			checkVisibleMembers();
			if (!IAmLeader && (Object)(object)LeaderComponent != (Object)null)
			{
				Vector3 val = base.Bot.Position - LeaderComponent.Position;
				DistanceToSquadLeader = ((Vector3)(ref val)).magnitude;
			}
		}
		base.ManualUpdate();
	}

	private void checkVisibleMembers()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		VisibleMembers.Clear();
		Vector3 eyePosition = base.Bot.Transform.EyePosition;
		foreach (BotComponent value in Members.Values)
		{
			if ((Object)(object)value != (Object)null && value.ProfileId != base.Bot.ProfileId)
			{
				Vector3 val = value.Transform.BodyPosition - eyePosition;
				float magnitude = ((Vector3)(ref val)).magnitude;
				if (!(magnitude > 100f) && !Physics.Raycast(eyePosition, val, magnitude, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
				{
					VisibleMembers.Add(value);
				}
			}
		}
	}
}
