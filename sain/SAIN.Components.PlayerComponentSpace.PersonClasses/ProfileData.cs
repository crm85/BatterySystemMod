using EFT;

namespace SAIN.Components.PlayerComponentSpace.PersonClasses;

public class ProfileData
{
	public string ProfileId { get; }

	public string Nickname { get; }

	public EPlayerSide Side { get; }

	public ProfileData(Player player)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		ProfileId = player.ProfileId;
		Nickname = player.Profile.Nickname;
		Side = player.Profile.Side;
	}
}
