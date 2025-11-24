using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeskSlot : MonoBehaviour, IDropHandler
{
    public Block currentBlock;

    void Start()
    {
        // 1. Image 컴포넌트 확인 및 추가
        Image img = GetComponent<Image>();
        if (img == null)
        {
            img = gameObject.AddComponent<Image>();
            // 디버깅을 위해 잠시 붉은색 반투명으로 설정합니다. 
            // 문제가 해결되면 new Color(0, 0, 0, 0)으로 투명하게 바꾸세요.
            img.color = new Color(1, 0, 0, 0.3f); 
        }
        img.raycastTarget = true;

        // 2. 크기가 0인지 확인하고 강제로 키움 (빈 오브젝트일 경우 클릭 영역이 없으므로)
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            if (rt.rect.width < 10 || rt.rect.height < 10)
            {
                Debug.LogWarning($"{name}의 크기가 너무 작아서 강제로 100x100으로 설정합니다.");
                rt.sizeDelta = new Vector2(100, 100);
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"{name}에 드롭 감지됨!");

        if (eventData.pointerDrag != null)
        {
            Block droppedBlock = eventData.pointerDrag.GetComponent<Block>();
            if (droppedBlock != null)
            {
                Debug.Log($"블록 {droppedBlock.name}이 {name}에 장착됨");

                // 이미 슬롯에 블록이 있다면 원래 위치(선반)로 돌려보냄
                if (currentBlock != null)
                {
                    currentBlock.ReturnToShelf();
                }

                currentBlock = droppedBlock;
                currentBlock.SetParent(transform);
            }
        }
    }
}
