using DrakiaXYZ.BigBrain.Brains;
using EFT;
using EFT.Communications;
using SAIN.Helpers;
using SAIN.SAINComponent.Classes;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Layers.Combat.Solo;

internal class FlankAction : CombatAction
{
	public FlankAction(BotOwner bot)
		: base(bot, "FlankAction")
	{
	}

	public void Toggle(bool value)
	{
		ToggleAction(value);
	}

	public override void Update(ActionData data)
	{
		StartProfilingSample("Update");
		Enemy enemy = base.Bot.Enemy;
		if (enemy != null)
		{
		}
		EndProfilingSample();
	}

	private FlankRoute FindFlankRoute()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = base.Bot.Enemy;
		if (enemy == null)
		{
			return null;
		}
		FlankRoute flankRoute = null;
		Vector3 enemyPosition = base.Bot.Enemy.EnemyPosition;
		Vector3 position = base.Bot.Position;
		int index;
		Vector3? val = FindMiddlePoint(enemy.Path.PathToEnemy, enemy.Path.PathLength, out index);
		if (val.HasValue)
		{
			Vector3 directionFromMiddle = enemyPosition - val.Value;
			flankRoute = FindFlank(val.Value, directionFromMiddle, position, enemy, SideTurn.right);
			if (flankRoute != null)
			{
				return flankRoute;
			}
			flankRoute = FindFlank(val.Value, directionFromMiddle, position, enemy, SideTurn.left);
			if (flankRoute != null)
			{
				return flankRoute;
			}
		}
		return null;
	}

	private FlankRoute FindFlank(Vector3 middleNode, Vector3 directionFromMiddle, Vector3 botPosition, Enemy enemy, SideTurn sideTurn)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		Vector3 point = Vector.Rotate90(directionFromMiddle, sideTurn);
		if (SamplePointAndCheckPath(point, middleNode, out var path))
		{
			point = path.corners[path.corners.Length - 1];
			NavMeshPath pathToEnemy = enemy.Path.PathToEnemy;
			NavMeshPath val = new NavMeshPath();
			if (NavMesh.CalculatePath(botPosition, point, -1, val) && ArePathsDifferent(pathToEnemy, val))
			{
				NavMeshPath val2 = new NavMeshPath();
				if (NavMesh.CalculatePath(point, enemy.EnemyPosition, -1, val2) && ArePathsDifferent(pathToEnemy, val2) && ArePathsDifferent(val, val2))
				{
					return new FlankRoute
					{
						FlankPoint = point,
						FirstPath = val,
						SecondPath = val2
					};
				}
			}
		}
		return null;
	}

	private bool ArePathsDifferent(NavMeshPath path1, NavMeshPath path2, float minRatio = 0.25f)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < path1.corners.Length; i++)
		{
			Vector3 val = path1.corners[i];
			bool flag = false;
			for (int j = 0; j < path2.corners.Length; j++)
			{
				Vector3 val2 = path2.corners[j];
				if (val2 == val)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				num++;
			}
			else
			{
				num2++;
			}
		}
		Logger.NotifyDebug(num / path1.corners.Length, (ENotificationDurationType)0);
		return (float)(num / path1.corners.Length) <= minRatio;
	}

	private bool SamplePointAndCheckPath(Vector3 point, Vector3 origin, out NavMeshPath path)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(point, ref val, 10f, -1))
		{
			path = new NavMeshPath();
			return NavMesh.CalculatePath(origin, ((NavMeshHit)(ref val)).position, -1, path);
		}
		path = null;
		return false;
	}

	private Vector3? FindMiddlePoint(NavMeshPath path, float pathLength, out int index)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < path.corners.Length - 1; i++)
		{
			Vector3 val = path.corners[i];
			Vector3 val2 = path.corners[i + 1];
			float num2 = num;
			Vector3 val3 = val - val2;
			num = num2 + ((Vector3)(ref val3)).magnitude;
			if (num >= pathLength / 2f)
			{
				index = i;
				return val;
			}
		}
		index = 0;
		return null;
	}

	public override void Start()
	{
		Toggle(value: true);
	}

	public override void Stop()
	{
		Toggle(value: false);
	}
}
