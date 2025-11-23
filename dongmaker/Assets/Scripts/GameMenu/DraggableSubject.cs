using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSubject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    private Transform startParent;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas canvas;
    public string subjectName;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Text 컴포넌트에서 과목 이름 가져오기
        Text textComp = GetComponentInChildren<Text>();
        if (textComp != null)
            subjectName = textComp.text;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        startParent = transform.parent;
        
        // 드래그 시 부모를 캔버스로 옮겨서 레이아웃 영향에서 벗어나고 맨 위에 표시되게 함
        if (canvas != null)
        {
            transform.SetParent(canvas.transform, true);
        }

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetPosition(eventData.position);
    }

    public void SetPosition(Vector2 screenPos)
    {
        if (canvas != null)
        {
            Vector3 globalMousePos;
            // 캔버스 평면 상의 월드 좌표로 변환
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out globalMousePos))
            {
                rectTransform.position = globalMousePos;
            }
        }
        else
        {
            transform.position = screenPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        
        // 원래 부모로 복귀
        transform.SetParent(startParent, true);
        transform.position = startPosition;
    }

    public void Consumed()
    {
        // 사용되었을 때 호출: 원래 위치로 되돌려놓고 비활성화
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(startParent, true);
        transform.position = startPosition;
        gameObject.SetActive(false);
    }
}
