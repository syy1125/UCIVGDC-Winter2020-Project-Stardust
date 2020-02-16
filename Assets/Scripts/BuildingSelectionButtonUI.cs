using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BuildingSelectionButtonUI : MonoBehaviour
{
	[HideInInspector]
	public BuildingSelectionUI BuildingSelection;
	[HideInInspector]
	public int Index;
	[HideInInspector]
	public BuildingTemplate Building;

	[Header("References")]
	public Text BuildingName;

	[Header("Config")]
	public Color SelectedColor;

	private ColorBlock _initialColors;

	private void Start()
	{
		BuildingName.text = Building.DisplayName;
		_initialColors = GetComponent<Button>().colors;
	}

	public void SetSelected(bool selected)
	{
		var button = GetComponent<Button>();

		if (selected)
		{
			ColorBlock colors = button.colors;
			colors.disabledColor *= SelectedColor;
			colors.highlightedColor *= SelectedColor;
			colors.normalColor *= SelectedColor;
			colors.pressedColor *= SelectedColor;
			colors.selectedColor *= SelectedColor;
			button.colors = colors;
		}
		else
		{
			button.colors = _initialColors;
		}
	}

	public void OnClick()
	{
		BuildingSelection.SetSelectedIndex(Index);
	}
}