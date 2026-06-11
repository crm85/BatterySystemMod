using System.Collections.Generic;

namespace SAIN.Preset.GlobalSettings.Categories;

public static class AIBrains
{
	public static readonly List<Brain> PMCs = new List<Brain>
	{
		Brain.PmcBear,
		Brain.PmcUsec
	};

	public static readonly List<Brain> Scavs = new List<Brain>
	{
		Brain.CursAssault,
		Brain.Assault
	};

	public static readonly List<Brain> Goons = new List<Brain>
	{
		Brain.Knight,
		Brain.BirdEye,
		Brain.BigPipe
	};

	public static readonly List<Brain> Others = new List<Brain> { Brain.Obdolbs };

	public static readonly List<Brain> Bosses = new List<Brain>
	{
		Brain.BossBully,
		Brain.BossGluhar,
		Brain.BossKojaniy,
		Brain.BossSanitar,
		Brain.Tagilla,
		Brain.BossTest,
		Brain.Gifter,
		Brain.Killa,
		Brain.SectantPriest,
		Brain.BossBoar,
		Brain.BossKolontay,
		Brain.BossPartisan
	};

	public static readonly List<Brain> Followers = new List<Brain>
	{
		Brain.FollowerBully,
		Brain.FollowerGluharAssault,
		Brain.FollowerGluharProtect,
		Brain.FollowerGluharScout,
		Brain.FollowerKojaniy,
		Brain.FollowerSanitar,
		Brain.TagillaFollower,
		Brain.FollowerBoar,
		Brain.FollowerBoarClose1,
		Brain.FollowerBoarClose2,
		Brain.BossBoarSniper,
		Brain.FollowerKolontayAssault,
		Brain.FollowerKolontaySecurity
	};

	public static List<Brain> GetAllowedScavBrains()
	{
		List<Brain> scavs = Scavs;
		scavs.Add(Brain.PMC);
		return scavs;
	}

	public static List<Brain> GetAllowedPlayerScavBrains()
	{
		return GetAllowedPMCBrains();
	}

	public static List<Brain> GetAllowedPMCBrains()
	{
		List<Brain> pMCs = PMCs;
		bool flag = true;
		pMCs.Add(Brain.PMC);
		return pMCs;
	}
}
