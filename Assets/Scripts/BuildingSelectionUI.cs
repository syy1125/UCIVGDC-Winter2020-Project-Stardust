using System;
using UnityEngine;

public class BuildingSelectionUI : MonoBehaviour
{
	public BuildingTemplate[] Buildings;
	public GameObject BuildingButtonPrefab;

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
	}

	public void SetSelectedIndex(int index)
	{
		Debug.Log($"Selecting index {index}");
		_selectedIndex = index;
	}

	public void UnloadBuildings()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}
}