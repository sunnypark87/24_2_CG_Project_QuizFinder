using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TriggerQuiz : MonoBehaviour
{
    public string displayText; // 표시할 텍스트
    public DynamicTextData textData; // 사용할 텍스트 데이터
    public DynamicTextData correctTextData; // 정답 표시에 사용할 텍스트 데이터
    public Vector3 textOffset; // 텍스트가 나타날 위치 오프셋
    bool check = false;
    public List<GameObject> answerObjects;
    GameObject questionText;
    public bool answer;
    public Transform playerTransform;
    public QuizManager quizManager;

    // Quiz data structure
    [System.Serializable]
    public class QuizData
    {
        public string question;
        public string answer;
    }

    // Wrapper for the list of quizzes
    [System.Serializable]
    public class QuizDataList
    {
        public List<QuizData> quizzes;
    }

    public List<QuizData> quizzes;  // List to hold the quiz data

    private void Start()
    {
        // Load the quiz data from a JSON file
        LoadQuizData();
    }

    private void LoadQuizData()
    {
        // Path to the JSON file (make sure the JSON is placed in Resources folder)
        string filePath = Path.Combine(Application.dataPath, "quizzes.json");  // Assuming you have placed it in the root of the project

        if (File.Exists(filePath))
        {
            // Read and parse the JSON file
            string json = File.ReadAllText(filePath);

            // Log the raw JSON string to ensure it's being read correctly
            Debug.Log("JSON Data: " + json);

            QuizDataList quizList = JsonUtility.FromJson<QuizDataList>(json);

            // Check if quizzes are loaded correctly
            if (quizList != null && quizList.quizzes != null)
            {
                quizzes = quizList.quizzes;
                Debug.Log("Loaded quizzes: " + quizzes.Count);

                // Optionally, log each quiz to ensure correct parsing
                foreach (var quiz in quizzes)
                {
                    Debug.Log("Question: " + quiz.question + ", Answer: " + quiz.answer);
                }
            }
            else
            {
                Debug.LogError("Failed to parse quiz data.");
            }
        }
        else
        {
            Debug.LogError("Quiz file not found at path: " + filePath);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 충돌했는지 확인
        if (other.CompareTag("Player") && !check)
        {
            // 텍스트를 생성
            Vector3 spawnPosition = transform.position + textOffset;

            // Choose a random quiz question
            int randomIndex;
            while (true)
            {
                randomIndex = Random.Range(0, quizzes.Count);
                if (quizManager.quizTracker(randomIndex))
                {
                    break;
                }
            }
            Debug.Log($"randomIndex: {randomIndex}");
            displayText = quizzes[randomIndex].question;

            questionText = DynamicTextManager.CreateText(spawnPosition, displayText, textData);

            ActivateAnswerObjects();

            // Set the correct answer
            answer = quizzes[randomIndex].answer.ToLower() == "o";

            check = true;
            this.gameObject.SetActive(false);
        }
    }

    private void ActivateAnswerObjects()
    {
        foreach (GameObject answerObject in answerObjects)
        {
            // 오브젝트 활성화
            answerObject.SetActive(true);
        }
    }

    public void SubmitAnswer(bool receivedAnswer)
    {
        foreach (GameObject answerObject in answerObjects)
        {
            // 오브젝트 비활성화
            answerObject.SetActive(false);
            quizManager.increaseScore();
        }

        Destroy(questionText);

        if (receivedAnswer == answer)
        {
            DynamicTextManager.CreateText(playerTransform.position + new Vector3(0, 4, 0), "Correct!", correctTextData);
        }
        else
        {
            Debug.Log("틀렸습니다");
            SceneController.Instance.LoadDeathgame();
            while (GameData.deathgameCompleted) ;
            if (GameData.deathgameResult)
            {
                quizManager.increaseScore();
            }
            else
            {
                Debug.Log("죽었습니다!");
                Application.Quit();
            }
        }
    }
}
