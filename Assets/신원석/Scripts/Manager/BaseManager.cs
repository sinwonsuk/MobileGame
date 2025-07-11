using System;
using UnityEngine;

public abstract class baseManager
{
    public GameController controller { get; set; }

    public Type type { get; set; }
    public abstract void Init();
    public abstract void Update();

    // Provide a default implementation for ActiveOff to fix CS0501  
    virtual public void ActiveOff()
    {
       
    }

    virtual public void GetController(GameController controller)
    {

    }

}
