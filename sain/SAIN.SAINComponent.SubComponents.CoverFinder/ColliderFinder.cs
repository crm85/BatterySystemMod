using System;
using System.Collections;
using System.Collections.Generic;
using SAIN.Components.CoverFinder;
using SAIN.Helpers;
using SAIN.SAINComponent.Classes;
using UnityEngine;

namespace SAIN.SAINComponent.SubComponents.CoverFinder;

public class ColliderFinder
{
	private CoverFinderComponent CoverFinderComponent;

	private static float _nextLogTime;

	private static readonly List<LayerMask> _layersToCheck;

	private static Quaternion _orientation;

	public int HitCount;

	private List<GameObject> debugObjects = new List<GameObject>();

	private static Dictionary<Collider, GUIObject> debugGUIObjects;

	private static Dictionary<Collider, GameObject> debugColliders;

	private Vector3 OriginPoint => CoverFinderComponent.OriginPoint;

	static ColliderFinder()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		_layersToCheck = new List<LayerMask>
		{
			LayerMaskClass.HighPolyWithTerrainMask,
			LayerMaskClass.LowPolyColliderLayerMask
		};
		debugGUIObjects = new Dictionary<Collider, GUIObject>();
		debugColliders = new Dictionary<Collider, GameObject>();
		_orientation = Quaternion.identity;
	}

	public ColliderFinder(CoverFinderComponent component)
	{
		CoverFinderComponent = component;
	}

	public IEnumerator GetNewColliders(Collider[] preAllocArray, int iterationMax = 10, float startBoxWidth = 2f, int hitThreshold = 100)
	{
		clearColliders(preAllocArray);
		float boxLength = startBoxWidth;
		float boxHeight = 0.25f;
		Vector3 boxOrigin = OriginPoint + Vector3.up * 0.25f;
		HitCount = 0;
		int hits = 0;
		int totalIterations = 0;
		bool foundEnough = false;
		int layerCount = _layersToCheck.Count;
		for (int l = 0; l < layerCount; l++)
		{
			LayerMask layer = _layersToCheck[l];
			for (int i = 0; i < iterationMax; i++)
			{
				totalIterations++;
				hits = GetCollidersInBox(boxLength, boxHeight, boxLength, boxOrigin, preAllocArray, layer);
				foundEnough = hits >= hitThreshold;
				if (foundEnough)
				{
					break;
				}
				boxOrigin += Vector3.down * 0.5f;
				boxHeight += 1f;
				boxLength += 2f;
				yield return null;
			}
			if (foundEnough)
			{
				break;
			}
		}
		HitCount = hits;
	}

	private static int GetCollidersInBox(float x, float y, float z, Vector3 boxOrigin, Collider[] array, LayerMask colliderMask)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		int hits = Physics.OverlapBoxNonAlloc(boxOrigin, new Vector3(x, y, z), array, _orientation, LayerMask.op_Implicit(colliderMask));
		return FilterColliders(array, hits);
	}

	private void destroyDebug()
	{
		for (int i = 0; i < debugObjects.Count; i++)
		{
			Object.Destroy((Object)(object)debugObjects[i]);
		}
		debugObjects.Clear();
	}

	private void clearColliders(Collider[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = null;
		}
	}

	public void SortArrayBotDist(Collider[] array)
	{
		Array.Sort(array, ColliderArrayBotDistComparer);
	}

	private static int FilterColliders(Collider[] array, int hits)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		float coverMinHeight = CoverFinderComponent.CoverMinHeight;
		int num = 0;
		for (int i = 0; i < hits; i++)
		{
			Bounds bounds = array[i].bounds;
			Vector3 size = ((Bounds)(ref bounds)).size;
			if (size.y < coverMinHeight || (size.x < 0.25f && size.z < 0.25f))
			{
				array[i] = null;
				num++;
			}
		}
		return hits - num;
	}

	public void UpdateDebugColliders(Collider[] array)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (SAINPlugin.DebugMode && SAINPlugin.LoadedPreset.GlobalSettings.General.Cover.DebugCoverFinder && CoverFinderComponent.Bot.Cover.CurrentCoverFinderState == CoverFinderState.on)
		{
			foreach (Collider val in array)
			{
				if ((Object)(object)val == (Object)null)
				{
					continue;
				}
				if (!debugGUIObjects.ContainsKey(val))
				{
					GUIObject gUIObject = DebugGizmos.CreateLabel(((Component)val).transform.position, ((Object)val).name);
					if (gUIObject != null)
					{
						debugGUIObjects.Add(val, gUIObject);
					}
				}
				if (!debugColliders.ContainsKey(val))
				{
					GameObject val2 = DebugGizmos.Sphere(((Component)val).transform.position, 0f);
					if ((Object)(object)val2 != (Object)null)
					{
						debugColliders.Add(val, val2);
					}
				}
			}
		}
		else
		{
			if (debugGUIObjects.Count <= 0 && debugColliders.Count <= 0)
			{
				return;
			}
			foreach (KeyValuePair<Collider, GUIObject> debugGUIObject in debugGUIObjects)
			{
				DebugGizmos.DestroyLabel(debugGUIObject.Value);
			}
			foreach (KeyValuePair<Collider, GameObject> debugCollider in debugColliders)
			{
				Object.Destroy((Object)(object)debugCollider.Value);
			}
			debugGUIObjects.Clear();
			debugColliders.Clear();
		}
	}

	public int ColliderArrayBotDistComparer(Collider A, Collider B)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)A == (Object)null && (Object)(object)B != (Object)null)
		{
			return 1;
		}
		if ((Object)(object)A != (Object)null && (Object)(object)B == (Object)null)
		{
			return -1;
		}
		if ((Object)(object)A == (Object)null && (Object)(object)B == (Object)null)
		{
			return 0;
		}
		Vector3 val = OriginPoint - ((Component)A).transform.position;
		float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
		val = OriginPoint - ((Component)B).transform.position;
		float sqrMagnitude2 = ((Vector3)(ref val)).sqrMagnitude;
		return sqrMagnitude.CompareTo(sqrMagnitude2);
	}
}
