using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Food/Explanation")]
public class ExplanationData : BaseScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public string key;
        public string explanation;
    }

    [SerializeField]
    private List<Entry> entries = new List<Entry>();

    public Dictionary<string, string> _map;

    public Dictionary<string, string> Map
    {
        get
        {
            if (_map == null)
            {
                BuildMap();
            }
            return _map;
        }
    }

    public bool TryGet(string key, out string explanation)
    {
        return Map.TryGetValue(key, out explanation);
    }

    private void BuildMap()
    {
        _map = new Dictionary<string, string>(entries.Count);
        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.key))
            {
                continue;
            }
            _map[entry.key] = entry.explanation;
        }
    }

    public void ClearMap()
    {
        if (_map != null)
        {
            _map.Clear();
        }
    }
}