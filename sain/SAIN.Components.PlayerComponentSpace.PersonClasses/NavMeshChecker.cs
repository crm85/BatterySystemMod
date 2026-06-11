using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Components.PlayerComponentSpace.PersonClasses;

public class NavMeshChecker : PersonSubClass
{
	private const float NAVMESH_CHECK_FREQUENCY = 0.33f;

	private const float NAVMESH_CHECK_FREQUENCY_AI = 0.66f;

	private float _nextCheckNavmeshTime;

	public Vector3 LastNavmeshPosition { get; private set; }

	public void Update()
	{
		checkOnNavMesh();
	}

	public bool IsOnNavMesh(out NavMeshHit hit, float range = 0.5f)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return NavMesh.SamplePosition(base.Person.Transform.Position, ref hit, range, -1);
	}

	private void checkOnNavMesh()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (_nextCheckNavmeshTime < Time.time)
		{
			float num = (base.Person.AIInfo.IsAI ? 0.66f : 0.33f);
			_nextCheckNavmeshTime = Time.time + num;
			if (IsOnNavMesh(out var hit))
			{
				LastNavmeshPosition = ((NavMeshHit)(ref hit)).position;
			}
		}
	}

	public NavMeshChecker(PersonClass person, PlayerData playerData)
		: base(person, playerData)
	{
	}
}
