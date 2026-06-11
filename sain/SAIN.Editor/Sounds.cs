using Comfort.Common;
using EFT.UI;
using HarmonyLib;
using UnityEngine;

namespace SAIN.Editor;

internal class Sounds
{
	private static UISoundsWrapper _soundsWrapper;

	private static AudioSource _audioSource;

	private static float SoundLimiter;

	private static GUISounds GUISounds => Singleton<GUISounds>.Instance;

	private static void getWrapper()
	{
		object value = AccessTools.Field(typeof(GUISounds), "uisoundsWrapper_0").GetValue(GUISounds);
		_soundsWrapper = (UISoundsWrapper)((value is UISoundsWrapper) ? value : null);
		object value2 = AccessTools.Field(typeof(GUISounds), "audioSource_0").GetValue(GUISounds);
		_audioSource = (AudioSource)((value2 is AudioSource) ? value2 : null);
	}

	public static void PlaySound(EUISoundType soundType, float volume = 1f)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		volume = Mathf.Clamp(volume, 0f, 1f);
		if ((Object)(object)_soundsWrapper == (Object)null)
		{
			getWrapper();
		}
		if (SoundLimiter < Time.time)
		{
			SoundLimiter = Time.time + 0.05f;
			playSound(soundType, volume);
		}
	}

	private static void playSound(EUISoundType soundType, float volume)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_soundsWrapper == (Object)null || (Object)(object)_audioSource == (Object)null)
		{
			Logger.LogWarning("null");
			Singleton<GUISounds>.Instance.PlayUISound(soundType);
		}
		else
		{
			AudioClip uIClip = _soundsWrapper.GetUIClip(soundType);
			if ((Object)(object)uIClip == (Object)null)
			{
				return;
			}
			_audioSource.PlayOneShot(uIClip, volume);
		}
		if (SAINPlugin.DebugMode)
		{
			Logger.LogDebug(soundType);
		}
	}
}
