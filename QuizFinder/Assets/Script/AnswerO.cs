using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerO : Answer
{

    protected void OnEnable()
    {
        // 부모의 quizAnswer 변수 참조
        Answer answer = GetComponentInParent<Answer>();
        if (answer != null)
        {
            answer.ShowText();
        }
    }
}
