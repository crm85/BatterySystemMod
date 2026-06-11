using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.Classes;

public sealed class FlankRoute
{
	public Vector3 FlankPoint;

	public Vector3 FlankPoint2;

	public NavMeshPath FirstPath;

	public NavMeshPath SecondPath;

	public NavMeshPath ThirdPath;
}
