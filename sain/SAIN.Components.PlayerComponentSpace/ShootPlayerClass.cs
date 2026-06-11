using System.Collections;
using System.Collections.Generic;
using SAIN.Helpers;
using SAIN.SAINComponent;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Components.PlayerComponentSpace;

public class ShootPlayerClass : PlayerComponentBase
{
	public sealed class FindPlacesToShootParameters
	{
		public float minPointDist = 5f;

		public float maxPointDist = 300f;

		public int iterationMax = 100;

		public int successMax = 5;

		public float yVal = 0.25f;

		public float navSampleRange = 0.25f;

		public float downDirDist = 10f;
	}

	public readonly List<Vector3> PlacesToShootMe = new List<Vector3>();

	public ShootPlayerClass(PlayerComponent component)
		: base(component)
	{
	}

	public IEnumerator FindPlaceToShoot(FindPlacesToShootParameters parameters)
	{
		yield return null;
	}

	public void FindPlacesToShoot(List<Vector3> places, Vector3 directionToBot, FindPlacesToShootParameters parameters)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		float minPointDist = parameters.minPointDist;
		float maxPointDist = parameters.maxPointDist;
		int iterationMax = parameters.iterationMax;
		int successMax = parameters.successMax;
		float yVal = parameters.yVal;
		float navSampleRange = parameters.navSampleRange;
		float downDirDist = parameters.downDirDist;
		LayerMask highPolyWithTerrainMask = LayerMaskClass.HighPolyWithTerrainMask;
		int num = 0;
		places.Clear();
		RaycastHit val2 = default(RaycastHit);
		NavMeshHit val4 = default(NavMeshHit);
		for (int i = 0; i < iterationMax; i++)
		{
			Vector3 headPosition = base.Transform.HeadPosition;
			Vector3 onUnitSphere = Random.onUnitSphere;
			onUnitSphere.y = Random.Range(0f - yVal, yVal);
			float num2 = Random.Range(minPointDist, maxPointDist);
			if (!Physics.Raycast(headPosition, onUnitSphere, num2, LayerMask.op_Implicit(highPolyWithTerrainMask)))
			{
				Vector3 val = headPosition + onUnitSphere * num2;
				if (Physics.Raycast(val, Vector3.down, ref val2, downDirDist, LayerMask.op_Implicit(highPolyWithTerrainMask)))
				{
					Vector3 val3 = ((RaycastHit)(ref val2)).point - headPosition;
					if (((Vector3)(ref val3)).sqrMagnitude > minPointDist * minPointDist && NavMesh.SamplePosition(((RaycastHit)(ref val2)).point, ref val4, navSampleRange, -1))
					{
						DebugGizmos.Sphere(((NavMeshHit)(ref val4)).position, 0.1f, Color.blue, 3f);
						DebugGizmos.Line(((NavMeshHit)(ref val4)).position, headPosition, 0.025f, Time.deltaTime, taperLine: true);
						places.Add(((NavMeshHit)(ref val4)).position);
						num++;
					}
				}
			}
			if (num >= successMax)
			{
				break;
			}
		}
	}
}
