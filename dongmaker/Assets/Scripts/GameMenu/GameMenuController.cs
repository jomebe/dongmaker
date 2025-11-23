using UnityEngine;

public class GameMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject schedulePanel;

    // 스케줄 선택 버튼 클릭 시 호출
    public void OpenSchedulePanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (schedulePanel != null) schedulePanel.SetActive(true);
    }

    // (옵션) 다시 메인으로 돌아오는 기능이 필요할 때 사용
    public void OpenMainPanel()
    {
        if (schedulePanel != null) schedulePanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }
}
