using System.Collections.Generic;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN.Models.Structs;

public struct BotPeekPlan
{
	private List<Vector3> DebugVectorList;

	private List<GameObject> DebugGameObjectList;

	public PeekPosition PeekStart { get; private set; }

	public PeekPosition PeekEnd { get; private set; }

	public Vector3 DangerPoint { get; private set; }

	public BotPeekPlan(Vector3 start, Vector3 end, Vector3 dangerPoint)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		PeekStart = new PeekPosition(start, dangerPoint);
		PeekEnd = new PeekPosition(end, dangerPoint);
		DangerPoint = dangerPoint;
		DebugVectorList = null;
		DebugGameObjectList = null;
	}

	private Vector3 MidPoint(Vector3 A, Vector3 B)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Lerp(A, B, 0.5f);
	}

	private bool CheckIfLeanable(float signAngle, float limit = 1f)
	{
		return Mathf.Abs(signAngle) > limit;
	}

	public LeanSetting GetDirectionToLean(float signAngle)
	{
		if (CheckIfLeanable(signAngle))
		{
			return (!(signAngle > 0f)) ? LeanSetting.Left : LeanSetting.Right;
		}
		return LeanSetting.None;
	}

	public void DrawDebug()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!SAINPlugin.DebugMode || !SAINPlugin.DebugSettings.Gizmos.DebugSearchGizmos)
		{
			DisposeDebug();
			return;
		}
		if (DebugVectorList == null)
		{
			DebugVectorList = new List<Vector3> { PeekStart.Point, PeekEnd.Point, DangerPoint };
		}
		if (DebugGameObjectList == null)
		{
			DebugGameObjectList = DebugGizmos.DrawLinesBetweenPoints(0.1f, 0.05f, DebugVectorList.ToArray());
		}
	}

	public void DisposeDebug()
	{
		if (DebugVectorList != null)
		{
			DebugVectorList.Clear();
			DebugVectorList = null;
		}
		if (DebugGameObjectList != null)
		{
			for (int i = 0; i < DebugGameObjectList.Count; i++)
			{
				Object.Destroy((Object)(object)DebugGameObjectList[i]);
			}
			DebugGameObjectList.Clear();
			DebugGameObjectList = null;
		}
	}
}
