using TMPro.EditorUtilities;
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

	public Vector3 GetPositionAt(float time)
	{
		return _fixed ? _position : default; // TODO use orbit
	}
}

[CustomEditor(typeof(CelestialBody))]
public class CelestialBodyEditor : Editor
{
	private SerializedProperty _gravitationalParameter;
	private SerializedProperty _fixed;
	private SerializedProperty _position;
	private SerializedProperty _orbit;

	private void OnEnable()
	{
		_gravitationalParameter = serializedObject.FindProperty("_gravitationalParameter");
		_fixed = serializedObject.FindProperty("_fixed");
		_position = serializedObject.FindProperty("_position");
		_orbit = serializedObject.FindProperty("_orbit");
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(_gravitationalParameter);

		EditorGUILayout.PropertyField(_fixed);

		if (_fixed.boolValue)
		{
			EditorGUILayout.PropertyField(_position);
		}
		else
		{
			EditorGUILayout.PropertyField(_orbit);
		}

		serializedObject.ApplyModifiedProperties();
	}
}