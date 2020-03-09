using UnityEngine;

public abstract class GameUI : MonoBehaviour
{
	public abstract void UpdateDisplay();

	protected virtual void OnEnable()
	{
		GameController.Instance.OnBodySelectionChanged.AddListener(UpdateDisplay);
		GameController.Instance.OnEndAdvanceTurn.AddListener(UpdateDisplay);
		UpdateDisplay();
	}

	private void OnDisable()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.OnBodySelectionChanged.AddListener(UpdateDisplay);
			GameController.Instance.OnEndAdvanceTurn.AddListener(UpdateDisplay);
		}
	}
}