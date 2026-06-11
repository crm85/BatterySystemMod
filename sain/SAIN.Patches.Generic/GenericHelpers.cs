using EFT;
using UnityEngine;

namespace SAIN.Patches.Generic;

public static class GenericHelpers
{
	public static bool CheckNotNull(BotOwner botOwner)
	{
		return (Object)(object)botOwner != (Object)null && (Object)(object)((Component)botOwner).gameObject != (Object)null && (Object)(object)((Component)botOwner).gameObject.transform != (Object)null && botOwner.Transform != null && !botOwner.IsDead;
	}
}
