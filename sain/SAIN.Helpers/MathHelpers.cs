using System;
using UnityEngine;

namespace SAIN.Helpers;

public static class MathHelpers
{
	public static float ClampObject(object value, float min, float max)
	{
		if (value != null)
		{
			Type type = value.GetType();
			if (type == typeof(float))
			{
				return Mathf.Clamp((float)value, min, max);
			}
			if (type == typeof(int))
			{
				return Mathf.Clamp(Mathf.RoundToInt((float)value), Mathf.RoundToInt(min), Mathf.RoundToInt(max));
			}
			Logger.LogError($"{type}");
		}
		else
		{
			Logger.LogError("Null!?");
		}
		return 0f;
	}

	public static float FloatClamp(this object value, float min, float max)
	{
		if (value != null)
		{
			Type type = value.GetType();
			if (type == typeof(float))
			{
				return Mathf.Clamp((float)value, min, max);
			}
			if (type == typeof(int))
			{
				return Mathf.Clamp(Mathf.RoundToInt((float)value), Mathf.RoundToInt(min), Mathf.RoundToInt(max));
			}
			Logger.LogError($"{type}");
		}
		else
		{
			Logger.LogError("Null!?");
		}
		return 0f;
	}

	public static float InverseScaleWithLogisticFunction(float originalValue, float k, float x0 = 20f)
	{
		float value = 1f - 1f / (1f + Mathf.Exp(k * (originalValue - x0)));
		return value.Round1000();
	}

	public static Vector3 VectorClamp(Vector3 vector, float min, float max)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		vector.x = Mathf.Clamp(vector.x, 0f - min, max);
		vector.y = Mathf.Clamp(vector.y, 0f - min, max);
		vector.z = Mathf.Clamp(vector.z, 0f - min, max);
		return vector;
	}
}
