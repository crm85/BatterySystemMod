namespace SAIN.Attributes;

public sealed class NameAndDescriptionAttribute : BaseAttribute
{
	public readonly string Name;

	public readonly string Description;

	public NameAndDescriptionAttribute(string name, string description = null)
	{
		Name = name;
		Description = description;
	}
}
