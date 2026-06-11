using System.Collections.Generic;
using SAIN.Components.PlayerComponentSpace;
using UnityEngine;

namespace SAIN.Components.BotControllerSpace.Classes.Raycasts;

public struct PlayerTickData
{
	public readonly PlayerComponent Owner;

	public readonly string OwnerProfileId;

	public Vector3 OwnerViewPosition;

	public Vector3 OwnerPosition;

	public Vector3 OwnerLookDirection;

	public List<OtherPlayerData> OtherPlayerData;

	public List<PlayerDirectionData> OtherPlayerDirectionData;

	public PlayerTickData(PlayerComponent inOwner)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		OtherPlayerData = new List<OtherPlayerData>();
		OtherPlayerDirectionData = new List<PlayerDirectionData>();
		Owner = inOwner;
		OwnerProfileId = inOwner.ProfileId;
		OwnerViewPosition = inOwner.Transform.EyePosition;
		OwnerPosition = inOwner.Position;
		OwnerLookDirection = inOwner.LookDirection;
	}

	public void Prepare(PlayerComponent Owner)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		OwnerViewPosition = Owner.Transform.EyePosition;
		OwnerPosition = Owner.Position;
		OwnerLookDirection = Owner.LookDirection;
		OtherPlayerDirectionData.Clear();
		OtherPlayerData.Clear();
		List<OtherPlayerData> dataList = Owner.OtherPlayersData.DataList;
		for (int i = 0; i < dataList.Count; i++)
		{
			OtherPlayerData otherPlayerData = dataList[i];
			if (otherPlayerData != null)
			{
				OtherPlayerDirectionData.Add(otherPlayerData.DistanceData.GetUpdatedDirectionData(Owner, otherPlayerData.PlayerComponent));
				OtherPlayerData.Add(otherPlayerData);
			}
		}
	}

	public void Execute()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < OtherPlayerDirectionData.Count; i++)
		{
			PlayerDirectionData value = OtherPlayerDirectionData[i];
			value.MainData.Update(OwnerPosition);
			value.MainData.UpdateDotProductAndCalcNormal(OwnerViewPosition, OwnerLookDirection);
			OtherPlayerDirectionData[i] = value;
		}
	}

	public void ReadData()
	{
		for (int i = 0; i < OtherPlayerDirectionData.Count; i++)
		{
			OtherPlayerData[i].DistanceData.SetPlayerDirectionData(OtherPlayerDirectionData[i]);
		}
		OtherPlayerData.Clear();
		OtherPlayerDirectionData.Clear();
	}
}
