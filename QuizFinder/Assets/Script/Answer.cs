using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer : MonoBehaviour
{
    public string displayText; // 표시할 텍스트 (O 또는 X)
    public DynamicTextData textData; // 사용할 텍스트 데이터
    public Vector3 textOffset;
    public TriggerQuiz triggerQuiz;
    protected bool thisAnswer;
    protected GameObject createdText;

    // 활성화될 때 호출
    private void OnEnable()
    {
        ShowText();

        if (triggerQuiz == null)
        {
            Debug.LogError("QuizTrigger를 찾을 수 없습니다.");
        }
    }

    // 텍스트를 표시하는 공통 메서드
    public virtual void ShowText()
    {
        if (textData != null && !string.IsNullOrEmpty(displayText))
        {
            // 텍스트 표시
            createdText = DynamicTextManager.CreateText(transform.position + textOffset, displayText, textData);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 충돌했는지 확인
        if (other.CompareTag("Player"))
        {
            triggerQuiz.SubmitAnswer(thisAnswer);
            createdText.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (createdText != null)
        {
            createdText.SetActive(false);
        }
    }
}
