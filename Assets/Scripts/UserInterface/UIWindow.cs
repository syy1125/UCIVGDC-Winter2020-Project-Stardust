using UnityEngine;

public class UIWindow : MonoBehaviour
{
	private void OnEnable()
	{
		GameController.Instance.OnEscapePressed.AddListener(HandleEscapePress);
	}

	private void HandleEscapePress(EscapeEvent e)
	{
		gameObject.SetActive(false);
		e.SetHandled();
	}

	private void OnDisable()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.OnEscapePressed.RemoveListener(HandleEscapePress);
		}
	}
}