using System.Collections;
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

	private IEnumerator DoTurnLogic()
	{
		Task logicTask = Task.Run(TurnLogicAction);
		yield return new WaitUntil(() => logicTask.IsCanceled);
	}

	private static void TurnLogicAction()
	{
		foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
		{
			ExecuteEvents.Execute<ITurnLogicListener>(root, null, (listener, _) => listener.DoTurnLogic());
		}
	}
}