using SAIN.Classes.Coverfinder;
using UnityEngine;

namespace SAIN.SAINComponent.SubComponents.CoverFinder;

public class SpottedCoverPoint
{
	public const float SPOTTED_PERIOD = 2f;

	private readonly float ExpireTime;

	public CoverPoint CoverPoint { get; private set; }

	public float TimeCreated { get; private set; }

	public float TimeSinceCreated => Time.time - TimeCreated;

	public bool IsValidAgain => TimeSinceCreated > ExpireTime;

	public SpottedCoverPoint(CoverPoint coverPoint)
	{
		ExpireTime = 2f;
		CoverPoint = coverPoint;
		TimeCreated = Time.time;
	}

	public bool TooClose(Vector3 coverInfoPosition, Vector3 newPos, float sqrdist = 2f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = coverInfoPosition - newPos;
		return ((Vector3)(ref val)).sqrMagnitude > sqrdist;
	}
}
