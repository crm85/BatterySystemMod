using System.Collections.Generic;
using SAIN.Classes.Coverfinder;
using UnityEngine;

namespace SAIN.Components.CoverFinder;

public class ColliderCoverDataClass
{
	public bool IsValid = false;

	public Collider Collider;

	public Vector3 ColliderPosition;

	public HashSet<CoverPointClass> CoverPoints { get; }

	public ColliderCoverDataClass(Collider collider)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Collider = collider;
		ColliderPosition = ((Component)collider).transform.position;
		CoverPoints = new HashSet<CoverPointClass>();
		base._002Ector();
	}
}
