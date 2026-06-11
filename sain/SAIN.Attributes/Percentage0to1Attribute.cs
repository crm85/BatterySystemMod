namespace SAIN.Attributes;

public sealed class Percentage0to1Attribute : GUIValuesAttribute
{
	public Percentage0to1Attribute(float min = 0f, float max = 1f, float rounding = 100f)
		: base(min, max, rounding)
	{
	}
}
