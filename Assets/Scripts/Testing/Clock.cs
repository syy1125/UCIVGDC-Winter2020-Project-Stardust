using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Clock : MonoBehaviour
{
	private Text _text;

	private void Start()
	{
		_text = GetComponent<Text>();
	}

	private void Update()
	{
		_text.text = Time.time.ToString("00.000");
	}
}
