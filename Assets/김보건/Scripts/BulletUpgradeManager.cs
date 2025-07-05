using UnityEngine;

public class BulletUpgradeManager : MonoBehaviour
{
    public BulletData currentBullet;

    public void SetBullet(BulletData newBullet)
    {
        currentBullet = newBullet;
    }

    public BulletData GetCurrentBullet()
    {
        return currentBullet;
    }
}
