using EFT;

namespace SAIN.Types.Jobs;

public interface IBotRaycastJobSingleOwner
{
	IPlayer Owner { get; }
}
