using EFT;

namespace SAIN.Types.Jobs;

public interface IBotRaycastJobSingleTarget
{
	IPlayer Target { get; }
}
