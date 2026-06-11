using System;
using System.Diagnostics;
using System.Reflection;
using BepInEx.Logging;
using EFT.Communications;
using EFT.UI;
using UnityEngine;

namespace SAIN;

internal static class Logger
{
	private static float _nextNotification;

	private static ManualLogSource SAINLogger;

	public static void LogInfo(object data)
	{
		Log((LogLevel)16, data);
	}

	public static void LogDebug(object data)
	{
		Log((LogLevel)32, data);
	}

	public static void LogWarning(object data)
	{
		Log((LogLevel)4, data);
	}

	public static void LogError(object data)
	{
		Log((LogLevel)2, data);
	}

	public static void NotifyInfo(object data, ENotificationDurationType duration = (ENotificationDurationType)0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		NotifyMessage(data, duration, (ENotificationIconType)4);
	}

	public static void NotifyDebug(object data, ENotificationDurationType duration = (ENotificationDurationType)0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		NotifyMessage(data, duration, (ENotificationIconType)4, Color.gray);
	}

	public static void NotifyWarning(object data, ENotificationDurationType duration = (ENotificationDurationType)0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		NotifyMessage(data, duration, (ENotificationIconType)1, Color.yellow);
	}

	public static void NotifyError(object data, ENotificationDurationType duration = (ENotificationDurationType)1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		NotifyMessage(data, duration, (ENotificationIconType)1, Color.red, Error: true);
	}

	public static void LogAndNotifyInfo(object data, ENotificationDurationType duration = (ENotificationDurationType)0)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Log((LogLevel)16, data);
		NotifyMessage(data, duration, (ENotificationIconType)4);
	}

	public static void LogAndNotifyDebug(object data, ENotificationDurationType duration = (ENotificationDurationType)0)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Log((LogLevel)32, data);
		NotifyMessage(data, duration, (ENotificationIconType)4, Color.gray);
	}

	public static void LogAndNotifyWarning(object data, ENotificationDurationType duration = (ENotificationDurationType)0)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Log((LogLevel)4, data);
		NotifyMessage(data, duration, (ENotificationIconType)1, Color.yellow);
	}

	public static void LogAndNotifyError(object data, ENotificationDurationType duration = (ENotificationDurationType)1)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Log((LogLevel)2, data);
		string text = CreateErrorMessage(data);
		NotificationManagerClass.DisplayMessageNotification(text, duration, (ENotificationIconType)1, (Color?)Color.red);
	}

	public static void NotifyMessage(object data, ENotificationDurationType durationType = (ENotificationDurationType)0, ENotificationIconType iconType = (ENotificationIconType)0, Color? textColor = null, bool Error = false)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (_nextNotification < Time.time && SAINPlugin.DebugMode)
		{
			_nextNotification = Time.time + 0.1f;
			string text = (Error ? CreateErrorMessage(data) : data.ToString());
			NotificationManagerClass.DisplayMessageNotification(text, durationType, iconType, textColor);
		}
	}

	private static string CreateErrorMessage(object data)
	{
		StackTrace stackTrace = new StackTrace();
		int num = Mathf.Clamp(stackTrace.FrameCount, 0, 10);
		for (int i = 0; i < num; i++)
		{
			MethodBase methodBase = stackTrace.GetFrame(i)?.GetMethod();
			Type type = methodBase?.DeclaringType;
			if (type != null && type.DeclaringType != typeof(Logger))
			{
				return $"[{type} : {methodBase}]: ERROR: {data}";
			}
		}
		return data.ToString();
	}

	private static void Log(LogLevel level, object data)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Invalid comparison between Unknown and I4
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Invalid comparison between Unknown and I4
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		string text = string.Empty;
		Type type = null;
		if ((int)level != 32)
		{
			int maxFrames = GetMaxFrames(level);
			StackTrace stackTrace = new StackTrace(2);
			maxFrames = Mathf.Clamp(maxFrames, 0, stackTrace.FrameCount);
			for (int i = 0; i < maxFrames; i++)
			{
				MethodBase method = stackTrace.GetFrame(i).GetMethod();
				if (!(method.DeclaringType == typeof(Logger)))
				{
					if (type == null)
					{
						type = method.DeclaringType;
					}
					if (!GClass1437.IsNullOrEmpty(text))
					{
						text = "." + text;
					}
					text = method.Name + "()" + text;
				}
			}
			text = "[" + text + "]:";
		}
		string text2 = $"[{type}] : [{text}] : [{data}]";
		if (SAINLogger == null)
		{
			SAINLogger = Logger.CreateLogSource("SAIN");
		}
		if (((int)level != 2 && (int)level != 1) || (Object)(object)MonoBehaviourSingleton<PreloaderUI>.Instance?.Console != (Object)null)
		{
		}
		SAINLogger.Log(level, (object)text2);
	}

	private static int GetMaxFrames(LogLevel level)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected I4, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		switch (level - 1)
		{
		default:
			if ((int)level != 16 && (int)level != 32)
			{
				break;
			}
			return 1;
		case 3:
			return 2;
		case 1:
			return 3;
		case 0:
			return 4;
		case 2:
			break;
		}
		return 1;
	}
}
