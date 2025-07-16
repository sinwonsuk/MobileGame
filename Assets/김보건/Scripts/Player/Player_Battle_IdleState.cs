using UnityEngine;

public interface IShooterState
{
    void Enter();
    void Update();
    void Exit();
}

public class Player_Battle_IdleState : IShooterState
{
    AutoShooter shooter;

    public Player_Battle_IdleState(AutoShooter shooter)
    {
        this.shooter = shooter;
    }

    public void Enter()
    {
        shooter.SetAttackAnimation(false);
    }

    public void Update()
    {
        if (shooter.IsEnemyNearby())
        {
            shooter.SetState(shooter.attackState);
        }
    }

    public void Exit() { }
}
