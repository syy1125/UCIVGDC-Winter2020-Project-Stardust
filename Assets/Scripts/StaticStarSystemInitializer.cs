using System;
using System.Linq;
using UnityEngine;

public class StaticStarSystemInitializer : MonoBehaviour
{
	public CelestialBody[] Bodies;

	private void Awake()
	{
		GameState state = GameController.Instance.State;

		foreach (CelestialBody body in Bodies)
		{
			state.StarSystem.Add(new CelestialBodyLogic(state, body));
		}
	}
}