using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    private static SceneController instance;

    public static SceneController Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject controller = new GameObject("SceneController");
                instance = controller.AddComponent<SceneController>();
                DontDestroyOnLoad(controller);
            }
            return instance;
        }
    }

    [SerializeField] private string minigameSceneName = "DeathGame";

    [SerializeField] private Camera mainSceneCamera;
    [SerializeField] private Camera miniGameSceneCamera;

    private string mainSceneName;

    private void OnEnable()
    {
        // ���� �ε�� �� ī�޶� ���� ����
        SceneManager.sceneLoaded += OnSceneLoaded;
        // ���� ��ε�� �� ī�޶� ���� ����
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        // �� �ε�, ��ε� �̺�Ʈ ���� ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // ���� �ε�� �� ȣ��Ǵ� �޼���
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("scene loaded");
        Debug.Log(scene.name);
        
        if (scene.name == "main")
        {
            // MainScene�� ī�޶� Ȱ��ȭ

            mainSceneCamera = GameObject.Find("MainSceneCamera").GetComponent<Camera>();
            if (mainSceneCamera != null)
            {
                mainSceneCamera.gameObject.SetActive(true);
            }
            miniGameSceneCamera.gameObject.SetActive(false);
            Debug.Log("MainScene ī�޶� Ȱ��ȭ");
        }
        else if (scene.name == "DeathGame")
        {
            // MinigameScene�� ī�޶� Ȱ��ȭ
            miniGameSceneCamera = GameObject.Find("DeathgameSceneCamera").GetComponent<Camera>();
            if (miniGameSceneCamera != null)
            {
                miniGameSceneCamera.gameObject.SetActive(true);
            }
            mainSceneCamera.gameObject.SetActive(false);
            Debug.Log("MinigameScene ī�޶� Ȱ��ȭ");
        }
    }

    // ���� ��ε�� �� ȣ��Ǵ� �޼���
    private void OnSceneUnloaded(Scene scene)
    {
        
        if (scene.name == minigameSceneName)
        {
            // MinigameScene ��ε� �� MainScene ī�޶� Ȱ��ȭ
            miniGameSceneCamera = GameObject.Find("DeathgameSceneCamera").GetComponent<Camera>();
            mainSceneCamera.gameObject.SetActive(true);
            miniGameSceneCamera.gameObject.SetActive(false);
            Debug.Log("MinigameScene ��ε�, MainScene ī�޶� Ȱ��ȭ");
        }
    }

    private void Start()
    {
        mainSceneName = SceneManager.GetActiveScene().name;
        Debug.Log(mainSceneName);
    }

    public void LoadDeathgame()
    {
        
        Scene minigameScene = SceneManager.GetSceneByName(minigameSceneName);
        
        Debug.Log("Start mini game");
        StartCoroutine(LoadDeathgameScene());
        
    }

    private IEnumerator LoadDeathgameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(minigameSceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameData.deathgameResult = false;
        GameData.deathgameCompleted = false;

        Debug.Log("Minigame Scene Loaded");
    }

    public void UnloadDeathgame()
    {
        StartCoroutine(UnloadDeathgameScene());
    }

    private IEnumerator UnloadDeathgameScene()
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(minigameSceneName);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        if (GameData.deathgameCompleted)
        {
            Debug.Log($"Minigame Result: {GameData.deathgameResult}");
        }

        Debug.Log("Minigame Scene Unloaded");
    }
}
