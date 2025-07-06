using UnityEngine;

public class Slime : EnemyBase
{
    protected override void Start()
    {
        base.Start();
        maxHp = 50f;  
        currentHp = maxHp;
    }

    protected override void Die()
    {
        Debug.Log("������ ���");
        base.Die();
    }
}
