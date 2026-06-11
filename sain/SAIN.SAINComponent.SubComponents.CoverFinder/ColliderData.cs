using SAIN.Models.Structs;
using UnityEngine;

namespace SAIN.SAINComponent.SubComponents.CoverFinder;

public class ColliderData
{
	public Vector3 dirColliderToTarget;

	public Vector3 dirColliderToTargetNormal;

	public float ColliderToTargetMagnitude;

	public Vector3 dirTargetToCollider;

	public Vector3 dirTargetToColliderNormal;

	public Vector3 dirBotToCollider;

	public Vector3 dirBotToColliderNormal;

	public float ColliderDistanceToBot;

	public ColliderData(SAINHardColliderData hardData, TargetData targetDirs)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = hardData.Position;
		Vector3 val = (dirColliderToTarget = targetDirs.TargetPosition - position);
		dirColliderToTargetNormal = ((Vector3)(ref val)).normalized;
		ColliderToTargetMagnitude = ((Vector3)(ref val)).magnitude;
		dirTargetToCollider = -dirColliderToTarget;
		dirTargetToColliderNormal = -dirColliderToTargetNormal;
		Vector3 val2 = (dirBotToCollider = position - targetDirs.BotPosition);
		dirBotToColliderNormal = ((Vector3)(ref val2)).normalized;
		ColliderDistanceToBot = ((Vector3)(ref val2)).magnitude;
	}
}
