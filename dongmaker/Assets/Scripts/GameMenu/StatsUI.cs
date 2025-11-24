using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [Header("Stat Texts")]
    public Text understandingText;
    public Text accuracyText;
    public Text logicText;
    public Text concentrationText;
    public Text stressText;
    public Text confidenceText;
    public Text conditionText;

    [Header("Stat Sliders (Optional)")]
    public Slider understandingSlider;
    public Slider accuracySlider;
    public Slider logicSlider;
    public Slider concentrationSlider;
    public Slider stressSlider;
    public Slider confidenceSlider;
    public Slider conditionSlider;

    void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (GameManager.Instance == null) return;

        // 텍스트 업데이트
        if (understandingText != null) understandingText.text = GameManager.Instance.understanding.ToString();
        if (accuracyText != null) accuracyText.text = GameManager.Instance.accuracy.ToString();
        if (logicText != null) logicText.text = GameManager.Instance.logic.ToString();
        if (concentrationText != null) concentrationText.text = GameManager.Instance.concentration.ToString();
        if (stressText != null) stressText.text = GameManager.Instance.stress.ToString();
        if (confidenceText != null) confidenceText.text = GameManager.Instance.confidence.ToString();
        if (conditionText != null) conditionText.text = GameManager.Instance.condition.ToString();

        // 슬라이더 업데이트 (최대값 100 기준 예시)
        if (understandingSlider != null) understandingSlider.value = GameManager.Instance.understanding;
        if (accuracySlider != null) accuracySlider.value = GameManager.Instance.accuracy;
        if (logicSlider != null) logicSlider.value = GameManager.Instance.logic;
        if (concentrationSlider != null) concentrationSlider.value = GameManager.Instance.concentration;
        if (stressSlider != null) stressSlider.value = GameManager.Instance.stress;
        if (confidenceSlider != null) confidenceSlider.value = GameManager.Instance.confidence;
        if (conditionSlider != null) conditionSlider.value = GameManager.Instance.condition;
    }
}
