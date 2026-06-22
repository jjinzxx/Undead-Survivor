using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 적(Enemy) 이동 속도 
    public float speed;
    // 현재 체력, 최대 체력
    public float health;
    public float maxHealth;
    
    // 몬스터 종류별 애니메이션 컨트롤러 배열 (Init 함수에서 spriteType으로 갈아끼움)
    public RuntimeAnimatorController[] animCon;
    
    // 애니메이터 - 몬스터 종류 전환
    Animator anim;
    
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
        anim = GetComponent<Animator>();
    }

    // OnEnable(): Enemy 객체가 활성화 될 때마다 호출
    private void OnEnable()
    {
        // 프리펩은 Player를 참조(할당) 하지 못하므로
        // GameManager를 통해서 매번 플레이어를 target으로 할당
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        
        // 풀에서 다시 사용될 때(죽음->리스폰)
        isAlive = true;
        health = maxHealth;
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

    void LateUpdate()
    {
        sr.flipX = target.position.x < rigid.position.x;
    }

    // 난이도 데이터를 파라미터로 전달받아 적 객체 초기화
    public void Init(SpawnData sd)
    {
        anim.runtimeAnimatorController = animCon[sd.spriteType];
        speed = sd.speed;
        maxHealth = sd.health;
        health = sd.health;
    }

}
