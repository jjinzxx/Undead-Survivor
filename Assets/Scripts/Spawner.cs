using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    // 적 생성 위치들 (스포너 + 스폰포인트들)
    public Transform[] spawnPoints;
    // 난이도별 스폰 설정 데이터
    public SpawnData[] spawnData;
    
    // 누적 시간 변수
    private float timer;
    // 현재 난이도 (게임 시간에 따라 변함)
    private int difficulty;
    
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
        
        // 난이도 상승
        difficulty = (int)(GameManager.instance.gameTime / 10f);
        
        // 난이도가 데이터 개수를 넘지 않도록 제한 (배열범위 초과 방지)
        if (difficulty >= spawnData.Length)
        {
            difficulty = spawnData.Length - 1;
        }
        // 현재 난이도의 스폰 간격을 넘으면 스폰 호출
        if (timer > spawnData[difficulty].spawnTime)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        // 현재 난이도 데이터가 지정한 종류(spriteType)의 프리펨을 풀에서 꺼냄
        GameObject enemy = GameManager.instance.pool.Get(0);
        
        // 스폰 포인트 중 하나에 배치(랜덤으로, point 들에서만 시작되도록 0 제외)
        enemy.transform.position = spawnPoints[Random.Range(1, spawnPoints.Length)].position;
        
        // 현재 난이도 데이터로 Enemy 능력치 초기화 (체력, 속도)
        enemy.GetComponent<Enemy>().Init(spawnData[difficulty]);
    }
}
