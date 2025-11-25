using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmButton : MonoBehaviour
{
    [Header("Schedule Slots")]
    public ScheduleSlot period1;
    public ScheduleSlot period2;
    public ScheduleSlot period3;
    public ScheduleSlot period4;

    public void OnConfirmClick()
    {
        // 각 슬롯에서 과목 이름 가져오기
        string p1 = period1.GetAssignedSubjectName();
        string p2 = period2.GetAssignedSubjectName();
        string p3 = period3.GetAssignedSubjectName();
        string p4 = period4.GetAssignedSubjectName();

        // 하나라도 비어있으면 진행하지 않음 (선택 사항: 필요 없으면 이 조건문 삭제)
        if (string.IsNullOrEmpty(p1) || string.IsNullOrEmpty(p2) || string.IsNullOrEmpty(p3) || string.IsNullOrEmpty(p4))
        {
            Debug.Log("모든 교시를 채워주세요!");
            return; 
        }

        // GameManager가 없으면 생성 (혹시 모를 오류 방지)
        if (GameManager.Instance == null)
        {
            GameObject go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }

        // 스케줄 저장
        GameManager.Instance.SaveSchedule(p1, p2, p3, p4);

        // InGame 씬으로 이동
        SceneManager.LoadScene("InGame");
    }
}
