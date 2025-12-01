using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 1교시부터 4교시까지의 과목 이름을 저장 (인덱스 0: 1교시, 1: 2교시...)
    public string[] schedule = new string[4];

    [Header("Player Stats")]
    public int understanding = 10;  // 이해도
    public int accuracy = 20;       // 정확도
    public int logic = 10;          // 논리력
    public int concentration = 10;  // 집중력
    public int stress = 10;         // 스트레스
    public int confidence = 10;     // 자신감
    public int condition = 4;       // 컨디션

    [Header("Game Progress")]
    public int currentCycle = 1;    // 현재 사이클 (1~16)

    // 하루 성공/실패 카운트
    [HideInInspector] public int dailySuccessCount = 0;
    [HideInInspector] public int dailyFailCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveSchedule(string p1, string p2, string p3, string p4)
    {
        schedule[0] = p1;
        schedule[1] = p2;
        schedule[2] = p3;
        schedule[3] = p4;

        // 하루 시작 시 카운트 초기화
        dailySuccessCount = 0;
        dailyFailCount = 0;

        Debug.Log($"스케줄 저장 완료: {p1}, {p2}, {p3}, {p4}");
    }

    // 배열 게임 성공
    public void ApplyArraySuccess()
    {
        understanding += 2;
        accuracy += 3;
        logic += 1;
        concentration += 1;
        confidence += 1;
        stress -= 1;
        dailySuccessCount++;
        Debug.Log("배열 게임 성공! 스탯 적용됨.");
    }

    // 배열 게임 실패
    public void ApplyArrayFail()
    {
        accuracy -= 2;
        concentration -= 1;
        confidence -= 2;
        stress += 2;
        dailyFailCount++;
        Debug.Log("배열 게임 실패. 스탯 감소.");
    }

    // 연결 리스트 게임 성공
    public void ApplyLinkedListSuccess()
    {
        understanding += 3;
        logic += 2;
        accuracy += 1;
        concentration += 2;
        confidence += 1;
        stress -= 1;
        dailySuccessCount++;
        Debug.Log("연결 리스트 게임 성공! 스탯 적용됨.");
    }

    // 연결 리스트 게임 실패
    public void ApplyLinkedListFail()
    {
        understanding -= 1;
        logic -= 2;
        concentration -= 1;
        confidence -= 2;
        stress += 2;
        dailyFailCount++;
        Debug.Log("연결 리스트 게임 실패. 스탯 감소.");
    }

    // 원형 리스트 게임 성공
    public void ApplyCircleListSuccess()
    {
        understanding += 2;
        logic += 2;
        accuracy += 1;
        concentration += 2;
        confidence += 1;
        stress -= 1;
        dailySuccessCount++;
        Debug.Log("원형 리스트 게임 성공! 스탯 적용됨.");
    }

    // 원형 리스트 게임 실패
    public void ApplyCircleListFail()
    {
        logic -= 2;
        concentration -= 1;
        confidence -= 1;
        stress += 1;
        dailyFailCount++;
        Debug.Log("원형 리스트 게임 실패. 스탯 감소.");
    }

    // 원형 연결 리스트 게임 성공
    public void ApplyCircleLinkedListSuccess()
    {
        understanding += 3;
        logic += 3;
        accuracy += 1;
        concentration += 2;
        confidence += 1;
        stress -= 2;
        dailySuccessCount++;
        Debug.Log("원형 연결 리스트 게임 성공! 스탯 적용됨.");
    }

    // 원형 연결 리스트 게임 실패
    public void ApplyCircleLinkedListFail()
    {
        understanding -= 1;
        logic -= 2;
        concentration -= 1;
        confidence -= 2;
        stress += 2;
        dailyFailCount++;
        Debug.Log("원형 연결 리스트 게임 실패. 스탯 감소.");
    }

    // 스택 게임 성공
    public void ApplyStackSuccess()
    {
        accuracy += 3;
        understanding += 1;
        concentration += 2;
        confidence += 1;
        stress -= 1;
        dailySuccessCount++;
        Debug.Log("스택 게임 성공! 스탯 적용됨.");
    }

    // 스택 게임 실패
    public void ApplyStackFail()
    {
        accuracy -= 2;
        concentration -= 2;
        confidence -= 1;
        stress += 1;
        dailyFailCount++;
        Debug.Log("스택 게임 실패. 스탯 감소.");
    }

    // 큐 게임 성공
    public void ApplyQueueSuccess()
    {
        understanding += 2;
        accuracy += 2;
        concentration += 1;
        confidence += 1;
        stress -= 1;
        dailySuccessCount++;
        Debug.Log("큐 게임 성공! 스탯 적용됨.");
    }

    // 큐 게임 실패
    public void ApplyQueueFail()
    {
        accuracy -= 2;
        concentration -= 1;
        confidence -= 1;
        stress += 1;
        dailyFailCount++;
        Debug.Log("큐 게임 실패. 스탯 감소.");
    }

    // 하루(4교시) 종료 시 호출
    public void ApplyDailyEnd()
    {
        // 컨디션 자동 감소
        condition -= 1;
        if (stress >= 60)
        {
            condition -= 1;
        }

        // 스트레스 해소/증가
        if (dailySuccessCount >= 3)
        {
            stress -= 2;
        }
        if (dailyFailCount >= 3)
        {
            stress += 2;
        }

        Debug.Log($"하루 종료. 컨디션: {condition}, 스트레스: {stress}");
    }
}
