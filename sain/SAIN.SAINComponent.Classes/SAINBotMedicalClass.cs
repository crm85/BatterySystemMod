using SAIN.Components;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SAINBotMedicalClass : BotComponentClassBase
{
	private float _nextCancelTime;

	private float _cancelFreq = 1f;

	public BotSurgery Surgery { get; private set; }

	public SAINBotHitReaction HitReaction { get; private set; }

	public BotHitByEnemyClass HitByEnemy { get; private set; }

	public float TimeLastShot { get; private set; }

	public float TimeSinceShot => Time.time - TimeLastShot;

	public SAINBotMedicalClass(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
		Surgery = new BotSurgery(sain);
		HitReaction = new SAINBotHitReaction(sain);
		HitByEnemy = new BotHitByEnemyClass(sain);
	}

	public void TryCancelHeal()
	{
		if (!(_nextCancelTime < Time.time))
		{
			return;
		}
		_nextCancelTime = Time.time + _cancelFreq;
		BotMedecine medecine = base.BotOwner.Medecine;
		if (medecine != null)
		{
			GClass475 stimulators = medecine.Stimulators;
			if (stimulators != null)
			{
				((GClass468)stimulators).CancelCurrent();
			}
		}
		BotMedecine medecine2 = base.BotOwner.Medecine;
		if (medecine2 != null)
		{
			BotFirstAidClass firstAid = medecine2.FirstAid;
			if (firstAid != null)
			{
				((GClass468)firstAid).CancelCurrent();
			}
		}
	}

	public override void Init()
	{
		base.Player.BeingHitAction += GetHit;
		Surgery.Init();
		HitReaction.Init();
		HitByEnemy.Init();
		base.Init();
	}

	public override void ManualUpdate()
	{
		Surgery.ManualUpdate();
		HitReaction.ManualUpdate();
		HitByEnemy.ManualUpdate();
		base.ManualUpdate();
	}

	public override void Dispose()
	{
		if ((Object)(object)base.Player != (Object)null)
		{
			base.Player.BeingHitAction -= GetHit;
		}
		Surgery?.Dispose();
		HitReaction?.Dispose();
		HitByEnemy?.Dispose();
		base.Dispose();
	}

	public void GetHit(DamageInfoStruct DamageInfoStruct, EBodyPart bodyPart, float floatVal)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		TimeLastShot = Time.time;
		HitByEnemy.GetHit(DamageInfoStruct, bodyPart, floatVal);
		HitReaction.GetHit(DamageInfoStruct, bodyPart, floatVal);
		base.Bot.Cover.GetHit(DamageInfoStruct, bodyPart, floatVal);
	}
}
