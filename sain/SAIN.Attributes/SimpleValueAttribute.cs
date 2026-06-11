namespace SAIN.Attributes;

public sealed class SimpleValueAttribute : BoolAttribute
{
	public SimpleValueAttribute()
		: base(value: true)
	{
	}
}
