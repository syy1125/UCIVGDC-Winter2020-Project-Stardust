using System;
using UnityEngine;
using UnityEngine.UI;

public class PlanetInfoUI : MonoBehaviour
{
	public Button PlanetInfoButton;
	public GameObject PlanetInfoPanel;

	private void Start()
	{
		GameController.Instance.OnBodySelectionChanged.AddListener(UpdateDisplay);
		UpdateDisplay();
	}

	public void ToggleInfoPanel()
	{
		PlanetInfoPanel.SetActive(!PlanetInfoPanel.activeSelf);
	}

	private void UpdateDisplay()
	{
		if (GameController.Instance.SelectedBody == null)
		{
			PlanetInfoButton.interactable = false;
			PlanetInfoPanel.SetActive(false);
		}
		else
		{
			PlanetInfoButton.interactable = true;
		}
	}
}