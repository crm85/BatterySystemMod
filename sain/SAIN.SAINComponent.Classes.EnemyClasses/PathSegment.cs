using System.Collections.Generic;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.EnemyClasses;

public struct PathSegment
{
	public Vector3 Corner;

	public Vector3 EndPoint;

	public Vector3 Direction;

	public Vector3 DirectionNormal;

	public float SegmentLength;

	public int Index;

	public List<Vector3> SegmentPoints;
}
