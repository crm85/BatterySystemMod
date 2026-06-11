using System;
using System.Collections.Generic;
using EFT;
using SAIN.Components;
using SAIN.Components.BotController;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.Models.Structs;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class HearingInputClass : BotSubClass<SAINHearingSensorClass>, IBotClass, IDisposable
{
	private const float BOT_DEAF_TIME_INTERVAL = 1.5f;

	private const float DeafenCoef_Gunfire = 0.33f;

	private const float DeafenCoef_Suppressed = 0.33f;

	private const float DeafenCoef_Convo = 0.4f;

	private const float DeafenCoef_Generic = 0.3f;

	private const float IMPACT_HEAR_FREQUENCY = 0.5f;

	private const float IMPACT_HEAR_FREQUENCY_FAR = 0.05f;

	private const float IMPACT_MAX_HEAR_DISTANCE = 2500f;

	private const float IMPACT_DISPERSION = 25f;

	public readonly List<AISoundData> SoundDataToReactTo = new List<AISoundData>();

	public readonly List<AISoundData> AISoundCachedEvents = new List<AISoundData>();

	public readonly List<AISoundData> AISoundCachedEvents_Conversations = new List<AISoundData>();

	public readonly List<AISoundData> AISoundCachedEvents_Gunshots = new List<AISoundData>();

	public readonly List<AISoundData> AISoundCachedEvents_Gunshots_Suppressed = new List<AISoundData>();

	private float _BotDeafedTime = -1f;

	private bool _hearingStarted;

	private float _nextHearImpactTime;

	private float _ignoreUntilTime;

	public bool IgnoreUnderFire { get; private set; }

	public bool IgnoreHearing { get; private set; }

	public bool IsBotDeafened
	{
		get
		{
			if (_BotDeafedTime > 0f)
			{
				if (_BotDeafedTime < Time.time)
				{
					return true;
				}
				_BotDeafedTime = -1f;
			}
			return false;
		}
	}

	public event Action<AISoundData> OnFriendlySoundHeard;

	public HearingInputClass(SAINHearingSensorClass hearing)
		: base(hearing)
	{
	}

	public override void Init()
	{
		base.PlayerComponent.OnBulletFlyBy += OnBulletFlyBy;
		BotManagerComponent.Instance.BotHearing.BulletImpact += bulletImpacted;
		base.Init();
	}

	protected void OnBulletFlyBy(PlayerComponent Source, EftBulletClass Bullet)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = base.Bot.EnemyController.CheckAddEnemy((IPlayer)(object)Source.Player);
		if (enemy != null)
		{
			SoundEvent inSound = new SoundEvent(SAINSoundType.BulletImpact, Source.Position, Source, 100f, 1f, 999f, (EPhraseTrigger)0, (ETagStatus)1);
			AISoundData sound = new AISoundData(inSound, base.Bot, base.PlayerComponent.GetDistanceToPlayer(Source.ProfileId), enemy);
			Vector3 currentPosition = Bullet.CurrentPosition;
			Vector3 headPosition = base.Bot.Person.Transform.HeadPosition;
			Vector3 val = currentPosition - headPosition;
			float magnitude = ((Vector3)(ref val)).magnitude;
			base.BaseClass.ReactToBulletFlyBy(sound, magnitude);
		}
	}

	public override void ManualUpdate()
	{
		checkResetHearing();
		base.ManualUpdate();
	}

	public void CheckAddSoundToCache(SoundEvent Sound, float PlayerDistance)
	{
		if ((!Sound.SoundType.IsGunShot() && IgnoreHearing) || base.Bot.EnemyController == null)
		{
			return;
		}
		Enemy enemy = base.Bot.EnemyController.CheckAddEnemy((IPlayer)(object)Sound.GetPlayer());
		if (enemy != null || Sound.SoundType == SAINSoundType.Conversation)
		{
			AISoundData item = new AISoundData(Sound, base.Bot, PlayerDistance, enemy);
			switch (Sound.SoundType)
			{
			case SAINSoundType.Shot:
				AISoundCachedEvents_Gunshots.Add(item);
				break;
			case SAINSoundType.SuppressedShot:
				AISoundCachedEvents_Gunshots_Suppressed.Add(item);
				break;
			case SAINSoundType.Conversation:
				AISoundCachedEvents_Conversations.Add(item);
				break;
			default:
				AISoundCachedEvents.Add(item);
				break;
			}
		}
	}

	public void ProcessAISoundCache()
	{
		bool flag = false;
		bool previouslyDeaf = IsBotDeafened;
		if (AISoundCachedEvents_Gunshots.Count > 0 && ProcessGunshots(AISoundCachedEvents_Gunshots, previouslyDeaf: false, 0.33f, SoundDataToReactTo))
		{
			flag = true;
			previouslyDeaf = true;
		}
		if (AISoundCachedEvents_Gunshots_Suppressed.Count > 0 && ProcessGunshots(AISoundCachedEvents_Gunshots_Suppressed, previouslyDeaf, 0.33f, SoundDataToReactTo))
		{
			flag = true;
			previouslyDeaf = true;
		}
		if (AISoundCachedEvents_Conversations.Count > 0)
		{
			ProcessSounds(AISoundCachedEvents, previouslyDeaf, 0.4f, SoundDataToReactTo);
		}
		if (AISoundCachedEvents.Count > 0)
		{
			ProcessSounds(AISoundCachedEvents, previouslyDeaf, 0.3f, SoundDataToReactTo);
		}
		bool flag2 = false;
		for (int num = SoundDataToReactTo.Count - 1; num >= 0; num--)
		{
			if (SoundDataToReactTo[num].CanReport(0.2f))
			{
				TryReactToSound(SoundDataToReactTo[num]);
				SoundDataToReactTo.RemoveAt(num);
				flag2 = true;
			}
		}
		if (flag2)
		{
			SoundDataToReactTo.TrimExcess();
		}
		if (flag)
		{
			_BotDeafedTime = Time.time + 1.5f;
		}
	}

	private void TryReactToSound(AISoundData Sound)
	{
		if (Sound.Enemy != null)
		{
			base.BaseClass.ReactToHeardSound(Sound);
		}
		else
		{
			this.OnFriendlySoundHeard?.Invoke(Sound);
		}
	}

	private static bool ProcessSounds(List<AISoundData> Sounds, bool PreviouslyDeaf, float DeafenCoef, List<AISoundData> Results)
	{
		bool result = false;
		int count = Sounds.Count;
		if (count > 0)
		{
			Sounds.Sort((AISoundData a, AISoundData b) => a.PlayerDistance.CompareTo(b.PlayerDistance));
			for (int num = 0; num < count; num++)
			{
				AISoundData item = Sounds[num];
				if (item.PlayerDistance <= item.Sound.BaseRangeWithVolume && (!PreviouslyDeaf || !(item.PlayerDistance > item.Sound.BaseRangeWithVolume * DeafenCoef)))
				{
					Results.Add(item);
				}
			}
			Sounds.Clear();
		}
		return result;
	}

	private static bool ProcessGunshots(List<AISoundData> Sounds, bool previouslyDeaf, float DeafenCoef, List<AISoundData> Results)
	{
		bool flag = false;
		int count = Sounds.Count;
		if (count > 0)
		{
			Sounds.Sort((AISoundData a, AISoundData b) => a.PlayerDistance.CompareTo(b.PlayerDistance));
			for (int num = 0; num < count; num++)
			{
				AISoundData item = Sounds[num];
				if (!(item.PlayerDistance <= item.Sound.BaseRangeWithVolume))
				{
					continue;
				}
				bool flag2 = item.PlayerDistance <= item.Sound.BaseRangeWithVolume * DeafenCoef;
				if (!previouslyDeaf || flag2)
				{
					if (!flag)
					{
						flag = flag2;
					}
					Results.Add(item);
				}
			}
			Sounds.Clear();
		}
		return flag;
	}

	private void checkResetHearing()
	{
		if (!IgnoreHearing)
		{
			if (IgnoreUnderFire)
			{
				IgnoreUnderFire = false;
			}
			return;
		}
		if (_ignoreUntilTime > 0f && _ignoreUntilTime < Time.time)
		{
			IgnoreHearing = false;
			IgnoreUnderFire = false;
			return;
		}
		EnemyList enemyList = base.Bot.EnemyController.EnemyLists.GetEnemyList(EEnemyListType.Visible);
		if (enemyList != null && enemyList.Count > 0)
		{
			IgnoreHearing = false;
			IgnoreUnderFire = false;
		}
	}

	public override void Dispose()
	{
		BotManagerComponent.Instance.BotHearing.BulletImpact -= bulletImpacted;
		base.PlayerComponent.OnBulletFlyBy -= OnBulletFlyBy;
		base.Dispose();
	}

	private void soundHeard(SAINSoundType soundType, Vector3 soundPosition, PlayerComponent playerComponent, float power, float volume)
	{
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		if (volume <= 0f || !canHearSounds() || playerComponent.ProfileId == base.Bot.ProfileId || !soundListenerStarted(playerComponent))
		{
			return;
		}
		bool flag = soundType.IsGunShot();
		if (IgnoreHearing && !flag)
		{
			return;
		}
		Enemy enemy = base.Bot.EnemyController.GetEnemy(playerComponent.ProfileId, mustBeActive: true);
		if (enemy == null)
		{
			if (base.BotOwner.BotsGroup.IsEnemy(playerComponent.IPlayer))
			{
				enemy = base.Bot.EnemyController.CheckAddEnemy(playerComponent.IPlayer);
			}
			if (enemy == null)
			{
				return;
			}
		}
		if (!base.PlayerComponent.AIData.PlayerLocation.InBunker)
		{
			SAINWeatherClass instance = SAINWeatherClass.Instance;
			if (instance != null)
			{
				power = ((base.PlayerComponent.Player.AIData.EnvironmentId != 0) ? (power * instance.RainSoundModifierIndoor) : (power * instance.RainSoundModifierOutdoor));
			}
		}
		float num = power * volume;
		if (flag || !(enemy.RealDistance > num))
		{
			SoundInfoData info = new SoundInfoData
			{
				SourcePlayer = playerComponent,
				IsAI = playerComponent.IsAI,
				Position = soundPosition,
				Power = power,
				Volume = volume,
				SoundType = soundType,
				IsGunShot = flag
			};
			BotSound botSound = new BotSound(info, enemy, num);
		}
	}

	private void bulletImpacted(EftBulletClass bullet)
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		if (!canHearSounds() || _nextHearImpactTime > Time.time || base.Bot.HasEnemy)
		{
			return;
		}
		IPlayerOwner player = bullet.Player;
		IPlayer val = ((player != null) ? player.iPlayer : null);
		if (val == null)
		{
			return;
		}
		Enemy enemy = base.Bot.EnemyController.GetEnemy(val.ProfileId, mustBeActive: true);
		if (enemy != null && soundListenerStarted(enemy.EnemyPlayerComponent) && base.Bot.PlayerComponent.AIData.PlayerLocation.InBunker == enemy.EnemyPlayerComponent.AIData.PlayerLocation.InBunker)
		{
			Vector3 val2 = bullet.CurrentPosition - base.Bot.Position;
			float sqrMagnitude = ((Vector3)(ref val2)).sqrMagnitude;
			if (sqrMagnitude > 2500f)
			{
				_nextHearImpactTime = Time.time + 0.05f;
				return;
			}
			_nextHearImpactTime = Time.time + 0.5f;
			float num = sqrMagnitude / 25f;
			Vector3 val3 = Random.onUnitSphere;
			val3.y = 0f;
			val3 = ((Vector3)(ref val3)).normalized * num;
			Vector3 position = enemy.EnemyPosition + val3;
			SAINHearingReport heard = new SAINHearingReport
			{
				position = position,
				soundType = SAINSoundType.BulletImpact,
				placeType = EEnemyPlaceType.Hearing,
				isDanger = (sqrMagnitude < 625f),
				shallReportToSquad = true
			};
			enemy.Hearing.SetHeard(heard);
		}
	}

	private bool canHearSounds()
	{
		if (!base.Bot.BotActive)
		{
			return false;
		}
		if (base.Bot.GameEnding)
		{
			return false;
		}
		return true;
	}

	private bool soundListenerStarted(PlayerComponent player)
	{
		if (!player.Person.AIInfo.IsAI)
		{
			return true;
		}
		if (!_hearingStarted)
		{
			if (!base.PlayerComponent.AIData.AISoundPlayer.SoundMakerStarted)
			{
				return false;
			}
			_hearingStarted = true;
		}
		return true;
	}

	public bool SetIgnoreHearingExternal(bool value, bool ignoreUnderFire, float duration, out string reason)
	{
		if (value)
		{
			Enemy enemy = base.Bot.Enemy;
			if (enemy != null && enemy.IsVisible)
			{
				reason = "Enemy Visible";
				return false;
			}
			if (base.BotOwner.Memory.IsUnderFire && !ignoreUnderFire)
			{
				reason = "Under Fire";
				return false;
			}
		}
		IgnoreUnderFire = ignoreUnderFire;
		IgnoreHearing = value;
		if (value && duration > 0f)
		{
			_ignoreUntilTime = Time.time + duration;
		}
		else
		{
			_ignoreUntilTime = -1f;
		}
		reason = string.Empty;
		return true;
	}
}
