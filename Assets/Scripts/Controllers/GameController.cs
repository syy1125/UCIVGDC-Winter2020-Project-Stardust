using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TurnAnimationController))]
public class GameController : MonoBehaviour
{
	public static GameController Instance { get; private set; }

	public int CurrentTurn { get; private set; }

	private bool _advancingTurn;
	public UnityEvent OnStartAdvanceTurn = new UnityEvent();
	public UnityEvent OnEndAdvanceTurn = new UnityEvent();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Debug.LogError(
				"Multiple game controller is being instantiated.\n"
				+ $"Destroying the new controller on {gameObject}"
			);
			Destroy(this);
		}
	}

	public void AdvanceTurn()
	{
		if (_advancingTurn) return;

		StartCoroutine(DoAdvanceTurn());
	}

	private IEnumerator DoAdvanceTurn()
	{
		_advancingTurn = true;
		OnStartAdvanceTurn.Invoke();

		Debug.Log("Starting turn animation");
		yield return GetComponent<TurnAnimationController>().AnimateTurn(CurrentTurn, CurrentTurn + 1);
		Debug.Log("Ending turn animation");

		Debug.Log("Starting turn logic");
		yield return GetComponent<TurnLogicController>().ExecuteTurnLogic();
		Debug.Log("Ending turn logic");

		_advancingTurn = false;
		OnEndAdvanceTurn.Invoke();
		CurrentTurn++;
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
}