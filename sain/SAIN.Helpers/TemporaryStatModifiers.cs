namespace SAIN.Helpers;

public class TemporaryStatModifiers
{
	public GClass595 Modifiers;

	public TemporaryStatModifiers(float precision = 1f, float accuracySpeed = 1f, float gainSight = 1f, float scatter = 1f, float priorityScatter = 1f, float visibleDistance = 1f, float hearingDistance = 1f)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		Modifiers = new GClass595
		{
			PrecicingSpeedCoef = precision,
			AccuratySpeedCoef = accuracySpeed,
			GainSightCoef = gainSight,
			ScatteringCoef = scatter,
			PriorityScatteringCoef = priorityScatter,
			VisibleDistCoef = visibleDistance,
			HearingDistCoef = hearingDistance
		};
	}
}
