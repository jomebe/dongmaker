using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class InGameFlowManager : MonoBehaviour
{
    public static InGameFlowManager Instance;

    [Header("Game Panels")]
    public GameObject arrayPanel;
    public GameObject linkedListPanel;
    public GameObject circleListPanel;
    public GameObject circleLinkedListPanel;
    public GameObject stackPanel;
    // Queue is not implemented so no panel needed or just keep it null

    [Header("Stats Panel")]
    public GameObject statsPanel;
    public Text resultText;           // Perfect 또는 Fail 표시
    public Transform statsContainer;  // Stats 부모 오브젝트 (Understanding, Accuracy 등의 부모)

    [Header("Animation Settings")]
    public float statAnimDuration = 0.5f;
    public Color increaseColor = Color.green;
    public Color decreaseColor = Color.red;

    [Header("Managers")]
    public LinkedManager linkedManager; // Handles Array and LinkedList

    private int currentPeriodIndex = 0;
    private string[] schedule;
    private string currentGameType = "";

    // 이전 스탯 저장용
    private Dictionary<string, int> previousStats = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // StatsPanel 시작 시 끄기
        if (statsPanel != null) statsPanel.SetActive(false);

        // 초기 스탯 저장
        SaveCurrentStats();
        SaveStartStatsToPrefs();

        if (GameManager.Instance != null)
        {
            schedule = GameManager.Instance.schedule;
            StartPeriod(0);
        }
        else
        {
            Debug.LogWarning("GameManager not found! Starting with default schedule for testing.");
            // Default schedule for testing
            schedule = new string[] { "Array", "LinkedList", "Stack", "Queue" };
            StartPeriod(0);
        }
    }

    void SaveStartStatsToPrefs()
    {
        if (GameManager.Instance == null) return;

        // 하루 시작 시 스탯을 PlayerPrefs에 저장 (GameEnd에서 변화량 계산용)
        PlayerPrefs.SetInt("StartUnderstanding", GameManager.Instance.understanding);
        PlayerPrefs.SetInt("StartAccuracy", GameManager.Instance.accuracy);
        PlayerPrefs.SetInt("StartLogic", GameManager.Instance.logic);
        PlayerPrefs.SetInt("StartConcentration", GameManager.Instance.concentration);
        PlayerPrefs.SetInt("StartStress", GameManager.Instance.stress);
        PlayerPrefs.SetInt("StartConfidence", GameManager.Instance.confidence);
        PlayerPrefs.SetInt("StartCondition", GameManager.Instance.condition);
        PlayerPrefs.Save();
    }

    void SaveCurrentStats()
    {
        if (GameManager.Instance == null) return;

        previousStats["Understanding"] = GameManager.Instance.understanding;
        previousStats["Accuracy"] = GameManager.Instance.accuracy;
        previousStats["Logical"] = GameManager.Instance.logic;
        previousStats["Concentration"] = GameManager.Instance.concentration;
        previousStats["Stress"] = GameManager.Instance.stress;
        previousStats["Confidence"] = GameManager.Instance.confidence;
        previousStats["Condition"] = GameManager.Instance.condition;
    }

    public void StartPeriod(int index)
    {
        if (index >= 4)
        {
            Debug.Log("All classes finished!");
            // 하루 종료 처리
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ApplyDailyEnd();
            }
            // GameEnd 씬으로 이동
            SceneManager.LoadScene("GameEnd");
            return;
        }

        // 이전 게임의 연결선 제거
        SubjectConnector.ClearAllLines();

        // StatsPanel 끄기
        if (statsPanel != null) statsPanel.SetActive(false);

        currentPeriodIndex = index;
        string subject = schedule[index];
        if (string.IsNullOrEmpty(subject)) subject = "Empty";

        Debug.Log($"Starting Period {index + 1}: {subject}");

        // Disable all panels first
        if(arrayPanel) arrayPanel.SetActive(false);
        if(linkedListPanel) linkedListPanel.SetActive(false);
        if(circleListPanel) circleListPanel.SetActive(false);
        if(circleLinkedListPanel) circleLinkedListPanel.SetActive(false);
        if(stackPanel) stackPanel.SetActive(false);

        // Enable the correct panel and setup manager
        if (subject.Contains("Array") || subject.Contains("배열"))
        {
            currentGameType = "Array";
            if(arrayPanel) arrayPanel.SetActive(true);
            if(linkedManager) linkedManager.InitializeGame(LinkedManager.GameType.Array);
        }
        else if (subject.Contains("원형 연결") || subject.Contains("CircleLinked"))
        {
            currentGameType = "CircleLinkedList";
            if(circleLinkedListPanel) circleLinkedListPanel.SetActive(true);
        }
        else if (subject.Contains("원형") || subject.Contains("Circle"))
        {
            currentGameType = "CircleList";
            if(circleListPanel) circleListPanel.SetActive(true);
        }
        else if (subject.Contains("Linked") || subject.Contains("연결"))
        {
            currentGameType = "LinkedList";
            if(linkedListPanel) linkedListPanel.SetActive(true);
            if(linkedManager) linkedManager.InitializeGame(LinkedManager.GameType.LinkedList);
        }
        else if (subject.Contains("Stack") || subject.Contains("스택"))
        {
            currentGameType = "Stack";
            if(stackPanel) stackPanel.SetActive(true);
        }
        else if (subject.Contains("Queue") || subject.Contains("큐"))
        {
            currentGameType = "Queue";
            Debug.Log("Queue is not implemented. Skipping...");
            StartCoroutine(WaitAndNextPeriod(1f)); 
        }
        else
        {
            currentGameType = "Unknown";
            Debug.LogWarning($"Unknown subject: {subject}. Skipping...");
            StartCoroutine(WaitAndNextPeriod(1f));
        }
    }

    public void OnGameFinished(bool success)
    {
        Debug.Log($"Period {currentPeriodIndex + 1} Finished. Success: {success}");

        // 이전 스탯 저장
        SaveCurrentStats();

        // 각 교시 결과 저장 (GameEnd에서 사용)
        PlayerPrefs.SetInt($"Period{currentPeriodIndex}Result", success ? 1 : 0);
        PlayerPrefs.Save();

        // 스탯 적용
        ApplyStats(success);

        // 모든 게임 패널 끄기
        if(arrayPanel) arrayPanel.SetActive(false);
        if(linkedListPanel) linkedListPanel.SetActive(false);
        if(circleListPanel) circleListPanel.SetActive(false);
        if(circleLinkedListPanel) circleLinkedListPanel.SetActive(false);
        if(stackPanel) stackPanel.SetActive(false);

        // StatsPanel 보여주기 (애니메이션 포함)
        ShowStatsPanel(success);

        // 다음 교시로
        StartCoroutine(WaitAndNextPeriod(3f));
    }

    void ApplyStats(bool success)
    {
        if (GameManager.Instance == null) return;

        switch (currentGameType)
        {
            case "Array":
                if (success) GameManager.Instance.ApplyArraySuccess();
                else GameManager.Instance.ApplyArrayFail();
                break;
            case "LinkedList":
                if (success) GameManager.Instance.ApplyLinkedListSuccess();
                else GameManager.Instance.ApplyLinkedListFail();
                break;
            case "CircleList":
                if (success) GameManager.Instance.ApplyCircleListSuccess();
                else GameManager.Instance.ApplyCircleListFail();
                break;
            case "CircleLinkedList":
                if (success) GameManager.Instance.ApplyCircleLinkedListSuccess();
                else GameManager.Instance.ApplyCircleLinkedListFail();
                break;
            case "Stack":
                if (success) GameManager.Instance.ApplyStackSuccess();
                else GameManager.Instance.ApplyStackFail();
                break;
            case "Queue":
                if (success) GameManager.Instance.ApplyQueueSuccess();
                else GameManager.Instance.ApplyQueueFail();
                break;
        }
    }

    void ShowStatsPanel(bool success)
    {
        if (statsPanel == null) return;

        statsPanel.SetActive(true);

        // Perfect / Fail 표시
        string resultStr = success ? "Perfect" : "Fail";
        if (resultText != null) resultText.text = resultStr;

        // 스탯 표시 - statsContainer 하위에서 각 스탯 찾아서 애니메이션과 함께 업데이트
        if (GameManager.Instance != null && statsContainer != null)
        {
            StartCoroutine(AnimateStatValue("Understanding", GameManager.Instance.understanding));
            StartCoroutine(AnimateStatValue("Accuracy", GameManager.Instance.accuracy));
            StartCoroutine(AnimateStatValue("Logical", GameManager.Instance.logic));
            StartCoroutine(AnimateStatValue("Concentration", GameManager.Instance.concentration));
            StartCoroutine(AnimateStatValue("Stress", GameManager.Instance.stress));
            StartCoroutine(AnimateStatValue("Confidence", GameManager.Instance.confidence));
            StartCoroutine(AnimateStatValue("Condition", GameManager.Instance.condition));
        }
    }

    IEnumerator AnimateStatValue(string statName, int targetValue)
    {
        if (statsContainer == null) yield break;

        Transform statTransform = statsContainer.Find(statName);
        if (statTransform == null) yield break;

        // 이전 값 가져오기
        int previousValue = previousStats.ContainsKey(statName) ? previousStats[statName] : targetValue;
        int diff = targetValue - previousValue;

        // Value 텍스트 찾기
        Transform valueTransform = statTransform.Find("Value");
        Text valueText = valueTransform != null ? valueTransform.GetComponent<Text>() : null;

        // Slider 찾기
        Transform sliderTransform = statTransform.Find("Slider");
        Slider slider = sliderTransform != null ? sliderTransform.GetComponent<Slider>() : null;

        // 변화가 있으면 색상 효과 및 애니메이션
        Color originalColor = valueText != null ? valueText.color : Color.white;
        Color flashColor = diff > 0 ? increaseColor : (diff < 0 ? decreaseColor : originalColor);

        float elapsed = 0f;
        float startValue = previousValue;

        while (elapsed < statAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / statAnimDuration;
            
            // Ease out
            t = 1f - (1f - t) * (1f - t);

            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, t));

            if (valueText != null)
            {
                valueText.text = currentValue.ToString();
                // 변화가 있으면 색상 깜빡임
                if (diff != 0)
                {
                    valueText.color = Color.Lerp(flashColor, originalColor, t);
                }
            }

            if (slider != null)
            {
                slider.value = currentValue;
            }

            yield return null;
        }

        // 최종 값 설정
        if (valueText != null)
        {
            valueText.text = targetValue.ToString();
            valueText.color = originalColor;
        }
        if (slider != null)
        {
            slider.value = targetValue;
        }
    }

    void UpdateStatValue(string statName, int value)
    {
        if (statsContainer == null) return;

        Transform statTransform = statsContainer.Find(statName);
        if (statTransform != null)
        {
            // Value 텍스트 업데이트
            Transform valueTransform = statTransform.Find("Value");
            if (valueTransform != null)
            {
                Text valueText = valueTransform.GetComponent<Text>();
                if (valueText != null)
                {
                    valueText.text = value.ToString();
                }
            }

            // Slider 값 업데이트
            Transform sliderTransform = statTransform.Find("Slider");
            if (sliderTransform != null)
            {
                Slider slider = sliderTransform.GetComponent<Slider>();
                if (slider != null)
                {
                    slider.value = value;
                }
            }
        }
    }

    IEnumerator WaitAndNextPeriod(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartPeriod(currentPeriodIndex + 1);
    }
}
