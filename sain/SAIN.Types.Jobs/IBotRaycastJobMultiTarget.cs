using EFT;

namespace SAIN.Types.Jobs;

public interface IBotRaycastJobMultiTarget
{
	IPlayer[] Targets { get; }
}
