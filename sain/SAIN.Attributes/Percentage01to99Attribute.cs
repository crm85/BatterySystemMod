namespace SAIN.Attributes;

public sealed class Percentage01to99Attribute : GUIValuesAttribute
{
	public Percentage01to99Attribute()
		: base(0.01f, 0.99f, 100f)
	{
	}
}
