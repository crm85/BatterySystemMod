using System.Collections.Generic;
using EFT.HealthSystem;
using SAIN.Components;
using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SAINBotHitReaction : BotBase
{
	private const float StunDamageThreshold = 50f;

	private const float BaseStunTime = 3f;

	private float TimeStunHappened;

	private float StunTime;

	private bool _isStunned;

	public Dictionary<EBodyPart, BodyPartStatus> BodyParts = new Dictionary<EBodyPart, BodyPartStatus>();

	public EHitReaction HitReaction { get; private set; }

	public IHealthController HealthController => base.Player.HealthController;

	public BodyPartHitEffectClass BodyHitEffect { get; private set; }

	public AimHitEffectClass AimHitEffect { get; private set; }

	public EInjurySeverity LeftArmInjury { get; private set; }

	public EInjurySeverity RightArmInjury { get; private set; }

	public bool ArmsInjured => BodyHitEffect.LeftArmInjury != EInjurySeverity.None || BodyHitEffect.RightArmInjury != EInjurySeverity.None;

	public bool IsStunned
	{
		get
		{
			if (_isStunned && StunTime < Time.time)
			{
				_isStunned = false;
			}
			return _isStunned;
		}
		set
		{
			if (value)
			{
				TimeStunHappened = Time.time;
				StunTime = Time.time + 3f * Random.Range(0.75f, 1.25f);
			}
			_isStunned = value;
		}
	}

	public SAINBotHitReaction(BotComponent bot)
		: base(bot)
	{
		BodyHitEffect = new BodyPartHitEffectClass(bot);
		AimHitEffect = new AimHitEffectClass(bot);
		addPart((EBodyPart)0);
		addPart((EBodyPart)1);
		addPart((EBodyPart)3);
		addPart((EBodyPart)4);
		addPart((EBodyPart)5);
		addPart((EBodyPart)6);
		addPart((EBodyPart)2);
	}

	private void addPart(EBodyPart part)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		BodyParts.Add(part, new BodyPartStatus(part, this));
	}

	public override void ManualUpdate()
	{
		BodyHitEffect.ManualUpdate();
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		BodyHitEffect.Dispose();
		BodyParts.Clear();
		AimHitEffect.Dispose();
		base.Dispose();
	}

	public void GetHit(DamageInfoStruct DamageInfoStruct, EBodyPart bodyPart, float floatVal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		BodyHitEffect.GetHit(DamageInfoStruct, bodyPart, floatVal);
		AimHitEffect.GetHit(DamageInfoStruct);
	}

	private bool IsStunnedFromDamage(DamageInfoStruct DamageInfoStruct)
	{
		return false;
	}
}
