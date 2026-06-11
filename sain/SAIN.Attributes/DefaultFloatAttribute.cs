namespace SAIN.Attributes;

public sealed class DefaultFloatAttribute : BaseAttribute
{
	public readonly float Value;

	public DefaultFloatAttribute(float defaultVal)
	{
		Value = defaultVal;
	}
}
