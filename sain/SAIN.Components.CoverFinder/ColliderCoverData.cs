using SAIN.Components.BotControllerSpace.Classes.Raycasts;
using UnityEngine;

namespace SAIN.Components.CoverFinder;

public struct ColliderCoverData
{
	public bool Analyzed;

	public bool IsValid;

	public int Index;

	public Collider Collider;

	public Vector3 ColliderPosition;

	public Vector3 TargetPosition;

	public Vector3 BotPosition;

	public DirCalcData BotToCoverDirectionData;

	public DirCalcData TargetToCoverDirectionData;

	public DirCalcData BotToTargetDirectionData;

	public float DotFromBotToTargetToCollider;

	public float DotFromTargetToBotToCollider;

	public ColliderCoverData(int index, Collider collider, Vector3 targetPos, Vector3 botPos, DirCalcData botToTargetData)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		DotFromBotToTargetToCollider = 0f;
		DotFromTargetToBotToCollider = 0f;
		Analyzed = false;
		IsValid = true;
		Index = index;
		Collider = collider;
		ColliderPosition = ((Component)collider).transform.position;
		TargetPosition = targetPos;
		BotPosition = botPos;
		BotToCoverDirectionData = default(DirCalcData);
		TargetToCoverDirectionData = default(DirCalcData);
		BotToTargetDirectionData = botToTargetData;
	}
}
