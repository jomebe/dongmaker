using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SubjectConnection
{
    public string fromSubject;
    public string toSubject;
    
    public SubjectConnection(string from, string to)
    {
        fromSubject = from;
        toSubject = to;
    }
}

public class CircleList : MonoBehaviour
{
    public List<SubjectConnection> connections = new List<SubjectConnection>();
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
    
    // 연결 추가
    public void AddConnection(string from, string to)
    {
        // 중복 체크
        foreach (var conn in connections)
        {
            if (conn.fromSubject == from && conn.toSubject == to)
            {
                return;
            }
        }
        
        connections.Add(new SubjectConnection(from, to));
        Debug.Log($"연결 추가: {from} -> {to}");
        PrintAllConnections();
    }
    
    // 연결 제거
    public void RemoveConnection(string from, string to)
    {
        connections.RemoveAll(conn => conn.fromSubject == from && conn.toSubject == to);
    }
    
    // 모든 연결 출력
    public void PrintAllConnections()
    {
        Debug.Log("=== 현재 연결 목록 ===");
        foreach (var conn in connections)
        {
            Debug.Log($"{conn.fromSubject} -> {conn.toSubject}");
        }
    }
    
    // 모든 연결 초기화
    public void ClearAllConnections()
    {
        connections.Clear();
        Debug.Log("모든 연결이 제거되었습니다.");
    }
}
