using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct ResourceCapacityEntry
{
	public Resource Resource;
	public int Amount;
}

public class PlanetResourceController : MonoBehaviour, ITurnLogicListener
{
	public ResourceCapacityEntry[] BaseCapacity;

	private Dictionary<Resource, int> _storage;

	private void Start()
	{
		_storage = new Dictionary<Resource, int>();
	}

	public void DoTurnLogic()
	{
		var resourceDelta = new Dictionary<Resource, int>();
		var resourceConsumers = new Dictionary<Resource, List<Tuple<BuildingInstance, EffectGroupInstance>>>();

		BuildingInstance[] buildings = GetComponent<PlanetBuildingController>().GetBuildings();

		foreach (BuildingInstance building in buildings)
		{
			ComposeProductionEffect(resourceDelta, resourceConsumers, building);
		}

		var disabledGroups = new HashSet<EffectGroupInstance>();

		while (FindDeficit(resourceDelta, out Resource resource))
		{
			(BuildingInstance building, EffectGroupInstance effectGroup) =
				ChooseRandomConsumer(resource, resourceConsumers);

			disabledGroups.Add(effectGroup);

			RemoveEffectGroup(resourceDelta, effectGroup);

			Debug.Log($"Disabled an effect group of {building.Template.DisplayName} because of resource deficit");
			// TODO maybe send message warning about resource deficit
		}

		ApplyResourceDelta(resourceDelta);

		Dictionary<Resource, int> capacity = ComputeResourceCapacity(buildings, disabledGroups);
		EnforceCapacity(capacity);
	}

	private static void ComposeProductionEffect(
		IDictionary<Resource, int> resourceDelta,
		IDictionary<Resource, List<Tuple<BuildingInstance, EffectGroupInstance>>> resourceConsumers,
		BuildingInstance building
	)
	{
		foreach (EffectGroupInstance group in building.EffectGroups)
		{
			foreach (BuildingEffect effect in group.Effects)
			{
				switch (effect.Type)
				{
					case BuildingEffect.EffectType.Produce:
						AddValue(resourceDelta, effect.Resource, effect.Amount);
						break;
					case BuildingEffect.EffectType.Consume:
						AddValue(resourceDelta, effect.Resource, -effect.Amount);
						AppendList(
							resourceConsumers,
							effect.Resource,
							new Tuple<BuildingInstance, EffectGroupInstance>(building, group)
						);
						break;
				}
			}
		}
	}

	private static void AddValue<T>(IDictionary<T, int> dict, T key, int delta)
	{
		if (dict.TryGetValue(key, out int value))
		{
			dict[key] += delta;
		}
		else
		{
			dict[key] = delta;
		}
	}

	private static void AppendList<TKey, TEntry, TList>(IDictionary<TKey, TList> dict, TKey key, TEntry entry)
		where TList : IList<TEntry>, new()
	{
		if (!dict.TryGetValue(key, out TList list))
		{
			list = dict[key] = new TList();
		}

		list.Add(entry);
	}

	private bool FindDeficit(IDictionary<Resource, int> resourceDelta, out Resource deficit)
	{
		deficit = null;

		foreach (KeyValuePair<Resource, int> entry in resourceDelta)
		{
			int currentStorage = _storage.TryGetValue(entry.Key, out int value) ? value : 0;
			if (currentStorage + entry.Value < 0)
			{
				deficit = entry.Key;
				return true;
			}
		}

		return false;
	}

	private static Tuple<BuildingInstance, EffectGroupInstance> ChooseRandomConsumer(
		Resource resource,
		IReadOnlyDictionary<Resource, List<Tuple<BuildingInstance, EffectGroupInstance>>> resourceConsumers
	)
	{
		List<Tuple<BuildingInstance, EffectGroupInstance>> consumers = resourceConsumers[resource];
		Debug.Assert(consumers.Count > 0, $"Resource {resource.DisplayName} is in deficit but no consumers found");
		return consumers[(int) (Random.value * consumers.Count)];
	}

	private static void RemoveEffectGroup(IDictionary<Resource, int> resourceDelta, EffectGroupInstance group)
	{
		foreach (BuildingEffect effect in group.Effects)
		{
			switch (effect.Type)
			{
				case BuildingEffect.EffectType.Produce:
					resourceDelta[effect.Resource] -= effect.Amount;
					break;
				case BuildingEffect.EffectType.Consume:
					resourceDelta[effect.Resource] += effect.Amount;
					break;
			}
		}
	}

	private void ApplyResourceDelta(IDictionary<Resource, int> resourceDelta)
	{
		foreach (KeyValuePair<Resource, int> entry in resourceDelta)
		{
			AddValue(_storage, entry.Key, entry.Value);
		}
	}

	private Dictionary<Resource, int> ComputeResourceCapacity(
		IEnumerable<BuildingInstance> buildings,
		ICollection<EffectGroupInstance> disabledGroups
	)
	{
		var capacity = new Dictionary<Resource, int>();

		foreach (ResourceCapacityEntry entry in BaseCapacity)
		{
			capacity[entry.Resource] = entry.Amount;
		}

		foreach (BuildingInstance building in buildings)
		{
			foreach (EffectGroupInstance group in building.EffectGroups)
			{
				if (disabledGroups.Contains(group)) continue;

				foreach (BuildingEffect effect in group.Effects)
				{
					if (effect.Type == BuildingEffect.EffectType.CapacityAdd)
					{
						AddValue(capacity, effect.Resource, effect.Amount);
					}
				}
			}
		}

		return capacity;
	}

	private void EnforceCapacity(IReadOnlyDictionary<Resource, int> capacity)
	{
		foreach (KeyValuePair<Resource, int> entry in _storage)
		{
			if (!capacity.TryGetValue(entry.Key, out int max)) max = 0;
			_storage[entry.Key] = Mathf.Min(entry.Value, max);
		}
	}
}