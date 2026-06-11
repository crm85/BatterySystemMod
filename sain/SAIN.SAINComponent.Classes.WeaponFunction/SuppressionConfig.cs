namespace SAIN.SAINComponent.Classes.WeaponFunction;

public class SuppressionConfig
{
	public float Threshold;

	public float PrecisionSpeedCoef;

	public float AccuracySpeedCoef;

	public float VisibleDistCoef;

	public float GainSightCoef;

	public float ScatteringCoef;

	public float HearingDistCoef;

	public bool IsActive(float num)
	{
		return num >= Threshold;
	}
}
