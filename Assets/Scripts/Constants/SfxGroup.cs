using System.ComponentModel;

public enum SfxGroup
{
	[Description("FartsShort")]  BasicFartShort,
	[Description("FartsMedium")] BasicFartMedium,
	[Description("FartsLong")]   BasicFartLong,

	[Description("WalkingGrassLeft")]  WalkingGrassLeft,
	[Description("WalkingGrassRight")] WalkingGrassRight,
	[Description("LandingGrass")]      LandingGrass,

	[Description("Carrots")]       Carrots,
	[Description("Coins")]         Coins,
	[Description("RespawnPoints")] RespawnPoints
}
