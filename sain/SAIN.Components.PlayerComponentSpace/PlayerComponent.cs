using System;
using System.Collections;
using System.Collections.Generic;
using EFT;
using EFT.Ballistics;
using EFT.Interactive;
using EFT.InventoryLogic;
using SAIN.Classes;
using SAIN.Components.BotController;
using SAIN.Components.BotControllerSpace.Classes.Raycasts;
using SAIN.Components.PlayerComponentSpace.Classes;
using SAIN.Components.PlayerComponentSpace.Classes.Equipment;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.Helpers;
using SAIN.SAINComponent.Classes.Info;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Components.PlayerComponentSpace;

public class PlayerComponent : MonoBehaviour, IDisposable
{
	private const int MaxCachedSounds = 4;

	private readonly List<int> _aggroIndexes = new List<int>();

	private GUIObject _hitLabel;

	private Coroutine _gearCoroutine;

	private Item _currentItem = null;

	private Weapon _currentWeapon = null;

	public string ProfileId { get; private set; }

	public FlashLightClass Flashlight { get; private set; }

	public PersonClass Person { get; private set; }

	public SAINAIData AIData { get; private set; }

	public SAINEquipmentClass Equipment { get; private set; }

	public bool IsActive => Person.Active;

	public Vector3 Position => Person.Transform.Position;

	public Vector3 LookDirection => Person.Transform.LookDirection;

	public Vector3 LookSensorPosition => Transform.EyePosition;

	public PersonTransformClass Transform => Person.Transform;

	public Player Player => Person.Player;

	public IPlayer IPlayer => Person.IPlayer;

	public string Name => Person.Name;

	public BotOwner BotOwner => Person.AIInfo.BotOwner;

	public BotComponent BotComponent => Person.AIInfo.BotComponent;

	public bool IsAI => Person.AIInfo.IsAI;

	public bool IsSAINBot => Person.AIInfo.IsSAINBot;

	public PlayerMovementSmoothingClass SmoothController { get; private set; } = new PlayerMovementSmoothingClass();

	public PlayerTickData PlayerTickData { get; private set; }

	public OtherPlayersData OtherPlayersData { get; private set; }

	public BodyPartsClass BodyParts { get; private set; }

	public Weapon CurrentWeapon
	{
		get
		{
			return _currentWeapon;
		}
		private set
		{
			if (_currentWeapon != value)
			{
				Weapon currentWeapon = _currentWeapon;
				string[] obj = new string[7] { "[", null, null, null, null, null, null };
				Player player = Player;
				obj[1] = ((player != null) ? player.Profile.Nickname : null);
				obj[2] = "] Equipped Weapon [";
				obj[3] = ((value != null) ? ((Item)value).ShortName : null);
				obj[4] = "] Last Weapon [";
				obj[5] = ((currentWeapon != null) ? ((Item)currentWeapon).ShortName : null);
				obj[6] = "]";
				Logger.LogDebug(string.Concat(obj));
				_currentWeapon = value;
				this.OnWeaponEquipped?.Invoke(value, currentWeapon);
			}
		}
	}

	public Item ItemInHands
	{
		get
		{
			return _currentItem;
		}
		private set
		{
			if (_currentItem != value)
			{
				Item currentItem = _currentItem;
				string[] obj = new string[7] { "[", null, null, null, null, null, null };
				Player player = Player;
				obj[1] = ((player != null) ? player.Profile.Nickname : null);
				obj[2] = "] Equipped Item [";
				obj[3] = ((value != null) ? value.ShortName : null);
				obj[4] = "] Last Item [";
				obj[5] = ((currentItem != null) ? currentItem.ShortName : null);
				obj[6] = "]";
				Logger.LogDebug(string.Concat(obj));
				_currentItem = value;
				this.OnItemEquipped?.Invoke(value, currentItem);
			}
		}
	}

	public List<SoundEvent> AISoundCachedEvents { get; private set; } = new List<SoundEvent>();

	public event Action<WeaponInfo, Vector3> OnShoot;

	public event Action<PlayerComponent, EftBulletClass> OnBulletFlyBy;

	public event Action<string> OnComponentDestroyed;

	public event Action<Weapon, Weapon> OnWeaponEquipped;

	public event Action<Item, Item> OnItemEquipped;

	public void SetItemEquippedInHands(Item Item)
	{
		ItemInHands = Item;
		CurrentWeapon = (Weapon)(object)((Item is Weapon) ? Item : null);
	}

	public void PlayAISound(SAINSoundType InSoundType, Vector3 InPosition, float InRange, float InVolume, EPhraseTrigger Phrase = (EPhraseTrigger)0, ETagStatus TagStatus = (ETagStatus)1)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (IsActive && AIData.AISoundPlayer.ShallPlayAISound())
		{
			if (Player.IsYourPlayer)
			{
				Logger.LogDebug($"Sound Cached: [{InSoundType}, {InRange}, {InVolume}]");
			}
			AddCachedAISoundEvent(InSoundType, InPosition, InRange, InVolume, Phrase, TagStatus);
		}
	}

	protected void AddCachedAISoundEvent(SAINSoundType InSoundType, Vector3 InPosition, float InRange, float InVolume, EPhraseTrigger Phrase = (EPhraseTrigger)0, ETagStatus TagStatus = (ETagStatus)1)
	{
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		float inSoundSpeed = 343f;
		if (InSoundType.IsGunShot())
		{
			WeaponInfo weaponInfo = Equipment?.CurrentWeaponInfo;
			if (weaponInfo != null)
			{
				inSoundSpeed = weaponInfo.BulletSpeed;
			}
			InVolume *= SAINPlugin.LoadedPreset.GlobalSettings.Hearing.GunshotAudioMultiplier;
		}
		else
		{
			InVolume *= SAINPlugin.LoadedPreset.GlobalSettings.Hearing.FootstepAudioMultiplier;
		}
		if (!AIData.PlayerLocation.InBunker)
		{
			SAINWeatherClass instance = SAINWeatherClass.Instance;
			if (instance != null)
			{
				InVolume = ((Player.AIData.EnvironmentId != 0) ? (InVolume * instance.RainSoundModifierIndoor) : (InVolume * instance.RainSoundModifierOutdoor));
			}
		}
		int count = AISoundCachedEvents.Count;
		if (count >= 4)
		{
			bool flag = false;
			float num = InRange * InVolume;
			for (int i = 0; i < count; i++)
			{
				if (!(num < AISoundCachedEvents[i].BaseRangeWithVolume))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				AISoundCachedEvents.Add(new SoundEvent(InSoundType, InPosition, this, InRange, InVolume, inSoundSpeed, Phrase, TagStatus));
				AISoundCachedEvents.Sort((SoundEvent a, SoundEvent b) => b.BaseRangeWithVolume.CompareTo(a.BaseRangeWithVolume));
				AISoundCachedEvents.RemoveAt(AISoundCachedEvents.Count - 1);
			}
		}
		else
		{
			AISoundCachedEvents.Add(new SoundEvent(InSoundType, InPosition, this, InRange, InVolume, inSoundSpeed, Phrase, TagStatus));
		}
	}

	public float GetDistanceToPlayer(string ProfileId)
	{
		if (OtherPlayersData.DataDictionary.TryGetValue(ProfileId, out var value))
		{
			return value.DistanceData.Distance;
		}
		return float.MaxValue;
	}

	public void ManualUpdate(float currentTime, float deltaTime)
	{
		Person.Update();
		if (!Person.ActivationClass.PlayerActive)
		{
			return;
		}
		if (IsAI)
		{
			BotOwner botOwner = Person.AIInfo.BotOwner;
			if ((Object)(object)botOwner != (Object)null)
			{
				SmoothController.ManualUpdate(currentTime, deltaTime, Player, botOwner, Person.AIInfo.BotComponent);
			}
		}
		if (!IsAI || Person.ActivationClass.BotActive)
		{
			drawTransformGizmos();
			Flashlight.Update();
			Equipment.Update();
		}
	}

	public void ManualLateUpdate()
	{
		Person?.LateUpdate();
	}

	public PlayerTickData GetPreparedTickData()
	{
		PlayerTickData playerTickData = PlayerTickData;
		playerTickData.Prepare(this);
		PlayerTickData = playerTickData;
		return playerTickData;
	}

	public void SetTickData(PlayerTickData data)
	{
		PlayerTickData = data;
	}

	public void RegisterFlyBy(PlayerComponent Source, EftBulletClass Bullet)
	{
		this.OnBulletFlyBy?.Invoke(Source, Bullet);
	}

	public void OnMakingShot(IWeapon Weapon, Vector3 Force)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (Player.IsYourPlayer)
		{
		}
		if (!IsActive)
		{
			return;
		}
		WeaponInfo currentWeaponInfo = Equipment.CurrentWeaponInfo;
		if (currentWeaponInfo != null)
		{
			this.OnShoot?.Invoke(currentWeaponInfo, Force);
			PlayAISound(currentWeaponInfo.SoundType, Transform.WeaponFirePort, currentWeaponInfo.CalculatedAudibleRange, 1f, (EPhraseTrigger)0, (ETagStatus)1);
			if (!Player.IsYourPlayer)
			{
			}
		}
	}

	private void StartCoroutines()
	{
		if (_gearCoroutine == null)
		{
			_gearCoroutine = ((MonoBehaviour)this).StartCoroutine(Equipment.GearInfo.GearUpdateLoop());
		}
	}

	private void StopCoroutines()
	{
		if (_gearCoroutine != null)
		{
			((MonoBehaviour)this).StopCoroutine(_gearCoroutine);
			_gearCoroutine = null;
		}
		((MonoBehaviour)this).StopAllCoroutines();
	}

	public bool Init(IPlayer iPlayer)
	{
		ProfileId = iPlayer.ProfileId;
		try
		{
			PlayerData playerData = new PlayerData(this, (Player)(object)((iPlayer is Player) ? iPlayer : null), iPlayer);
			Person = new PersonClass(playerData);
			OtherPlayersData = new OtherPlayersData(this);
			PlayerTickData = new PlayerTickData(this);
			BodyParts = new BodyPartsClass(this);
			Flashlight = new FlashLightClass(this);
			Equipment = new SAINEquipmentClass(this);
			AIData = new SAINAIData(Equipment.GearInfo, this);
			Person.ActivationClass.OnPlayerActiveChanged += handleCoroutines;
			handleCoroutines(active: true);
		}
		catch (Exception data)
		{
			Logger.LogError(data);
			return false;
		}
		((MonoBehaviour)this).StartCoroutine(DelayInit());
		return true;
	}

	private void handleCoroutines(bool active)
	{
		if (active)
		{
			StartCoroutines();
		}
		else
		{
			StopCoroutines();
		}
	}

	private IEnumerator DelayInit()
	{
		yield return null;
		Equipment.Init();
		AbstractHandsController handsController = Player.HandsController;
		if (((handsController != null) ? handsController.Item : null) != null)
		{
			SetItemEquippedInHands(Player.HandsController.Item);
		}
	}

	public void InitBotOwner(BotOwner botOwner)
	{
		Person.ActivationClass.OnPlayerActiveChanged -= handleCoroutines;
		Person.ActivationClass.OnBotActiveChanged += handleCoroutines;
		Person.InitBot(botOwner);
	}

	public void InitBotComponent(BotComponent bot)
	{
		Person.InitBot(bot);
	}

	private void OnDisable()
	{
		Person.ActivationClass.Disable();
		StopCoroutines();
	}

	public void Dispose()
	{
		Logger.LogDebug($"Destroying Playing Component for [Name: {Person?.Name} : Nickname: {Person?.Nickname}, ProfileID: {Person?.ProfileId}, at time: {Time.time}]");
		this.OnComponentDestroyed?.Invoke(ProfileId);
		StopCoroutines();
		Person.ActivationClass.OnBotActiveChanged -= handleCoroutines;
		Person.ActivationClass.OnPlayerActiveChanged -= handleCoroutines;
		Equipment?.Dispose();
		OtherPlayersData?.Dispose();
		Object.Destroy((Object)(object)this);
	}

	private void navRayCastAllDir()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (!SAINPlugin.DebugMode || !SAINPlugin.DrawDebugGizmos || !Player.IsYourPlayer)
		{
			return;
		}
		Vector3 position = Position;
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(position, ref val, 1f, -1))
		{
			position = ((NavMeshHit)(ref val)).position;
		}
		int num = 5;
		NavMeshHit val4 = default(NavMeshHit);
		for (int i = 0; i < num; i++)
		{
			Vector3 val2 = Random.onUnitSphere;
			val2.y = 0f;
			val2 = ((Vector3)(ref val2)).normalized * 30f;
			Vector3 val3 = position + val2;
			if (NavMesh.Raycast(position, val3, ref val4, -1))
			{
				val3 = ((NavMeshHit)(ref val4)).position;
			}
			DebugGizmos.Line(position, val3, 0.05f, 0.25f, taperLine: true);
		}
	}

	private void testObjectInFront()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Unknown result type (might be due to invalid IL or missing references)
		if (!Player.IsYourPlayer)
		{
			return;
		}
		if (_hitLabel == null)
		{
			_hitLabel = DebugGizmos.CreateLabel(Vector3.zero, string.Empty);
		}
		if (_hitLabel == null)
		{
			return;
		}
		_hitLabel.StringBuilder.Clear();
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(Transform.EyePosition, Transform.LookDirection, ref val, 100f, LayerMaskClass.DoorLayer))
		{
			_hitLabel.Enabled = true;
			_hitLabel.WorldPos = ((RaycastHit)(ref val)).point;
			_hitLabel.StringBuilder.AppendLine(((Object)((Component)((RaycastHit)(ref val)).collider).gameObject).name ?? "");
			_hitLabel.StringBuilder.AppendLine(LayerMask.LayerToName(((Component)((RaycastHit)(ref val)).collider).gameObject.layer) ?? "");
			_hitLabel.StringBuilder.AppendLine($"{((RaycastHit)(ref val)).distance}");
			Door component = ((Component)((RaycastHit)(ref val)).collider).gameObject.GetComponent<Door>();
			if ((Object)(object)component != (Object)null)
			{
				_hitLabel.StringBuilder.AppendLine("Found Door: [" + ((WorldInteractiveObject)component).Id + "]");
			}
			NavMeshDoorLink component2 = ((Component)((RaycastHit)(ref val)).collider).gameObject.GetComponent<NavMeshDoorLink>();
			if ((Object)(object)component2 != (Object)null)
			{
				_hitLabel.StringBuilder.AppendLine($"Found Link: [{component2.Id}]");
			}
		}
		if (Physics.Raycast(Transform.EyePosition, Transform.LookDirection, ref val, 100f, LayerMask.op_Implicit(LayerMaskClass.PlayerStaticDoorMask)))
		{
			_hitLabel.Enabled = true;
			_hitLabel.WorldPos = ((RaycastHit)(ref val)).point;
			_hitLabel.StringBuilder.AppendLine(((Object)((Component)((RaycastHit)(ref val)).collider).gameObject).name ?? "");
			_hitLabel.StringBuilder.AppendLine(LayerMask.LayerToName(((Component)((RaycastHit)(ref val)).collider).gameObject.layer) ?? "");
			_hitLabel.StringBuilder.AppendLine($"{((RaycastHit)(ref val)).distance}");
			Door component3 = ((Component)((RaycastHit)(ref val)).collider).gameObject.GetComponent<Door>();
			if ((Object)(object)component3 != (Object)null)
			{
				_hitLabel.StringBuilder.AppendLine("Found Door: [" + ((WorldInteractiveObject)component3).Id + "]");
			}
			NavMeshDoorLink component4 = ((Component)((RaycastHit)(ref val)).collider).gameObject.GetComponent<NavMeshDoorLink>();
			if ((Object)(object)component4 != (Object)null)
			{
				_hitLabel.StringBuilder.AppendLine($"Found Link: [{component4.Id}]");
			}
		}
		if (Physics.Raycast(Transform.EyePosition, Transform.LookDirection, ref val, 100f, LayerMask.op_Implicit(LayerMaskClass.InteractiveMask)))
		{
			_hitLabel.Enabled = true;
			_hitLabel.WorldPos = ((RaycastHit)(ref val)).point;
			_hitLabel.StringBuilder.AppendLine(((Object)((Component)((RaycastHit)(ref val)).collider).gameObject).name ?? "");
			_hitLabel.StringBuilder.AppendLine(LayerMask.LayerToName(((Component)((RaycastHit)(ref val)).collider).gameObject.layer) ?? "");
			_hitLabel.StringBuilder.AppendLine($"{((RaycastHit)(ref val)).distance}");
			Door component5 = ((Component)((RaycastHit)(ref val)).collider).gameObject.GetComponent<Door>();
			if ((Object)(object)component5 != (Object)null)
			{
				_hitLabel.StringBuilder.AppendLine("Found Door: [" + ((WorldInteractiveObject)component5).Id + "]");
			}
			NavMeshDoorLink component6 = ((Component)((RaycastHit)(ref val)).collider).gameObject.GetComponent<NavMeshDoorLink>();
			if ((Object)(object)component6 != (Object)null)
			{
				_hitLabel.StringBuilder.AppendLine($"Found Link: [{component6.Id}]");
			}
		}
		else if (Physics.Raycast(Transform.EyePosition, Transform.LookDirection, ref val, 100f, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMaskAI)))
		{
			_hitLabel.Enabled = true;
			_hitLabel.WorldPos = ((RaycastHit)(ref val)).point;
			_hitLabel.StringBuilder.AppendLine(((Object)((Component)((RaycastHit)(ref val)).collider).gameObject).name ?? "");
			_hitLabel.StringBuilder.AppendLine(LayerMask.LayerToName(((Component)((RaycastHit)(ref val)).collider).gameObject.layer) ?? "");
			_hitLabel.StringBuilder.AppendLine($"{((RaycastHit)(ref val)).distance}");
			BallisticCollider component7 = ((Component)((RaycastHit)(ref val)).collider).gameObject.GetComponent<BallisticCollider>();
			if ((Object)(object)component7 != (Object)null)
			{
				_hitLabel.StringBuilder.AppendLine($"Found Ballistic: [{((Object)component7).name}, {component7.PenetrationChance}, {component7.PenetrationLevel}]");
			}
			Component[] componentsInChildren = ((Component)((RaycastHit)(ref val)).collider).gameObject.GetComponentsInChildren(typeof(Component));
			Component[] array = componentsInChildren;
			foreach (Component val2 in array)
			{
				_hitLabel.StringBuilder.AppendLine($"Found [{((Object)val2).name}] : Type [{((object)val2).GetType()}]");
			}
		}
		else
		{
			_hitLabel.Enabled = false;
		}
		if (_hitLabel.Enabled)
		{
			DebugGizmos.Sphere(_hitLabel.WorldPos, 0.025f, 0.05f);
		}
	}

	private void testNavMeshNodes()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		List<Vector3> list = new List<Vector3>();
		Vector3 eyePosition = Transform.EyePosition;
		Vector3[] vertices = NavMesh.CalculateTriangulation().vertices;
		Vector3[] array = vertices;
		foreach (Vector3 val in array)
		{
			Vector3 val2 = val - eyePosition;
			float sqrMagnitude = ((Vector3)(ref val2)).sqrMagnitude;
			if (sqrMagnitude > 10000f)
			{
				continue;
			}
			float num = Mathf.Sqrt(sqrMagnitude);
			if (!Physics.Raycast(eyePosition, val2, num, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
			{
				list.Add(val);
				continue;
			}
			val2.y += 0.5f;
			if (!Physics.Raycast(eyePosition, val2, num, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
			{
				list.Add(val);
				continue;
			}
			val2.y += 0.5f;
			if (!Physics.Raycast(eyePosition, val2, num, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
			{
				list.Add(val);
				continue;
			}
			val2.y += 0.5f;
			if (!Physics.Raycast(eyePosition, val2, num, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMask)))
			{
				list.Add(val);
			}
		}
		foreach (Vector3 item in list)
		{
			DebugGizmos.Ray(item, Vector3.up, Color.green, 1.5f, 0.025f, temporary: true, 0.25f);
		}
	}

	private IEnumerator voiceTest()
	{
		while (true)
		{
			yield return playPhrases((EPhraseTrigger)111);
			yield return playPhrases((EPhraseTrigger)1);
			yield return playPhrases((EPhraseTrigger)88);
			yield return playPhrases((EPhraseTrigger)108);
			yield return playPhrases((EPhraseTrigger)107);
			yield return playPhrases((EPhraseTrigger)106);
			yield return null;
		}
	}

	public bool PlayVoiceLine(EPhraseTrigger phrase, ETagStatus mask, bool aggressive)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		PhraseSpeakerClass speaker = Player.Speaker;
		if (speaker.Speaking || speaker.Busy)
		{
			return false;
		}
		return (Object)(object)speaker.Play(phrase, mask, true, (int?)null) != (Object)null;
	}

	private IEnumerator playPhrases(EPhraseTrigger trigger)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		PhraseSpeakerClass speaker = Player.Speaker;
		if (speaker.PhrasesBanks.TryGetValue(trigger, out var phrasesBank))
		{
			int count = phrasesBank.Clips.Length;
			Logger.LogDebug($" Playing {trigger} {count}");
			for (int i = 0; i < count; i++)
			{
				bool said = false;
				while (!said)
				{
					if (!speaker.Speaking && !speaker.Busy)
					{
						speaker.PlayDirect(trigger, i);
						Logger.LogDebug($"{trigger} :: {((Object)phrasesBank.Clips[i].Clip).name} :: {i}");
						said = true;
					}
					yield return null;
				}
			}
		}
		else
		{
			Logger.LogDebug($"{trigger} no phrases");
		}
	}

	private void drawTransformGizmos()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		if (SAINPlugin.DebugMode && SAINPlugin.DrawDebugGizmos && SAINPlugin.DebugSettings.Gizmos.DrawTransformGizmos)
		{
			DebugGizmos.Sphere(Transform.EyePosition, 0.05f, Color.white, 0.1f);
			Vector3 eyePosition = Transform.EyePosition;
			Vector3 headLookDirection = Transform.HeadLookDirection;
			Color white = Color.white;
			Vector3 val = Transform.HeadLookDirection;
			DebugGizmos.Ray(eyePosition, headLookDirection, white, ((Vector3)(ref val)).magnitude, 0.025f, temporary: true, 0.1f);
			DebugGizmos.Sphere(Transform.HeadPosition, 0.075f, Color.yellow, 0.1f);
			Vector3 headPosition = Transform.HeadPosition;
			Vector3 lookDirection = Transform.LookDirection;
			Color yellow = Color.yellow;
			val = Transform.LookDirection;
			DebugGizmos.Ray(headPosition, lookDirection, yellow, ((Vector3)(ref val)).magnitude, 0.025f, temporary: true, 0.1f);
			DebugGizmos.Sphere(Transform.WeaponFirePort, 0.075f, Color.green, 0.1f);
			Vector3 weaponFirePort = Transform.WeaponFirePort;
			Vector3 weaponPointDirection = Transform.WeaponPointDirection;
			Color green = Color.green;
			val = Transform.WeaponPointDirection;
			DebugGizmos.Ray(weaponFirePort, weaponPointDirection, green, ((Vector3)(ref val)).magnitude, 0.05f, temporary: true, 0.1f);
			DebugGizmos.Sphere(Transform.BodyPosition, 0.1f, Color.blue, 0.1f);
			Vector3 bodyPosition = Transform.BodyPosition;
			Vector3 lookDirection2 = Transform.LookDirection;
			Color blue = Color.blue;
			val = Transform.LookDirection;
			DebugGizmos.Ray(bodyPosition, lookDirection2, blue, ((Vector3)(ref val)).magnitude, 0.05f, temporary: true, 0.1f);
		}
	}
}
