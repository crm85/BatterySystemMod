using SAIN.Components;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class BodyPartHitEffectClass : BotBase
{
	private float _updateHealthTime;

	public EInjurySeverity LeftArmInjury { get; private set; }

	public EInjurySeverity RightArmInjury { get; private set; }

	public EHitReaction HitReaction { get; private set; }

	public BodyPartHitEffectClass(BotComponent bot)
		: base(bot)
	{
	}

	public override void ManualUpdate()
	{
		if (_updateHealthTime < Time.time)
		{
			checkArmInjuries();
		}
	}

	private void checkArmInjuries()
	{
		_updateHealthTime = Time.time + 1f;
		LeftArmInjury = base.Bot.Medical.HitReaction.BodyParts[(EBodyPart)3].InjurySeverity;
		RightArmInjury = base.Bot.Medical.HitReaction.BodyParts[(EBodyPart)4].InjurySeverity;
	}

	public void GetHit(DamageInfoStruct DamageInfoStruct, EBodyPart bodyPart, float floatVal)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected I4, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		switch ((int)bodyPart)
		{
		case 0:
			GetHitInHead(DamageInfoStruct);
			break;
		case 1:
		case 2:
			GetHitInCenter(DamageInfoStruct);
			break;
		case 5:
		case 6:
			GetHitInLegs(DamageInfoStruct);
			break;
		default:
			GetHitInArms(DamageInfoStruct);
			break;
		}
	}

	private void GetHitInLegs(DamageInfoStruct DamageInfoStruct)
	{
		HitReaction = EHitReaction.Legs;
	}

	private void GetHitInArms(DamageInfoStruct DamageInfoStruct)
	{
		HitReaction = EHitReaction.Arms;
		checkArmInjuries();
	}

	private void GetHitInCenter(DamageInfoStruct DamageInfoStruct)
	{
		HitReaction = EHitReaction.Center;
	}

	private void GetHitInHead(DamageInfoStruct DamageInfoStruct)
	{
		HitReaction = EHitReaction.Head;
	}
}
