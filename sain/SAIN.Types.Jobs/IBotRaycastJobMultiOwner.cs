using EFT;

namespace SAIN.Types.Jobs;

public interface IBotRaycastJobMultiOwner
{
	IPlayer[] Owners { get; }
}
