using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using EFT.UI.Ragfair;
using UnityEngine;

namespace SAIN.Helpers;

public static class EFTMath
{
	public const float LOW_ACCURACY_DELTA = 0.001f;

	private static readonly Random random_0 = new Random();

	private static bool bool_1 = true;

	private static double double_0;

	public const float MAX_NAVMESH_HIT_OFFSET = 0.04f;

	public static float InverseScaleWithLogisticFunction(float originalValue, float k, float x0 = 20f)
	{
		float num = 1f - 1f / (1f + Mathf.Exp(k * (originalValue - x0)));
		return (float)Math.Round(num, 3);
	}

	private static string TimeString(float seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
	}

	public static int RandomInclude(int a, int b)
	{
		b++;
		if (a > b)
		{
			return random_0.Next(b, a);
		}
		return random_0.Next(a, b);
	}

	public static int RandomSing()
	{
		if (Random(0f, 100f) < 50f)
		{
			return 1;
		}
		return -1;
	}

	public static float Random(float a, float b)
	{
		float num = (float)random_0.NextDouble();
		return a + (b - a) * num;
	}

	public static bool IsTrue100(float v)
	{
		return Random(0f, 100f) < v;
	}

	public static bool RandomBool(float chanceInPercent = 50f)
	{
		return IsTrue100(chanceInPercent);
	}

	public static T ParseEnum<T>(this string value)
	{
		return (T)Enum.Parse(typeof(T), value, ignoreCase: true);
	}

	public static float NextFloat(this Random random, int min, int max)
	{
		float num = (float)(random.NextDouble() * 2.0 - 1.0);
		double num2 = Math.Pow(2.0, random.Next(min, max));
		return (float)((double)num * num2);
	}

	public static bool ApproxEquals(this float value, float value2)
	{
		return Math.Abs(value - value2) < float.Epsilon;
	}

	public static bool ApproxEquals(this double value, double value2)
	{
		return Math.Abs(value - value2) < 1.401298464324817E-45;
	}

	public static bool LowAccuracyApprox(this float value, float value2)
	{
		return Math.Abs(value - value2) < 0.001f;
	}

	public static bool IsZero(this float value)
	{
		return Math.Abs(value) < float.Epsilon;
	}

	public static bool IsZero(this double value)
	{
		return Math.Abs(value) < 1.401298464324817E-45;
	}

	public static bool Positive(this double value)
	{
		return value >= 1.401298464324817E-45;
	}

	public static bool Positive(this float value)
	{
		return value >= float.Epsilon;
	}

	public static bool Negative(this double value)
	{
		return value <= -1.401298464324817E-45;
	}

	public static bool Negative(this float value)
	{
		return value <= -1E-45f;
	}

	public static bool ZeroOrNegative(this float value)
	{
		return value < float.Epsilon;
	}

	public static bool ZeroOrPositive(this float value)
	{
		return value > -1E-45f;
	}

	public static double Clamp01(this double value)
	{
		if (value < 0.0)
		{
			return 0.0;
		}
		if (value <= 1.0)
		{
			return value;
		}
		return 1.0;
	}

	public static double Clamp(this double value, double limit1, double limit2)
	{
		if (limit1 < limit2)
		{
			value = Math.Max(value, limit1);
			value = Math.Min(value, limit2);
		}
		else
		{
			value = Math.Max(value, limit2);
			value = Math.Min(value, limit1);
		}
		return value;
	}

	public static Rect Scale(this Rect rect, Vector2 scale)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		return new Rect(((Rect)(ref rect)).x * scale.x, ((Rect)(ref rect)).y * scale.y, ((Rect)(ref rect)).width * scale.x, ((Rect)(ref rect)).height * scale.y);
	}

	public static T GetRandomItem<T>(this List<T> list, T excludedItem)
	{
		if (list == null)
		{
			return default(T);
		}
		int count = list.Count;
		switch (count)
		{
		case 0:
			return default(T);
		case 1:
			return list[0];
		default:
		{
			int num = 0;
			T result;
			do
			{
				int index = Random.Range(0, count);
				result = list[index];
				num++;
			}
			while (!result.Equals(excludedItem) || num == 100);
			return result;
		}
		}
	}

	public static T GetRandomItem<T>(this List<T> list)
	{
		if (list != null && list.Count != 0)
		{
			int index = Random.Range(0, list.Count);
			return list[index];
		}
		return default(T);
	}

	public static double ExactLength(this AudioClip clip)
	{
		return (double)clip.samples / (double)clip.frequency;
	}

	private static Func<object, T> smethod_0<T>(MethodInfo methodInfo)
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "obj");
		UnaryExpression arg = Expression.Convert(parameterExpression, methodInfo.GetParameters().First().ParameterType);
		return Expression.Lambda<Func<object, T>>(Expression.Call(methodInfo, arg), new ParameterExpression[1] { parameterExpression }).Compile();
	}

	private static Action<object, T> smethod_1<T>(MethodInfo methodInfo)
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "obj");
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(T), "value");
		UnaryExpression arg = Expression.Convert(parameterExpression, methodInfo.GetParameters().First().ParameterType);
		UnaryExpression arg2 = Expression.Convert(parameterExpression2, methodInfo.GetParameters().Last().ParameterType);
		return Expression.Lambda<Action<object, T>>(Expression.Call(methodInfo, arg, arg2), new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
	}

	public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> collection, int n)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		if (n < 0)
		{
			throw new ArgumentOutOfRangeException("n", "n must be 0 or greater");
		}
		LinkedList<T> linkedList = new LinkedList<T>();
		foreach (T item in collection)
		{
			linkedList.AddLast(item);
			if (linkedList.Count > n)
			{
				linkedList.RemoveFirst();
			}
		}
		return linkedList;
	}

	public static Func<TOBjectType, TValueType> CreateGetter<TOBjectType, TValueType>(FieldInfo fieldInfo)
	{
		Type typeFromHandle = typeof(TValueType);
		Type typeFromHandle2 = typeof(TOBjectType);
		return (Func<TOBjectType, TValueType>)CreateGetterFieldDynamicMethod(fieldInfo, typeFromHandle2, typeFromHandle).CreateDelegate(typeof(Func<TOBjectType, TValueType>));
	}

	public static Func<object, TValueType> CreateGetter<TValueType>(FieldInfo fieldInfo, Type objectType)
	{
		Type typeFromHandle = typeof(TValueType);
		return smethod_0<TValueType>(CreateGetterFieldDynamicMethod(fieldInfo, objectType, typeFromHandle).GetBaseDefinition());
	}

	public static DynamicMethod CreateGetterFieldDynamicMethod(FieldInfo fieldInfo, Type objectType, Type valueType)
	{
		DynamicMethod dynamicMethod = new DynamicMethod(fieldInfo.ReflectedType.FullName + ".get_" + fieldInfo.Name, valueType, new Type[1] { objectType }, restrictedSkipVisibility: true);
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		if (fieldInfo.IsStatic)
		{
			iLGenerator.Emit(OpCodes.Ldsfld, fieldInfo);
		}
		else
		{
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldfld, fieldInfo);
		}
		iLGenerator.Emit(OpCodes.Ret);
		return dynamicMethod;
	}

	public static List<Transform> GetChildsName(Transform transform, string name, bool onlyActive = true)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		List<Transform> list = new List<Transform>();
		foreach (object item in transform)
		{
			Transform val = (Transform)item;
			if (!((Object)val).name.Contains(name))
			{
				continue;
			}
			if (onlyActive)
			{
				if (((Component)val).gameObject.activeSelf)
				{
					list.Add(val);
				}
			}
			else
			{
				list.Add(val);
			}
		}
		return list;
	}

	public static Transform GetChildName(Transform transform, string name, string nocontains = "")
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		foreach (object item in transform)
		{
			Transform val = (Transform)item;
			if (((Object)val).name.Contains(name) && ((Component)val).gameObject.activeSelf)
			{
				if (nocontains.Length <= 0)
				{
					return val;
				}
				if (!((Object)val).name.Contains(nocontains))
				{
					return val;
				}
			}
		}
		return null;
	}

	public static bool IsCloseDebug(Vector3 v, float x, float z)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.1f;
		return v.x > x - num && v.x < x + num && v.z > z - num && v.z < z + num;
	}

	public static bool IsCloseDebug(Vector3 v, float x, float y, float z)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.1f;
		return v.x > x - num && v.x < x + num && v.z > z - num && v.z < z + num && v.y > y - num && v.y < y + num;
	}

	public static Action<TOBjectType, TValueType> CreateSetter<TOBjectType, TValueType>(FieldInfo field)
	{
		Type typeFromHandle = typeof(TOBjectType);
		Type typeFromHandle2 = typeof(TValueType);
		return (Action<TOBjectType, TValueType>)smethod_2(field, typeFromHandle, typeFromHandle2).CreateDelegate(typeof(Action<TOBjectType, TValueType>));
	}

	public static Action<object, TValueType> CreateSetter<TValueType>(FieldInfo field, Type objectType)
	{
		Type typeFromHandle = typeof(TValueType);
		return smethod_1<TValueType>(smethod_2(field, objectType, typeFromHandle).GetBaseDefinition());
	}

	private static DynamicMethod smethod_2(FieldInfo field, Type objType, Type valueType)
	{
		DynamicMethod dynamicMethod = new DynamicMethod(field.ReflectedType.FullName + ".set_" + field.Name, null, new Type[2] { objType, valueType }, restrictedSkipVisibility: true);
		ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
		if (field.IsStatic)
		{
			iLGenerator.Emit(OpCodes.Ldarg_1);
			iLGenerator.Emit(OpCodes.Stsfld, field);
		}
		else
		{
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldarg_1);
			iLGenerator.Emit(OpCodes.Stfld, field);
		}
		iLGenerator.Emit(OpCodes.Ret);
		return dynamicMethod;
	}

	public static float RandomNormal(float min, float max)
	{
		double num = 3.5;
		double num2;
		while ((num2 = BoxMuller((double)min + (double)(max - min) / 2.0, (double)(max - min) / 2.0 / num)) > (double)max || num2 < (double)min)
		{
		}
		return (float)num2;
	}

	public static double BoxMuller(double mean, double standard_deviation)
	{
		return mean + BoxMuller() * standard_deviation;
	}

	public static double BoxMuller()
	{
		if (bool_1)
		{
			bool_1 = false;
			return double_0;
		}
		double num;
		double num2;
		double num3;
		do
		{
			num = 2.0 * random_0.NextDouble() - 1.0;
			num2 = 2.0 * random_0.NextDouble() - 1.0;
			num3 = num * num + num2 * num2;
		}
		while (num3 >= 1.0 || num3 == 0.0);
		num3 = Math.Sqrt(-2.0 * Math.Log(num3) / num3);
		double_0 = num2 * num3;
		bool_1 = true;
		return num * num3;
	}

	public static bool RemoveFromQueue<T>(T item, Queue<T> q)
	{
		bool result = false;
		Queue<T> queue = new Queue<T>();
		while (q.Count > 0)
		{
			T item2 = q.Dequeue();
			if (item2.Equals(item))
			{
				result = true;
			}
			else
			{
				queue.Enqueue(item2);
			}
		}
		while (queue.Count > 0)
		{
			q.Enqueue(queue.Dequeue());
		}
		return result;
	}

	public static Mesh MakeFullScreenMesh(Camera cam)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		Mesh val = new Mesh
		{
			name = "Utils MakeFullScreenMesh"
		};
		cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
		Vector3[] vertices = (Vector3[])(object)new Vector3[4]
		{
			cam.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)),
			cam.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)),
			cam.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)),
			cam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f))
		};
		val.vertices = vertices;
		Vector2[] uv = (Vector2[])(object)new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		val.uv = uv;
		int[] triangles = new int[6] { 2, 1, 0, 2, 3, 1 };
		val.triangles = triangles;
		return val;
	}

	public static void ProcessException(Exception exception)
	{
		Debug.LogException(exception);
	}

	public static bool IsDisplayChildCount(this EViewListType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		return (int)type == 3 || (int)type == 4 || (int)type == 1 || (int)type == 5;
	}

	public static bool IsUpdateChildStatus(this EViewListType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		return (int)type == 0 || (int)type == 2 || (int)type == 1 || (int)type == 5;
	}

	public static void ClearTransform(this Transform t)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		foreach (object item in t)
		{
			Object.Destroy((Object)(object)((Component)(Transform)item).gameObject);
		}
	}

	public static void ClearTransformImmediate(this Transform t)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		List<Transform> list = new List<Transform>();
		foreach (object item2 in t)
		{
			Transform item = (Transform)item2;
			list.Add(item);
		}
		Transform[] array = list.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			Object.DestroyImmediate((Object)(object)((Component)array[i]).gameObject);
		}
	}

	public static bool IsOdd(int value)
	{
		return value % 2 != 0;
	}

	public static float GreateRandom(float val)
	{
		return val * Random(0.8f, 1.2f);
	}

	public static float GreateRandom(float val, float fraction)
	{
		return val * Random(1f - fraction, 1f + fraction);
	}

	public static float GreateRandom(int val)
	{
		return (int)((float)val * Random(0.8f, 1.2f));
	}
}
