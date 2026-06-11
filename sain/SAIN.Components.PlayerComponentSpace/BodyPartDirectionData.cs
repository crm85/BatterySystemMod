namespace SAIN.Components.PlayerComponentSpace;

public struct BodyPartDirectionData
{
	public DirectionData DirectionData;

	public EBodyPart BodyPart;

	public BodyPartDirectionData(EBodyPart Part)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		DirectionData = default(DirectionData);
		BodyPart = Part;
	}
}
