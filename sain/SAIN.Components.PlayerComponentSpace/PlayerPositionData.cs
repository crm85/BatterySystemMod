using System.Collections.Generic;
using EFT;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public struct PlayerPositionData
{
	private readonly Dictionary<BodyPartType, EnemyPart> BodyParts;

	private readonly string PlayerNickname;

	public readonly EnemyPart Head;

	public readonly EnemyPart MainBody;

	public Vector3 Forward;

	public Vector3 Right;

	public Vector3 Position;

	public Vector3 LookDirection;

	public Vector3 HeadPosition;

	public Vector3 BodyPosition;

	public bool HasWeaponEquipped;

	public Vector3 WeaponFireport;

	public Vector3 WeaponPointDirection;

	public bool IsOnNavMesh;

	public Vector3 NavMeshPosition;

	public Vector3 LastValidNavMeshPosition;

	public PlayerPositionData(Player Player)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		BodyParts = null;
		PlayerNickname = null;
		Head = null;
		MainBody = null;
		Forward = default(Vector3);
		Right = default(Vector3);
		Position = default(Vector3);
		LookDirection = default(Vector3);
		HeadPosition = default(Vector3);
		BodyPosition = default(Vector3);
		HasWeaponEquipped = false;
		WeaponFireport = default(Vector3);
		WeaponPointDirection = default(Vector3);
		IsOnNavMesh = false;
		NavMeshPosition = default(Vector3);
		LastValidNavMeshPosition = default(Vector3);
		if ((Object)(object)Player == (Object)null)
		{
			Logger.LogError("Player == null");
			return;
		}
		if (Player.Profile == null)
		{
			Logger.LogError("Player.Profile == null");
		}
		PlayerNickname = Player.Profile.Nickname;
		if (Player.MainParts == null)
		{
			Logger.LogError("Player.MainParts == null");
			return;
		}
		BodyParts = Player.MainParts;
		if (BodyParts.TryGetValue((BodyPartType)0, out var value))
		{
			Head = value;
		}
		if (BodyParts.TryGetValue((BodyPartType)1, out var value2))
		{
			MainBody = value2;
		}
	}

	public void Update(Player Player)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Player == (Object)null)
		{
			Logger.LogError("Player Null");
			return;
		}
		Vector3 zeroVector = Vector3.zeroVector;
		BifacialTransform transform = Player.Transform;
		if (transform == null)
		{
			Logger.LogError("Player Transform Null");
		}
		else
		{
			Position = transform.position;
		}
		MovementContext movementContext = Player.MovementContext;
		if (movementContext == null)
		{
			Logger.LogError("Player MovementContext Null");
		}
		else
		{
			LookDirection = movementContext.LookDirection;
			Forward = movementContext.PlayerRealForward;
			Right = movementContext.PlayerRealRight;
		}
		EnemyPart head = Head;
		if (head == null)
		{
			Logger.LogError(PlayerNickname + "'s Head Part is null");
			HeadPosition = zeroVector;
		}
		else
		{
			HeadPosition = head.Position;
		}
		EnemyPart mainBody = MainBody;
		if (mainBody == null)
		{
			Logger.LogError(PlayerNickname + "'s MainBody Part is null");
			BodyPosition = zeroVector;
		}
		else
		{
			BodyPosition = mainBody.Position;
		}
		BifacialTransform weaponRoot = Player.WeaponRoot;
		if (weaponRoot == null)
		{
			HasWeaponEquipped = false;
			WeaponFireport = zeroVector;
			WeaponPointDirection = zeroVector;
		}
		else
		{
			HasWeaponEquipped = true;
			WeaponFireport = weaponRoot.position;
			WeaponPointDirection = weaponRoot.forward;
		}
	}

	public readonly bool GetBodyPartPosition(BodyPartType PartType, out Vector3 Result)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (1 == 0)
		{
		}
		Vector3 val = (((int)PartType == 0) ? HeadPosition : (((int)PartType != 1) ? GetBodyPartPosition(PartType) : BodyPosition));
		if (1 == 0)
		{
		}
		Result = val;
		return Result != Vector3.zero;
	}

	private readonly Vector3 GetBodyPartPosition(BodyPartType PartType)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EnemyPart bodyPart = GetBodyPart(PartType);
		return (bodyPart != null) ? bodyPart.Position : Vector3.zero;
	}

	private readonly EnemyPart GetBodyPart(BodyPartType PartType)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		EnemyPart value = null;
		if (BodyParts == null)
		{
			Logger.LogError("[" + PlayerNickname + "] Body Parts Dictionary Null");
			return value;
		}
		if (!BodyParts.TryGetValue(PartType, out value))
		{
			Logger.LogError($"[{PlayerNickname}] Body Part [{PartType}] is not in Parts Dictionary");
			return null;
		}
		if (value == null)
		{
			Logger.LogError($"[{PlayerNickname}] Body Part [{PartType}] is Null");
		}
		return value;
	}
}
