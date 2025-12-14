using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배치된 MarkerInstance 관리 전용
/// </summary>
public class MarkerInstanceRegistry : MonoBehaviour
{
    public static MarkerInstanceRegistry Instance { get; private set; }

    private readonly List<MarkerInstance> instances = new();

    private void Awake()
    {
        Instance = this;
    }

    public void Register(MarkerInstance instance)
    {
        if (!instances.Contains(instance))
            instances.Add(instance);
    }

    public void Unregister(MarkerInstance instance)
    {
        instances.Remove(instance);
    }

    public void RemoveAllByDefinition(string definitionId)
    {
        for (int i = instances.Count - 1; i >= 0; i--)
        {
            if (instances[i].DefinitionId == definitionId)
            {
                Destroy(instances[i].gameObject);
                instances.RemoveAt(i);
            }
        }
    }

    public List<MarkerInstance> GetFavoriteInstances()
    {
        List<MarkerInstance> result = new();

        foreach (var inst in instances)
        {
            var def = MarkerDefinitionRepository.Instance
                .GetById(inst.DefinitionId);

            if (def != null && def.IsFavorite)
                result.Add(inst);
        }

        return result;
    }
}
