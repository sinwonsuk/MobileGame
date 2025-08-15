using UnityEngine;

public class EatItem : MonoBehaviour
{
    public float pickupRange = 200f;
    public float moveSpeed = 5f;

    private Transform targetPlayer;

    void Start()
    {
        targetPlayer = GameController.instance?.playerTransform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, targetPlayer.position);
        if (distance <= pickupRange)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPlayer.position,
                moveSpeed * Time.deltaTime
            );

            if (distance <= 0.2f)
                OnCollected();
        }
    }

    void OnCollected()
    {
        var drop = GetComponent<DroppableItem>();
        if (drop != null)
        {
            var dungeonMgr = GameController.instance?.GetManager<DungeonManager>();
            dungeonMgr?.AddTempItem(drop.IngredientIndate, drop.amount);

        }
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.ItemPickup, false);
        Destroy(gameObject);
    }
}
