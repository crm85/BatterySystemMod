using System;
using System.Collections.Generic;
using EFT;
using SAIN.BotController.Classes;
using SAIN.Components;
using SAIN.Helpers;
using SAIN.SAINComponent.Classes.EnemyClasses;
using UnityEngine;

namespace SAIN.SAINComponent.Classes.Talk;

public class SAINBotTalkClass : BotComponentClassBase
{
	private BotTalkPackage? _botTalkPackage;

	private BotTalkPackage? _talkDelayPackage;

	private bool _talkCacheActive = false;

	private float _talkCacheTimer = 0f;

	private float _allTalkDelay = 0f;

	private float _nextGetHitTime;

	private float _timeCanTalk;

	private float _talkDelayTimer = 0f;

	private readonly Dictionary<EPhraseTrigger, PhraseInfo> _phraseDictionary = new Dictionary<EPhraseTrigger, PhraseInfo>();

	public bool CanTalk => base.Bot.Info.FileSettings.Mind.CanTalk && _timeCanTalk < Time.time;

	public bool IsSpeaking => base.Player.Speaker?.Speaking ?? false;

	public EnemyTalk EnemyTalk { get; private set; }

	public GroupTalk GroupTalk { get; private set; }

	public SAINBotTalkClass(BotComponent sain)
		: base(sain)
	{
		base.TickRequirement = ESAINTickState.OnlyNoSleep;
		PhraseObjectsAdd(_phraseDictionary);
		GroupTalk = new GroupTalk(sain);
		EnemyTalk = new EnemyTalk(sain);
		_timeCanTalk = Time.time + Random.Range(1f, 2f);
	}

	public override void Init()
	{
		GroupTalk.Init();
		EnemyTalk.Init();
		base.Init();
	}

	private void GetHit(DamageInfoStruct DamageInfoStruct, EBodyPart bodyPart, float floatVal)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)base.Player == (Object)null) && !((Object)(object)base.BotOwner == (Object)null) && !((Object)(object)base.Bot == (Object)null) && EFTMath.RandomBool(25f) && _nextGetHitTime < Time.time && GroupTalk.FriendIsClose)
		{
			_nextGetHitTime = Time.time + 1f;
			EPhraseTrigger phrase = (EPhraseTrigger)15;
			ETagStatus value = (ETagStatus)6;
			GroupSay(phrase, value, withGroupDelay: false, 100f);
		}
	}

	public override void ManualUpdate()
	{
		base.ManualUpdate();
		GroupTalk.ManualUpdate();
		if (!SAINPlugin.LoadedPreset.GlobalSettings.Talk.DisableBotTalkPatching && !IsSpeaking && CanTalk && _timeCanTalk < Time.time)
		{
			EnemyTalk.ManualUpdate();
			if (_allTalkDelay < Time.time)
			{
				checkTalk();
			}
		}
	}

	private void checkTalk()
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Invalid comparison between Unknown and I4
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Invalid comparison between Unknown and I4
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Invalid comparison between Unknown and I4
		BotTalkPackage? botTalkPackage = null;
		if (_talkCacheTimer < Time.time && _botTalkPackage.HasValue)
		{
			botTalkPackage = _botTalkPackage;
			_botTalkPackage = null;
			_talkCacheActive = false;
		}
		else if (_talkDelayTimer < Time.time && _talkDelayPackage.HasValue)
		{
			botTalkPackage = _talkDelayPackage;
			_talkDelayPackage = null;
		}
		if (!botTalkPackage.HasValue)
		{
			return;
		}
		_allTalkDelay = Time.time + base.Bot.Info.FileSettings.Mind.TalkFrequency;
		if (((int)botTalkPackage.Value.phraseInfo.Phrase == 67 || (int)botTalkPackage.Value.phraseInfo.Phrase == 62) && base.Bot.Squad.VisibleMembers != null && (Object)(object)base.Bot.Squad.LeaderComponent != (Object)null && base.Bot.Squad.VisibleMembers.Contains(base.Bot.Squad.LeaderComponent))
		{
			Enemy enemy = base.Bot.Enemy;
			if (enemy != null && !enemy.IsVisible)
			{
				if ((int)botTalkPackage.Value.phraseInfo.Phrase == 67)
				{
					base.Player.HandsController.ShowGesture((EInteraction)5);
				}
				else
				{
					base.Player.HandsController.ShowGesture((EInteraction)6);
				}
				return;
			}
		}
		tellSpeakerToSay(botTalkPackage.Value.phraseInfo.Phrase, botTalkPackage.Value.Mask);
	}

	public override void Dispose()
	{
		if ((Object)(object)base.Player != (Object)null)
		{
		}
		_phraseDictionary.Clear();
		GroupTalk.Dispose();
		EnemyTalk.Dispose();
		base.Dispose();
	}

	public bool CanSay(EPhraseTrigger trigger, bool withGroupDelay, bool skipCheck)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		PhraseSpeakerClass val = base.Player?.Speaker;
		if (val == null)
		{
			return false;
		}
		if ((int)trigger == 26 || (int)trigger == 9)
		{
			return true;
		}
		if (val.Speaking)
		{
			return false;
		}
		if (!CanTalk)
		{
			return false;
		}
		if (skipCheck)
		{
			return true;
		}
		if (!checkDictionaryDelay(trigger))
		{
			return false;
		}
		if (withGroupDelay && !base.BotOwner.BotsGroup.GroupTalk.CanSay(base.BotOwner, trigger))
		{
			return false;
		}
		return true;
	}

	public bool Say(EPhraseTrigger phrase, ETagStatus? additionalMask = null, bool withGroupDelay = false, bool skipCheck = false)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (SAINPlugin.LoadedPreset.GlobalSettings.Talk.DisableBotTalkPatching)
		{
			return false;
		}
		if (!CanSay(phrase, withGroupDelay, skipCheck))
		{
			return false;
		}
		ETagStatus mask = SetETagMask(additionalMask);
		if (skipCheck)
		{
			tellSpeakerToSay(phrase, mask);
			return true;
		}
		if (!_phraseDictionary.ContainsKey(phrase))
		{
			tellSpeakerToSay(phrase, mask);
			return true;
		}
		PhraseInfo phrase2 = _phraseDictionary[phrase];
		BotTalkPackage value = new BotTalkPackage(phrase2, mask);
		_botTalkPackage = CheckPriority(value, _botTalkPackage);
		if (!_talkCacheActive)
		{
			_talkCacheActive = true;
			_talkCacheTimer = Time.time + 0.25f;
		}
		return true;
	}

	public bool GroupSay(EPhraseTrigger phrase, ETagStatus? additionalMask = null, bool withGroupDelay = false, float chance = 60f)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		SquadPersonalitySettings squadPersonalitySettings = base.Bot.Squad.SquadInfo?.SquadPersonalitySettings;
		if (squadPersonalitySettings != null)
		{
			float num = squadPersonalitySettings.VocalizationLevel * 10f - 25f;
			chance += num;
		}
		return EFTMath.RandomBool(chance) && GroupTalk.FriendIsClose && Say(phrase, additionalMask, withGroupDelay);
	}

	public void TalkAfterDelay(EPhraseTrigger phrase, ETagStatus? mask = null, float delay = 0.5f)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (CanTalk && _timeCanTalk < Time.time)
		{
			if (!_phraseDictionary.ContainsKey(phrase))
			{
				Logger.LogWarning($"Phrase: [{phrase}] Not in Dictionary, adding it manually.");
				_phraseDictionary.Add(phrase, new PhraseInfo(phrase, 10, 5f));
			}
			BotTalkPackage value = new BotTalkPackage(_phraseDictionary[phrase], SetETagMask(mask));
			_talkDelayPackage = CheckPriority(value, _talkDelayPackage, out var ChangeTalk);
			if (ChangeTalk)
			{
				_talkDelayTimer = Time.time + delay;
			}
		}
	}

	private void tellSpeakerToSay(EPhraseTrigger phrase, ETagStatus mask)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		tellSpeakerToSay(phrase, mask, (int)mask == 4);
	}

	private void tellSpeakerToSay(EPhraseTrigger trigger, ETagStatus mask = (ETagStatus)0, bool aggressive = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Invalid comparison between Unknown and I4
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if ((int)trigger == 104)
		{
			trigger = (EPhraseTrigger)((aggressive || Time.time < base.Player.Awareness) ? 27 : 28);
		}
		ETagStatus val = (ETagStatus)((!aggressive && !(base.Player.Awareness > Time.time)) ? 1 : 4);
		if (base.PlayerComponent.PlayVoiceLine(trigger, (ETagStatus)(base.Bot.Memory.Health.HealthStatus | mask | val), aggressive))
		{
			BotManagerComponent.Instance?.BotHearing.PlayerTalked(trigger, val, base.Player);
			base.BotOwner.BotsGroup.GroupTalk.PhraseSad(base.BotOwner, trigger);
			if (_phraseDictionary.TryGetValue(trigger, out var value))
			{
				value.TimeLastSaid = Time.time;
			}
		}
	}

	private ETagStatus SetETagMask(ETagStatus? additionaMask = null)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected I4, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		ETagStatus val = ((base.BotOwner.BotsGroup.MembersCount <= 1) ? ((ETagStatus)8) : ((ETagStatus)16));
		if (base.BotOwner.Memory.IsUnderFire || base.Bot.Suppression.IsSuppressed || base.Bot.Suppression.IsHeavySuppressed)
		{
			val = (ETagStatus)(val | 4);
		}
		else if (base.Bot.Enemy == null)
		{
			val = (ETagStatus)(base.Bot.EnemyController.AtPeace ? (val | 1) : (val | 2));
		}
		else
		{
			val = (ETagStatus)((!base.Bot.Enemy.Seen || !(base.Bot.Enemy.TimeSinceSeen < 30f)) ? (val | 2) : (val | 4));
			EPlayerSide side = base.Bot.Enemy.EnemyIPlayer.Side;
			EPlayerSide val2 = side;
			switch (val2 - 1)
			{
			case 0:
				val = (ETagStatus)(val | 0x40);
				break;
			case 1:
				val = (ETagStatus)(val | 0x20);
				break;
			case 3:
				val = (ETagStatus)(val | 0x80);
				break;
			}
		}
		if (additionaMask.HasValue)
		{
			val |= additionaMask.Value;
		}
		return val;
	}

	private bool checkDictionaryDelay(EPhraseTrigger trigger)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!_phraseDictionary.ContainsKey(trigger))
		{
			_phraseDictionary.Add(trigger, new PhraseInfo(trigger, 10, 5f));
		}
		if (_phraseDictionary.ContainsKey(trigger))
		{
			PhraseInfo phraseInfo = _phraseDictionary[trigger];
			if (phraseInfo.TimeLastSaid + phraseInfo.TimeDelay < Time.time)
			{
				return true;
			}
		}
		return false;
	}

	private BotTalkPackage? CheckPriority(BotTalkPackage? newTalk, BotTalkPackage? oldTalk)
	{
		if (!oldTalk.HasValue)
		{
			return newTalk;
		}
		if (!newTalk.HasValue)
		{
			return oldTalk;
		}
		int priority = newTalk.Value.phraseInfo.Priority;
		int priority2 = oldTalk.Value.phraseInfo.Priority;
		return (priority2 < priority) ? newTalk : oldTalk;
	}

	private BotTalkPackage? CheckPriority(BotTalkPackage? newTalk, BotTalkPackage? oldTalk, out bool ChangeTalk)
	{
		if (!oldTalk.HasValue)
		{
			ChangeTalk = true;
			return newTalk;
		}
		if (!newTalk.HasValue)
		{
			ChangeTalk = false;
			return oldTalk;
		}
		int priority = newTalk.Value.phraseInfo.Priority;
		int priority2 = oldTalk.Value.phraseInfo.Priority;
		ChangeTalk = priority2 < priority;
		return ChangeTalk ? newTalk : oldTalk;
	}

	private static void PhraseObjectsAdd(Dictionary<EPhraseTrigger, PhraseInfo> dictionary)
	{
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		AddPhrase((EPhraseTrigger)10, 1, 60f, dictionary);
		AddPhrase((EPhraseTrigger)29, 3, 15f, dictionary);
		AddPhrase((EPhraseTrigger)78, 4, 3f, dictionary);
		AddPhrase((EPhraseTrigger)82, 5, 120f, dictionary);
		AddPhrase((EPhraseTrigger)28, 6, 20f, dictionary);
		AddPhrase((EPhraseTrigger)18, 7, 10f, dictionary);
		AddPhrase((EPhraseTrigger)17, 8, 30f, dictionary);
		AddPhrase((EPhraseTrigger)33, 9, 40f, dictionary);
		AddPhrase((EPhraseTrigger)34, 10, 40f, dictionary);
		AddPhrase((EPhraseTrigger)59, 11, 60f, dictionary);
		AddPhrase((EPhraseTrigger)27, 38, 1f, dictionary);
		AddPhrase((EPhraseTrigger)61, 37, 1f, dictionary);
		AddPhrase((EPhraseTrigger)19, 13, 3f, dictionary);
		AddPhrase((EPhraseTrigger)13, 14, 10f, dictionary);
		AddPhrase((EPhraseTrigger)21, 15, 5f, dictionary);
		AddPhrase((EPhraseTrigger)12, 16, 5f, dictionary);
		AddPhrase((EPhraseTrigger)16, 17, 35f, dictionary);
		AddPhrase((EPhraseTrigger)106, 18, 75f, dictionary);
		AddPhrase((EPhraseTrigger)53, 19, 60f, dictionary);
		AddPhrase((EPhraseTrigger)23, 20, 10f, dictionary);
		AddPhrase((EPhraseTrigger)20, 21, 15f, dictionary);
		AddPhrase((EPhraseTrigger)54, 22, 60f, dictionary);
		AddPhrase((EPhraseTrigger)55, 23, 30f, dictionary);
		AddPhrase((EPhraseTrigger)49, 24, 30f, dictionary);
		AddPhrase((EPhraseTrigger)48, 25, 30f, dictionary);
		AddPhrase((EPhraseTrigger)56, 26, 20f, dictionary);
		AddPhrase((EPhraseTrigger)14, 27, 10f, dictionary);
		AddPhrase((EPhraseTrigger)81, 28, 2f, dictionary);
		AddPhrase((EPhraseTrigger)88, 29, 30f, dictionary);
		AddPhrase((EPhraseTrigger)45, 30, 40f, dictionary);
		AddPhrase((EPhraseTrigger)69, 31, 5f, dictionary);
		AddPhrase((EPhraseTrigger)72, 32, 5f, dictionary);
		AddPhrase((EPhraseTrigger)89, 33, 15f, dictionary);
		AddPhrase((EPhraseTrigger)101, 34, 15f, dictionary);
		AddPhrase((EPhraseTrigger)22, 35, 10f, dictionary);
		AddPhrase((EPhraseTrigger)11, 36, 10f, dictionary);
		AddPhrase((EPhraseTrigger)38, 37, 1f, dictionary);
		AddPhrase((EPhraseTrigger)15, 38, 1f, dictionary);
		AddPhrase((EPhraseTrigger)9, 39, 1f, dictionary);
		AddPhrase((EPhraseTrigger)26, 40, 1f, dictionary);
		AddPhrase((EPhraseTrigger)47, 10, 80f, dictionary);
		AddPhrase((EPhraseTrigger)74, 15, 10f, dictionary);
		AddPhrase((EPhraseTrigger)73, 15, 20f, dictionary);
		AddPhrase((EPhraseTrigger)31, 15, 45f, dictionary);
		AddPhrase((EPhraseTrigger)36, 6, 60f, dictionary);
		AddPhrase((EPhraseTrigger)43, 20, 15f, dictionary);
		AddPhrase((EPhraseTrigger)67, 10, 30f, dictionary);
		AddPhrase((EPhraseTrigger)62, 10, 30f, dictionary);
		AddPhrase((EPhraseTrigger)8, 1, 1f, dictionary);
		AddPhrase((EPhraseTrigger)35, 25, 30f, dictionary);
		AddPhrase((EPhraseTrigger)40, 25, 15f, dictionary);
		AddPhrase((EPhraseTrigger)68, 25, 30f, dictionary);
		AddPhrase((EPhraseTrigger)30, 25, 45f, dictionary);
		AddPhrase((EPhraseTrigger)80, 5, 120f, dictionary);
		AddPhrase((EPhraseTrigger)105, 34, 5f, dictionary);
		AddPhrase((EPhraseTrigger)104, 10, 35f, dictionary);
		AddPhrase((EPhraseTrigger)32, 10, 45f, dictionary);
		AddPhrase((EPhraseTrigger)94, 5, 30f, dictionary);
		AddPhrase((EPhraseTrigger)95, 5, 30f, dictionary);
		AddPhrase((EPhraseTrigger)96, 5, 30f, dictionary);
		AddPhrase((EPhraseTrigger)92, 5, 30f, dictionary);
		AddPhrase((EPhraseTrigger)97, 5, 30f, dictionary);
		AddPhrase((EPhraseTrigger)100, 5, 30f, dictionary);
		AddPhrase((EPhraseTrigger)98, 5, 30f, dictionary);
		AddPhrase((EPhraseTrigger)65, 5, 30f, dictionary);
		foreach (EPhraseTrigger value in Enum.GetValues(typeof(EPhraseTrigger)))
		{
			AddPhrase(value, 25, 5f, dictionary);
		}
	}

	private static void AddPhrase(EPhraseTrigger phrase, int priority, float timeDelay, Dictionary<EPhraseTrigger, PhraseInfo> dictionary)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!dictionary.ContainsKey(phrase))
		{
			dictionary.Add(phrase, new PhraseInfo(phrase, priority, timeDelay));
		}
	}
}
