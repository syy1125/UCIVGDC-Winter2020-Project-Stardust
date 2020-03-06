using System;
using System.Collections;
using System.Security;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TurnAnimationController))]
public class GameController : MonoBehaviour
{
	public static GameController Instance { get; private set; }

	public readonly GameState State = new GameState();
	public CelestialBody SelectedBody { get; private set; }

	private bool _advancingTurn;
	public UnityEvent OnStartAdvanceTurn = new UnityEvent();
	public UnityEvent OnEndAdvanceTurn = new UnityEvent();
	[NonSerialized]
	public readonly UnityEvent OnBodySelectionChanged = new UnityEvent();
	public readonly EscapeEventBus OnEscapePressed = new EscapeEventBus();

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

	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Escape)) return;

		bool handled = OnEscapePressed.Invoke();
		if (handled) return;

		// Default escape behaviour
		if (SelectedBody != null)
		{
			SetSelectedBody(null);
		}
	}

	public void SetSelectedBody(CelestialBody body)
	{
		SelectedBody = body;
		OnBodySelectionChanged.Invoke();
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
		yield return GetComponent<TurnAnimationController>().AnimateTurn(State.CurrentTurn, State.CurrentTurn + 1);
		Debug.Log("Ending turn animation");

		Debug.Log("Starting turn logic");
		State.DoTurnLogic();
		Debug.Log("Ending turn logic");

		_advancingTurn = false;
		State.CurrentTurn++;
		OnEndAdvanceTurn.Invoke();
	}

	public CelestialBodyLogic GetSelectedLogic()
	{
		if (SelectedBody == null) return null;
		return State.FindLogicComponent(SelectedBody);
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
}