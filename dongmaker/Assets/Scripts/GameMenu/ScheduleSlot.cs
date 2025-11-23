using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ScheduleSlot : MonoBehaviour, IDropHandler
{
    [Header("UI References")]
    public Image periodImage; // Period 오브젝트의 Image 컴포넌트
    public Sprite periodBackgroundSprite; // 변경할 PeriodBackground 이미지
    public Text periodLabel; // "1교시" 텍스트
    public Text selectedSubjectText; // "SelectedSubject" 텍스트

    [Header("Target Settings - Period Label")]
    public Vector2 labelTargetPos = new Vector2(113, 26);
    public Vector2 labelTargetSize = new Vector2(53, 29);
    public int labelTargetFontSize = 24;

    [Header("Target Settings - Selected Subject")]
    public Vector2 subjectTargetPos = new Vector2(104, 56);
    public Vector2 subjectTargetSize = new Vector2(70, 48);
    public int subjectTargetFontSize = 40;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DraggableSubject droppedSubject = eventData.pointerDrag.GetComponent<DraggableSubject>();
            if (droppedSubject != null)
            {
                UpdateSlot(droppedSubject.subjectName);
                droppedSubject.Consumed();
            }
        }
    }

    void UpdateSlot(string subjectName)
    {
        // 1. 배경 이미지 변경
        if (periodImage != null && periodBackgroundSprite != null)
        {
            periodImage.sprite = periodBackgroundSprite;
        }

        // 2. "1교시" 텍스트 애니메이션
        if (periodLabel != null)
        {
            StartCoroutine(AnimateText(periodLabel, labelTargetPos, labelTargetSize, labelTargetFontSize));
        }

        // 3. SelectedSubject 활성화 및 설정
        if (selectedSubjectText != null)
        {
            selectedSubjectText.gameObject.SetActive(true);
            selectedSubjectText.text = subjectName;
            StartCoroutine(AnimateText(selectedSubjectText, subjectTargetPos, subjectTargetSize, subjectTargetFontSize));
        }
    }

    IEnumerator AnimateText(Text target, Vector2 targetPos, Vector2 targetSize, int targetFontSize)
    {
        RectTransform rect = target.rectTransform;
        float duration = 0.5f; // 0.5초 동안 부드럽게 이동
        float elapsed = 0f;

        Vector2 startPos = rect.anchoredPosition;
        Vector2 startSize = rect.sizeDelta;
        int startFontSize = target.fontSize;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Smooth step (부드러운 가속/감속)
            t = t * t * (3f - 2f * t);

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            rect.sizeDelta = Vector2.Lerp(startSize, targetSize, t);
            target.fontSize = Mathf.RoundToInt(Mathf.Lerp(startFontSize, targetFontSize, t));

            yield return null;
        }

        // 최종 값 보정
        rect.anchoredPosition = targetPos;
        rect.sizeDelta = targetSize;
        target.fontSize = targetFontSize;
    }
}
