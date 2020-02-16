using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectionUI : MonoBehaviour
{
	[Header("References")]
	public BuildingTemplate[] Buildings;
	public GameObject BuildingButtonPrefab;
	public GameObject ExtraButtons;
	public Button CancelButton;

	private int _selectedIndex;
	private const int UNSELECT_INDEX = -1; // Index for when nothing is selected

	public void LoadBuildings()
	{
		_selectedIndex = UNSELECT_INDEX;

		for (int index = 0; index < Buildings.Length; index++)
		{
			GameObject button = Instantiate(BuildingButtonPrefab, transform);
			var buttonController = button.GetComponent<BuildingSelectionButtonUI>();

			buttonController.BuildingSelection = this;
			buttonController.Index = index;
			buttonController.Building = Buildings[index];
		}
		
		ExtraButtons.SetActive(true);
	}

	public void SetSelectedIndex(int index)
	{
		_selectedIndex = index;

		foreach (Transform child in transform)
		{
			var button = child.GetComponent<BuildingSelectionButtonUI>();
			button.SetSelected(button.Index == _selectedIndex);
		}

		CancelButton.interactable = index != UNSELECT_INDEX;
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
		
		ExtraButtons.SetActive(false);
	}
}