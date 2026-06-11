using EFT;
using EFT.Interactive;
using UnityEngine;

namespace SAIN.Plugin;

public class ExtractionInfo
{
	public readonly string BotNickname;

	public readonly string ProfileID;

	public readonly string Reason;

	public readonly string ExtractionPoint;

	public readonly float TimeExtracted;

	public ExtractionInfo(BotOwner bot, string reason, ExfiltrationPoint exfil)
	{
		BotNickname = bot.Profile.Nickname;
		ProfileID = bot.GetPlayer.ProfileId;
		Reason = reason;
		TimeExtracted = Time.time;
		ExtractionPoint = exfil.Settings.Name;
	}
}
