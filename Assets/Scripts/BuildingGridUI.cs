using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class BuildingGridUI : MonoBehaviour
{
	public GameObject BuildingTilePrefab;

	public bool LoadedPlanet => _planet != null;
	private PlanetBuildingController _planet;
	private GameObject[][] _gridTiles;

	public void LoadBuildingGrid(PlanetBuildingController planet)
	{
		if (LoadedPlanet) UnloadBuildingGrid();

		_planet = planet;

		SetupGridLayout();
		SpawnGridTiles();
		RefreshBuildings();
	}

	private Vector2Int GetGridSize()
	{
		return new Vector2Int(_planet.Body.BuildingGridWidth, _planet.Body.BuildingGridHeight);
	}

	private void SetupGridLayout()
	{
		Vector2Int gridSize = GetGridSize();

		var rectTransform = GetComponent<RectTransform>();
		var grid = GetComponent<GridLayoutGroup>();

		Vector2 panelSize = rectTransform.rect.size;
		grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		grid.constraintCount = gridSize.x;
		grid.cellSize = Vector2.one * Mathf.Min(panelSize.x / gridSize.x, panelSize.y / gridSize.y);
	}

	private void SpawnGridTiles()
	{
		Vector2Int gridSize = GetGridSize();

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

	public void RefreshBuildings()
	{
		Vector2Int gridSize = GetGridSize();

		Debug.Assert(_gridTiles.Length == gridSize.x);
		Debug.Assert(_gridTiles[0].Length == gridSize.y);

		for (int x = 0; x < gridSize.x; x++)
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				Tuple<Sprite, int> data = _planet.GetSpriteAndRotationAt(new Vector2Int(x, y));
				if (data != null)
				{
					GameObject tile = _gridTiles[x][y];
					tile.GetComponent<Image>().sprite = data.Item1;
					tile.transform.rotation = Quaternion.Euler(0, 0, 90 * data.Item2);
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

		_planet = null;
		_gridTiles = null;
	}
}