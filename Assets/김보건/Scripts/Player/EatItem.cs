using UnityEngine;

public class EatItem : MonoBehaviour
{
    public float pickupRange = 200f;
    public float moveSpeed = 5f;

    private Transform targetPlayer;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPlayer == null)
        {
            FindPlayer();
        }
        else
        {
            float distance = Vector2.Distance(transform.position, targetPlayer.position);
            if (distance <= pickupRange)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);

                if (distance <= 0.2f)
                {
                    OnCollected();
                }
            }
        }
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player"); 
        if (player != null)
        {
            targetPlayer = player.transform;
        }
    }

    void OnCollected()
    {
        var drop = GetComponent<DroppableItem>();
        if (drop != null)
        {
            var gc = FindAnyObjectByType<GameController>();
            var dungeonMgr = gc?.GetManager<DungeonManager>();
            dungeonMgr?.AddTempItem(drop.IngredientIndate, drop.amount);
        }
        SoundManager.GetInstance().SfxPlay(SoundManager.sfx.ItemPickup, false);
        Debug.Log($"{gameObject.name} È¹µæ");
        Destroy(gameObject);
    }
}
