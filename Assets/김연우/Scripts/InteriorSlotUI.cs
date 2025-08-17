// InteriorSlotUI.cs (교체)
using UnityEngine;
using TMPro;

public class InteriorSlotUI : MonoBehaviour
{
    [Header("UI(없으면 자동 탐색)")]
    public TMP_Text nameText;            // 슬롯 상단 이름(옵션)
    public Transform skinsContent;       // 내부 ScrollView의 Content
    public GameObject skinSlotPrefab;    // Skinslot 프리팹(Image+SkinSlotUI)

    private InteriorSlot slot;

    private void Awake()
    {
        // 자동 바인딩: 프리팹 구조가 바뀌어도 안전
        if (nameText == null) nameText = GetComponentInChildren<TMP_Text>(true);

        if (skinsContent == null)
        {
            var t = transform.Find("Scroll View/Viewport/Content");
            if (t == null)
            {
                foreach (var rt in GetComponentsInChildren<RectTransform>(true))
                {
                    if (rt.name.ToLower().Contains("content")) { t = rt; break; }
                }
            }
            skinsContent = t;
        }
    }

    public void SetSlot(InteriorSlot s)
    {
        slot = s;
        if (nameText) nameText.text = s.data.interiorName;
        BuildSkins();
    }

    private void BuildSkins()
    {
        if (skinsContent == null || skinSlotPrefab == null || slot == null) return;

        // 비우기
        for (int i = skinsContent.childCount - 1; i >= 0; i--)
            Destroy(skinsContent.GetChild(i).gameObject);

        // InteriorData.skins 기준으로 생성 (0=기본 포함)
        if (slot.data.skins != null && slot.data.skins.Count > 0)
        {
            for (int i = 0; i < slot.data.skins.Count; i++)
            {
                var spr = slot.data.skins[i]?.icon ?? slot.data.icon;
                var go = Instantiate(skinSlotPrefab, skinsContent);
                var ui = go.GetComponent<InteriorSkinSlotUI>() ?? go.AddComponent<InteriorSkinSlotUI>();
                ui.Setup(slot, i, spr);
            }
        }
        else
        {
            // 스킨 리스트가 비어 있으면 기본 아이콘 1개만
            var go = Instantiate(skinSlotPrefab, skinsContent);
            var ui = go.GetComponent<InteriorSkinSlotUI>() ?? go.AddComponent<InteriorSkinSlotUI>();
            ui.Setup(slot, 0, slot.data.icon);
        }
    }
}
