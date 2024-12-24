using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // M Ű�� ���� Minigame ȣ��
        {
            SceneController.Instance.LoadDeathgame(); // �ʱ� ������ ����
        }

        if (GameData.deathgameCompleted)
        {
            Debug.Log($"Received Minigame Result: {GameData.deathgameResult}");
            GameData.deathgameCompleted = false; // ��� Ȯ�� �� �ʱ�ȭ
        }
    }
}
