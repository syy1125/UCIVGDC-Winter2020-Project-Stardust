﻿using UnityEngine;

public class TestBuildingGrid : MonoBehaviour
{
	public PlanetBuildingController Planet;
	public BuildingTemplate Nexus;
	public BuildingTemplate Harvester;
	public BuildingTemplate PowerPlant;
	public BuildingTemplate ReplicationChamber;

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
				Debug.Log("Loading planet");
				gridUI.LoadBuildingGrid(Planet);
			}
		}

		if (gridUI.LoadedPlanet)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				Planet.ConstructBuilding(Nexus, Vector2Int.zero, 0);
				gridUI.RefreshBuildings(Planet);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				Planet.ConstructBuilding(Nexus, Vector2Int.zero, 1);
				gridUI.RefreshBuildings(Planet);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				Planet.ConstructBuilding(Nexus, Vector2Int.zero, 2);
				gridUI.RefreshBuildings(Planet);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				Planet.ConstructBuilding(Nexus, Vector2Int.zero, 3);
				gridUI.RefreshBuildings(Planet);
			}
		}
	}
}