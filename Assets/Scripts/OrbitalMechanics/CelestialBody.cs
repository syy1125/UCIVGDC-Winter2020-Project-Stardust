using System;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = System.Diagnostics.Debug;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Scriptable Objects/Celestial Body")]
public class CelestialBody : ScriptableObject
{
	[Header("Physics")]
	[SerializeField]
	private float _gravitationalParameter;
	public float GravitationalParameter => _gravitationalParameter;
	[SerializeField]
	private bool _fixed;
	public bool Fixed => _fixed;
	[SerializeField]
	private Vector3 _position;
	[SerializeField]
	private Orbit _orbit;
	public Orbit Orbit => _orbit;
	[SerializeField]
	private float _physicalRadius;
	public float PhysicalRadius => _physicalRadius;

	[Header("Gameplay")]
	[SerializeField]
	private bool _colonizable;
	public bool Colonizable => _colonizable;
	[SerializeField]
	private int _buildingGridWidth;
	public int BuildingGridWidth => _buildingGridWidth;
	[SerializeField]
	private int _buildingGridHeight;
	public int BuildingGridHeight => _buildingGridHeight;
	[FormerlySerializedAs("_dragCost")]
	[SerializeField]
	private float _atmosphereDragCost;
	public float AtmosphereDragCost => _atmosphereDragCost;


	[Header("Display")]
	[FormerlySerializedAs("_radius")]
	[SerializeField]
	private float _outlineRadius;
	public float OutlineRadius => _outlineRadius;
	[SerializeField]
	private Color _overviewColor;
	public Color OverviewColor => _overviewColor;
	[SerializeField]
	private Material _planetMaterial;
	public Material PlanetMaterial => _planetMaterial;
	[FormerlySerializedAs("_outlinerMaterial")]
	[SerializeField]
	private Material _outlineMaterial;
	public Material OutlineMaterial => _outlineMaterial;
	[SerializeField]
	[FormerlySerializedAs("DisplayName")]
	private string _displayName;
	public string DisplayName => _displayName;

	public Tuple<Vector3, Vector3> GetGlobalPositionAndVelocityAt(float time)
	{
		return _fixed
			? new Tuple<Vector3, Vector3>(_position, Vector3.zero)
			: _orbit.GetGlobalPositionAndVelocityAt(time);
	}

	public void SetOrbit(Orbit orbit)
	{
		_orbit = orbit;
		_fixed = false;
	}

	public float GetOrbitalSpeed()
	{
		if (Mathf.Approximately(_physicalRadius, 0)) return 0;
		return Mathf.Sqrt(_gravitationalParameter / _physicalRadius);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(CelestialBody))]
public class CelestialBodyEditor : Editor
{
	private SerializedProperty _orbit;

	private void OnEnable()
	{
		_orbit = serializedObject.FindProperty("_orbit");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var body = target as CelestialBody;
		Debug.Assert(body != null, nameof(body) + " != null");

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Spacecraft Characteristics", EditorStyles.boldLabel);

		EditorGUILayout.LabelField(
			"Orbital Speed",
			(body.GetOrbitalSpeed() * GameUnits.GAME_TO_PHYSICAL_LENGTH / GameUnits.GAME_TO_PHYSICAL_TIME)
			.ToString("0.0")
		);

		if (_orbit.isExpanded)
		{
			Orbit orbit = body.Orbit;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Keplerian Elements", EditorStyles.boldLabel);

			EditorGUILayout.LabelField("Semimajor Axis", orbit.SemimajorAxis.ToString("#00.00"));
			EditorGUILayout.LabelField("Eccentricity", orbit.Eccentricity.ToString("#0.000"));
			EditorGUILayout.LabelField("Inclination (deg)", (orbit.Inclination * Mathf.Rad2Deg).ToString("#0.000"));
			EditorGUILayout.LabelField(
				"L of AN (deg)",
				(orbit.LongitudeOfAscendingNode * Mathf.Rad2Deg).ToString("#0.000")
			);
			EditorGUILayout.LabelField(
				"Arg of PE (deg)",
				(orbit.ArgumentOfPeriapsis * Mathf.Rad2Deg).ToString("#0.000")
			);
			EditorGUILayout.LabelField(
				"TA at Epoch (deg)",
				(orbit.TrueAnomalyAtEpoch * Mathf.Rad2Deg).ToString("#0.000")
			);

			EditorGUILayout.LabelField("Orbit Characteristics", EditorStyles.boldLabel);

			EditorGUILayout.LabelField("Period", orbit.Period.ToString("#0.000"));
		}
	}
}
#endif