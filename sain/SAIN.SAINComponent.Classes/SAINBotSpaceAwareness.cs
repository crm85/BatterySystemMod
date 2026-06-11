using SAIN.Components;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.Classes;

public class SAINBotSpaceAwareness : BotComponentClassBase
{
	public SAINBotSpaceAwareness(BotComponent sain)
		: base(sain)
	{
		base.CanEverTick = false;
	}

	public static bool ArePathsDifferent(NavMeshPath path1, NavMeshPath path2, float minRatio = 0.5f, float sqrDistCheck = 0.05f)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] corners = path1.corners;
		int num = corners.Length;
		Vector3[] corners2 = path2.corners;
		int num2 = corners2.Length;
		int num3 = 0;
		for (int i = 0; i < num; i++)
		{
			Vector3 val = corners[i];
			if (i < num2)
			{
				Vector3 val2 = corners2[i];
				if (GClass835.IsEqual(val, val2, sqrDistCheck))
				{
					num3++;
				}
			}
		}
		float num4 = num3 / num;
		return num4 <= minRatio;
	}
}
