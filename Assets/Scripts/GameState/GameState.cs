using System.Collections.Generic;

public class GameState: IHasTurnLogic
{
	public int CurrentTurn;
	public List<CelestialBodyLogic> StarSystem = new List<CelestialBodyLogic>();

	public void DoTurnLogic()
	{
		foreach (CelestialBodyLogic logic in StarSystem)
		{
			logic.DoTurnLogic();
		}
	}
}