using UnityEngine;
using System.Collections;

public class InGameFlowManager : MonoBehaviour
{
    public static InGameFlowManager Instance;

    [Header("Game Panels")]
    public GameObject arrayPanel;
    public GameObject linkedListPanel;
    public GameObject circleLinkedListPanel;
    public GameObject stackPanel;
    // Queue is not implemented so no panel needed or just keep it null

    [Header("Managers")]
    public LinkedManager linkedManager; // Handles Array and LinkedList

    private int currentPeriodIndex = 0;
    private string[] schedule;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
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

    public void StartPeriod(int index)
    {
        if (index >= 4)
        {
            Debug.Log("All classes finished!");
            // TODO: Show Result Screen or return to Menu
            return;
        }

        currentPeriodIndex = index;
        string subject = schedule[index];
        if (string.IsNullOrEmpty(subject)) subject = "Empty";

        Debug.Log($"Starting Period {index + 1}: {subject}");

        // Disable all panels first
        if(arrayPanel) arrayPanel.SetActive(false);
        if(linkedListPanel) linkedListPanel.SetActive(false);
        if(circleLinkedListPanel) circleLinkedListPanel.SetActive(false);
        if(stackPanel) stackPanel.SetActive(false);

        // Enable the correct panel and setup manager
        // Note: Subject names depend on the Text in DraggableSubject prefabs.
        // Assuming they contain "Array", "Linked", "Stack", "Queue", "Circle" etc.
        
        if (subject.Contains("Array") || subject.Contains("배열"))
        {
            if(arrayPanel) arrayPanel.SetActive(true);
            if(linkedManager) linkedManager.InitializeGame(LinkedManager.GameType.Array);
        }
        else if (subject.Contains("Circle") || subject.Contains("원형"))
        {
             if(circleLinkedListPanel) circleLinkedListPanel.SetActive(true);
             // CircleListManager logic here if needed
        }
        else if (subject.Contains("Linked") || subject.Contains("연결"))
        {
            if(linkedListPanel) linkedListPanel.SetActive(true);
            if(linkedManager) linkedManager.InitializeGame(LinkedManager.GameType.LinkedList);
        }
        else if (subject.Contains("Stack") || subject.Contains("스택"))
        {
            if(stackPanel) stackPanel.SetActive(true);
            // StackManager logic here if needed
        }
        else if (subject.Contains("Queue") || subject.Contains("큐"))
        {
            Debug.Log("Queue is not implemented. Skipping...");
            // Skip immediately or show a message
            StartCoroutine(WaitAndNextPeriod(1f)); 
        }
        else
        {
            Debug.LogWarning($"Unknown subject: {subject}. Skipping...");
            StartCoroutine(WaitAndNextPeriod(1f));
        }
    }

    public void OnGameFinished(bool success)
    {
        if (success)
        {
            Debug.Log($"Period {currentPeriodIndex + 1} Cleared!");
            StartCoroutine(WaitAndNextPeriod(2f));
        }
        else
        {
            Debug.Log("Game Over");
            // Handle Game Over logic (restart period or fail entire game)
        }
    }

    IEnumerator WaitAndNextPeriod(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartPeriod(currentPeriodIndex + 1);
    }
}
