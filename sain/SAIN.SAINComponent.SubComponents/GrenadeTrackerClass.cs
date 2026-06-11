using EFT;
using SAIN.Components;
using UnityEngine;

namespace SAIN.SAINComponent.SubComponents;

public class GrenadeTrackerClass
{
	private readonly BotComponent Bot;

	private bool _sentToBot;

	private bool _updated;

	private readonly float ReactionTime;

	private float _nextCheckRaycastTime;

	private BotOwner BotOwner => Bot.BotOwner;

	public float GrenadeDistance { get; private set; }

	private float _timeSpotted { get; set; }

	public float TimeSinceSpotted => _spotted ? (Time.time - _timeSpotted) : 0f;

	public Grenade Grenade { get; private set; }

	public Vector3 DangerPoint { get; set; }

	private bool _spotted { get; set; }

	public bool CanReact => _spotted && TimeSinceSpotted > ReactionTime;

	public GrenadeTrackerClass(BotComponent bot, Grenade grenade, Vector3 dangerPoint, float reactionTime)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Bot = bot;
		ReactionTime = reactionTime;
		DangerPoint = dangerPoint;
		Grenade = grenade;
		Vector3 val = ((Component)grenade).transform.position - bot.Position;
		if (((Vector3)(ref val)).magnitude < 10f)
		{
			setSpotted();
		}
	}

	public void CheckHeardGrenadeCollision(float maxRange)
	{
		if (!_spotted)
		{
			maxRange *= 0.75f;
			if (GrenadeDistance < maxRange)
			{
				setSpotted();
			}
		}
	}

	public void Update()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Invalid comparison between Unknown and I4
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)BotOwner == (Object)null || BotOwner.IsDead || (Object)(object)Grenade == (Object)null || _sentToBot)
		{
			return;
		}
		if (!_sentToBot && CanReact)
		{
			_sentToBot = true;
			CollisionSounds collisionSound = Grenade.GrenadeSettings.CollisionSound;
			EPhraseTrigger phrase = (EPhraseTrigger)(((int)collisionSound == 0) ? 11 : 35);
			Bot.Talk.GroupSay(phrase, (ETagStatus)4, withGroupDelay: false, 70f);
			Vector3 dangerPoint = DangerPoint;
			BotOwner.BewareGrenade.AddGrenadeDanger(dangerPoint, Grenade);
		}
		else
		{
			if (_spotted)
			{
				return;
			}
			Vector3 val = ((Component)Grenade).transform.position - BotOwner.Position;
			GrenadeDistance = ((Vector3)(ref val)).magnitude;
			if (GrenadeDistance < 3f)
			{
				setSpotted();
			}
			else if (_nextCheckRaycastTime < Time.time)
			{
				_nextCheckRaycastTime = Time.time + 0.05f;
				if (checkVisibility())
				{
					setSpotted();
				}
			}
		}
	}

	private void setSpotted()
	{
		if (!_spotted)
		{
			_timeSpotted = Time.time;
			_spotted = true;
		}
	}

	private bool checkVisibility()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)Grenade).transform.position + Vector3.up * 0.05f;
		if (!BotOwner.LookSensor.IsPointInVisibleSector(val))
		{
			return false;
		}
		Vector3 headPoint = BotOwner.LookSensor._headPoint;
		Vector3 val2 = val - headPoint;
		return !Physics.Raycast(headPoint, ((Vector3)(ref val2)).normalized, ((Vector3)(ref val2)).magnitude, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMaskAI));
	}

	public void UpdateGrenadeDanger(Vector3 Danger)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		DangerPoint = Danger;
		if (_sentToBot && !_updated)
		{
			_updated = true;
			BotOwner.BewareGrenade.AddGrenadeDanger(Danger, Grenade);
		}
	}
}
