using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BuildingGridUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public GameObject BuildingTilePrefab;
	public GameObject PreviewTilePrefab;
	public GridLayoutGroup BuildingGrid;
	public GridLayoutGroup PreviewGrid;

	public bool LoadedPlanet => _planet != null;
	private PlanetBuildingController _planet;
	private GameObject[][] _gridTiles;
	private GameObject[][] _previewTiles;

	private bool _hover;

	public void LoadBuildingGrid(PlanetBuildingController planet)
	{
		if (LoadedPlanet) UnloadBuildingGrid();

		_planet = planet;

		SetupGridLayout(BuildingGrid);
		SetupGridLayout(PreviewGrid);
		SpawnGridTiles();
		RefreshBuildings();
	}

	private Vector2Int GetGridSize()
	{
		return new Vector2Int(_planet.Body.BuildingGridWidth, _planet.Body.BuildingGridHeight);
	}

	private void SetupGridLayout(GridLayoutGroup grid)
	{
		Vector2Int gridSize = GetGridSize();

		var rectTransform = GetComponent<RectTransform>();

		Vector2 panelSize = rectTransform.rect.size;
		grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		grid.constraintCount = gridSize.x;
		grid.cellSize = Vector2.one * Mathf.Min(panelSize.x / gridSize.x, panelSize.y / gridSize.y);
	}

	private void SpawnGridTiles()
	{
		Vector2Int gridSize = GetGridSize();

		_gridTiles = new GameObject[gridSize.x][];
		_previewTiles = new GameObject[gridSize.x][];
		for (int x = 0; x < gridSize.x; x++)
		{
			_gridTiles[x] = new GameObject[gridSize.y];
			_previewTiles[x] = new GameObject[gridSize.y];
		}

		for (int y = 0; y < gridSize.y; y++)
		{
			for (int x = 0; x < gridSize.x; x++)
			{
				_gridTiles[x][y] = Instantiate(BuildingTilePrefab, BuildingGrid.transform);
				_gridTiles[x][y].GetComponent<PlanetTile>().TilePosition = new Vector2Int(x, y);
				_previewTiles[x][y] = Instantiate(PreviewTilePrefab, PreviewGrid.transform);
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

	public void OnPointerEnter(PointerEventData eventData)
	{
		_hover = true;
	}

	private void Update()
	{
		if (!_hover) return;

		var rectTransform = GetComponent<RectTransform>();
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
			rectTransform,
			Input.mousePosition,
			null,
			out Vector2 hoverPosition
		)) return;

		Rect rect = rectTransform.rect;
		hoverPosition = Vector2.Scale(hoverPosition - rect.min, new Vector2(1 / rect.size.x, 1 / rect.size.y));
		Vector2 gridPosition = Vector2.Scale(GetGridSize(), hoverPosition);
		
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log(eventData.pointerPressRaycast.gameObject.GetComponent<PlanetTile>().TilePosition);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_hover = false;
	}

	public void UnloadBuildingGrid()
	{
		foreach (Transform child in BuildingGrid.transform)
		{
			Destroy(child.gameObject);
		}

		foreach (Transform child in PreviewGrid.transform)
		{
			Destroy(child.gameObject);
		}

		_planet = null;
		_gridTiles = null;
	}
}