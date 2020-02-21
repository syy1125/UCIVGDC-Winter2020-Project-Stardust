using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TurnLogicController : MonoBehaviour
{
	public Coroutine ExecuteTurnLogic()
	{
		return StartCoroutine(DoTurnLogic());
	}

	private static IEnumerator DoTurnLogic()
	{
		var roots = new List<GameObject>();

		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			foreach (GameObject root in SceneManager.GetSceneAt(i).GetRootGameObjects())
			{
				ExecuteTurnLogicInChildren(root);
				yield return null;
			}
		}
	}

	private static void ExecuteTurnLogicInChildren(GameObject root)
	{
		ExecuteEvents.Execute<ITurnLogicListener>(
			root,
			null,
			(listener, _) =>
			{
				try
				{
					listener.DoTurnLogic();
				}
				catch (Exception e)
				{
					Debug.LogError($"Unhandled exception during turn logic in ${root}: {e.Message}\n{e.StackTrace}");
				}
			}
		);
		foreach (Transform child in root.transform)
		{
			ExecuteTurnLogicInChildren(child.gameObject);
		}
	}
}