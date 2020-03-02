using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Tab
{
	public IndexSelectionButton Button;
	public GameObject Panel;
}

public class TabsController : MonoBehaviour, ICanSelectIndex
{
	public Tab[] Tabs;
	private int _selectedIndex;

	private void Awake()
	{
		for (int i = 0; i < Tabs.Length; i++)
		{
			Tabs[i].Button.Controller = this;
			Tabs[i].Button.Index = i;
		}
	}

	private void Start()
	{
		if (Tabs.Length > 0)
		{
			SelectIndex(0);
		}
	}

	public void SelectIndex(int index)
	{
		_selectedIndex = index;

		for (int i = 0; i < Tabs.Length; i++)
		{
			Tabs[i].Button.SetSelected(i == _selectedIndex);
			Tabs[i].Panel.SetActive(i == _selectedIndex);
		}
	}
}