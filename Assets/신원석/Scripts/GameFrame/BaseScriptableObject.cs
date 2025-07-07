using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Manager")]

public abstract class BaseScriptableObject : ScriptableObject
{
    public Type type {  get; set; }
}