namespace SAIN.SAINComponent.Classes;

public class SoundRangeData
{
	public float FinalRange;

	public float BaseRange;

	public SoundRangeModifiers Modifiers;

	public SoundRangeData(float baseRange)
	{
		FinalRange = baseRange;
		BaseRange = baseRange;
		Modifiers = new SoundRangeModifiers();
	}
}
