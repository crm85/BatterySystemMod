using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SAIN.Patches.Generic;

public class FindRequestForMePatch : ModulePatch
{
	protected override MethodBase GetTargetMethod()
	{
		return AccessTools.Method(typeof(BotGroupRequestController), "FindForMe", (Type[])null, (Type[])null);
	}

	[PatchPrefix]
	public static bool Patch(BotOwner executer, List<BotRequest> ____listOfRequests)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		if (!SAINEnableClass.GetSAIN(executer, out var sain))
		{
			return true;
		}
		if (!sain.HasEnemy)
		{
			return true;
		}
		BotRequest val = null;
		foreach (BotRequest ____listOfRequest in ____listOfRequests)
		{
			IPlayer requester = ____listOfRequest.Requester;
			if ((requester == null || !requester.IsAI) && (____listOfRequest.CanExecuteByMyself || (Object)(Player)____listOfRequest.Requester != (Object)(object)executer.GetPlayer) && (!executer.Boss.IamBoss || executer.Boss.AllowRequestSelf || executer.GetPlayer.Id != ____listOfRequest.Requester.Id) && ____listOfRequest.CanStartExecute(executer))
			{
				val = ____listOfRequest;
				break;
			}
		}
		if (val != null)
		{
			val.Take(executer);
			____listOfRequests.Remove(val);
		}
		return false;
	}
}
