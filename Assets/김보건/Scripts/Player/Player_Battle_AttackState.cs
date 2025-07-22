using UnityEngine;

public class Player_Battle_AttackState : IShooterState
{
    AutoShooter shooter;
    float remainTime;

    public Player_Battle_AttackState(AutoShooter shooter)
    {
        this.shooter = shooter;
    }

    public void Enter()
    {
        //shooter.SetAttackAnimation(true); // 손 뻗는 애니메이션 켜기
        remainTime = shooter.manualAttackHoldTime;
    }

    public void Exit()
    {
        //shooter.SetAttackAnimation(false);
    }

    public void Update()
    {
        if (shooter == null)
            return;

        // 자동공격 상태일 경우 계속 유지
        if (shooter.IsEnemyNearby())
        {
            //shooter.TryAutoFire();
            //remainTime = shooter.manualAttackHoldTime; // 자동공격 중이면 시간 리셋
            shooter.TryAutoFire();
            return;
        }

        // 사용자가 터치 중이라면 유지 시간 연장
        if (Input.GetMouseButton(0))
        {
            remainTime = shooter.manualAttackHoldTime;
        }

        // 시간 흐름
        remainTime -= Time.deltaTime;

        // 유지시간이 다 되면 Idle로 복귀
        if (remainTime <= 0f)
        {
            shooter.SetState(shooter.idleState);
        }
    }
}
