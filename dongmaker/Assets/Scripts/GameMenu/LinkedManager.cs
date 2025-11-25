using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class LinkedManager : MonoBehaviour
{
    public enum GameType
    {
        LinkedList,
        Array
    }

    [Header("Game Settings")]
    public GameType currentGameType;
    public GameObject gamePanel;          // 이 게임 모드의 패널 (예: LinkedList)

    [Header("UI Settings")]
    public TextMeshProUGUI chanceTextTMP; // TextMeshPro를 사용하는 경우 여기에 연결
    public Text chanceTextLegacy;         // 일반 Text를 사용하는 경우 여기에 연결
    public GameObject correctPanel;       // 정답 시 띄울 패널 (CorrectPanel)
    public GameObject incorrectPanel;     // 기회 소진 시 띄울 패널 (InCorrectPannel)
    
    [Header("Array Game Settings")]
    public Transform desksParent; // Desks 부모 오브젝트를 여기에 연결하면 슬롯을 자동으로 찾습니다.
    public DeskSlot[] deskSlots;    // 책상 위의 슬롯들 (순서대로)
    public string[] rowA_Answers = new string[5]; // A열 정답 (0~4)
    public string[] rowB_Answers = new string[5]; // B열 정답 (0~4)
    public string[] rowC_Answers = new string[5]; // C열 정답 (0~4)

    private int currentChance = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateChanceUI();
        
        // 시작할 때 패널들이 켜져있다면 끄기
        if (correctPanel != null) correctPanel.SetActive(false);
        if (incorrectPanel != null) incorrectPanel.SetActive(false);

        // [자동 감지] ArrayPannel이 씬에 있고 활성화되어 있다면 Array 모드로 자동 전환
        GameObject arrayPanelObj = GameObject.Find("ArrayPannel");
        if (arrayPanelObj != null && arrayPanelObj.activeInHierarchy)
        {
            currentGameType = GameType.Array;
            Debug.Log("LinkedManager: ArrayPannel이 활성화되어 있어 GameType을 Array로 자동 설정했습니다.");
        }

        InitGameLogic();
    }

    void InitGameLogic()
    {
        if (currentGameType == GameType.Array)
        {
            // Desks 부모가 연결되어 있지 않다면 "Desks"라는 이름의 오브젝트를 찾아봄
            if (desksParent == null)
            {
                GameObject desksObj = GameObject.Find("Desks");
                if (desksObj != null) desksParent = desksObj.transform;
            }

            // Desks 부모가 연결되어 있고 deskSlots가 비어있다면 자동으로 자식에서 찾아서 할당
            if (desksParent != null && (deskSlots == null || deskSlots.Length == 0))
            {
                deskSlots = desksParent.GetComponentsInChildren<DeskSlot>();
            }

            // 정답 배열이 비어있는지 확인 (인스펙터에서 설정하지 않은 경우)
            bool isAnswersEmpty = true;
            foreach(var s in rowA_Answers) if(!string.IsNullOrEmpty(s)) isAnswersEmpty = false;
            foreach(var s in rowB_Answers) if(!string.IsNullOrEmpty(s)) isAnswersEmpty = false;
            foreach(var s in rowC_Answers) if(!string.IsNullOrEmpty(s)) isAnswersEmpty = false;

            // 정답이 비어있다면 스크린샷의 기본 문제 정답으로 초기화
            if (isAnswersEmpty)
            {
                // A[1] = Red
                if (rowA_Answers.Length > 1) rowA_Answers[1] = "Red";
                // B[2] = Yellow, B[4] = Pink
                if (rowB_Answers.Length > 4) 
                {
                    rowB_Answers[2] = "Yellow";
                    rowB_Answers[4] = "Pink";
                }
                // C[3] = Blue
                if (rowC_Answers.Length > 3) rowC_Answers[3] = "Blue";
                
                Debug.Log("LinkedManager: 정답 배열이 비어있어 기본값(스크린샷 문제)으로 초기화했습니다.");
            }

            // [디버깅용] 슬롯 순서 확인을 위해 게임 오브젝트 이름을 인덱스 번호로 변경
            if (deskSlots != null)
            {
                for (int i = 0; i < deskSlots.Length; i++)
                {
                    if (deskSlots[i] != null)
                    {
                        // 예: DeskSlot_00, DeskSlot_01 ...
                        deskSlots[i].name = $"DeskSlot_{i:D2}"; 
                        
                        // 텍스트 컴포넌트가 자식에 있다면 번호 표시 (선택사항)
                        Text slotText = deskSlots[i].GetComponentInChildren<Text>();
                        if (slotText) slotText.text = i.ToString();
                    }
                }
            }
        }
    }

    public void InitializeGame(GameType type)
    {
        currentGameType = type;
        currentChance = 3;
        UpdateChanceUI();
        
        if (correctPanel != null) correctPanel.SetActive(false);
        if (incorrectPanel != null) incorrectPanel.SetActive(false);
        
        InitGameLogic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 제출 버튼 클릭 시 호출할 함수
    public void OnSubmit()
    {
        Debug.Log($"OnSubmit Called. Current Game Type: {currentGameType}");

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

        if (currentGameType == GameType.LinkedList)
        {
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
        else if (currentGameType == GameType.Array)
        {
            if (CheckArrayAnswer())
            {
                Debug.Log("정확함");
                if (correctPanel != null) correctPanel.SetActive(true);
                
                // 게임 클리어 알림
                if (InGameFlowManager.Instance != null)
                {
                    InGameFlowManager.Instance.OnGameFinished(true);
                }
            }
            else
            {
                Debug.Log("틀렸음");
                DecreaseChance();
            }
        }
    }

    bool CheckArrayAnswer()
    {
        // 슬롯이 충분한지 확인 (최소 15개 가정)
        if (deskSlots.Length < 15) 
        {
            Debug.LogWarning("슬롯 개수가 부족합니다. (최소 15개 필요: A 5개, B 5개, C 5개)");
            // 강제 실패 처리하거나 로직에 따라 return false
        }

        for (int i = 0; i < deskSlots.Length; i++)
        {
            // 15개까지만 검사 (A, B, C 각 5개)
            if (i >= 15) break;

            string expected = "";

            // 인덱스 범위를 기준으로 A, B, C 정답 배열에서 값 가져오기
            if (i < 5) 
            {
                // 0~4: A열
                if (i < rowA_Answers.Length) expected = rowA_Answers[i];
            }
            else if (i < 10) 
            {
                // 5~9: B열
                int indexB = i - 5;
                if (indexB < rowB_Answers.Length) expected = rowB_Answers[indexB];
            }
            else 
            {
                // 10~14: C열
                int indexC = i - 10;
                if (indexC < rowC_Answers.Length) expected = rowC_Answers[indexC];
            }

            Block current = deskSlots[i].currentBlock;

            // 정답이 비어있는 경우 (빈 슬롯이어야 함)
            if (string.IsNullOrEmpty(expected))
            {
                if (current != null) 
                {
                    Debug.Log($"[오답] {deskSlots[i].name} (인덱스 {i}): 비어있어야 하는데 '{current.blockValue}' 블록이 있습니다.");
                    return false; // 블록이 있으면 오답
                }
            }
            // 정답이 있는 경우
            else
            {
                if (current == null) 
                {
                    Debug.Log($"[오답] {deskSlots[i].name} (인덱스 {i}): '{expected}' 블록이 필요한데 비어있습니다.");
                    return false; // 블록이 없으면 오답
                }
                if (current.blockValue != expected) 
                {
                    Debug.Log($"[오답] {deskSlots[i].name} (인덱스 {i}): '{expected}' 블록이 필요한데 '{current.blockValue}' 블록이 있습니다.");
                    return false; // 값이 다르면 오답
                }
            }
        }
        return true;
    }

    public void CheckSequence(List<SubjectConnection> connections)
    {
        bool redToYellow = false;
        bool yellowToBlue = false;
        bool blueToPink = false;

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
        }

        if (redToYellow && yellowToBlue && blueToPink)
        {
            Debug.Log("정확함");
            if (correctPanel != null) correctPanel.SetActive(true);
            
            // 게임 클리어 알림
            if (InGameFlowManager.Instance != null)
            {
                InGameFlowManager.Instance.OnGameFinished(true);
            }
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
