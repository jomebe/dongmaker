using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArrayGameController : MonoBehaviour
{
    [Header("Game Settings")]
    public float timeLimit = 60f;
    public int maxChances = 3;

    [Header("UI References")]
    public Text startText;      // "시작!" 텍스트
    public Text timerText;      // 타이머 텍스트
    public Text resultText;     // 결과 메시지 (정답, 다시 시도, 실패)
    public GameObject submitButton; // 제출 버튼

    [Header("Game Objects")]
    public Transform desksParent; // Desks 부모 오브젝트를 여기에 연결하면 슬롯을 자동으로 찾습니다.
    public DeskSlot[] deskSlots;    // 책상 위의 슬롯들 (순서대로)

    [Header("Correct Answers (Row A, B, C)")]
    public string[] rowA_Answers = new string[5]; // A열 정답 (0~4)
    public string[] rowB_Answers = new string[5]; // B열 정답 (0~4)
    public string[] rowC_Answers = new string[5]; // C열 정답 (0~4)

    private float currentTime;
    private int currentChances;
    private bool isGameActive = false;

    void Start()
    {
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

        StartCoroutine(GameRoutine());
    }

    IEnumerator GameRoutine()
    {
        // 초기화
        currentChances = maxChances;
        currentTime = timeLimit;
        if(resultText) resultText.text = "";
        if(submitButton) submitButton.SetActive(false);
        if(timerText) timerText.text = "";

        // 시작 텍스트 표시
        if(startText)
        {
            startText.gameObject.SetActive(true);
            startText.text = "시작!";
            yield return new WaitForSeconds(2f);
            startText.gameObject.SetActive(false);
        }

        // 게임 시작
        isGameActive = true;
        if(submitButton) submitButton.SetActive(true);
    }

    void Update()
    {
        if (isGameActive)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = 0;
                GameOver(false);
            }
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText)
        {
            timerText.text = $"남은 시간: {Mathf.Ceil(currentTime)}초";
        }
    }

    // 제출 버튼에 연결할 함수
    public void OnSubmit()
    {
        if (!isGameActive) return;

        if (CheckAnswer())
        {
            GameOver(true);
        }
        else
        {
            currentChances--;
            if (currentChances <= 0)
            {
                GameOver(false);
            }
            else
            {
                StartCoroutine(ShowMessage($"틀렸습니다! 남은 기회: {currentChances}", 1.5f));
            }
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

    void GameOver(bool isSuccess)
    {
        isGameActive = false;
        if(submitButton) submitButton.SetActive(false);

        if (isSuccess)
        {
            if(resultText) resultText.text = "정답!";
            Debug.Log("게임 성공!");
            // 성공 시 로직 (예: 스탯 상승, 씬 이동 등)
        }
        else
        {
            if(resultText) resultText.text = "실패...";
            Debug.Log("게임 실패!");
            // 실패 시 로직
        }
    }

    IEnumerator ShowMessage(string message, float duration)
    {
        if(resultText)
        {
            resultText.gameObject.SetActive(true); // 텍스트 켜기
            resultText.text = message;
            yield return new WaitForSeconds(duration);
            resultText.text = "";
            resultText.gameObject.SetActive(false); // 텍스트 끄기
        }
    }
}
