namespace SAIN.Attributes;

public sealed class DefaultDictionaryAttribute : StringAttribute
{
	public DefaultDictionaryAttribute(string dictionaryName)
		: base(dictionaryName)
	{
	}
}
