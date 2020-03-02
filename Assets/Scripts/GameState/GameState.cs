using System.Collections.Generic;
using System.Linq;

public class GameState : IHasTurnLogic
{
	public int CurrentTurn;
	public readonly List<CelestialBodyLogic> StarSystem;
	public readonly List<Spaceship> Spaceships;

	public GameState()
	{
		StarSystem = new List<CelestialBodyLogic>();
		Spaceships = new List<Spaceship>();
	}

	public void DoTurnLogic()
	{
		foreach (CelestialBodyLogic logic in StarSystem)
		{
			logic.DoTurnLogic();
		}
	}

	public CelestialBodyLogic FindLogicComponent(CelestialBody body)
	{
		return StarSystem.SkipWhile(logic => logic.Body != body).FirstOrDefault();
	}
}