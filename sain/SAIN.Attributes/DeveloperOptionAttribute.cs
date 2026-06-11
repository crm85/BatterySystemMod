namespace SAIN.Attributes;

public sealed class DeveloperOptionAttribute : BoolAttribute
{
	public DeveloperOptionAttribute()
		: base(value: true)
	{
	}
}
