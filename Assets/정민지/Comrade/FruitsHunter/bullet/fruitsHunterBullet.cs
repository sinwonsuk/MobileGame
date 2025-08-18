using UnityEngine;

public class fruitsHunterBullet : MonoBehaviour
{
    public float rotateSpeed = 360f;

    void Update()
    {
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }
}
