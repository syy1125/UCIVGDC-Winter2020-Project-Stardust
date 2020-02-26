using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTransferButton : MonoBehaviour
{
	[Header("References")]
	public Dropdown FromPlanetSelect;
	public GameObject TransferPanel;

	private void HandleEscape(EscapeEvent e)
	{
		ToggleTransferPanel();
		e.Handle();
	}

	public void ToggleTransferPanel()
	{
		if (!TransferPanel.activeSelf)
		{
			TransferPanel.SetActive(true);
			FromPlanetSelect.value = GameController.Instance.State.StarSystem
				.FindIndex(item => item.Body == GameController.Instance.SelectedBody);
			GameController.Instance.OnEscapePressed.AddListener(HandleEscape);
		}
		else
		{
			TransferPanel.SetActive(false);
			GameController.Instance.OnEscapePressed.RemoveListener(HandleEscape);
		}
	}
}