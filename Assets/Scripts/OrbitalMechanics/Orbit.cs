using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[CreateAssetMenu(menuName = "Scriptable Objects/Orbit")]
public class Orbit : ScriptableObject
{
	[SerializeField]
	private CelestialBody _centralBody;
	public CelestialBody CentralBody => _centralBody;

	// Based on orbital elements that describe the orbit, with modifications to avoid singularities 
	// References:
	// https://en.wikipedia.org/wiki/Orbital_elements
	// https://en.wikipedia.org/wiki/Kepler_orbit
	[SerializeField]
	private float _eccentricity;
	public float Eccentricity => _eccentricity;

	[SerializeField]
	// Replaces semi-major axis to avoid singularity in parabolic case
	private float _semilatusRectum;
	public float SemilatusRectum => _semilatusRectum;

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

	// Derived attributes
	public float SemimajorAxis => _semilatusRectum / (1 - Mathf.Pow(_eccentricity, 2));
	public float SemiminorAxis => _semilatusRectum / Mathf.Sqrt(1 - Mathf.Pow(_eccentricity, 2));

	public float EnergyPerUnitMass => -_centralBody.GravitationalParameter * (1 - Mathf.Pow(_eccentricity, 2))
	                                  / (2 * _semilatusRectum);

	private static float Repeat(float value, float min, float max)
	{
		Debug.Assert(max > min, "max > min");
		float diff = max - min;
		value = (value - min) % diff + min;
		if (value < min) value += diff;
		return value;
	}

	private void OnValidate()
	{
		_eccentricity = Mathf.Max(_eccentricity, 0);
		_semilatusRectum = Mathf.Max(_semilatusRectum, float.Epsilon);
		_inclination = Repeat(_inclination, 0, Mathf.PI * 2);
		_longitudeOfAscendingNode = Repeat(_longitudeOfAscendingNode, 0, Mathf.PI * 2);
		_argumentOfPeriapsis = Repeat(_argumentOfPeriapsis, 0, Mathf.PI * 2);
		float angleLimit = _eccentricity < 1 ? Mathf.PI : Mathf.Acos(-1 / _eccentricity);
		_trueAnomalyAtEpoch = Mathf.Clamp(_trueAnomalyAtEpoch, -angleLimit, angleLimit);
	}

	// Config variables
	private const int SOLVER_ITERATION_LIMIT = 10;
	private const float SOLVER_SMALL_THRESHOLD = 1e-15f;

	/** Is a basis that makes "right" point toward the periapsis and "up" point out of the orbital plane */
	private Quaternion GetBasis() => Quaternion.AngleAxis(-_longitudeOfAscendingNode * Mathf.Rad2Deg, Vector3.up)
	                                 * Quaternion.AngleAxis(-_inclination * Mathf.Rad2Deg, Vector3.right)
	                                 * Quaternion.AngleAxis(-_argumentOfPeriapsis * Mathf.Rad2Deg, Vector3.up);

	private float GetRadius(float trueAnomaly)
	{
		return _semilatusRectum / (1 + _eccentricity * Mathf.Cos(trueAnomaly));
	}

	public Vector3[] GetDrawPositions(int segmentCount)
	{
		Quaternion basis = GetBasis();

		var points = new Vector3[segmentCount + 1];

		float angleLimit = _eccentricity < 1 ? Mathf.PI : Mathf.Acos(-1 / _eccentricity) * 0.99f;
		for (var i = 0; i < segmentCount + 1; i++)
		{
			float angle = Mathf.Lerp(-angleLimit, angleLimit, (float) i / segmentCount);
			float r = GetRadius(angle);
			points[i] = basis * new Vector3(r * Mathf.Cos(angle), 0, r * Mathf.Sin(angle));
		}

		return points;
	}

	private float GetTrueAnomalyAt(float time)
	{
		if (Mathf.Approximately(time, 0))
		{
			return _trueAnomalyAtEpoch;
		}

		if (Mathf.Approximately(_eccentricity, 1)) // Parabolic case
		{
			// Reference: https://en.wikipedia.org/wiki/Parabolic_trajectory#Barker's_equation

			// Solve for time of periapsis passage
			float parabolicAnomalyAtEpoch = Mathf.Tan(_trueAnomalyAtEpoch / 2);
			float periapsisTime = -Mathf.Sqrt(Mathf.Pow(_semilatusRectum, 3) / _centralBody.GravitationalParameter) / 2
			                      * (parabolicAnomalyAtEpoch + Mathf.Pow(parabolicAnomalyAtEpoch, 3) / 2);
			// Use the substitutions given in the reference
			float a = 3 * Mathf.Sqrt(_centralBody.GravitationalParameter / Mathf.Pow(_semilatusRectum, 3))
			            * (time - periapsisTime);
			float b = Mathf.Pow(a + Mathf.Sqrt(Mathf.Pow(a, 2) + 1), 1 / 3f);
			return 2 * Mathf.Atan(b - 1 / b);
		}

		if (_eccentricity < 1) // Elliptic case
		{
			// References:
			// https://en.wikipedia.org/wiki/Mean_anomaly#Formulae
			// https://en.wikipedia.org/wiki/True_anomaly#From_the_eccentric_anomaly

			float eccentricAnomalyAtEpoch =
				2 * Mathf.Atan2(
					Mathf.Sqrt((1 - _eccentricity) / (1 + _eccentricity)) * Mathf.Sin(_trueAnomalyAtEpoch / 2),
					Mathf.Cos(_trueAnomalyAtEpoch / 2)
				);

			float meanAnomalyAtEpoch = eccentricAnomalyAtEpoch - _eccentricity * Mathf.Sin(eccentricAnomalyAtEpoch);
			float meanMotion = Mathf.Sqrt(
				_centralBody.GravitationalParameter * Mathf.Pow((1 - Mathf.Pow(_eccentricity, 2)) / _semilatusRectum, 3)
			);
			float meanAnomaly = (meanAnomalyAtEpoch + meanMotion * time) % (2 * Mathf.PI);

			// Use Newton's solver
			float eccentricAnomaly = meanAnomaly;
			for (var i = 0; i < SOLVER_ITERATION_LIMIT; i++)
			{
				float dE = -(eccentricAnomaly - _eccentricity * Mathf.Sin(eccentricAnomaly) - meanAnomaly)
				           / (1 - _eccentricity * Mathf.Cos(eccentricAnomaly));
				eccentricAnomaly += dE;
				if (Mathf.Abs(dE) < SOLVER_SMALL_THRESHOLD) break;
			}

			return 2 * Mathf.Atan2(
				       Mathf.Sqrt(1 + _eccentricity) * Mathf.Sin(eccentricAnomaly / 2),
				       Mathf.Sqrt(1 - _eccentricity) * Mathf.Cos(eccentricAnomaly / 2)
			       );
		}
		else // Hyperbolic case
		{
			// Reference: https://en.wikipedia.org/wiki/Hyperbolic_trajectory#Equations_of_motion
			float hyperbolicAnomalyAtEpoch = 2 * MathUtils.Atanh(
				                                 Mathf.Sqrt((_eccentricity - 1) / (_eccentricity + 1))
				                                 * Mathf.Tan(_trueAnomalyAtEpoch / 2)
			                                 );
			float meanAnomalyAtEpoch =
				_eccentricity * (float) Math.Sinh(hyperbolicAnomalyAtEpoch) - hyperbolicAnomalyAtEpoch;

			float meanMotion = Mathf.Sqrt(
				_centralBody.GravitationalParameter * Mathf.Pow((Mathf.Pow(_eccentricity, 2) - 1) / _semilatusRectum, 3)
			);
			float meanAnomaly = meanAnomalyAtEpoch + meanMotion * time;

			// Use Newton's solver
			// Initial value estimate provided by Danby, Fundamentals of Celesital Mechanics (p.176)
			float hyperbolicAnomaly = Mathf.Log(2 * meanAnomaly / _eccentricity + 1.8f);
			for (var i = 0; i < SOLVER_ITERATION_LIMIT; i++)
			{
				float dH = -(_eccentricity * (float) Math.Sinh(hyperbolicAnomaly) - hyperbolicAnomaly - meanAnomaly)
				           / (_eccentricity * (float) Math.Cosh(hyperbolicAnomaly) - 1);
				hyperbolicAnomaly += dH;
				if (Mathf.Abs(dH) < SOLVER_SMALL_THRESHOLD) break;
			}

			return 2 * Mathf.Atan2(
				       Mathf.Sqrt(_eccentricity + 1) * (float) Math.Sinh(hyperbolicAnomaly / 2),
				       Mathf.Sqrt(_eccentricity - 1) * (float) Math.Cosh(hyperbolicAnomaly / 2)
			       );
		}
	}

	private Tuple<Vector3, Vector3> GetLocalPositionAndVelocityAt(float time)
	{
		float trueAnomaly = GetTrueAnomalyAt(time);
		float radius = _semilatusRectum / (1 + _eccentricity * Mathf.Cos(trueAnomaly));

		var position = new Vector3(
			radius * Mathf.Cos(trueAnomaly),
			0,
			radius * Mathf.Sin(trueAnomaly)
		);

		// Direction solved using mathematica
		Vector3 velocityDirection = new Vector3(
			-Mathf.Sin(trueAnomaly),
			0,
			_eccentricity + Mathf.Cos(trueAnomaly)
		).normalized;
		float speed = Mathf.Sqrt(2 * (EnergyPerUnitMass + _centralBody.GravitationalParameter / radius));

		Quaternion basis = GetBasis();

		return new Tuple<Vector3, Vector3>(
			basis * position,
			basis * (speed * velocityDirection)
		);
	}

	public Tuple<Vector3, Vector3> GetGlobalPositionAndVelocityAt(float time)
	{
		(Vector3 centralPosition, Vector3 centralVelocity) = _centralBody.GetGlobalPositionAndVelocityAt(time);
		(Vector3 localPosition, Vector3 localVelocity) = GetLocalPositionAndVelocityAt(time);
		return new Tuple<Vector3, Vector3>(centralPosition + localPosition, centralVelocity + localVelocity);
	}

	// TODO This function needs testing
	public static Orbit FromPositionAndVelocity(
		CelestialBody centralBody,
		Vector3 position,
		Vector3 velocity,
		float time
	)
	{
		// Reference: https://downloads.rene-schwarz.com/download/M002-Cartesian_State_Vectors_to_Keplerian_Orbit_Elements.pdf
		// With adaptations to account for Unity's left-handed system and y-direction being up.
		Vector3 orbitMomentum = Vector3.Cross(position, velocity);

		Vector3 eccentricityVector = Vector3.Cross(velocity, orbitMomentum) / centralBody.GravitationalParameter
		                             - position.normalized;

		Vector3 ascendingNodeDirection = Vector3.Cross(Vector3.up, orbitMomentum);

		float trueAnomaly = Vector3.SignedAngle(eccentricityVector, position, orbitMomentum);
		float inclination = Mathf.Acos(orbitMomentum.y / orbitMomentum.magnitude);
		float eccentricity = eccentricityVector.magnitude;
		float longitudeOfAscendingNode = Mathf.Atan2(ascendingNodeDirection.z, ascendingNodeDirection.x);
		float argumentOfPeriapsis = Vector3.SignedAngle(ascendingNodeDirection, eccentricityVector, orbitMomentum);
		float semilatusRectum = position.magnitude * (1 + eccentricity * Mathf.Cos(trueAnomaly));

		var orbit = CreateInstance<Orbit>();
		orbit._centralBody = centralBody;
		orbit._eccentricity = eccentricity;
		orbit._semilatusRectum = semilatusRectum;
		orbit._inclination = inclination;
		orbit._longitudeOfAscendingNode = longitudeOfAscendingNode;
		orbit._argumentOfPeriapsis = argumentOfPeriapsis;

		// To find true anomaly at epoch, we first pretend that the current true anomaly is at "epoch".
		// Then we can take advantage of the function already written in order to run time backwards and find the actual true anomaly at epoch.
		orbit._trueAnomalyAtEpoch = trueAnomaly;
		orbit._trueAnomalyAtEpoch = orbit.GetTrueAnomalyAt(-time);

		return orbit;
	}
}