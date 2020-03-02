using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class IndexSelectionButton : MonoBehaviour
{
	[NonSerialized]
	public ICanSelectIndex Controller;
	[NonSerialized]
	public int Index;

	[Header("Config")]
	public Color SelectedColor = Color.cyan;

	private ColorBlock _initialColors;
	private void Awake()
	{
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
		Controller.SelectIndex(Index);
	}
}