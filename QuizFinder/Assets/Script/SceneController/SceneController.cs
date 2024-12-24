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

    [SerializeField] private string minigameSceneName = "Deathgame";

    private string mainSceneName;

    private void Start()
    {
        mainSceneName = SceneManager.GetActiveScene().name;
    }

    public void LoadDeathgame()
    {
        /*
        Scene minigameScene = SceneManager.GetSceneByName(minigameSceneName);
        if (!minigameScene.isLoaded)
        {
            StartCoroutine(LoadDeathgameScene());
        }*/
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
