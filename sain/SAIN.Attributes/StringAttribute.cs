namespace SAIN.Attributes;

public abstract class StringAttribute : BaseAttribute
{
	public readonly string Value;

	public StringAttribute(string value)
	{
		Value = value;
	}
}
