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
        Debug.Log(scene.name);
        
        if (scene.name == "main")
        {
            // MainScene�� ī�޶� Ȱ��ȭ

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
            mainSceneCamera.gameObject.SetActive(true);
            Debug.Log("MinigameScene ��ε�, MainScene ī�޶� Ȱ��ȭ");
        }
    }

    private void Start()
    {
        mainSceneName = SceneManager.GetActiveScene().name;
        mainSceneCamera = GameObject.Find("MainSceneCamera").GetComponent<Camera>();
    }

    public void LoadDeathgame()
    {
        Scene minigameScene = SceneManager.GetSceneByName(minigameSceneName);
        
        StartCoroutine(LoadDeathgamewithTimer(10f));
    }

    private IEnumerator LoadDeathgamewithTimer(float delay)
    {
        yield return LoadDeathgameScene();
        yield return new WaitForSeconds(delay);
       
        
        yield return UnloadDeathgameScene();
        
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

    }

    public void UnloadDeathgame()
    {
        StartCoroutine(UnloadDeathgameScene());
    }

    private IEnumerator UnloadDeathgameScene()
    {
        if (miniGameSceneCamera == null)
        {
            GameObject cameraObject = GameObject.Find("DeathgameSceneCamera");
            if (cameraObject != null)
            {
                miniGameSceneCamera = cameraObject.GetComponent<Camera>();
                miniGameSceneCamera.gameObject.SetActive(false);
            }
        }

        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(minigameSceneName);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }
    }
}
