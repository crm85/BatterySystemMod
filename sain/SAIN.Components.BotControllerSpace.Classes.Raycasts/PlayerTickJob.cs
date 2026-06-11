using Unity.Collections;
using Unity.Jobs;

namespace SAIN.Components.BotControllerSpace.Classes.Raycasts;

public struct PlayerTickJob : IJobFor
{
	[ReadOnly]
	public NativeArray<PlayerTickData> Input;

	[WriteOnly]
	public NativeArray<PlayerTickData> Output;

	public void Execute(int index)
	{
		PlayerTickData playerTickData = Input[index];
		playerTickData.Execute();
		Output[index] = playerTickData;
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
