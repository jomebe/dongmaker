using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ScheduleSlot : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
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

    // 초기 상태 저장용 변수
    private Sprite defaultSprite;
    private Vector2 defaultLabelPos;
    private Vector2 defaultLabelSize;
    private int defaultLabelFontSize;
    
    private DraggableSubject assignedSubject; // 현재 할당된 과목
    
    [HideInInspector]
    public DraggableSubject currentDraggingSubject; // 현재 이 슬롯에서 드래그 중인 과목 (다른 슬롯에서 참조용)

    void Start()
    {
        // 초기 상태 저장
        if (periodImage != null) defaultSprite = periodImage.sprite;
        if (periodLabel != null)
        {
            defaultLabelPos = periodLabel.rectTransform.anchoredPosition;
            defaultLabelSize = periodLabel.rectTransform.sizeDelta;
            defaultLabelFontSize = periodLabel.fontSize;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DraggableSubject droppedSubject = eventData.pointerDrag.GetComponent<DraggableSubject>();

            // 만약 드래그 대상이 DraggableSubject가 아니라면(즉, 다른 슬롯에서 드래그 시작했다면)
            if (droppedSubject == null)
            {
                ScheduleSlot sourceSlot = eventData.pointerDrag.GetComponent<ScheduleSlot>();
                if (sourceSlot != null)
                {
                    droppedSubject = sourceSlot.currentDraggingSubject;
                }
            }

            if (droppedSubject != null)
            {
                // 이미 할당된 과목이 있다면 먼저 제거(반환)
                if (assignedSubject != null)
                {
                    ResetSlot();
                }

                assignedSubject = droppedSubject;
                UpdateSlot(droppedSubject.subjectName);
                droppedSubject.Consumed();
            }
        }
    }

    // 드래그 시작: 할당된 과목이 있다면 "뽑아내는" 효과
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (assignedSubject != null)
        {
            currentDraggingSubject = assignedSubject;
            
            // 슬롯 초기화 (과목 버튼은 다시 활성화됨)
            ResetSlot();

            // 과목 버튼의 드래그 시작 로직 수동 호출
            currentDraggingSubject.OnBeginDrag(eventData);
            
            // 마우스 위치로 즉시 이동시켜서 자연스럽게 이어지도록 함
            currentDraggingSubject.SetPosition(eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentDraggingSubject != null)
        {
            currentDraggingSubject.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentDraggingSubject != null)
        {
            currentDraggingSubject.OnEndDrag(eventData);
            currentDraggingSubject = null;
        }
    }

    void ResetSlot()
    {
        // 1. 과목 버튼 다시 활성화
        if (assignedSubject != null)
        {
            assignedSubject.gameObject.SetActive(true);
            assignedSubject = null;
        }

        // 2. 배경 이미지 복구
        if (periodImage != null && defaultSprite != null)
        {
            periodImage.sprite = defaultSprite;
        }

        // 3. "1교시" 텍스트 복구
        if (periodLabel != null)
        {
            StopAllCoroutines(); // 진행 중인 애니메이션 중지
            StartCoroutine(AnimateText(periodLabel, defaultLabelPos, defaultLabelSize, defaultLabelFontSize));
        }

        // 4. SelectedSubject 비활성화
        if (selectedSubjectText != null)
        {
            selectedSubjectText.gameObject.SetActive(false);
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
            StopAllCoroutines();
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

    public string GetAssignedSubjectName()
    {
        if (assignedSubject != null)
        {
            return assignedSubject.subjectName;
        }
        return null;
    }
}
