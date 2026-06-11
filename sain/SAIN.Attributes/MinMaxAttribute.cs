namespace SAIN.Attributes;

public class MinMaxAttribute : GUIValuesAttribute
{
	public MinMaxAttribute(float min, float max, float rounding = 100f)
		: base(min, max, rounding)
	{
	}
}
