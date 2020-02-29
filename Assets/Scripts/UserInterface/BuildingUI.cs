using System;
using UnityEngine;

public class BuildingUI : MonoBehaviour
{
	public BuildingGridUI GridUI;
	public BuildingSelectionUI SelectionUI;

	private void OnEnable()
	{
		GameController.Instance.OnBodySelectionChanged.AddListener(UpdateDisplay);
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		CelestialBody selected = GameController.Instance.SelectedBody;

		if (selected == null)
		{
			GridUI.UnloadBuildingGrid();
			SelectionUI.UnloadBuildings();
		}
		else
		{
			CelestialBodyLogic logic = GameController.Instance.State.FindLogicComponent(selected);
			GridUI.LoadBuildingGrid(logic);
			SelectionUI.LoadBuildings(logic);
		}
	}

	private void OnDisable()
	{
		GridUI.UnloadBuildingGrid();
		SelectionUI.UnloadBuildings();
		GameController.Instance.OnBodySelectionChanged.RemoveListener(UpdateDisplay);
	}
}