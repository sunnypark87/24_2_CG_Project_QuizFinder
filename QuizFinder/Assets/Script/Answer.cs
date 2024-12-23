using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer : MonoBehaviour
{
    public string displayText; // 표시할 텍스트 (O 또는 X)
    public DynamicTextData textData; // 사용할 텍스트 데이터
    public Vector3 textOffset = new Vector3(0, 2, 0); // 텍스트 위치 오프셋

    // 활성화될 때 호출
    private void OnEnable()
    {
        ShowText();
    }

    // 텍스트를 표시하는 공통 메서드
    protected virtual void ShowText()
    {
        if (textData != null && !string.IsNullOrEmpty(displayText))
        {
            // 텍스트 표시
            DynamicTextManager.CreateText(transform.position + textOffset, displayText, textData);
        }
    }
}
