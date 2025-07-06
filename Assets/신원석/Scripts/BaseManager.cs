using System;
using UnityEngine;

public abstract class baseManager 
{
    public Type type { get; set; }
    public abstract void Init();
    public abstract void Update();
}
