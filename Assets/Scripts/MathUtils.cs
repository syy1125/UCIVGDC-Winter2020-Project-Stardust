using UnityEngine;

public class MathUtils
{
	public static float Atanh(float value)
	{
		return Mathf.Log((1 + value) / (1 - value)) / 2;
	}
}