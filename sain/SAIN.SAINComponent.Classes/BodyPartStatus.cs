using EFT.HealthSystem;

namespace SAIN.SAINComponent.Classes;

public class BodyPartStatus
{
	private readonly EBodyPart _bodyPart;

	private readonly SAINBotHitReaction _hitReaction;

	private IHealthController _healthController => _hitReaction.HealthController;

	public EInjurySeverity InjurySeverity
	{
		get
		{
			float partHealthNormalized = PartHealthNormalized;
			if (partHealthNormalized > 0.75f)
			{
				return EInjurySeverity.None;
			}
			if (partHealthNormalized > 0.4f)
			{
				return EInjurySeverity.Injury;
			}
			if (partHealthNormalized > 0.01f)
			{
				return EInjurySeverity.HeavyInjury;
			}
			return EInjurySeverity.Destroyed;
		}
	}

	public float PartHealth => ((GInterface355)_healthController).GetBodyPartHealth(_bodyPart, false).Current;

	public float PartHealthNormalized
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			ValueStruct bodyPartHealth = ((GInterface355)_healthController).GetBodyPartHealth(_bodyPart, false);
			return ((ValueStruct)(ref bodyPartHealth)).Normalized;
		}
	}

	public bool PartDestoyed => _healthController.IsBodyPartDestroyed(_bodyPart);

	public BodyPartStatus(EBodyPart part, SAINBotHitReaction hitReactionClass)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		_bodyPart = part;
		_hitReaction = hitReactionClass;
	}
}
