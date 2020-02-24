using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TurnAnimationController : MonoBehaviour
{
	public static TurnAnimationController Instance { get; private set; }

	public AnimationCurve TimeCurve;
	public float AnimationLength;
	public float TurnTime { get; private set; }

	public UnityEvent OnTimeChanged = new UnityEvent();

	private void Awake()
	{
		Instance = this;
	}

	public Coroutine AnimateTurn(int currentTurn, int nextTurn)
	{
		return StartCoroutine(DoAnimateTurn(currentTurn, nextTurn));
	}

	private IEnumerator DoAnimateTurn(int currentTurn, int nextTurn)
	{
		float startTime = Time.time;
		while (Time.time - startTime < AnimationLength)
		{
			TurnTime = Mathf.Lerp(
				currentTurn,
				nextTurn,
				TimeCurve.Evaluate((Time.time - startTime) / AnimationLength)
			);
			OnTimeChanged.Invoke();
			yield return null;
		}

		TurnTime = nextTurn;
	}
}