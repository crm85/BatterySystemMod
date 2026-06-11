namespace SAIN.Attributes;

public sealed class DifficultyModAttribute : MinMaxAttribute
{
	public DifficultyModAttribute()
		: base(0.01f, 10f)
	{
	}
}
