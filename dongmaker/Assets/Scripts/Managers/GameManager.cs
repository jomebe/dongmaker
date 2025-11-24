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

        Debug.Log($"스케줄 저장 완료: {p1}, {p2}, {p3}, {p4}");
    }
}
