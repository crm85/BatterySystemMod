using SAIN.Components.PlayerComponentSpace;
using Unity.Collections;
using Unity.Jobs;

namespace SAIN.Components.BotControllerSpace.Classes.Raycasts;

public struct PlayerDirectionDataJob : IJobFor
{
	[ReadOnly]
	public NativeArray<PlayerDirectionData> Input;

	[WriteOnly]
	public NativeArray<PlayerDirectionData> Output;

	public void Execute(int index)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		PlayerDirectionData playerDirectionData = Input[index];
		playerDirectionData.MainData.Update(playerDirectionData.OwnerPosition);
		playerDirectionData.MainData.UpdateDotProductAndCalcNormal(playerDirectionData.OwnerViewPosition, playerDirectionData.OwnerLookDirection);
		Output[index] = playerDirectionData;
	}

	public void Dispose()
	{
		if (Input.IsCreated)
		{
			Input.Dispose();
		}
		if (Output.IsCreated)
		{
			Output.Dispose();
		}
	}
}
