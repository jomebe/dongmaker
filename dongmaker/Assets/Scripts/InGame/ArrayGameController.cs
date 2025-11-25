using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ArrayGameController : MonoBehaviour
{
    [Header("Game Settings")]
    public GameObject gamePanel;          // 이 게임 모드의 패널

    [Header("UI Settings")]
    public TextMeshProUGUI chanceTextTMP; // TextMeshPro를 사용하는 경우 여기에 연결
    public Text chanceTextLegacy;         // 일반 Text를 사용하는 경우 여기에 연결
    public GameObject correctPanel;       // 정답 시 띄울 패널 (CorrectPanel)
    public GameObject incorrectPanel;     // 기회 소진 시 띄울 패널 (InCorrectPannel)

    [Header("Game Objects")]
    public Transform desksParent; // Desks 부모 오브젝트를 여기에 연결하면 슬롯을 자동으로 찾습니다.
    public DeskSlot[] deskSlots;    // 책상 위의 슬롯들 (순서대로)

    [Header("Correct Answers (Row A, B, C)")]
    public string[] rowA_Answers = new string[5]; // A열 정답 (0~4)
    public string[] rowB_Answers = new string[5]; // B열 정답 (0~4)
    public string[] rowC_Answers = new string[5]; // C열 정답 (0~4)

    private int currentChance = 3;

    void Start()
    {
        Debug.Log($"ArrayGameController is attached to: {gameObject.name}");

        UpdateChanceUI();
        
        // 시작할 때 패널들이 켜져있다면 끄기
        if (correctPanel != null) correctPanel.SetActive(false);
        if (incorrectPanel != null) incorrectPanel.SetActive(false);

        // Desks 부모가 연결되어 있고 deskSlots가 비어있다면 자동으로 자식에서 찾아서 할당
        if (desksParent != null && (deskSlots == null || deskSlots.Length == 0))
        {
            deskSlots = desksParent.GetComponentsInChildren<DeskSlot>();
        }

        // [디버깅용] 슬롯 순서 확인을 위해 게임 오브젝트 이름을 인덱스 번호로 변경
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

    // 제출 버튼에 연결할 함수
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

        if (CheckAnswer())
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

    bool CheckAnswer()
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
