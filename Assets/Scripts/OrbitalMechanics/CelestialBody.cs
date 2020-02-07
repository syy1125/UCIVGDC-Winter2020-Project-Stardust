using System;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
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
	[SerializeField]
	private Vector3 _position;
	[SerializeField]
	private Orbit _orbit;
	public Orbit Orbit => _orbit;

	[Header("Gameplay")]
	[SerializeField]
	private int _buildingGridWidth;
	public int BuildingGridWidth => _buildingGridWidth;
	[SerializeField]
	private int _buildingGridHeight;
	public int BuildingGridHeight => _buildingGridHeight;

	[Header("Rendering")]
	[SerializeField]
	private float _radius;
	[SerializeField]
	private Color _overviewColor;
	public Color OverviewColor => _overviewColor;

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
}

#if UNITY_EDITOR
[CustomEditor(typeof(CelestialBody))]
public class CelestialBodyEditor : Editor
{
	private SerializedProperty _gravitationalParameter;
	private SerializedProperty _fixed;
	private SerializedProperty _position;
	private SerializedProperty _orbit;
	private SerializedProperty _buildingGridWidth;
	private SerializedProperty _buildingGridHeight;
	private SerializedProperty _radius;
	private SerializedProperty _overviewColor;

	private void OnEnable()
	{
		_gravitationalParameter = serializedObject.FindProperty("_gravitationalParameter");
		_fixed = serializedObject.FindProperty("_fixed");
		_position = serializedObject.FindProperty("_position");
		_orbit = serializedObject.FindProperty("_orbit");
		_buildingGridWidth = serializedObject.FindProperty("_buildingGridWidth");
		_buildingGridHeight = serializedObject.FindProperty("_buildingGridHeight");
		_radius = serializedObject.FindProperty("_radius");
		_overviewColor = serializedObject.FindProperty("_overviewColor");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (_orbit.isExpanded)
		{
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Keplerian Elements", EditorStyles.boldLabel);

			Orbit orbit = ((CelestialBody) target).Orbit;
			EditorGUILayout.LabelField("Semimajor Axis", orbit.SemimajorAxis.ToString("#00.00"));
			EditorGUILayout.LabelField("Eccentricity", orbit.Eccentricity.ToString("#0.000"));
			EditorGUILayout.LabelField("Inclination (deg)", (orbit.Inclination * Mathf.Rad2Deg).ToString("#0.000"));
			EditorGUILayout.LabelField(
				"L of AN (deg)", (orbit.LongitudeOfAscendingNode * Mathf.Rad2Deg).ToString("#0.000")
			);
			EditorGUILayout.LabelField(
				"Arg of PE (deg)", (orbit.ArgumentOfPeriapsis * Mathf.Rad2Deg).ToString("#0.000")
			);
			EditorGUILayout.LabelField(
				"TA at Epoch (deg)", (orbit.TrueAnomalyAtEpoch * Mathf.Rad2Deg).ToString("#0.000")
			);

			EditorGUILayout.LabelField("Orbit Characteristics", EditorStyles.boldLabel);

			EditorGUILayout.LabelField("Period", orbit.Period.ToString("#0.000"));
		}
	}
}
#endif