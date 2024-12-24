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
    private QuizManager quizManager;
    public void SetQuizManager(QuizManager manager)
    {
        quizManager = manager;
    }

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

    public IEnumerator LoadDeathgame()
    {
        Scene minigameScene = SceneManager.GetSceneByName(minigameSceneName);
        
        yield return StartCoroutine(LoadDeathgamewithTimer(10f));
    }
    public IEnumerator HandleDeathgame()
    {
        yield return SceneController.Instance.LoadDeathgame();

        float elapsedTime = 0f;

        while (elapsedTime < 15f)
        {
            if (GameData.deathgameCompleted) // 특정 조건이 충족되면 즉시 탈출
            {
                yield break; // 코루틴 종료
            }

            elapsedTime += Time.deltaTime; // 경과 시간 업데이트
            yield return null; // 다음 프레임까지 대기
        }

        Debug.Log($"Game Done : {GameData.deathgameCompleted}");
        Debug.Log($"Result : {GameData.deathgameResult}");
        if (GameData.deathgameResult)
        {
            Debug.Log("살았습니다!");
            quizManager.increaseScore();
        }
        else
        {
            Debug.Log("죽었습니다!");
            Application.Quit();
        }
    }

    public IEnumerator LoadDeathgamewithTimer(float delay)
    {
        yield return StartCoroutine(LoadDeathgameScene());

        bool sceneUnloaded = false;
        yield return StartCoroutine(WaitForConditionOrTime(delay, () => IsSceneUnloaded(), result => sceneUnloaded = result));

        if (!sceneUnloaded)
        {
            yield return StartCoroutine(UnloadDeathgameScene());
        }

        Debug.Log($"Game Done : {GameData.deathgameCompleted}");
        Debug.Log($"Result : {GameData.deathgameResult}");
        if (GameData.deathgameResult)
        {
            Debug.Log("살았습니다!");
            quizManager.increaseScore();
        }
        else
        {
            Debug.Log("죽었습니다!");
            Application.Quit();
        }

    }

    private IEnumerator WaitForConditionOrTime(float maxWaitTime, System.Func<bool> condition, System.Action<bool> onComplete)
    {
        float elapsedTime = 0f;
        while (elapsedTime < maxWaitTime)
        {
            if (condition())
            {
                onComplete(true);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        onComplete(false);
    }

    private bool IsSceneUnloaded()
    {
        Scene minigameScene = SceneManager.GetSceneByName(minigameSceneName);
        return !minigameScene.isLoaded;
    }

    private IEnumerator LoadDeathgameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(minigameSceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameData.deathgameResult = true;

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

        GameData.deathgameCompleted = true;
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(minigameSceneName);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }
    }
}
