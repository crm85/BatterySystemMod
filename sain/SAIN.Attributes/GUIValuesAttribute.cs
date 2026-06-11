using SAIN.Helpers;

namespace SAIN.Attributes;

public abstract class GUIValuesAttribute : BaseAttribute
{
	public readonly float Min;

	public readonly float Max;

	public readonly float Rounding;

	public GUIValuesAttribute(float min, float max, float rounding)
	{
		Min = min;
		Max = max;
		Rounding = rounding;
	}

	public float Clamp(object value)
	{
		return MathHelpers.ClampObject(value, Min, Max);
	}

	public float Round(float value)
	{
		return value.Round(Rounding);
	}
}
