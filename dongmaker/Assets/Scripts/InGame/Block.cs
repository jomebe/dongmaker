using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Block : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string blockValue; // 정답 확인용 값
    
    [HideInInspector] public Transform originalParent; // 원래 있던 곳 (Shelf 등)
    [HideInInspector] public Vector3 originalPosition; // 원래 있던 위치

    private Transform startParent; // 드래그 시작 시 부모
    private Vector3 startPosition; // 드래그 시작 시 위치
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        // 초기 부모와 위치 저장
        originalParent = transform.parent;
        originalPosition = transform.position;

        // 값이 비어있다면 텍스트 컴포넌트에서 가져오기 시도
        if(string.IsNullOrEmpty(blockValue))
        {
            Text t = GetComponentInChildren<Text>();
            if(t) blockValue = t.text;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        startParent = transform.parent;
        
        // 드래그 시작 시 부모가 슬롯이었다면 참조 해제
        DeskSlot slot = startParent.GetComponent<DeskSlot>();
        if (slot != null && slot.currentBlock == this)
        {
            slot.currentBlock = null;
        }

        canvasGroup.blocksRaycasts = false;
        
        // 드래그 중에는 캔버스 바로 아래로 옮겨서 맨 위에 그려지게 함
        if (canvas != null) transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        else
        {
            transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        
        // 드롭된 곳이 슬롯이 아니라면 (부모가 여전히 캔버스라면) 원래 위치로 복귀
        if (canvas != null && transform.parent == canvas.transform)
        {
            ReturnToStart();
        }
        else if (canvas == null)
        {
             ReturnToStart();
        }
    }
    
    public void SetParent(Transform newParent)
    {
        transform.SetParent(newParent);
        rectTransform.anchoredPosition = Vector2.zero; // 슬롯 중앙에 위치
    }
    
    public void ReturnToStart()
    {
        transform.SetParent(startParent, true);
        transform.position = startPosition;

        // 원래 슬롯으로 돌아왔다면 참조 복구
        DeskSlot slot = startParent.GetComponent<DeskSlot>();
        if (slot != null)
        {
            slot.currentBlock = this;
        }
    }

    // 아예 초기 위치(선반)로 되돌리기
    public void ReturnToShelf()
    {
        transform.SetParent(originalParent, true);
        transform.position = originalPosition;
        rectTransform.anchoredPosition = Vector3.zero; // 필요 시 조정
    }
}
