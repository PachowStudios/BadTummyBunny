using System.ComponentModel;

public enum SfxGroups
{
	// Farts
	[Description("FartsShort")]  BasicFartShort,
	[Description("FartsMedium")] BasicFartMedium,
	[Description("FartsLong")]   BasicFartLong,

	// Walking
	[Description("WalkingGrassLeft")]  WalkingGrassLeft,
	[Description("WalkingGrassRight")] WalkingGrassRight,
	[Description("LandingGrass")]      LandingGrass,
	
	// Collectables
	[Description("Carrots")]       Carrots,
	[Description("Coins")]         Coins,
	[Description("RespawnPoints")] RespawnPoints
}
