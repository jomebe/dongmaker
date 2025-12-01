using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameEndManager : MonoBehaviour
{
    [Header("Summary Panel (첫번째 화면)")]
    public GameObject summaryPanel;
    public Text statChangesText;  // 스탯 변화량 표시

    [Header("Result Panel (두번째 화면)")]
    public GameObject resultPanel;
    public Text conditionText;     // 띄운 사람: 큐비, 짹짹, 동
    public Text scheduleResultText; // 1교시 배열 - 성공 등

    [Header("Event Panel (세번째 화면)")]
    public GameObject eventPanel;
    public Text eventTitleText;    // 이벤트 제목
    public Text eventDescText;     // 이벤트 설명
    public Text eventStatText;     // 이벤트 스탯 변화

    [Header("Settings")]
    public float autoNextDelay = 5f; // 자동으로 다음 화면 넘어가는 시간

    private int successCount = 0;
    private int failCount = 0;
    private int currentCycle = 1;

    // 시작 시 스탯 저장
    private int startUnderstanding;
    private int startAccuracy;
    private int startLogic;
    private int startConcentration;
    private int startStress;
    private int startConfidence;
    private int startCondition;

    // 이벤트 데이터
    private struct CycleEvent
    {
        public string title;
        public string description;
        public string statChanges;
        public int understanding;
        public int accuracy;
        public int logic;
        public int concentration;
        public int stress;
        public int confidence;
        public int condition;

        public CycleEvent(string t, string d, string s, int u = 0, int a = 0, int l = 0, int c = 0, int st = 0, int cf = 0, int cd = 0)
        {
            title = t;
            description = d;
            statChanges = s;
            understanding = u;
            accuracy = a;
            logic = l;
            concentration = c;
            stress = st;
            confidence = cf;
            condition = cd;
        }
    }

    private CycleEvent[] cycleEvents;

    void Awake()
    {
        InitializeCycleEvents();
    }

    void InitializeCycleEvents()
    {
        cycleEvents = new CycleEvent[]
        {
            new CycleEvent("새 학기의 시작", "새 학기가 시작되었다. 새로운 2학년 생활이 열린다.", "자신감 +1 / 스트레스 -1", 0, 0, 0, 0, -1, 1, 0),
            new CycleEvent("적응기", "조금씩 학교 생활에 익숙해지고 있다.", "논리력 +1 / 집중력 +1", 0, 0, 1, 1, 0, 0, 0),
            new CycleEvent("첫 수행평가 예고", "자료구조 수행평가가 곧 있을 예정이라고 한다.", "스트레스 +1 / 이해도 +1", 1, 0, 0, 0, 1, 0, 0),
            new CycleEvent("첫 수행평가", "첫 수행평가를 치렀다. 생각보다 잘한 것 같다.", "이해도 +1 / 자신감 +1", 1, 0, 0, 0, 0, 1, 0),
            new CycleEvent("심화 학습 시작", "수업이 조금 더 어려워지기 시작했다.", "이해도 +1 / 논리력 +1", 1, 0, 1, 0, 0, 0, 0),
            new CycleEvent("중간고사 예고", "중간고사가 임박했다. 조금 긴장되는 분위기다.", "스트레스 +1 / 집중력 +1", 0, 0, 0, 1, 1, 0, 0),
            new CycleEvent("중간고사", "중간고사를 치렀다. 한숨 돌리는 순간이다.", "이해도 +2 / 스트레스 +1", 2, 0, 0, 0, 1, 0, 0),
            new CycleEvent("선린제 준비", "선린제가 가까워지며 학교 분위기가 들뜬다.", "스트레스 -1 / 자신감 +1", 0, 0, 0, 0, -1, 1, 0),
            new CycleEvent("선린제 당일", "선린제가 열렸다. 동은 즐거운 하루를 보냈다.", "스트레스 -2 / 자신감 +2", 0, 0, 0, 0, -2, 2, 0),
            new CycleEvent("프로젝트 시즌", "축제가 끝나고 프로젝트 준비로 바빠지기 시작했다.", "논리력 +1 / 집중력 +1", 0, 0, 1, 1, 0, 0, 0),
            new CycleEvent("기말고사 예고", "기말고사가 다가오고 있다. 마지막 시험이다.", "스트레스 +2 / 집중력 +1", 0, 0, 0, 1, 2, 0, 0),
            new CycleEvent("기말고사", "한 해의 마지막 시험을 치렀다.", "이해도 +2 / 논리력 +1 / 스트레스 +1", 2, 0, 1, 0, 1, 0, 0),
            new CycleEvent("방학 시작", "기말고사가 끝나며 방학이 시작되었다.", "스트레스 -3 / 컨디션 +1", 0, 0, 0, 0, -3, 0, 1),
            new CycleEvent("회고의 시간", "올 한 해를 돌아보는 시간이 찾아왔다.", "이해도 +1 / 논리력 +1", 1, 0, 1, 0, 0, 0, 0),
            new CycleEvent("마지막 준비", "2학년의 마지막이 다가왔다. 이제 정말 끝이 보인다.", "집중력 +1 / 자신감 +1", 0, 0, 0, 1, 0, 1, 0),
            new CycleEvent("최종 평가", "동의 1년이 마무리되었다. 이제 엔딩이 결정된다.", "", 0, 0, 0, 0, 0, 0, 0)
        };
    }

    void Start()
    {
        // 모든 패널 끄고 요약 패널만 켜기
        if (resultPanel != null) resultPanel.SetActive(false);
        if (eventPanel != null) eventPanel.SetActive(false);
        if (summaryPanel != null) summaryPanel.SetActive(true);

        // 현재 사이클 가져오기
        if (GameManager.Instance != null)
        {
            currentCycle = GameManager.Instance.currentCycle;
            successCount = GameManager.Instance.dailySuccessCount;
            failCount = GameManager.Instance.dailyFailCount;

            LoadStartStats();
            ShowSummary();
        }

        // 5초 후 다음 화면
        StartCoroutine(AutoNextScreen());
    }

    void LoadStartStats()
    {
        // PlayerPrefs에서 하루 시작 시 저장한 스탯 불러오기
        startUnderstanding = PlayerPrefs.GetInt("StartUnderstanding", 10);
        startAccuracy = PlayerPrefs.GetInt("StartAccuracy", 20);
        startLogic = PlayerPrefs.GetInt("StartLogic", 10);
        startConcentration = PlayerPrefs.GetInt("StartConcentration", 10);
        startStress = PlayerPrefs.GetInt("StartStress", 10);
        startConfidence = PlayerPrefs.GetInt("StartConfidence", 10);
        startCondition = PlayerPrefs.GetInt("StartCondition", 4);
    }

    void ShowSummary()
    {
        if (statChangesText == null || GameManager.Instance == null) return;

        // 스탯 변화량 계산
        int diffUnderstanding = GameManager.Instance.understanding - startUnderstanding;
        int diffAccuracy = GameManager.Instance.accuracy - startAccuracy;
        int diffLogic = GameManager.Instance.logic - startLogic;
        int diffConcentration = GameManager.Instance.concentration - startConcentration;
        int diffStress = GameManager.Instance.stress - startStress;
        int diffConfidence = GameManager.Instance.confidence - startConfidence;
        int diffCondition = GameManager.Instance.condition - startCondition;

        // 변화량 텍스트 생성
        List<string> changes = new List<string>();
        if (diffUnderstanding != 0) changes.Add($"이해도 {FormatChange(diffUnderstanding)}");
        if (diffAccuracy != 0) changes.Add($"정확성 {FormatChange(diffAccuracy)}");
        if (diffLogic != 0) changes.Add($"논리력 {FormatChange(diffLogic)}");
        if (diffConcentration != 0) changes.Add($"집중력 {FormatChange(diffConcentration)}");
        if (diffStress != 0) changes.Add($"스트레스 {FormatChange(diffStress)}");
        if (diffConfidence != 0) changes.Add($"자신감 {FormatChange(diffConfidence)}");
        if (diffCondition != 0) changes.Add($"컨디션 {FormatChange(diffCondition)}");

        statChangesText.text = string.Join(" / ", changes);
    }

    string FormatChange(int value)
    {
        return value > 0 ? $"+{value}" : value.ToString();
    }

    IEnumerator AutoNextScreen()
    {
        yield return new WaitForSeconds(autoNextDelay);
        ShowResultPanel();
    }

    IEnumerator AutoNextToEvent()
    {
        yield return new WaitForSeconds(autoNextDelay);
        ShowEventPanel();
    }

    public void ShowResultPanel()
    {
        if (summaryPanel != null) summaryPanel.SetActive(false);
        if (eventPanel != null) eventPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(true);

        ShowScheduleResults();
        ShowCondition();

        // 5초 후 이벤트 패널로
        StartCoroutine(AutoNextToEvent());
    }

    public void ShowEventPanel()
    {
        if (summaryPanel != null) summaryPanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
        if (eventPanel != null) eventPanel.SetActive(true);

        ShowCycleEvent();
    }

    void ShowCycleEvent()
    {
        if (cycleEvents == null || currentCycle < 1 || currentCycle > cycleEvents.Length) return;

        CycleEvent evt = cycleEvents[currentCycle - 1];

        if (eventTitleText != null) eventTitleText.text = evt.title;
        if (eventDescText != null) eventDescText.text = evt.description;
        if (eventStatText != null) eventStatText.text = evt.statChanges;

        // 이벤트 스탯 적용
        ApplyEventStats(evt);
    }

    void ApplyEventStats(CycleEvent evt)
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.understanding += evt.understanding;
        GameManager.Instance.accuracy += evt.accuracy;
        GameManager.Instance.logic += evt.logic;
        GameManager.Instance.concentration += evt.concentration;
        GameManager.Instance.stress += evt.stress;
        GameManager.Instance.confidence += evt.confidence;
        GameManager.Instance.condition += evt.condition;

        // 다음 사이클로
        GameManager.Instance.currentCycle++;
    }

    void ShowScheduleResults()
    {
        if (scheduleResultText == null || GameManager.Instance == null) return;

        string[] schedule = GameManager.Instance.schedule;
        bool[] results = GetPeriodResults();

        string text = "";
        for (int i = 0; i < 4; i++)
        {
            string subject = schedule[i];
            string result = results[i] ? "성공" : "실패";
            text += $"{i + 1}교시  {subject}  -  {result}\n";
        }

        scheduleResultText.text = text.TrimEnd('\n');
    }

    bool[] GetPeriodResults()
    {
        // PlayerPrefs에서 각 교시 결과 가져오기
        bool[] results = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            results[i] = PlayerPrefs.GetInt($"Period{i}Result", 0) == 1;
        }
        return results;
    }

    void ShowCondition()
    {
        if (conditionText == null) return;

        // 성공/실패 개수에 따라 메시지 표시
        // 전부 퍼펙트 or 퍼펙트3개 성공 1개 → "동의 집중력이 오늘따라 좋았다."
        // 2개 실패 2개 성공 or 3개 실패 1개 성공 → "동이 조금 지쳐 보인다."
        // 다 실패 → "스트레스가 조금 오른 것 같다."

        if (failCount == 0)
        {
            // 전부 퍼펙트 (4성공 0실패)
            conditionText.text = "\"동의 집중력이 오늘따라 좋았다.\"";
        }
        else if (failCount == 1 && successCount >= 3)
        {
            // 3성공 1실패
            conditionText.text = "\"동의 집중력이 오늘따라 좋았다.\"";
        }
        else if (failCount == 4)
        {
            // 다 실패 (0성공 4실패)
            conditionText.text = "\"스트레스가 조금 오른 것 같다.\"";
        }
        else
        {
            // 2실패 2성공 or 3실패 1성공
            conditionText.text = "\"동이 조금 지쳐 보인다.\"";
        }
    }

    // 버튼으로 호출 - 메인 메뉴로 돌아가기
    public void ReturnToMenu()
    {
        // 사이클 16 이후면 엔딩으로
        if (GameManager.Instance != null && GameManager.Instance.currentCycle > 16)
        {
            SceneManager.LoadScene("Ending");
        }
        else
        {
            SceneManager.LoadScene("GameMenu");
        }
    }

    // 화면 클릭 시 다음으로 넘어가기
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (summaryPanel != null && summaryPanel.activeInHierarchy)
            {
                StopAllCoroutines();
                ShowResultPanel();
            }
            else if (resultPanel != null && resultPanel.activeInHierarchy)
            {
                StopAllCoroutines();
                ShowEventPanel();
            }
            else if (eventPanel != null && eventPanel.activeInHierarchy)
            {
                // 이벤트 패널에서 클릭하면 메뉴로 돌아가기
                ReturnToMenu();
            }
        }
    }
}
