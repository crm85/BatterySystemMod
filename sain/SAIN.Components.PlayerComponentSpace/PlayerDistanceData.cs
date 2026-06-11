using System.Collections.Generic;
using SAIN.Components.PlayerComponentSpace.PersonClasses;
using SAIN.Models.Structs;
using UnityEngine;

namespace SAIN.Components.PlayerComponentSpace;

public class PlayerDistanceData
{
	public PlayerDirectionData Data { get; private set; }

	public Vector3 Position => Data.MainData.Position;

	public Vector3 Direction => Data.MainData.Direction;

	public Vector3 DirectionNormal => Data.MainData.DirectionNormalized;

	public float DotProduct => Data.MainData.Dot;

	public float Distance => Data.MainData.Distance;

	public Dictionary<EBodyPart, BodyPartDirectionData> BodyPartDirectionData { get; } = new Dictionary<EBodyPart, BodyPartDirectionData>
	{
		{
			(EBodyPart)0,
			default(BodyPartDirectionData)
		},
		{
			(EBodyPart)1,
			default(BodyPartDirectionData)
		},
		{
			(EBodyPart)2,
			default(BodyPartDirectionData)
		},
		{
			(EBodyPart)3,
			default(BodyPartDirectionData)
		},
		{
			(EBodyPart)4,
			default(BodyPartDirectionData)
		},
		{
			(EBodyPart)5,
			default(BodyPartDirectionData)
		},
		{
			(EBodyPart)6,
			default(BodyPartDirectionData)
		}
	};

	public PlayerDistanceData(PlayerComponent OtherPlayer)
	{
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		SAINBodyPart[] partsArray = OtherPlayer.BodyParts.PartsArray;
		Data = new PlayerDirectionData
		{
			MainData = default(DirectionData),
			BodyParts = new BodyPartDirectionData[partsArray.Length]
		};
		for (int i = 0; i < partsArray.Length; i++)
		{
			Data.BodyParts[i] = new BodyPartDirectionData(partsArray[i].Type);
		}
	}

	public PlayerDirectionData GetPlayerDirectionData()
	{
		return Data;
	}

	public void SetPlayerDirectionData(PlayerDirectionData data)
	{
		Data = data;
	}

	public PlayerDirectionData GetUpdatedDirectionData(PlayerComponent Owner, PlayerComponent OtherPlayer)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		PlayerDirectionData data = Data;
		PersonTransformClass transform = Owner.Transform;
		data.OwnerPosition = transform.Position;
		data.OwnerLookDirection = transform.LookDirection;
		data.OwnerViewPosition = transform.EyePosition;
		data.MainData.Position = OtherPlayer.Position;
		Data = data;
		return Data;
	}

	public BodyPartDirectionData GetBodyPartData(EBodyPart part)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return BodyPartDirectionData[part];
	}
}
