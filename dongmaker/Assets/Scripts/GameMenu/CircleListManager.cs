using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CircleListManager : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI chanceTextTMP; // TextMeshPro를 사용하는 경우 여기에 연결
    public Text chanceTextLegacy;         // 일반 Text를 사용하는 경우 여기에 연결
    public GameObject correctPanel;       // 정답 시 띄울 패널 (CorrectPanel)
    public GameObject incorrectPanel;     // 기회 소진 시 띄울 패널 (InCorrectPannel)
    
    [Header("Game Mode Settings")]
    public GameObject gameContent;        // 이 게임 모드의 최상위 오브젝트 (예: CircleListPannel)
    
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
        // 해당 게임 모드 오브젝트가 꺼져있으면 실행하지 않음
        if (gameContent != null && !gameContent.activeInHierarchy)
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
        bool yellowToBlue = false;
        bool blueToPink = false;
        bool pinkToRed = false;

        foreach (var conn in connections)
        {
            string from = conn.fromSubject;
            string to = conn.toSubject;

            // Red -> Yellow (빨강 -> 노랑)
            bool isRedFrom = from.Contains("Red") || from.Contains("빨");
            bool isYellowTo = to.Contains("Yellow") || to.Contains("노");
            
            if (isRedFrom && isYellowTo) redToYellow = true;

            // Yellow -> Blue (노랑 -> 파랑)
            bool isYellowFrom = from.Contains("Yellow") || from.Contains("노");
            bool isBlueTo = to.Contains("Blue") || to.Contains("파");
            
            if (isYellowFrom && isBlueTo) yellowToBlue = true;

            // Blue -> Pink (파랑 -> 핑크/분홍)
            bool isBlueFrom = from.Contains("Blue") || from.Contains("파");
            bool isPinkTo = to.Contains("Pink") || to.Contains("핑") || to.Contains("분");
            
            if (isBlueFrom && isPinkTo) blueToPink = true;

            // Pink -> Red (핑크/분홍 -> 빨강) - 원형 연결 추가
            bool isPinkFrom = from.Contains("Pink") || from.Contains("핑") || from.Contains("분");
            bool isRedTo = to.Contains("Red") || to.Contains("빨");

            if (isPinkFrom && isRedTo) pinkToRed = true;
        }

        if (redToYellow && yellowToBlue && blueToPink && pinkToRed)
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
