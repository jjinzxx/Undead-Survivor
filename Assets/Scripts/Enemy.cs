using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 적(Enemy) 이동 속도 
    public float speed;
    // 현재 체력, 최대 체력
    public float health;
    public float maxHealth;
    // 적(Enemy)의 생존여부
    bool isAlive;

    
    // 몬스터 종류별 애니메이션 컨트롤러 배열 (Init 함수에서 spriteType으로 갈아끼움)
    public RuntimeAnimatorController[] animCon;
    // 애니메이터 - 몬스터 종류 전환
    Animator anim;
    
    // 추적할 대상(Player) 물리 컴포넌트 target
    public Rigidbody2D target;
    // 사망 시 충돌 안되도록 충돌 콜라이더 컴포넌트
    private Collider2D coll;
    // 적(Enemy) 물리 컴포넌트 
    Rigidbody2D rigid;
    
    // flipX을 통한 적 좌우 반전용
    SpriteRenderer sr;
    // Coroutine에서 쓸 "물리 프레임 대기"
    private WaitForFixedUpdate wait;

    void Awake()
    {
        // 미리 컴포넌트들을 로드하여 메모리에 캐싱
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
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
        
        // 사망에서 바뀌었던 상태를 모두 원복(재사용위해)
        isAlive = true;
        coll.enabled = true;
        rigid.simulated = true;
        sr.sortingOrder = 9;    
        anim.SetBool("Dead", false);
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive) return; // 일시정지 상태에서는 추적 중단
        
        // 죽은 상태(!isAlive) 히트 상태(넉백 중)인 상태에는 아래 추적 이동을 멈충
        if (!isAlive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;
        
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
        if (!GameManager.instance.isLive) return; // 일시정지 상태에서는 중단
        if(!isAlive) return;
        sr.flipX = target.position.x < rigid.position.x;
    }
    
    // 트리거에 무언가 닿으면 호출
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 총알(Bullet 태그)에 맞은 경우에 처리하기 위한 필터(벽, 플레이어 등 무시)
        // !isAlive 연속 경험치 획득 방지 추가
        if (!collision.CompareTag("Bullet")|| !isAlive) return;
        // 충돌된 Bullet 컴포넌트의 데미지 만큼 체력 감소
        health -= collision.GetComponent<Bullet>().damage;

        if (health > 0)
        {
            // 피격 반응
            anim.SetTrigger("Hit");       // Hit 애니메이션 재생
            StartCoroutine(KnockBack()); // 넉백 코루틴 실행
        }
        else
        {
            // Dead();
            isAlive = false;        // 시체의 이동, 반응 정지(FixedUpate 필터용)
            coll.enabled = false;   // 시체의 충돌 제거
            rigid.simulated = false;// 물리 정지(밀리거나 움직임 정지)
            sr.sortingOrder = 8;    // 정렬을 내림
            anim.SetBool("Dead", true); // 사망 애니메이션 재생을 위한 파라미터 값 전달
            
            GameManager.instance.kill++;
            GameManager.instance.GetExp();
        }
    }

    // 적 사망은 객체 Destroy가 아니라 오브젝트 풀에서 재사용 하기 위해 비활성화 처리
    void Dead()
    {
        gameObject.SetActive(false);
    }

    // 난이도 데이터를 파라미터로 전달받아 적 객체 초기화
    public void Init(SpawnData sd)
    {
        anim.runtimeAnimatorController = animCon[sd.spriteType];
        speed = sd.speed;
        maxHealth = sd.health;
        health = sd.health;
    }

    // KnockBack : 맞는 순간 물리적으로 밀려나는 코루틴
    IEnumerator KnockBack()
    {
        yield return wait; // 다음 물리 프레임까지 1프레임을 대기
        
        // 플레이어 반대 방향으로 충격 = Enemy 위치 - Player 위치
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        
        // 그 방향으로 순간 물리 충격(Impulse)를 가함
        rigid.AddForce(dirVec.normalized * 3,  ForceMode2D.Impulse);
    }
}
