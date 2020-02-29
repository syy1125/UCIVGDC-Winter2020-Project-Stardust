using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
	[Header("References")]
	public Button BuildButton;
	public GameObject BuildPanel;

	private void Start()
	{
		GameController.Instance.OnBodySelectionChanged.AddListener(UpdateDisplay);
		UpdateDisplay();
	}

	public void UpdateDisplay()
	{
		CelestialBody selected = GameController.Instance.SelectedBody;
		BuildButton.interactable = selected != null && selected.Colonizable;
	}

	public void ToggleBuildingScreen()
	{
		BuildPanel.SetActive(!BuildPanel.activeSelf);
	}
}