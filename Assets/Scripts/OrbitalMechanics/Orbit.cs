using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEngine;

public struct Ellipse
{
	public readonly Vector3 Center;
	public readonly Vector3 SemimajorAxis;
	public readonly Vector3 SemiminorAxis;

	public Ellipse(Vector3 center, Vector3 semimajorAxis, Vector3 semiminorAxis)
	{
		Center = center;
		SemimajorAxis = semimajorAxis;
		SemiminorAxis = semiminorAxis;
	}
}

[CreateAssetMenu(menuName = "Scriptable Objects/Orbit")]
public class Orbit : ScriptableObject
{
	[SerializeField]
	private CelestialBody _centralBody;
	public CelestialBody CentralBody => _centralBody;

	// Orbital elements that describe the orbit
	// Reference: https://en.wikipedia.org/wiki/Orbital_elements
	[SerializeField]
	private float _eccentricity;
	public float Eccentricity => _eccentricity;
	[SerializeField]
	private float _semimajorAxis;
	public float SemimajorAxis => _semimajorAxis;
	[SerializeField]
	private float _inclination;
	public float Inclination => _inclination;
	[SerializeField]
	private float _longitudeOfAscendingNode;
	public float LongitudeOfAscendingNode => _longitudeOfAscendingNode;
	[SerializeField]
	private float _argumentOfPeriapsis;
	public float ArgumentOfPeriapsis => _argumentOfPeriapsis;
	[SerializeField]
	private float _trueAnomalyAtEpoch;
	public float TrueAnomalyAtEpoch => _trueAnomalyAtEpoch;

	// Utility functions
	private const float ECCENTRIC_ANOMALY_SOLVE_THRESHOLD = 1E-6f;

	private float MeanToEccentricAnomaly(float meanAnomaly)
	{
		// Reference: https://space.stackexchange.com/questions/8911/determining-orbital-position-at-a-future-point-in-time
		float eccentricAnomaly = meanAnomaly;

		var i = 0; 
		for (; i < 5; i++)
		{
			float dE = (eccentricAnomaly - _eccentricity * Mathf.Sin(eccentricAnomaly) - meanAnomaly)
			           / (1 - _eccentricity * Mathf.Cos(eccentricAnomaly));
			eccentricAnomaly -= dE;
			if (Mathf.Abs(dE) < ECCENTRIC_ANOMALY_SOLVE_THRESHOLD) break;
		}
		
		return eccentricAnomaly;
	}

	// Derived attributes
	public float SemiminorAxis => _semimajorAxis * Mathf.Sqrt(1 - _eccentricity * _eccentricity);

	public float MeanAnomalyAtEpoch
	{
		get
		{
			// Reference: https://en.wikipedia.org/wiki/Kepler%27s_laws_of_planetary_motion#Position_as_a_function_of_time
			float eccentricAnomalyAtEpoch =
				2 * Mathf.Atan2(
					Mathf.Sqrt((1 - _eccentricity) / (1 + _eccentricity)) * Mathf.Sin(_trueAnomalyAtEpoch / 2),
					Mathf.Cos(_trueAnomalyAtEpoch / 2)
				);
			return eccentricAnomalyAtEpoch - _eccentricity * Mathf.Sin(eccentricAnomalyAtEpoch);
		}
	}

	private float MeanAngularVelocity => Mathf.Sqrt(_centralBody.GravitationalParameter / Mathf.Pow(_semimajorAxis, 3));

	/** Is a basis that makes "right" point toward the periapsis and "up" point out of the orbital plane */
	private Quaternion GetBasis() => Quaternion.AngleAxis(-_longitudeOfAscendingNode * Mathf.Rad2Deg, Vector3.up)
	                                 * Quaternion.AngleAxis(-_inclination * Mathf.Rad2Deg, Vector3.right)
	                                 * Quaternion.AngleAxis(-_argumentOfPeriapsis * Mathf.Rad2Deg, Vector3.up);

	public Ellipse GetLocalEllipse()
	{
		Quaternion basis = GetBasis();
		Vector3 center = -Vector3.right * _semimajorAxis * _eccentricity;
		return new Ellipse(
			basis * center,
			basis * (Vector3.right * _semimajorAxis),
			basis * (Vector3.forward * SemiminorAxis)
		);
	}

	public Vector3 GetLocalPositionAt(float time)
	{
		float meanAnomaly = (MeanAnomalyAtEpoch + time * MeanAngularVelocity) % (2 * Mathf.PI);
		float eccentricAnomaly = MeanToEccentricAnomaly(meanAnomaly);

		Ellipse ellipse = GetLocalEllipse();
		return ellipse.Center
		       + ellipse.SemimajorAxis * Mathf.Cos(eccentricAnomaly)
		       + ellipse.SemiminorAxis * Mathf.Sin(eccentricAnomaly);
	}

	public Vector3 GetGlobalPositionAt(float time)
	{
		return _centralBody.GetGlobalPositionAt(time) + GetLocalPositionAt(time);
	}
}