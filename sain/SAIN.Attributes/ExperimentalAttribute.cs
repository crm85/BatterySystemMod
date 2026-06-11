namespace SAIN.Attributes;

public sealed class ExperimentalAttribute : BoolAttribute
{
	public ExperimentalAttribute()
		: base(value: true)
	{
	}
}
