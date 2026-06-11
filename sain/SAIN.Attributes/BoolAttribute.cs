namespace SAIN.Attributes;

public abstract class BoolAttribute : BaseAttribute
{
	public readonly bool Value;

	public BoolAttribute(bool value)
	{
		Value = value;
	}
}
