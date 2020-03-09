using System.Collections.Generic;
using UnityEditor;

public class CelestialBodyLogic : IHasTurnLogic
{
	private readonly GameState _state;
	public CelestialBody Body { get; private set; }
	public CelestialBodyBuildings Buildings { get; }
	public CelestialBodyResources Resources { get; }
	public readonly List<SpaceshipTemplate> ShipQueue;

	public CelestialBodyLogic(GameState state, CelestialBody body)
	{
		_state = state;
		Body = body;
		Buildings = new CelestialBodyBuildings(this);
		Resources = new CelestialBodyResources(this);
		ShipQueue = new List<SpaceshipTemplate>();
	}

	public void DoTurnLogic()
	{
		Resources.DoTurnLogic();
		foreach (SpaceshipTemplate template in ShipQueue)
		{
			_state.Spaceships.Add(new Spaceship(template, Body));
		}

		ShipQueue.Clear();
	}
}