using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // M 키를 눌러 Minigame 호출
        {
            SceneController.Instance.LoadDeathgame(); // 초기 데이터 전달
        }

        if (GameData.deathgameCompleted)
        {
            Debug.Log($"Received Minigame Result: {GameData.deathgameResult}");
            GameData.deathgameCompleted = false; // 결과 확인 후 초기화
        }
    }
}
