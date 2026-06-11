using UnityEngine;

namespace SAIN.SAINComponent.Classes.Mover;

public struct BotCornerDetails
{
	public EBotCornerStatus Status;

	public EBotCornerType Type;

	public int Index;

	public Vector3 Position;

	public Vector3 Direction;

	public float Length;

	public float TimeStarted;

	public float TimeComplete;

	public readonly bool ShortCorner => Type == EBotCornerType.PathShortTurn;

	public readonly bool LastCorner => Type == EBotCornerType.PathEnd;

	public void SetStarted(float currentTime)
	{
		Status = EBotCornerStatus.Active;
		TimeStarted = currentTime;
	}

	public void SetComplete(float currentTime)
	{
		Status = EBotCornerStatus.Used;
		TimeComplete = currentTime;
	}

	public void UpdateType(EBotCornerType type)
	{
		Type = type;
	}

	public void SetDirection(Vector3 direction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Direction = direction;
		Length = ((Vector3)(ref direction)).magnitude;
	}

	public static BotCornerDetails Create(ref Vector3[] corners, float shortCornerConfigDistance, int count, int i)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		BotCornerDetails result = new BotCornerDetails
		{
			Position = corners[i],
			Index = i,
			Status = EBotCornerStatus.Awaiting
		};
		if (i < count - 1)
		{
			result.Direction = corners[i + 1] - result.Position;
			result.Length = ((Vector3)(ref result.Direction)).magnitude;
			if (i == 0)
			{
				result.Type = EBotCornerType.PathStart;
			}
			else if (i < count - 2)
			{
				bool flag = result.Length <= shortCornerConfigDistance;
				result.Type = (flag ? EBotCornerType.PathShortTurn : EBotCornerType.PathTurn);
			}
			else
			{
				result.Type = EBotCornerType.PathEndApproach;
			}
		}
		else if (i == count - 1)
		{
			result.Type = EBotCornerType.PathEnd;
		}
		return result;
	}

	public static BotCornerDetails Create(Vector3 corner, Vector3 nextCorner, EBotCornerType Type, int index)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		BotCornerDetails result = new BotCornerDetails
		{
			Position = corner,
			Index = index,
			Status = EBotCornerStatus.Awaiting,
			Type = Type
		};
		result.SetDirection(nextCorner - corner);
		return result;
	}

	public static BotCornerDetails Create(Vector3 corner, EBotCornerType Type, int index)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new BotCornerDetails
		{
			Position = corner,
			Index = index,
			Status = EBotCornerStatus.Awaiting,
			Type = Type
		};
	}
}
