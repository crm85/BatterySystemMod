using SAIN.Components.PlayerComponentSpace;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public struct SoundInfoData
{
	public PlayerComponent SourcePlayer;

	public bool IsAI;

	public Vector3 Position;

	public SAINSoundType SoundType;

	public bool IsGunShot;

	public float Power;

	public float Volume;
}
