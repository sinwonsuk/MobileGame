using UnityEngine;

public class Slime : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        maxHp = 10f;  
        currentHp = maxHp;
    }

    protected override void Die()
    {
        Debug.Log("ΩΩ∂Û¿” ªÁ∏¡");
        base.Die();
    }
}
