using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class BuildingGridUI : MonoBehaviour
{
	public GameObject BuildingTilePrefab;

	public bool LoadedPlanet => _gridTiles != null;
	private GameObject[][] _gridTiles;

	public void LoadBuildingGrid(PlanetBuildingController planet)
	{
		if (LoadedPlanet) UnloadBuildingGrid();
		var gridSize = new Vector2Int(planet.Body.BuildingGridWidth, planet.Body.BuildingGridHeight);

		SetupGridSize(gridSize);
		SpawnGridTiles(gridSize);
		RefreshBuildings(planet);
	}

	private void SetupGridSize(Vector2Int gridSize)
	{
		var rectTransform = GetComponent<RectTransform>();
		var grid = GetComponent<GridLayoutGroup>();

		Vector2 panelSize = rectTransform.rect.size;
		grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		grid.constraintCount = gridSize.x;
		grid.cellSize = Vector2.one * Mathf.Min(panelSize.x / gridSize.x, panelSize.y / gridSize.y);
	}

	private void SpawnGridTiles(Vector2Int gridSize)
	{
		_gridTiles = new GameObject[gridSize.x][];
		for (int x = 0; x < gridSize.x; x++) _gridTiles[x] = new GameObject[gridSize.y];

		for (int y = 0; y < gridSize.y; y++)
		{
			for (int x = 0; x < gridSize.x; x++)
			{
				_gridTiles[x][y] = Instantiate(BuildingTilePrefab, transform);
			}
		}
	}

	public void RefreshBuildings(PlanetBuildingController planet)
	{
		Debug.Assert(_gridTiles.Length == planet.Body.BuildingGridWidth);
		Debug.Assert(_gridTiles[0].Length == planet.Body.BuildingGridHeight);

		for (int x = 0; x < planet.Body.BuildingGridWidth; x++)
		{
			for (int y = 0; y < planet.Body.BuildingGridHeight; y++)
			{
				Tuple<Sprite, int> data = planet.GetSpriteAndRotationAt(new Vector2Int(x, y));
				if (data != null)
				{
					GameObject tile = _gridTiles[x][y];
					tile.GetComponent<Image>().sprite = data.Item1;
					tile.transform.rotation = Quaternion.Euler(0, 0, 90 * (4 - data.Item2));
				}
			}
		}
	}

	public void UnloadBuildingGrid()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		_gridTiles = null;
	}
}