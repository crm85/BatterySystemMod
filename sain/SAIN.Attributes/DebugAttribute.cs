namespace SAIN.Attributes;

public sealed class DebugAttribute : BoolAttribute
{
	public DebugAttribute()
		: base(value: true)
	{
	}
}
