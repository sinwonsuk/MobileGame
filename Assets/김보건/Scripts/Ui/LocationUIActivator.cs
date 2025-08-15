using UnityEngine;

public class LocationUIActivator : MonoBehaviour
{
    [Header("던전에서 보여줄 UI")]
    [SerializeField] private GameObject[] dungeonUIObjects;

    [Header("던전에 들어갈 때 생성할 프리팹")]
    [SerializeField] private GameObject[] dungeonUIPrefabs;
    [SerializeField] private Transform instantiateParent; // 비우면 this.transform

    private readonly System.Collections.Generic.List<GameObject> _instantiated = new();

    void OnEnable()
    {
        EventBus<LocationChangedEvent>.OnEvent += OnLocationChanged;

        // 씬 시작 시 현재 위치 기준으로 동기화
        Apply(LocationState.Current);
    }

    void OnDisable()
    {
        EventBus<LocationChangedEvent>.OnEvent -= OnLocationChanged;
    }

    private void OnLocationChanged(LocationChangedEvent e) => Apply(e.value);

    private void Apply(location loc)
    {
        bool isDungeon = (loc == location.Dungeon);

        // 이미 존재하는 오브젝트 토글
        if (dungeonUIObjects != null)
        {
            foreach (var go in dungeonUIObjects)
            {
                if (!go) continue;
                go.SetActive(isDungeon);
            }
        }

        // 프리팹 생성/토글 (선택)
        if (dungeonUIPrefabs != null && dungeonUIPrefabs.Length > 0)
        {
            if (isDungeon)
            {
                //아직 생성 안된 것만 생성
                foreach (var prefab in dungeonUIPrefabs)
                {
                    if (!prefab) continue;
                    var exist = _instantiated.Find(x => x && x.name.StartsWith(prefab.name));
                    if (!exist)
                    {
                        var parent = instantiateParent ? instantiateParent : this.transform;
                        var inst = Instantiate(prefab, parent);
                        inst.name = prefab.name; // (Clone) 지우기
                        _instantiated.Add(inst);
                    }
                }
                // 전부 활성화
                foreach (var go in _instantiated)
                    if (go) go.SetActive(true);
            }
            else
            {
                // 레스토랑: 생성된 것만 꺼두기(파괴 아님 → 재입장 빠름)
                foreach (var go in _instantiated)
                    if (go) go.SetActive(false);
            }
        }
    }
}
