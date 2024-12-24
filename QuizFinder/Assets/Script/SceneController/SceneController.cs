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
        // 씬이 로드될 때 카메라 상태 변경
        SceneManager.sceneLoaded += OnSceneLoaded;
        // 씬이 언로드될 때 카메라 상태 변경
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        // 씬 로드, 언로드 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // 씬이 로드될 때 호출되는 메서드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name);
        
        if (scene.name == "main")
        {
            // MainScene의 카메라 활성화

            if (mainSceneCamera != null)
            {
                mainSceneCamera.gameObject.SetActive(true);
            }
            miniGameSceneCamera.gameObject.SetActive(false);
            Debug.Log("MainScene 카메라 활성화");
        }
        else if (scene.name == "DeathGame")
        {
            // MinigameScene의 카메라 활성화
            miniGameSceneCamera = GameObject.Find("DeathgameSceneCamera").GetComponent<Camera>();
            if (miniGameSceneCamera != null)
            {
                miniGameSceneCamera.gameObject.SetActive(true);
            }
            mainSceneCamera.gameObject.SetActive(false);
            Debug.Log("MinigameScene 카메라 활성화");
        }
    }

    // 씬이 언로드될 때 호출되는 메서드
    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == minigameSceneName)
        {
            // MinigameScene 언로드 시 MainScene 카메라 활성화
            mainSceneCamera.gameObject.SetActive(true);
            Debug.Log("MinigameScene 언로드, MainScene 카메라 활성화");
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
