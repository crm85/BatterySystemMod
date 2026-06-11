using System.Collections.Generic;
using UnityEngine;

namespace SAIN.Components.CoverFinder;

public class ColliderCoverManager : MonoBehaviour
{
	private List<ColliderCoverComponent> CoverGenerationList { get; } = new List<ColliderCoverComponent>();

	public HashSet<ColliderCoverComponent> Colliders { get; } = new HashSet<ColliderCoverComponent>();

	public ColliderCoverComponent CreateCover(Collider collider)
	{
		ColliderCoverComponent colliderCoverComponent = ((Component)collider).gameObject.GetComponent<ColliderCoverComponent>();
		if ((Object)(object)colliderCoverComponent == (Object)null)
		{
			colliderCoverComponent = ((Component)collider).gameObject.AddComponent<ColliderCoverComponent>();
			colliderCoverComponent.Initialize(collider);
			CoverGenerationList.Insert(0, colliderCoverComponent);
		}
		return colliderCoverComponent;
	}

	private void Update()
	{
		GenerateCover();
	}

	private void GenerateCover(int maxPerFrame = 4)
	{
		int num = CoverGenerationList.Count - 1;
		int num2 = Mathf.Max(num - maxPerFrame, 0);
		for (int num3 = num; num3 >= num2; num3--)
		{
			ColliderCoverComponent colliderCoverComponent = CoverGenerationList[num3];
			CoverGenerationList.RemoveAt(num3);
			colliderCoverComponent.Generate();
			Colliders.Add(colliderCoverComponent);
		}
	}
}
