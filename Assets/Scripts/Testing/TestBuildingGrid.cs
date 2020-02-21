using System.Linq;
using UnityEngine;

public class TestBuildingGrid : MonoBehaviour
{
	public CelestialBody Target;

	private void Update()
	{
		var gridUI = GetComponent<BuildingGridUI>();
		if (Input.GetButtonDown("Submit"))
		{
			if (gridUI.LoadedPlanet)
			{
				Debug.Log("Unloading planet");
				gridUI.UnloadBuildingGrid();
			}
			else
			{
				Planet targetPlanet = GameController.Instance
					.State
					.Planets
					.SkipWhile(planet => planet.Body != Target)
					.First();
				Debug.Log("Loading planet");
				gridUI.LoadBuildingGrid(targetPlanet.Buildings);
			}
		}
	}
}