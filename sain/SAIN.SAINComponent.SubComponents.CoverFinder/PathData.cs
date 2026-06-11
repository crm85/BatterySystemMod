using UnityEngine;
using UnityEngine.AI;

namespace SAIN.SAINComponent.SubComponents.CoverFinder;

public class PathData
{
	private float _pathLength;

	public NavMeshPath Path { get; }

	public float PathLength
	{
		get
		{
			return _pathLength;
		}
		set
		{
			RoundedPathLength = Mathf.FloorToInt(value);
			_pathLength = value;
		}
	}

	public int RoundedPathLength { get; private set; }

	public PathData(NavMeshPath path)
	{
		Path = path;
	}
}
