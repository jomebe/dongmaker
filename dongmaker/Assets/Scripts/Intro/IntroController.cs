using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [Header("UI Components")]
    public Text titleText;
    public RectTransform skipTextRect;

    [Header("Dialogues")]
    [TextArea(3, 10)]
    public string[] dialogues = new string[]
    {
        "동은 선린인터넷고 2학년이 되었고",
        "당신은 동을 도와 학교 생활을 잘 보낼 수 있도록 지도하게 됩니다.",
        "앞으로 잘 부탁드립니다!"
    };

    [Header("Dialogue Settings")]
    public Vector2[] dialogueSizes = new Vector2[]
    {
        new Vector2(1291, 115),
        new Vector2(1306, 230),
        new Vector2(909, 115)
    };

    public Vector2[] dialoguePositions = new Vector2[]
    {
        new Vector2(314, -482),
        new Vector2(307, -369),
        new Vector2(505, -482)
    };

    public Vector2[] skipPositions = new Vector2[]
    {
        new Vector2(786, -650),
        new Vector2(786, -651),
        new Vector2(786, -650)
    };

    private int currentIndex = 0;

    void Start()
    {
        // 첫 번째 대사 출력
        if (dialogues.Length > 0)
        {
            UpdateText();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextDialogue();
        }
    }

    public void NextDialogue()
    {
        currentIndex++;
        if (currentIndex < dialogues.Length)
        {
            UpdateText();
        }
        else
        {
            OnIntroFinished();
        }
    }

    void UpdateText()
    {
        if (titleText != null)
        {
            titleText.text = dialogues[currentIndex];

            if (currentIndex < dialogueSizes.Length)
            {
                titleText.rectTransform.sizeDelta = dialogueSizes[currentIndex];
            }

            if (currentIndex < dialoguePositions.Length)
            {
                titleText.rectTransform.anchoredPosition = dialoguePositions[currentIndex];
            }
        }

        if (skipTextRect != null && currentIndex < skipPositions.Length)
        {
            skipTextRect.anchoredPosition = skipPositions[currentIndex];
        }
    }

    void OnIntroFinished()
    {
        Debug.Log("인트로 종료");
        SceneManager.LoadScene("GameMenu");
    }
}
