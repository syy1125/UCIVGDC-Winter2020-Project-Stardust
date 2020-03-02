using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TogglePanelButton : MonoBehaviour
{
	[Header("References")]
	public GameObject Panel;

	[Header("Config")]
	public bool AutoActivate;
	public bool RequireColonizable;

	private void Start()
	{
		GameController.Instance.OnBodySelectionChanged.AddListener(UpdateDisplay);
		UpdateDisplay();
		GetComponent<Button>().onClick.AddListener(TogglePanel);
	}

	private void UpdateDisplay()
	{
		CelestialBody selected = GameController.Instance.SelectedBody;
		bool shouldBeActive = selected != null
		                      && (!RequireColonizable || selected.Colonizable);

		GetComponent<Button>().interactable = shouldBeActive;
		if (AutoActivate && shouldBeActive)
		{
			Panel.SetActive(true);
		}
		else if (!shouldBeActive)
		{
			Panel.SetActive(false);
		}
	}

	public void TogglePanel()
	{
		Panel.SetActive(!Panel.activeSelf);
	}
}