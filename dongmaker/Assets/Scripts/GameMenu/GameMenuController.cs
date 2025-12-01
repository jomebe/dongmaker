using UnityEngine;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject schedulePanel;
    public GameObject statsPanel;

    [Header("Character Image")]
    public Image characterImage;  // 동 캐릭터 이미지
    public Sprite conditionSprite1;  // 컨디션 1 (최악)
    public Sprite conditionSprite2;  // 컨디션 2
    public Sprite conditionSprite3;  // 컨디션 3
    public Sprite conditionSprite4;  // 컨디션 4
    public Sprite conditionSprite5;  // 컨디션 5 (최상)

    void Start()
    {
        UpdateCharacterImage();
    }

    void OnEnable()
    {
        UpdateCharacterImage();
    }

    // 컨디션에 따라 캐릭터 이미지 변경
    public void UpdateCharacterImage()
    {
        if (characterImage == null || GameManager.Instance == null) return;

        int condition = Mathf.Clamp(GameManager.Instance.condition, 1, 5);

        switch (condition)
        {
            case 1:
                if (conditionSprite1 != null) characterImage.sprite = conditionSprite1;
                break;
            case 2:
                if (conditionSprite2 != null) characterImage.sprite = conditionSprite2;
                break;
            case 3:
                if (conditionSprite3 != null) characterImage.sprite = conditionSprite3;
                break;
            case 4:
                if (conditionSprite4 != null) characterImage.sprite = conditionSprite4;
                break;
            case 5:
                if (conditionSprite5 != null) characterImage.sprite = conditionSprite5;
                break;
        }
    }

    // 스케줄 선택 버튼 클릭 시 호출
    public void OpenSchedulePanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (schedulePanel != null) schedulePanel.SetActive(true);
        if (statsPanel != null) statsPanel.SetActive(false);
    }

    // 스탯 보기 버튼 클릭 시 호출
    public void OpenStatsPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (schedulePanel != null) schedulePanel.SetActive(false);
        if (statsPanel != null) statsPanel.SetActive(true);
    }

    // (옵션) 다시 메인으로 돌아오는 기능이 필요할 때 사용
    public void OpenMainPanel()
    {
        if (schedulePanel != null) schedulePanel.SetActive(false);
        if (statsPanel != null) statsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }
}
