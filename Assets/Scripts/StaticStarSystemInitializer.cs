using System;
using System.Linq;
using UnityEngine;

public class StaticStarSystemInitializer : MonoBehaviour
{
	public CelestialBody[] Bodies;

	private void Awake()
	{
		GameState state = GameController.Instance.State;

		state.StarSystem.CelestialBodies = Bodies.ToArray();
		foreach (CelestialBody body in Bodies)
		{
			state.Planets.Add(new Planet(body));
		}
	}
}