using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.SubComponents.CoverFinder;

public class TargetData
{
	public HardTargetData HardData { get; }

	public string TargetProfileID => HardData.ProfileId;

	public Enemy TargetEnemy => HardData.Enemy;

	public Vector3 BotPosition { get; private set; }

	public Vector3 TargetPosition { get; private set; }

	public Vector3 DirBotToTarget { get; private set; }

	public Vector3 DirBotToTargetNormal { get; private set; }

	public float TargetDistance { get; private set; }

	public float TargetDistanceSqr { get; private set; }

	public void Update(Vector3 targetPos, Vector3 botPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		BotPosition = botPos;
		TargetPosition = targetPos;
		UpdateDirections();
	}

	public void UpdateDirections()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = (DirBotToTarget = TargetPosition - BotPosition);
		float num = (TargetDistanceSqr = ((Vector3)(ref val)).sqrMagnitude);
		TargetDistance = Mathf.Sqrt(num);
		DirBotToTargetNormal = ((Vector3)(ref val)).normalized;
	}

	public TargetData(Enemy enemy)
	{
		HardData = new HardTargetData(enemy);
	}
}
