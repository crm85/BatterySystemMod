namespace SAIN.SAINComponent.SubComponents.CoverFinder;

public class CoverHitCounts
{
	public bool Spotted;

	public float TimeSpotted;

	public float TimeLastHit;

	public int Total;

	public int Unknown;

	public int ThirdParty;

	public int CantSee;

	public int Legs;

	public void Reset()
	{
		Spotted = false;
		TimeSpotted = 0f;
		Total = 0;
		Unknown = 0;
		ThirdParty = 0;
		CantSee = 0;
		Legs = 0;
	}
}
