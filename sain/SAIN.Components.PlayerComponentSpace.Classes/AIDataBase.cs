using SAIN.SAINComponent.Classes.Info;

namespace SAIN.Components.PlayerComponentSpace.Classes;

public abstract class AIDataBase
{
	protected readonly SAINAIData AIData;

	protected GearInfo GearInfo => AIData.PlayerComponent.Equipment.GearInfo;

	protected bool IsAI => AIData.IsAI;

	public AIDataBase(SAINAIData aidata)
	{
		AIData = aidata;
	}
}
