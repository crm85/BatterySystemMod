namespace SAIN.Attributes;

public sealed class HiddenAttribute : BoolAttribute
{
	public HiddenAttribute()
		: base(value: true)
	{
	}
}
