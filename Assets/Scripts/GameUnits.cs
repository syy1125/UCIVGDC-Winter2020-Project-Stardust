public static class GameUnits
{
	// 1 unit time in-game = 30 days real-time
	public const float GAME_TO_PHYSICAL_TIME = 2592000;
	public const float PHYSICAL_TO_GAME_TIME = 1 / 2592000f;

	// 1 unit length in-game = 10^9 m
	public const float GAME_TO_PHYSICAL_LENGTH = 1E9f;
	public const float PHYSICAL_TO_GAME_LENGTH = 1E-9f;
}