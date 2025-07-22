using UnityEngine;

public class InteriorsUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform contentParent;

    private void Start()
    {
        if (InteriorManager.Instance != null)
        {
            InteriorManager.Instance.OnInteriorChanged += RefreshUI;
            RefreshUI();
        }
    }

    private void OnDestroy()
    {
        if (InteriorManager.Instance != null)
            InteriorManager.Instance.OnInteriorChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        foreach (Transform c in contentParent) Destroy(c.gameObject);
        foreach (var slot in InteriorManager.Instance.slots)
        {
            if (!slot.runtimeData.isOwned) continue;
            var go = Instantiate(slotPrefab, contentParent);
            var slotUI = go.GetComponent<InteriorSlotUI>();
            if (slotUI == null)
            {
                Debug.LogError("¢º ½½·Ô ÇÁ¸®ÆÕ¿¡ InteriorSlotUI ÄÄÆ÷³ÍÆ®°¡ ¾ø½À´Ï´Ù!", go);
                return;
            }
            slotUI.SetSlot(slot);
        }
    }
}
