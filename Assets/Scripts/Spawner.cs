using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    // 적 생성 위치들 (스포너 + 스폰포인트들)
    public Transform[] spawnPoints;
    // 누적 시간 변수
    private float timer;

    private void Awake()
    {
        // 스포너 자신 + 자식트랜스폼을 한번에 가져옴
        // GetComponentsInChildren's' 복수형 주의
        spawnPoints = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        // 매 프레임마다 경과 시간 누적
        timer += Time.deltaTime;
        
        // 0.2초마다 함수 호출
        if (timer > 0.2f)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        // 게임 매니저를 통해서 풀에서 적을 꺼냄
        GameObject enemy = GameManager.instance.pool.Get(0);
        
        // 스폰 포인트 중 하나에 배치(랜덤으로, point 들에서만 시작되도록 0 제외)
        enemy.transform.position = spawnPoints[Random.Range(1, spawnPoints.Length)].position;
    }
}
