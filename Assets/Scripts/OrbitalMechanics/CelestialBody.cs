using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Celestial Body")]
public class CelestialBody : ScriptableObject
{
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

	[SerializeField]
	private float _radius;
	[SerializeField]
	private Color _overviewColor;
	public Color OverviewColor => _overviewColor;

	public Tuple<Vector3, Vector3> GetGlobalPositionAndVelocityAt(float time)
	{
		return _fixed ? new Tuple<Vector3, Vector3>(_position, Vector3.zero) : _orbit.GetGlobalPositionAndVelocityAt(time);
	}

	public void SetOrbit(Orbit orbit)
	{
		_orbit = orbit;
		_fixed = false;
	}
}

[CustomEditor(typeof(CelestialBody))]
public class CelestialBodyEditor : Editor
{
	private SerializedProperty _gravitationalParameter;
	private SerializedProperty _fixed;
	private SerializedProperty _position;
	private SerializedProperty _orbit;
	private SerializedProperty _radius;
	private SerializedProperty _overviewColor;

	private void OnEnable()
	{
		_gravitationalParameter = serializedObject.FindProperty("_gravitationalParameter");
		_fixed = serializedObject.FindProperty("_fixed");
		_position = serializedObject.FindProperty("_position");
		_orbit = serializedObject.FindProperty("_orbit");
		_radius = serializedObject.FindProperty("_radius");
		_overviewColor = serializedObject.FindProperty("_overviewColor");
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("Physics", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(_gravitationalParameter);
		EditorGUILayout.PropertyField(_fixed);
		EditorGUILayout.PropertyField(_fixed.boolValue ? _position : _orbit);

		EditorGUILayout.LabelField("Rendering", EditorStyles.boldLabel);

		EditorGUILayout.PropertyField(_radius);
		EditorGUILayout.PropertyField(_overviewColor);

		serializedObject.ApplyModifiedProperties();
	}
}