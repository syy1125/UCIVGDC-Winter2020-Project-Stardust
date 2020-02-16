using System;
using UnityEngine;

public class ForEachFrame : CustomYieldInstruction
{
	private readonly Action<float> _action;
	private readonly float _duration;
	private readonly bool _fraction;
	private readonly bool _realtime;
	private readonly float _startTime;

	private float GetTime() => _realtime ? Time.realtimeSinceStartup : Time.time;

	public ForEachFrame(Action<float> action, float duration, bool fraction = true, bool realtime = false)
	{
		_action = action;
		_duration = duration;
		_fraction = fraction;
		_realtime = realtime;

		_startTime = GetTime();
	}

	public override bool keepWaiting
	{
		get
		{
			float timePassed = GetTime() - _startTime;
			_action(
				_fraction
					? Mathf.Clamp01(timePassed / _duration)
					: Mathf.Clamp(timePassed, 0, _duration)
			);
			return timePassed < _duration;
		}
	}
}