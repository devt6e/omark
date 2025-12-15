using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬에 존재하는 MarkerInstance들의 단일 레지스트리
/// - DefinitionId ↔ Instance 1:1 보장
/// - 데이터 / UI / 필터 로직 없음
/// </summary>
public class MarkerInstanceRegistry : MonoBehaviour
{
    public static MarkerInstanceRegistry Instance { get; private set; }

    // definitionId → MarkerInstance
    private readonly Dictionary<string, MarkerInstance> instances
        = new Dictionary<string, MarkerInstance>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // =========================
    // Register / Unregister
    // =========================

    public void Register(MarkerInstance instance)
    {
        if (instance == null)
            return;

        string id = instance.DefinitionId;
        if (string.IsNullOrEmpty(id))
            return;

        instances[id] = instance;
    }

    public void Unregister(string definitionId)
    {
        if (string.IsNullOrEmpty(definitionId))
            return;

        instances.Remove(definitionId);
    }

    public void Unregister(MarkerInstance instance)
    {
        if (instance == null)
            return;

        Unregister(instance.DefinitionId);
    }

    // =========================
    // Query
    // =========================

    public MarkerInstance Get(string definitionId)
    {
        if (string.IsNullOrEmpty(definitionId))
            return null;

        instances.TryGetValue(definitionId, out var instance);
        return instance;
    }

    public IEnumerable<MarkerInstance> GetAll()
    {
        return instances.Values;
    }

    public bool Contains(string definitionId)
    {
        return instances.ContainsKey(definitionId);
    }
}
