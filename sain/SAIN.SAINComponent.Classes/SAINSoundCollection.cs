using System.Collections.Generic;
using EFT;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SAINSoundCollection
{
	public class SAINSound
	{
		public readonly float TimeCreated;

		public string SourcePlayerProfileId;

		public Vector3 Position;

		public float SoundPower;

		public bool WasHeard;

		public bool BulletFelt;

		public float DistanceAtCreation;

		public bool IsTooFar;

		public bool IsTooOld;

		public bool IsCheckedByBot;

		public SAINSound()
		{
			TimeCreated = Time.time;
		}
	}

	private float getPlayerTimer;

	private const float expireTime = 60f;

	private const float expireDistSqr = 2500f;

	private const float cleanupFreq = 5f;

	private float randomizationFactor;

	private readonly List<SAINSound> SoundsToRemove = new List<SAINSound>();

	public IPlayer IPlayer { get; private set; }

	public int Count => SoundList.Count;

	public Player Player { get; private set; }

	public List<SAINSound> SoundList { get; private set; } = new List<SAINSound>();

	public float TimeCreated { get; private set; }

	public float TimeCleanedUp { get; private set; }

	public SAINSoundCollection(IPlayer iPlayer)
	{
		IPlayer = iPlayer;
		Player = GameWorldInfo.GetAlivePlayer(iPlayer);
		TimeCreated = Time.time;
		randomizationFactor = Random.Range(0.75f, 1.25f);
	}

	public void UpdatePlayer()
	{
		if (getPlayerTimer < Time.time && (Object)(object)Player == (Object)null && IPlayer != null)
		{
			getPlayerTimer = Time.time + 1f;
			Player = GameWorldInfo.GetAlivePlayer(IPlayer);
			if (!((Object)(object)Player == (Object)null))
			{
			}
		}
	}

	public void Cleanup(bool force = false)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		float time = Time.time;
		if (force)
		{
			SoundList.Clear();
		}
		else
		{
			if (!(TimeCleanedUp < time + 5f * randomizationFactor))
			{
				return;
			}
			TimeCleanedUp = time;
			UpdatePlayer();
			SoundsToRemove.Clear();
			for (int i = 0; i < SoundList.Count; i++)
			{
				SAINSound sAINSound = SoundList[i];
				if (!force && sAINSound != null && IPlayer != null)
				{
					if (sAINSound.TimeCreated + 60f < time)
					{
						sAINSound.IsTooOld = true;
					}
					else if ((Object)(object)Player != (Object)null)
					{
						Vector3 val = sAINSound.Position - Player.Position;
						float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
						if (sqrMagnitude > 2500f)
						{
							sAINSound.IsTooFar = true;
						}
					}
				}
				if (IPlayer == null || (Object)(object)Player == (Object)null || sAINSound.IsTooOld || sAINSound.IsTooFar)
				{
					SoundsToRemove.Add(sAINSound);
				}
			}
			if (SoundsToRemove.Count <= 0)
			{
				return;
			}
			string text = $"Cleaning up {SoundsToRemove.Count} sounds... ";
			bool debugHearing = SAINPlugin.DebugSettings.Logs.DebugHearing;
			for (int j = 0; j < SoundsToRemove.Count; j++)
			{
				SAINSound sAINSound2 = SoundsToRemove[j];
				if (debugHearing)
				{
					ESoundCleanupReason cleanupReason = GetCleanupReason(sAINSound2, IPlayer, Player, force);
					text += $" [ [{cleanupReason}] ]";
				}
				SoundList.Remove(sAINSound2);
			}
			SoundsToRemove.Clear();
			if (debugHearing)
			{
				Logger.LogDebug(text);
			}
		}
	}

	private static ESoundCleanupReason GetCleanupReason(SAINSound sound, IPlayer iplayer, Player player, bool forced)
	{
		ESoundCleanupReason result = ESoundCleanupReason.None;
		if (forced)
		{
			result = ESoundCleanupReason.Forced;
		}
		else if (iplayer == null)
		{
			result = ESoundCleanupReason.IPlayerNull;
		}
		else if ((Object)(object)player == (Object)null)
		{
			result = ESoundCleanupReason.PlayerNull;
		}
		else if (sound == null)
		{
			result = ESoundCleanupReason.SoundNull;
		}
		else if (sound.IsTooOld)
		{
			result = ESoundCleanupReason.TooOld;
		}
		else if (sound.IsTooFar)
		{
			result = ESoundCleanupReason.TooFar;
		}
		return result;
	}
}
