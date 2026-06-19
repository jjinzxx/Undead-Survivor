using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 적(Enemy) 이동 속도 
    public float speed;
    
    // 추적할 대상(Player) 물리 컴포넌트 target
    public Rigidbody2D target;
    
    // 적(Enemy)의 생존여부
    bool isAlive;
    
    // 적(Enemy) 물리 컴포넌트 
    Rigidbody2D rigid;
    
    // flipX을 통한 적 좌우 반전용
    SpriteRenderer sr;

    void Awake()
    {
        // 미리 컴포넌트들을 로드하여 메모리에 캐싱
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // 1. 방향 구하기 (목표위치 - 내 위치) -> 플레이어 쪽을 바라보는 벡터
        Vector2 dirVec = target.position - rigid.position;
        
        // 2. 이동량 구하기 (방향 * 속도 * 프레임 시간)
        // normalized 하는 이유는 거리랑 상관없이 일정한 속도를 위함.
        Vector2 nextVec =  dirVec.normalized * (speed * Time.fixedDeltaTime);
        
        // 3. 객체 이동 (현재 위치 + 이동량)
        rigid.MovePosition(rigid.position + nextVec);
        
        // 4. 잔여 속도 제거(관성 제거), 플레이어와 충돌하면 물리 속도를 0으로
        rigid.linearVelocity = Vector2.zero;
        
    }

}
