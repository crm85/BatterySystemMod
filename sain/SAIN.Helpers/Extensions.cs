using System.Collections.Generic;
using EFT;
using EFT.UI;
using JetBrains.Annotations;
using SAIN.Editor;
using SAIN.SAINComponent.Classes.EnemyClasses;
using SAIN.SAINComponent.Classes.Mover;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Helpers;

public static class Extensions
{
	public static void Shuffle<T>(this IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = ThreadSafeRandom.ThisThreadsRandom.Next(num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static float CalcPathLength([NotNull] this List<BotCornerDetails> path)
	{
		float num = 0f;
		for (int i = 0; i < path.Count; i++)
		{
			num += path[i].Length;
		}
		return num;
	}

	public static void AddCornerToPath([NotNull] this List<BotCornerDetails> path, Vector3 corner, Vector3? nextCorner, EBotCornerType secondToLastType, EBotCornerType lastCornerType, EBotCornerType Type)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		int count = path.Count;
		BotCornerDetails value = path[count - 2];
		value.Type = secondToLastType;
		path[count - 2] = value;
		BotCornerDetails value2 = path[count - 1];
		value2.Type = lastCornerType;
		value2.SetDirection(corner - value2.Position);
		path[count - 1] = value2;
		if (nextCorner.HasValue)
		{
			path.Add(BotCornerDetails.Create(corner, nextCorner.Value, Type, count));
		}
		else
		{
			path.Add(BotCornerDetails.Create(corner, Type, count));
		}
	}

	public static bool IsSame([NotNull] this Enemy enemy, [NotNull] Enemy other)
	{
		return enemy.EnemyProfileId == other.EnemyProfileId;
	}

	public static bool IsDifferent([NotNull] this Enemy enemy, [NotNull] Enemy other)
	{
		return !enemy.IsSame(other);
	}

	public static Vector3? Position(this EnemyPlace place)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return place?.Position;
	}

	public static bool IsLegs(this EBodyPart part)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		if (part - 5 <= 1)
		{
			return true;
		}
		return false;
	}

	public static bool IsPMC(this WildSpawnType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		if (type - 51 <= 1)
		{
			return true;
		}
		return false;
	}

	public static bool IsBoss(this WildSpawnType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		if ((int)type <= 22)
		{
			if ((int)type <= 7)
			{
				if ((int)type == 3 || type - 6 <= 1)
				{
					goto IL_0052;
				}
			}
			else if ((int)type == 11 || (int)type == 22)
			{
				goto IL_0052;
			}
		}
		else if ((int)type <= 29)
		{
			if ((int)type == 26 || (int)type == 29)
			{
				goto IL_0052;
			}
		}
		else if ((int)type == 32 || (int)type == 43 || (int)type == 47)
		{
			goto IL_0052;
		}
		return false;
		IL_0052:
		return true;
	}

	public static bool IsOther(this WildSpawnType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		if ((int)type == 9 || (int)type == 24 || type - 34 <= 1)
		{
			return true;
		}
		return false;
	}

	public static bool IsFollower(this WildSpawnType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected I4, but got Unknown
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected I4, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Invalid comparison between Unknown and I4
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Invalid comparison between Unknown and I4
		if ((int)type <= 23)
		{
			switch (type - 5)
			{
			default:
				if ((int)type == 23)
				{
					break;
				}
				goto IL_0086;
			case 0:
			case 3:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
				break;
			case 1:
			case 2:
			case 4:
			case 5:
			case 6:
				goto IL_0086;
			}
		}
		else
		{
			switch (type - 27)
			{
			default:
				if ((int)type == 36 || type - 41 <= 1)
				{
					break;
				}
				goto IL_0086;
			case 0:
			case 1:
			case 3:
			case 6:
				break;
			case 2:
			case 4:
			case 5:
				goto IL_0086;
			}
		}
		return true;
		IL_0086:
		return false;
	}

	public static bool IsScav(this WildSpawnType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		if ((int)type <= 10)
		{
			if ((int)type <= 1 || (int)type == 10)
			{
				goto IL_0025;
			}
		}
		else if ((int)type == 19 || (int)type == 37)
		{
			goto IL_0025;
		}
		return false;
		IL_0025:
		return true;
	}

	public static bool IsGoons(this WildSpawnType type)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		if (type - 26 <= 2)
		{
			return true;
		}
		return false;
	}

	public static bool IsArms(this EBodyPart part)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		if (part - 3 <= 1)
		{
			return true;
		}
		return false;
	}

	public static Vector3? LastCorner(this NavMeshPath path)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (path == null)
		{
			return null;
		}
		Vector3[] corners = path.corners;
		if (corners == null)
		{
			return null;
		}
		if (corners.Length == 0)
		{
			return null;
		}
		return corners[corners.Length - 1];
	}

	public static SAINSoundType Convert(this AISoundType aiSoundType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)aiSoundType != 1)
		{
			if ((int)aiSoundType == 2)
			{
				return SAINSoundType.Shot;
			}
			return SAINSoundType.Generic;
		}
		return SAINSoundType.SuppressedShot;
	}

	public static AISoundType Convert(this SAINSoundType sainSoundType)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return (AISoundType)(sainSoundType switch
		{
			SAINSoundType.SuppressedShot => 1, 
			SAINSoundType.Shot => 2, 
			_ => 0, 
		});
	}

	public static bool IsGunShot(this SAINSoundType sainSoundType)
	{
		if ((uint)(sainSoundType - 14) <= 1u)
		{
			return true;
		}
		return false;
	}

	public static Vector3? GetCornerAtIndex(this NavMeshPath path, int index)
	{
		return path.corners?.GetVector3AtIndex(index);
	}

	public static Vector3? GetVector3AtIndex(this List<Vector3> list, int index)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (list.GetItemAtIndex(index, out var result))
		{
			return result;
		}
		return null;
	}

	public static Vector3? GetVector3AtIndex(this Vector3[] array, int index)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (array.GetItemAtIndex(index, out var result))
		{
			return result;
		}
		return null;
	}

	public static bool GetItemAtIndex<T>(this T[] array, int i, out T result)
	{
		int num = array.Length;
		if (i >= num)
		{
			result = default(T);
			return false;
		}
		result = array[i];
		return true;
	}

	public static bool GetItemAtIndex<T>(this List<T> list, int i, out T result)
	{
		int count = list.Count;
		if (i >= count)
		{
			result = default(T);
			return false;
		}
		result = list[i];
		return true;
	}

	public static Vector3 Normalize(this Vector3 value, out float magnitude)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		magnitude = ((Vector3)(ref value)).magnitude;
		if (magnitude > 1E-05f)
		{
			return value / magnitude;
		}
		return Vector3.zero;
	}

	public static Vector3? LastElement(this Vector3[] array)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (array == null)
		{
			return null;
		}
		int num = array.Length;
		if (num == 0)
		{
			return null;
		}
		return array[num - 1];
	}

	public static Vector3? LastElement(this Vector3[] array, out int length)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (array == null)
		{
			length = 0;
			return null;
		}
		length = array.Length;
		if (length == 0)
		{
			return null;
		}
		return array[length - 1];
	}

	public static Vector3 RotateHoriz(this Vector3 value, float angle)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.Euler(0f, angle, 0f);
		return val * value;
	}

	public static float Sqr(this float value)
	{
		return value * value;
	}

	public static float Sqrt(this float value)
	{
		return Mathf.Sqrt(value);
	}

	public static float Scale0to1(this float value, float scalingFactor)
	{
		return value.Scale(0f, 1f, 1f - scalingFactor, 1f + scalingFactor);
	}

	public static float Scale(this float value, float inputMin, float inputMax, float outputMin, float outputMax)
	{
		return outputMin + (outputMax - outputMin) * ((value - inputMin) / (inputMax - inputMin));
	}

	public static bool GUIToggle(this bool value, GUIContent content, EUISoundType? sound = null, params GUILayoutOption[] options)
	{
		bool flag = GUILayout.Toggle(value, content, GetStyle(Style.toggle), options);
		CompareValuePlaySound(value, flag, sound);
		return flag;
	}

	public static bool GUIToggle(this bool value, string name, string toolTip, EUISoundType? sound = null, params GUILayoutOption[] options)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return value.GUIToggle(new GUIContent(name, toolTip), sound, options);
	}

	public static bool GUIToggle(this bool value, string name, EUISoundType? sound = null, params GUILayoutOption[] options)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		return value.GUIToggle(new GUIContent(name), sound, options);
	}

	private static GUIStyle GetStyle(Style style)
	{
		return StylesClass.GetStyle(style);
	}

	private static bool CompareValuePlaySound(object oldValue, object newValue, EUISoundType? sound = null)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (oldValue.ToString() != newValue.ToString() && sound.HasValue)
		{
			Sounds.PlaySound(sound.Value);
			return true;
		}
		return false;
	}

	public static float Randomize(this float value, float a = 0.5f, float b = 2f)
	{
		return (value * Random(a, b)).Round100();
	}

	public static float RandomizeSum(this float value, float a = -1f, float b = 1f, float min = 0.001f)
	{
		float num = value + Random(a, b);
		if (num < min)
		{
			num = min;
		}
		return num.Round100();
	}

	public static float Random(float a, float b)
	{
		return Random.Range(a, b);
	}

	public static float Round(this float value, float round)
	{
		return Mathf.Round(value * round) / round;
	}

	public static float Round1(this float value)
	{
		return value.Round(1f);
	}

	public static float Round10(this float value)
	{
		return value.Round(10f);
	}

	public static float Round100(this float value)
	{
		return value.Round(100f);
	}

	public static float Round1000(this float value)
	{
		return value.Round(1000f);
	}
}
