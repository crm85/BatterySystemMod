namespace SAIN.Attributes;

public sealed class CopyValueAttribute : BoolAttribute
{
	public CopyValueAttribute()
		: base(value: true)
	{
	}
}
