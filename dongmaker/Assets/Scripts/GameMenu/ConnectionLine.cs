using UnityEngine;
using UnityEngine.UI;

public class ConnectionLine : MonoBehaviour
{
    public RectTransform startTransform;
    public RectTransform endTransform;
    private Image lineBody;
    private Image arrowHead;
    private RectTransform lineRect;
    private RectTransform arrowRect;
    private bool isTemporary;
    private float lineWidth = 5f;
    private Canvas _canvas;
    
    // 모든 ConnectionLine이 공유하는 화살표 스프라이트 (최적화)
    private static Sprite sharedTriangleSprite;
    
    public void Initialize(RectTransform start, RectTransform end, Color color, float width, bool temporary)
    {
        startTransform = start;
        endTransform = end;
        isTemporary = temporary;
        lineWidth = width;
        
        _canvas = GetComponentInParent<Canvas>();
        
        // 선 몸통
        GameObject bodyObj = new GameObject("LineBody");
        bodyObj.transform.SetParent(transform, false);
        lineRect = bodyObj.AddComponent<RectTransform>();
        lineBody = bodyObj.AddComponent<Image>();
        lineBody.color = color;
        lineBody.raycastTarget = false;
        
        // 화살촉
        GameObject headObj = new GameObject("ArrowHead");
        headObj.transform.SetParent(transform, false);
        arrowRect = headObj.AddComponent<RectTransform>();
        arrowHead = headObj.AddComponent<Image>();
        arrowHead.color = color;
        arrowHead.raycastTarget = false;
        
        // 삼각형 스프라이트 생성 (없을 때만 생성하고 재사용)
        if (sharedTriangleSprite == null)
        {
            sharedTriangleSprite = CreateTriangleSprite();
        }
        arrowHead.sprite = sharedTriangleSprite;
        
        UpdateLine();
    }
    
    void Update()
    {
        if (startTransform != null && (isTemporary || endTransform != null))
        {
            UpdateLine();
        }
    }
    
    private void UpdateLine()
    {
        if (startTransform == null || lineRect == null || arrowRect == null) return;
        
        // 1. 월드 좌표 구하기 (피벗에 상관없이 직사각형의 기하학적 중심 사용)
        Vector3 startWorldPos = GetRectCenterWorld(startTransform);
        Vector3 endWorldPos;
        
        if (endTransform != null)
        {
            endWorldPos = GetRectCenterWorld(endTransform);
        }
        else
        {
            // 마우스 위치 처리 (Screen Space -> World Space 변환)
            Camera cam = null;
            if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                cam = _canvas.worldCamera;
                if (cam == null) cam = Camera.main;
            }
            
            // 부모 RectTransform 평면상의 월드 좌표로 변환
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                transform.parent as RectTransform,
                Input.mousePosition,
                cam,
                out endWorldPos
            );
        }
        
        // 2. 로컬 좌표로 변환 (부모의 스케일 영향을 받지 않기 위해)
        // ConnectionLine 오브젝트 기준의 로컬 좌표계 사용
        Vector3 startLocal = transform.InverseTransformPoint(startWorldPos);
        Vector3 endLocal = transform.InverseTransformPoint(endWorldPos);
        
        // 3. 로컬 좌표계에서 거리와 각도 계산
        Vector2 direction = endLocal - startLocal;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // 4. UI 요소 설정 (로컬 좌표 사용)
        
        // 선 설정
        lineRect.localPosition = startLocal;
        // 거리가 20보다 작으면 선 길이를 0으로 (음수 방지)
        lineRect.sizeDelta = new Vector2(Mathf.Max(0, distance - 20f), lineWidth);
        lineRect.pivot = new Vector2(0, 0.5f);
        lineRect.localRotation = Quaternion.Euler(0, 0, angle);
        
        // 화살촉 설정
        arrowRect.localPosition = endLocal;
        arrowRect.sizeDelta = new Vector2(20f, 20f);
        // 피벗을 (1, 0.5)로 설정하여 화살표 끝(오른쪽)이 endPos에 오도록 함
        arrowRect.pivot = new Vector2(1f, 0.5f);
        arrowRect.localRotation = Quaternion.Euler(0, 0, angle);
    }

    // RectTransform의 기하학적 중심의 월드 좌표를 반환하는 헬퍼 함수
    private Vector3 GetRectCenterWorld(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        // 0: BottomLeft, 1: TopLeft, 2: TopRight, 3: BottomRight
        // 대각선(0번과 2번)의 중점이 사각형의 중심입니다.
        return (corners[0] + corners[2]) / 2f;
    }
    
    public void UpdateEndPosition(Vector2 screenPosition)
    {
        // Update()에서 자동으로 마우스 위치 추적
        UpdateLine();
    }
    
    private Sprite CreateTriangleSprite()
    {
        Texture2D tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[32 * 32];
        
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                // 오른쪽을 가리키는 삼각형 (Base at x=0, Tip at x=1)
                float normalizedX = x / 32f;
                float normalizedY = y / 32f;
                
                // 삼각형 내부인지 확인
                if (Mathf.Abs(normalizedY - 0.5f) <= 0.5f * (1.0f - normalizedX))
                {
                    pixels[y * 32 + x] = Color.white;
                }
                else
                {
                    pixels[y * 32 + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }
    
    void OnDestroy()
    {
        // 스프라이트를 공유하므로 여기서 파괴하지 않음
    }
}
