using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class BuildingGridUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	[Header("Prefabs")]
	public GameObject BuildingTilePrefab;
	public GameObject PreviewTilePrefab;

	[Header("References")]
	public GridLayoutGroup BuildingGrid;
	public GridLayoutGroup PreviewGrid;
	public BuildingSelectionUI BuildingSelection;

	[Header("Config")]
	public string RotateBuildingButton;
	public Color InvalidPositionColor = Color.red;

	// Non-serialized fields
	public bool LoadedPlanet => _buildingController != null;
	private CelestialBodyBuildings _buildingController;
	private GameObject[][] _gridTiles;
	private GameObject[][] _previewTiles;

	private bool _hover;

	private int _rotation;
	private BuildingTemplate _building;
	private Vector2Int _origin;

	public void LoadBuildingGrid(CelestialBodyBuildings buildings)
	{
		if (LoadedPlanet) UnloadBuildingGrid();

		_buildingController = buildings;

		SetupGridLayout(BuildingGrid);
		SetupGridLayout(PreviewGrid);
		SpawnGridTiles();
		RedrawBuildings();
	}

	private Vector2Int GetGridSize()
	{
		return new Vector2Int(_buildingController.GridWidth, _buildingController.GridHeight);
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

	public void RedrawBuildings()
	{
		Vector2Int gridSize = GetGridSize();

		Debug.Assert(_gridTiles.Length == gridSize.x);
		Debug.Assert(_gridTiles[0].Length == gridSize.y);

		for (int x = 0; x < gridSize.x; x++)
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				Tuple<Sprite, int> data = _buildingController.GetSpriteAndRotationAt(new Vector2Int(x, y));
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

		if (TryGetBuildingOrigin(out Vector2Int newOrigin))
		{
			BuildingTemplate newBuilding = BuildingSelection.GetSelectedBuilding();

			if (newBuilding != _building || newOrigin != _origin)
			{
				_building = newBuilding;
				_origin = newOrigin;

				RedrawBuildingPreview();
			}
		}
	}

	private bool TryGetBuildingOrigin(out Vector2Int origin)
	{
		origin = Vector2Int.zero;
		if (BuildingSelection.GetSelectedBuilding() == null) return false;

		var rectTransform = GetComponent<RectTransform>();
		if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
			rectTransform,
			Input.mousePosition,
			null,
			out Vector2 hoverPosition
		)) return false;

		Rect rect = rectTransform.rect;
		hoverPosition = Vector2.Scale(hoverPosition - rect.min, new Vector2(1 / rect.size.x, 1 / rect.size.y));
		// Subtract 0.5 to convert between cell-corner coordinate and cell-center coordinate.
		Vector2 gridPosition = Vector2.Scale(GetGridSize(), hoverPosition) - new Vector2(0.5f, 0.5f);
		Vector2 rawOrigin = gridPosition - (Vector2) (Quaternion.Euler(0, 0, 90 * _rotation)
		                                              * BuildingSelection.GetSelectedBuilding().GetCenterOfMass());
		origin = new Vector2Int(Mathf.RoundToInt(rawOrigin.x), Mathf.RoundToInt(rawOrigin.y));

		return true;
	}

	private void ClearPreviewGrid()
	{
		foreach (GameObject[] tiles in _previewTiles)
		{
			foreach (GameObject tile in tiles)
			{
				if (tile == null) continue;
				
				var image = tile.GetComponent<Image>();
				image.sprite = null;
				image.color = new Color(1, 1, 1, 0);
			}
		}
	}

	private void RedrawBuildingPreview()
	{
		ClearPreviewGrid();

		if (_building == null) return;

		Color color = _buildingController.IsValidBuilding(_building, _origin, _rotation)
			? Color.white
			: InvalidPositionColor;

		foreach (BuildingTile tile in _building.Tiles)
		{
			Vector2Int position = _origin + CelestialBodyBuildings.Rotate(tile.Offset, _rotation);

			if (!_buildingController.InBounds(position)) continue;

			var image = _previewTiles[position.x][position.y].GetComponent<Image>();
			image.sprite = tile.Sprite;
			image.color = color;
			image.transform.rotation = Quaternion.Euler(0, 0, 90 * _rotation);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			if (TryGetBuildingOrigin(out Vector2Int origin)
			    && _buildingController.IsValidBuilding(_building, origin, _rotation))
			{
				Debug.Log($"Building {BuildingSelection.GetSelectedBuilding().DisplayName} at {origin}");
				_buildingController.ConstructBuilding(_building, origin, _rotation);
				
				RedrawBuildings();
				RedrawBuildingPreview();
			}
		}
		else if (eventData.button == PointerEventData.InputButton.Right)
		{
			_rotation = (_rotation + 1) % 4;
			RedrawBuildingPreview();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_hover = false;
		ClearPreviewGrid();
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

		_buildingController = null;
		_gridTiles = null;
	}

	private void OnDrawGizmos()
	{}
}