using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlanetInfoButton : MonoBehaviour
{
	[FormerlySerializedAs("PlanetInfoButton")]
	public Button InfoButton;
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
			InfoButton.interactable = false;
			PlanetInfoPanel.SetActive(false);
		}
		else
		{
			InfoButton.interactable = true;
		}
	}
}