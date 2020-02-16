using UnityEngine;

public class BuildingSelectionButtonUI : MonoBehaviour
{
	[HideInInspector]
	public BuildingSelectionUI BuildingSelection;
	[HideInInspector]
	public int Index;
	[HideInInspector]
	public BuildingTemplate Building;

	public void OnClick()
	{
		BuildingSelection.SetSelectedIndex(Index);
	}
}