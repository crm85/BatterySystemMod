using SAIN.Attributes;

namespace SAIN.Components.RotationController;

public struct TurnSettings
{
	[MinMax(0f, 3f, 100f)]
	public float SmoothingValue;

	[MinMax(0.01f, 1000f, 100f)]
	public float MaxTurnSpeed;

	[Advanced]
	[Hidden]
	public EBotLookSmoothingMode SmoothingMode;

	public TurnSettings(float smoothingValue = 0.5f, float maxTurnSpeed = 360f)
	{
		SmoothingValue = smoothingValue;
		MaxTurnSpeed = maxTurnSpeed;
		SmoothingMode = EBotLookSmoothingMode.SmoothDamp;
	}
}
