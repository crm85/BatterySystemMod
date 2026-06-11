namespace SAIN.Preset.GlobalSettings;

public struct HearingDelaySettings
{
	public float AtPeace;

	public float ActiveEnemy;

	public float OtherEnemy_Known;

	public float OtherEnemy_Unknown;

	public float RandomizationMin;

	public float RandomizationMax;

	public HearingDelaySettings(float PeaceDelay, float ActiveEnemyDelay, float OtherEnemyDelay, float UnknownEnemyDelay, float InRandomizationMin = 0.75f, float InRandomizationMax = 1.25f)
	{
		AtPeace = PeaceDelay;
		ActiveEnemy = ActiveEnemyDelay;
		OtherEnemy_Known = OtherEnemyDelay;
		OtherEnemy_Unknown = UnknownEnemyDelay;
		RandomizationMin = InRandomizationMin;
		RandomizationMax = InRandomizationMax;
	}
}
