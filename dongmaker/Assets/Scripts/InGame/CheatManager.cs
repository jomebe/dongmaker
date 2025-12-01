using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 시연용 치트 매니저
/// 게임 플레이 중 단축키로 빠르게 테스트할 수 있습니다.
/// </summary>
public class CheatManager : MonoBehaviour
{
    public static CheatManager Instance;

    [Header("치트 활성화")]
    public bool enableCheats = true;

    [Header("단축키 설정")]
    public KeyCode forceWinKey = KeyCode.F1;        // 현재 교시 성공
    public KeyCode forceFailKey = KeyCode.F2;       // 현재 교시 실패
    public KeyCode skipPeriodKey = KeyCode.F3;      // 다음 교시로 스킵
    public KeyCode addStatsKey = KeyCode.F4;        // 스탯 증가
    public KeyCode maxStatsKey = KeyCode.F5;        // 스탯 최대치
    public KeyCode resetStatsKey = KeyCode.F6;      // 스탯 초기화
    public KeyCode skipDayKey = KeyCode.F7;         // 하루 스킵 (GameEnd로)

    [Header("치트 수치")]
    public int statAddAmount = 10;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!enableCheats) return;

        // F1: 현재 교시 강제 성공
        if (Input.GetKeyDown(forceWinKey))
        {
            ForceWin();
        }

        // F2: 현재 교시 강제 실패
        if (Input.GetKeyDown(forceFailKey))
        {
            ForceFail();
        }

        // F3: 다음 교시로 스킵
        if (Input.GetKeyDown(skipPeriodKey))
        {
            SkipPeriod();
        }

        // F4: 스탯 증가
        if (Input.GetKeyDown(addStatsKey))
        {
            AddStats(statAddAmount);
        }

        // F5: 스탯 최대치
        if (Input.GetKeyDown(maxStatsKey))
        {
            MaxStats();
        }

        // F6: 스탯 초기화
        if (Input.GetKeyDown(resetStatsKey))
        {
            ResetStats();
        }

        // F7: 하루 스킵
        if (Input.GetKeyDown(skipDayKey))
        {
            SkipDay();
        }
    }

    void ForceWin()
    {
        Debug.Log("<color=green>[치트] 강제 성공!</color>");
        if (InGameFlowManager.Instance != null)
        {
            InGameFlowManager.Instance.OnGameFinished(true);
        }
    }

    void ForceFail()
    {
        Debug.Log("<color=red>[치트] 강제 실패!</color>");
        if (InGameFlowManager.Instance != null)
        {
            InGameFlowManager.Instance.OnGameFinished(false);
        }
    }

    void SkipPeriod()
    {
        Debug.Log("<color=yellow>[치트] 다음 교시로 스킵!</color>");
        if (InGameFlowManager.Instance != null)
        {
            // Reflection으로 private 필드 접근 또는 public 메서드 호출
            var field = typeof(InGameFlowManager).GetField("currentPeriodIndex", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                int currentIndex = (int)field.GetValue(InGameFlowManager.Instance);
                InGameFlowManager.Instance.StartPeriod(currentIndex + 1);
            }
        }
    }

    void AddStats(int amount)
    {
        Debug.Log($"<color=cyan>[치트] 모든 스탯 +{amount}!</color>");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.understanding += amount;
            GameManager.Instance.accuracy += amount;
            GameManager.Instance.logic += amount;
            GameManager.Instance.concentration += amount;
            GameManager.Instance.confidence += amount;
            GameManager.Instance.condition += amount;
            // 스트레스는 낮추기
            GameManager.Instance.stress = Mathf.Max(0, GameManager.Instance.stress - amount);
        }
    }

    void MaxStats()
    {
        Debug.Log("<color=magenta>[치트] 스탯 최대치!</color>");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.understanding = 100;
            GameManager.Instance.accuracy = 100;
            GameManager.Instance.logic = 100;
            GameManager.Instance.concentration = 100;
            GameManager.Instance.confidence = 100;
            GameManager.Instance.condition = 100;
            GameManager.Instance.stress = 0;
        }
    }

    void ResetStats()
    {
        Debug.Log("<color=white>[치트] 스탯 초기화!</color>");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.understanding = 50;
            GameManager.Instance.accuracy = 50;
            GameManager.Instance.logic = 50;
            GameManager.Instance.concentration = 50;
            GameManager.Instance.confidence = 50;
            GameManager.Instance.condition = 50;
            GameManager.Instance.stress = 50;
        }
    }

    void SkipDay()
    {
        Debug.Log("<color=orange>[치트] 하루 스킵! GameEnd로 이동</color>");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ApplyDailyEnd();
        }
        SceneManager.LoadScene("GameEnd");
    }

    void OnGUI()
    {
        if (!enableCheats) return;

        // 화면 왼쪽 상단에 치트 키 안내 표시
        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.yellow;
        style.padding = new RectOffset(10, 10, 10, 10);

        string cheatHelp = 
            "<b>[ 치트 키 ]</b>\n" +
            "F1: 강제 성공\n" +
            "F2: 강제 실패\n" +
            "F3: 교시 스킵\n" +
            "F4: 스탯 +10\n" +
            "F5: 스탯 최대\n" +
            "F6: 스탯 초기화\n" +
            "F7: 하루 스킵";

        GUI.Label(new Rect(10, 10, 200, 200), cheatHelp, style);
    }
}
