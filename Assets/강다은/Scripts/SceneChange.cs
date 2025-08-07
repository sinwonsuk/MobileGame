using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum SceneName
{
    TitleScene,
    SampleScene,
    RobbyScene,
    WaitingRoom,
    GameScene,
}

public class SceneChange : MonoBehaviour
{
    public static SceneChange Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {

    }

    public void LoadSceneAsync(SceneName scene)
    {
        StartCoroutine(LoadSceneCoroutine(scene));
    }

    private IEnumerator LoadSceneCoroutine(SceneName scene)
    {
        string sceneStr = System.Enum.GetName(typeof(SceneName), scene);

        EventBus<ChangeLoadImageEvent>.Raise(new ChangeLoadImageEvent(true));

        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneStr);

        // 여기서 로딩 중 진행도 표시 가능
        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading progress: {asyncLoad.progress}");
            yield return null;
        }

        EventBus<ChangeLoadImageEvent>.Raise(new ChangeLoadImageEvent(false));
    }
}