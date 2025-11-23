using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scene Management")]
    public string sceneToLoad;

    [Header("Target Graphic (Optional)")]
    public Graphic targetGraphic;

    [Header("Scale Settings")]
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.0f);
    public Vector3 clickScale = new Vector3(0.9f, 0.9f, 1.0f);
    public float smoothSpeed = 15f;

    [Header("Color Settings")]
    public Color hoverColor = new Color(0.9f, 0.9f, 0.9f, 1f); // Light Gray
    public Color clickColor = new Color(0.6f, 0.6f, 0.6f, 1f); // Dark Gray

    private Vector3 defaultScale;
    private Color defaultColor;

    private Vector3 targetScale;
    private Color targetColor;

    void Start()
    {
        defaultScale = transform.localScale;
        targetScale = defaultScale;

        if (targetGraphic == null)
            targetGraphic = GetComponent<Graphic>();

        if (targetGraphic != null)
        {
            defaultColor = targetGraphic.color;
            targetColor = defaultColor;
        }
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);

        if (targetGraphic != null)
        {
            targetGraphic.color = Color.Lerp(targetGraphic.color, targetColor, Time.deltaTime * smoothSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = Vector3.Scale(defaultScale, hoverScale);
        if (targetGraphic != null) targetColor = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = defaultScale;
        if (targetGraphic != null) targetColor = defaultColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = Vector3.Scale(defaultScale, clickScale);
        if (targetGraphic != null) targetColor = clickColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.hovered.Contains(gameObject))
        {
            targetScale = Vector3.Scale(defaultScale, hoverScale);
            if (targetGraphic != null) targetColor = hoverColor;

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            targetScale = defaultScale;
            if (targetGraphic != null) targetColor = defaultColor;
        }
    }
}
