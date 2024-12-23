using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerQuiz : MonoBehaviour
{
    public string displayText; // 표시할 텍스트
    public DynamicTextData textData; // 사용할 텍스트 데이터
    public Vector3 textOffset = new Vector3(0, 2, 0); // 텍스트가 나타날 위치 오프셋
    bool check = false;
    public List<GameObject> answerObjects;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 충돌했는지 확인
        if (other.CompareTag("Player") && !check)
        {
            // 텍스트를 생성
            Vector3 spawnPosition = transform.position + textOffset;

            DynamicTextManager.CreateText(spawnPosition, displayText, textData);

            // Answer 태그를 가진 모든 오브젝트 활성화
            ActivateAnswerObjects();

            check = true;
        }
    }

    private void ActivateAnswerObjects()
    {
        foreach (GameObject answerObject in answerObjects)
        {
            Debug.Log(answerObject.name);
            // 오브젝트 활성화
            answerObject.SetActive(true);
        }
    }
}
