using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    private int score = 0;
    private int[] tracker = new int[15];
    public DynamicTextData clearTextData;
    public Transform playerTransform;

    public void Start()
    {
        for (int i = 0; i < tracker.Length; i++)
        {
            tracker[i] = 0;
        }
    }

    public void increaseScore()
    {
        score++;
        if (score == 5)
        {
            Debug.Log("Å¬¸®¾î");
            DynamicTextManager.CreateText(playerTransform.position + new Vector3(1, 4, 1), "Congratulations!", clearTextData);
        }
    }

    public bool quizTracker(int i)
    {
        if (tracker[i] == 0)
        {
            tracker[i] = 1;
            return true;
        }
        else
        {
            return false;
        }
    }
}
