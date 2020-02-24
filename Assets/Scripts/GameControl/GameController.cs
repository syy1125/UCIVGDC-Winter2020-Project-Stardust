﻿using System.Collections;
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
	public UnityEvent OnBodySelectionChanged = new UnityEvent();

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
		OnEndAdvanceTurn.Invoke();
		State.CurrentTurn++;
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
}