using System.Collections.Generic;

public class GameState: IHasTurnLogic
{
	public int CurrentTurn;
	public StarSystem StarSystem = new StarSystem();
	public List<Planet> Planets = new List<Planet>();

	public void DoTurnLogic()
	{
		foreach (Planet planet in Planets)
		{
			planet.DoTurnLogic();
		}
	}
}