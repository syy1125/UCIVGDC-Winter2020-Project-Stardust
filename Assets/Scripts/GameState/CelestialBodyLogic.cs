using System.Collections.Generic;
using UnityEditor;

public class CelestialBodyLogic : ISaveLoad<CelestialBodyLogic.Serialized>, IHasTurnLogic
{
	public struct Serialized
	{
		public string CelestialBodyPath;
		public CelestialBodyBuildings.Serialized Buildings;
		public CelestialBodyResources.Serialized Resources;
	}

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

	public Serialized Save()
	{
		return new Serialized
		{
			CelestialBodyPath = AssetDatabase.GetAssetPath(Body),
			Buildings = Buildings.Save(),
			Resources = Resources.Save()
		};
	}

	public void Load(Serialized serialized)
	{
		Body = AssetDatabase.LoadAssetAtPath<CelestialBody>(serialized.CelestialBodyPath);
		Buildings.Load(serialized.Buildings);
		Resources.Load(serialized.Resources);
	}
}