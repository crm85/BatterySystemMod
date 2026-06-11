using EFT;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public readonly struct SoundEvent
{
	public readonly SAINSoundType SoundType;

	public readonly EPhraseTrigger Phrase;

	public readonly ETagStatus TagStatus;

	public readonly Vector3 Position;

	public readonly float SoundSpeed;

	public readonly float Range;

	public readonly float Volume;

	public readonly float BaseRangeWithVolume;

	public readonly PlayerComponent PlayerComponent;

	public readonly float TimeCreated;

	public readonly bool IsGunShot;

	public readonly string ProfileId;

	public readonly bool IsAI;

	public readonly int EnvironmentId;

	public SoundEvent(SAINSoundType InSoundType, Vector3 InPosition, PlayerComponent InPlayerComponent, float InRange, float InVolume, float InSoundSpeed, EPhraseTrigger InPhrase = (EPhraseTrigger)0, ETagStatus InTagStatus = (ETagStatus)1)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		SoundType = InSoundType;
		Phrase = InPhrase;
		TagStatus = InTagStatus;
		Position = InPosition;
		SoundSpeed = InSoundSpeed;
		Range = InRange;
		Volume = InVolume;
		BaseRangeWithVolume = InRange * InVolume;
		PlayerComponent = InPlayerComponent;
		TimeCreated = Time.time;
		IsGunShot = InSoundType.IsGunShot();
		ProfileId = InPlayerComponent.ProfileId;
		IsAI = InPlayerComponent.IsAI;
		EnvironmentId = InPlayerComponent.Player.AIData.EnvironmentId;
	}

	public bool IsValid()
	{
		return (Object)(object)PlayerComponent != (Object)null && PlayerComponent.IsActive;
	}

	public Player GetPlayer()
	{
		return PlayerComponent?.Player;
	}
}
