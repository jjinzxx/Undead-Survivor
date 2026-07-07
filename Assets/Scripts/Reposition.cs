using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Reposition : MonoBehaviour
{
    // 적 생존 여부 판단용 (죽은 적은 콜리전을 끄기 위함)
    private Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }
    // OnTriggerExit2D 트리거 콜라이더에서 벗어났을 때, 호출되는 함수
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어 Area 콜라이더가 벗어난 경우에만 실행하기 위한 조건
        if (!collision.CompareTag("Area"))
        {
            return;
        }
        
        // 플레이어 위치와 내(타일맵)의 위치를 저장
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;
        
        // 태그에 따라 재배치 방식을 분기
        switch (transform.tag)
        {
            case "Ground":
                // input vector가 아닌, 실제 위치 차이로 방향을 판단
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;
                float dirX = Mathf.Sign(diffX);
                float dirY = Mathf.Sign(diffY);
                
                // 더 많이 멀어진 축으로 타일맵을 이동
                if(Mathf.Abs(diffX) > Mathf.Abs(diffY))
                    transform.Translate(Vector3.right*dirX*40);
                else
                    transform.Translate(Vector3.left*dirY*40);
                
                break;
            case "Enemy":
                // 살아있는 적만 재배치
                if (coll.enabled)
                {
                    // input vector가 아닌, 실제 위치 차이로 방향을 판단
                    Vector3 distVec = playerPos - myPos;
                    Vector3 ranVec = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
                    
                    // 위치 차이 * 2로 플레이어 앞쪽에 랜덤 배치
                    transform.Translate(distVec * 2 + ranVec);
                }
                break;
            
        }
    }
}
