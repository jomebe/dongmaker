using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image endingImage;      // 엔딩 이미지
    public Text titleText;         // 엔딩 제목
    public Text descriptionText;   // 엔딩 설명

    [Header("Ending Images (1~8)")]
    public Sprite ending1Sprite;   // 자료구조 괴물 3학년 동
    public Sprite ending2Sprite;   // 알고리즘/대회 준비반 합류 동
    public Sprite ending3Sprite;   // 연구동아리 스카웃 동
    public Sprite ending4Sprite;   // 실전형 빌더 3학년 동
    public Sprite ending5Sprite;   // 열정 과다, 이론은 살짝 부족한 동
    public Sprite ending6Sprite;   // 번아웃 직전 3학년 동
    public Sprite ending7Sprite;   // 평균적인 3학년 동
    public Sprite ending8Sprite;   // 간신히 버티고 올라간 3학년 동

    private int endingNumber = 7;  // 기본값: 노말 엔딩

    void Start()
    {
        DetermineEnding();
        ShowEnding();
    }

    void DetermineEnding()
    {
        if (GameManager.Instance == null)
        {
            endingNumber = 7;
            return;
        }

        int understanding = GameManager.Instance.understanding;
        int accuracy = GameManager.Instance.accuracy;
        int logic = GameManager.Instance.logic;
        int concentration = GameManager.Instance.concentration;
        int stress = GameManager.Instance.stress;
        int confidence = GameManager.Instance.confidence;
        int condition = GameManager.Instance.condition;

        // 우선순위 0: 번아웃 엔딩 (최우선 체크)
        if (stress >= 85 && condition <= 2)
        {
            endingNumber = 6;
            return;
        }

        // 우선순위 1: 자료구조 괴물 (S엔딩)
        if (understanding >= 85 && logic >= 80 && accuracy >= 75 && stress <= 50 && condition >= 3)
        {
            endingNumber = 1;
            return;
        }

        // 우선순위 2: 알고리즘/대회 준비반
        if (understanding >= 80 && logic >= 75 && concentration >= 75 && accuracy >= 70 && stress <= 60)
        {
            endingNumber = 2;
            return;
        }

        // 우선순위 3: 연구동아리 스카웃
        if (understanding >= 75 && logic >= 75 && concentration >= 65 && confidence >= 60 && stress <= 55)
        {
            endingNumber = 3;
            return;
        }

        // 우선순위 4: 실전형 빌더
        if (accuracy >= 75 && concentration >= 70 && understanding >= 60 && logic >= 55 && stress <= 70)
        {
            endingNumber = 4;
            return;
        }

        // 우선순위 5: 열정 과다
        if (confidence >= 75 && understanding < 70 && logic < 70 && stress < 80)
        {
            endingNumber = 5;
            return;
        }

        // 우선순위 7: 간신히 버티고 올라감 (로우 엔딩)
        int lowStatCount = 0;
        if (understanding <= 40) lowStatCount++;
        if (logic <= 40) lowStatCount++;
        if (accuracy <= 40) lowStatCount++;
        if (concentration <= 40) lowStatCount++;
        if (confidence <= 40) lowStatCount++;

        if (lowStatCount >= 3 && stress < 80)
        {
            endingNumber = 8;
            return;
        }

        // 우선순위 6: 평균적인 동 (노말 엔딩)
        // 위 조건에 모두 해당하지 않으면 노말 엔딩
        endingNumber = 7;
    }

    void ShowEnding()
    {
        switch (endingNumber)
        {
            case 1:
                SetEnding(ending1Sprite, 
                    "자료구조 괴물 3학년 동",
                    "동은 3학년이 되자마자 고급 알고리즘, 대회 준비반에 자연스럽게 끌려 들어갔다.\n모두가 인정하는 '자료구조 괴물'로 성장했다.");
                break;

            case 2:
                SetEnding(ending2Sprite,
                    "알고리즘/대회 준비반 합류 동",
                    "동은 3학년에서 알고리즘 대회 준비반에 들어가\n더 어려운 문제들에 도전하기로 했다.");
                break;

            case 3:
                SetEnding(ending3Sprite,
                    "연구동아리 스카웃 동",
                    "동은 3학년에서 심화 프로젝트와 연구를 하는 동아리에 스카웃되었다.\n'재밌는 걸 더 깊게 파보고 싶다'는 생각이 커졌다.");
                break;

            case 4:
                SetEnding(ending4Sprite,
                    "실전형 빌더 3학년 동",
                    "동은 3학년에서 각종 프로젝트와 실습에 적극적으로 참여하며\n'만들면서 배우는 개발자'를 꿈꾸기 시작했다.");
                break;

            case 5:
                SetEnding(ending5Sprite,
                    "열정 과다, 이론은 살짝 부족한 동",
                    "동은 3학년으로 올라가면서\n\"아직 완벽하진 않지만, 더 해볼 수 있을 것 같다\"는 마음으로\n계속 도전하기로 했다.");
                break;

            case 6:
                SetEnding(ending6Sprite,
                    "번아웃 직전 3학년 동",
                    "동은 겨우겨우 3학년이 되었다.\n하지만 마음 한켠에는 \"이대로 계속 버틸 수 있을까?\"라는 불안이 남아 있다.");
                break;

            case 7:
                SetEnding(ending7Sprite,
                    "평균적인 3학년 동",
                    "동은 크게 눈에 띄는 일은 없었지만,\n무난하게 3학년으로 올라갔다.\n평범하지만 나쁘지 않은 1년이었다.");
                break;

            case 8:
                SetEnding(ending8Sprite,
                    "간신히 버티고 올라간 3학년 동",
                    "동은 여러 번 흔들리면서도 어떻게든 2학년을 마쳤다.\n3학년에서는 조금은 다르게 해볼 수 있을까, 조용히 다짐해본다.");
                break;
        }
    }

    void SetEnding(Sprite sprite, string title, string description)
    {
        if (endingImage != null && sprite != null)
            endingImage.sprite = sprite;

        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;
    }

    // 클릭 시 타이틀로 돌아가기
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            ReturnToTitle();
        }
    }

    public void ReturnToTitle()
    {
        // GameManager 초기화 (새 게임을 위해)
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }
        
        SceneManager.LoadScene("Menu");
    }
}
