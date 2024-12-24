using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathgameSceneController : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"Starting Minigame with Initial Data: {GameData.deathgameResult}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) // C 키를 눌러 완료
        {
            CompleteMinigame(true); // 결과 값 예시
        }
    }

    private void CompleteMinigame(bool result)
    {
        GameData.deathgameResult = result;
        GameData.deathgameCompleted = true;

        SceneController.Instance.UnloadDeathgame();
    }
}
