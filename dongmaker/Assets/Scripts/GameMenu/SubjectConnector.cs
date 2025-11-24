using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SubjectConnector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string subjectName;
    
    private static SubjectConnector dragStartSubject = null;
    private static ConnectionLine tempLine = null;
    private static GameObject lineContainer = null;
    
    [Header("Connection Settings")]
    public float lineWidth = 5f;
    public Color lineColor = Color.white;
    
    private List<ConnectionLine> connections = new List<ConnectionLine>();
    private CircleList circleList;
    
    void Start()
    {
        // EventSystem 확인
        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystem이 씬에 없습니다! Hierarchy에서 우클릭 -> UI -> Event System을 추가하세요.");
        }
        
        // Image의 Raycast Target 강제로 켜기
        Image img = GetComponent<Image>();
        if (img != null)
        {
            img.raycastTarget = true;
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Image 컴포넌트가 없습니다!");
        }
        
        // Canvas 하위에 선을 그릴 컨테이너 생성
        if (lineContainer == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                lineContainer = new GameObject("ConnectionLines");
                lineContainer.transform.SetParent(canvas.transform, false);
                
                // RectTransform 추가
                RectTransform rect = lineContainer.AddComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = Vector2.zero;
                rect.anchoredPosition = Vector2.zero;
                
                // 순서 조정: CircleListPannel 바로 뒤에 배치 (배경 위, 패널 아래)
                // 이렇게 해야 선이 배경보다는 위에 그려지고, 사각형들보다는 아래에 그려집니다.
                Transform panel = canvas.transform.Find("CircleListPannel");
                if (panel != null)
                {
                    lineContainer.transform.SetSiblingIndex(panel.GetSiblingIndex());
                }
                else
                {
                    // 패널을 못 찾으면 배경 바로 다음에 배치
                    Transform background = canvas.transform.Find("Background");
                    if (background == null) background = canvas.transform.Find("BlackBackground");
                    
                    if (background != null)
                    {
                        lineContainer.transform.SetSiblingIndex(background.GetSiblingIndex() + 1);
                    }
                    else
                    {
                        // 배경이 없으면 맨 위에 (사각형들 위)
                        lineContainer.transform.SetAsLastSibling();
                    }
                }
                
                Debug.Log("ConnectionLines 컨테이너 생성됨");
            }
        }
        
        // CircleList 찾기
        circleList = FindObjectOfType<CircleList>();
        
        if (string.IsNullOrEmpty(subjectName))
        {
            subjectName = gameObject.name;
        }
        
        Debug.Log($"SubjectConnector 초기화: {subjectName}");
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"드래그 시작: {subjectName}");
        dragStartSubject = this;
        CreateTempLine();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (tempLine != null && dragStartSubject == this)
        {
            tempLine.UpdateEndPosition(eventData.position);
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragStartSubject != this) return;
        
        // 마우스 아래에 있는 오브젝트 찾기
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (var result in results)
        {
            SubjectConnector targetSubject = result.gameObject.GetComponent<SubjectConnector>();
            if (targetSubject != null && targetSubject != this)
            {
                // 다른 사각형에 놓음 - 연결!
                CompleteConnection(this, targetSubject);
                break;
            }
        }
        
        // 임시 선 제거
        CancelConnection();
    }
    
    private void CreateTempLine()
    {
        if (lineContainer == null)
        {
            Debug.LogError("lineContainer가 null입니다!");
            return;
        }
        
        GameObject lineObj = new GameObject("TempLine");
        lineObj.transform.SetParent(lineContainer.transform, false);
        
        tempLine = lineObj.AddComponent<ConnectionLine>();
        tempLine.Initialize(transform as RectTransform, null, lineColor, lineWidth, true);
    }
    
    private void CompleteConnection(SubjectConnector from, SubjectConnector to)
    {
        // 이미 연결되어 있는지 확인
        foreach (var conn in from.connections)
        {
            if (conn != null && conn.endTransform == to.transform as RectTransform)
            {
                Debug.Log($"{from.subjectName}와 {to.subjectName}는 이미 연결되어 있습니다.");
                return;
            }
        }
        
        // 새 연결선 생성
        GameObject lineObj = new GameObject($"Connection_{from.subjectName}_to_{to.subjectName}");
        lineObj.transform.SetParent(lineContainer.transform, false);
        
        ConnectionLine line = lineObj.AddComponent<ConnectionLine>();
        line.Initialize(
            from.transform as RectTransform, 
            to.transform as RectTransform, 
            from.lineColor, 
            lineWidth, 
            false
        );
        
        from.connections.Add(line);
        
        // CircleList에 연결 정보 추가
        if (circleList != null)
        {
            circleList.AddConnection(from.subjectName, to.subjectName);
        }
        
        Debug.Log($"{from.subjectName}와 {to.subjectName}를 연결했습니다!");
    }
    
    private void CancelConnection()
    {
        dragStartSubject = null;
        
        if (tempLine != null)
        {
            Destroy(tempLine.gameObject);
            tempLine = null;
        }
    }
    
    // 모든 연결 제거
    public void ClearConnections()
    {
        foreach (var conn in connections)
        {
            if (conn != null)
            {
                Destroy(conn.gameObject);
            }
        }
        connections.Clear();
    }
}
