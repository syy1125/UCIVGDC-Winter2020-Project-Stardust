using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
	[Header("References")]
	public Button BuildButton;
	public BuildingGridUI BuildingGrid;
	public BuildingSelectionUI BuildingSelection;

	public bool BuildingScreenOpen { get; private set; }

	private void Start()
	{
		GameController.Instance.OnBodySelectionChanged.AddListener(UpdateDisplay);
		UpdateDisplay();
	}

	public void UpdateDisplay()
	{
		CelestialBody selected = GameController.Instance.SelectedBody;
		BuildButton.interactable = selected != null && selected.Colonizable;

		if (BuildingScreenOpen)
		{
			ToggleBuildingScreen();
		}
	}

	public void ToggleBuildingScreen()
	{
		if (BuildingScreenOpen)
		{
			BuildingGrid.UnloadBuildingGrid();
			BuildingSelection.UnloadBuildings();
			BuildingScreenOpen = false;
		}
		else
		{
			CelestialBodyLogic logic =
				GameController.Instance.State.FindLogicComponent(GameController.Instance.SelectedBody);
			if (logic == null) return;
			BuildingGrid.LoadBuildingGrid(logic);
			BuildingSelection.LoadBuildings(logic);
			BuildingScreenOpen = true;
		}
	}
}