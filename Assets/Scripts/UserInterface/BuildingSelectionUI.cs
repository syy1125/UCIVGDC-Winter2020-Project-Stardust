using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectionUI : MonoBehaviour
{
	[Header("References")]
	public BuildingTemplate[] Buildings;
	public GameObject BuildingButtonPrefab;
	public GameObject ExtraButtons;
	public Button CancelButton;

	private bool LoadedPlanet => _selectedBody != null;
	private CelestialBodyLogic _selectedBody;
	private int _selectedIndex;
	private const int UNSELECT_INDEX = -1; // Index for when nothing is selected

	public BuildingTemplate GetSelectedBuilding() =>
		_selectedIndex == UNSELECT_INDEX ? null : Buildings[_selectedIndex];

	public void LoadBuildings(CelestialBodyLogic bodyLogic)
	{
		if (LoadedPlanet) UnloadBuildings();

		_selectedBody = bodyLogic;
		_selectedIndex = UNSELECT_INDEX;

		for (int index = 0; index < Buildings.Length; index++)
		{
			GameObject button = Instantiate(BuildingButtonPrefab, transform);
			var buttonController = button.GetComponent<BuildingSelectionButtonUI>();

			buttonController.BuildingSelection = this;
			buttonController.Index = index;
			buttonController.Building = Buildings[index];
		}

		RefreshBuildingButtons();
	}

	public void SetSelectedIndex(int index)
	{
		_selectedIndex = index;

		RefreshBuildingButtons();

		CancelButton.interactable = index != UNSELECT_INDEX;
	}

	private void RefreshBuildingButtons()
	{
		foreach (Transform child in transform)
		{
			var buttonUI = child.GetComponent<BuildingSelectionButtonUI>();
			buttonUI.SetSelected(buttonUI.Index == _selectedIndex);
			child.GetComponent<Button>().interactable = CanBuild(buttonUI.Building);
		}
	}

	private bool CanBuild(BuildingTemplate building)
	{
		return building.Costs.All(cost => _selectedBody.Resources[cost.Resource] >= cost.Amount);
	}

	public void CancelSelection()
	{
		SetSelectedIndex(UNSELECT_INDEX);
	}

	public void UnloadBuildings()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		_selectedBody = null;
	}
}