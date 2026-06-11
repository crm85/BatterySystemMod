using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SoundRangeModifiers
{
	public float FinalModifier;

	public float EnvironmentModifier;

	public float ConditionModifier;

	public float OcclusionModifier;

	public float PreClampedMod => EnvironmentModifier * ConditionModifier * OcclusionModifier;

	public SoundRangeModifiers(float defaults = 1f)
	{
		FinalModifier = defaults;
		EnvironmentModifier = defaults;
		ConditionModifier = defaults;
		OcclusionModifier = defaults;
	}

	public float CalcFinalModifier(float min, float max)
	{
		return Mathf.Clamp(PreClampedMod, min, max);
	}
}
