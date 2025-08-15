using UnityEngine;

public class EatItem : MonoBehaviour
{
    public float pickupRange = 200f;
    public float moveSpeed = 5f;

    private Transform targetPlayer;

    private DroppableItem droppableItem;
    private DungeonManager dungeonManager;
    void Start()
    {
        droppableItem = GetComponent<DroppableItem>();
        targetPlayer = GameController.instance?.playerTransform;
        dungeonManager = GameController.instance?.GetManager<DungeonManager>();
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
       
        if (droppableItem != null)
        {

            dungeonManager?.AddTempItem(droppableItem.IngredientIndate, droppableItem.amount);
            
        }
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.ItemPickup, false);

        //gameObject.SetActive(false);

       Destroy(gameObject);
    }
}
