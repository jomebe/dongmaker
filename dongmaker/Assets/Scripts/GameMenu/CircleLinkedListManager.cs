using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CircleLinkedListManager : MonoBehaviour
{
    [Header("Game Settings")]
    public GameObject gamePanel;          // 이 게임 모드의 패널 (예: CircleLinkedList)

    [Header("UI Settings")]
    public TextMeshProUGUI chanceTextTMP; // TextMeshPro를 사용하는 경우 여기에 연결
    public Text chanceTextLegacy;         // 일반 Text를 사용하는 경우 여기에 연결
    public GameObject correctPanel;       // 정답 시 띄울 패널 (CorrectPanel)
    public GameObject incorrectPanel;     // 기회 소진 시 띄울 패널 (InCorrectPannel)
    
    private int currentChance = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateChanceUI();
        
        // 시작할 때 패널들이 켜져있다면 끄기
        if (correctPanel != null) correctPanel.SetActive(false);
        if (incorrectPanel != null) incorrectPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 제출 버튼 클릭 시 호출할 함수
    public void OnSubmit()
    {
        // 패널이 할당되어 있고, 현재 비활성화 상태라면 로직을 수행하지 않음
        if (gamePanel != null && !gamePanel.activeInHierarchy)
        {
            return;
        }

        if (currentChance <= 0)
        {
            Debug.Log("기회가 없습니다.");
            return;
        }

        CircleList circleList = FindObjectOfType<CircleList>();
        if (circleList != null)
        {
            CheckSequence(circleList.connections);
        }
        else
        {
            Debug.LogError("CircleList 스크립트를 찾을 수 없습니다.");
        }
    }

    public void CheckSequence(List<SubjectConnection> connections)
    {
        bool redToYellow = false;
        bool yellowToPink = false;
        bool pinkToBlue = false;

        foreach (var conn in connections)
        {
            string from = conn.fromSubject;
            string to = conn.toSubject;

            // Red -> Yellow (빨강 -> 노랑)
            bool isRedFrom = from.Contains("Red") || from.Contains("빨");
            bool isYellowTo = to.Contains("Yellow") || to.Contains("노");
            
            if (isRedFrom && isYellowTo) redToYellow = true;

            // Yellow -> Pink (노랑 -> 핑크/분홍)
            bool isYellowFrom = from.Contains("Yellow") || from.Contains("노");
            bool isPinkTo = to.Contains("Pink") || to.Contains("핑") || to.Contains("분");
            
            if (isYellowFrom && isPinkTo) yellowToPink = true;

            // Pink -> Blue (핑크/분홍 -> 파랑)
            bool isPinkFrom = from.Contains("Pink") || from.Contains("핑") || from.Contains("분");
            bool isBlueTo = to.Contains("Blue") || to.Contains("파");
            
            if (isPinkFrom && isBlueTo) pinkToBlue = true;
        }

        if (redToYellow && yellowToPink && pinkToBlue)
        {
            Debug.Log("정확함");
            if (correctPanel != null) correctPanel.SetActive(true);
        }
        else
        {
            Debug.Log("틀렸음");
            DecreaseChance();
        }
    }

    private void DecreaseChance()
    {
        if (currentChance > 0)
        {
            currentChance--;
            UpdateChanceUI();
            
            if (currentChance <= 0)
            {
                Debug.Log("기회를 모두 소진했습니다.");
                if (incorrectPanel != null) incorrectPanel.SetActive(true);
            }
        }
    }

    private void UpdateChanceUI()
    {
        if (chanceTextTMP != null)
            chanceTextTMP.text = currentChance.ToString();
        
        if (chanceTextLegacy != null)
            chanceTextLegacy.text = currentChance.ToString();
    }
}
