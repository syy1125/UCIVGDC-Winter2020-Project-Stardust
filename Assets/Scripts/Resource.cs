using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Resource", fileName = "Resource")]
public class Resource : ScriptableObject
{
	public string DisplayName;
	public Sprite Icon;
}