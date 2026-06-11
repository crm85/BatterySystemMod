using EFT;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public struct AISoundData
{
	public bool Reported;

	public readonly SoundEvent Sound;

	public readonly BotComponent Bot;

	public readonly Enemy Enemy;

	public readonly float PlayerDistance;

	public readonly float SoundTravelTime;

	public readonly Player HeardPlayer => Sound.GetPlayer();

	public readonly PlayerComponent HeardPlayerComponent => Sound.PlayerComponent;

	public readonly SAINSoundType SoundType => Sound.SoundType;

	public readonly bool IsGunShot => Sound.IsGunShot;

	public readonly string HeardProfileId => Sound.ProfileId;

	public readonly bool IsAI => Sound.IsAI;

	public readonly int EnvironmentId => Sound.EnvironmentId;

	public Vector3 Position => Sound.Position;

	public AISoundData(SoundEvent InSound, BotComponent InBot, float InPlayerDistance, Enemy InEnemy)
	{
		Reported = false;
		Sound = InSound;
		Bot = InBot;
		Enemy = InEnemy;
		PlayerDistance = InPlayerDistance;
		SoundTravelTime = InPlayerDistance / InSound.SoundSpeed;
	}

	public readonly bool CanReport(float ReactionDelay)
	{
		return Sound.IsValid() && Time.time - Sound.TimeCreated >= SoundTravelTime + ReactionDelay;
	}
}
