namespace SAIN.Attributes;

public sealed class PercentageAttribute : GUIValuesAttribute
{
	public PercentageAttribute(float min = 0f, float max = 100f, float rounding = 1f)
		: base(min, max, rounding)
	{
	}
}
