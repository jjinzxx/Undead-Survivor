using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;          // 무기 종류 (0=근접, 1=원거리)
    public int prefavId;    // 풀 매니저에 등록된 무기 인덱스(어떤 무기 외형일지)
    public float damage;    // 데미지
    public int count;       // 근접 = 칼날 개수 / 원거리 = 관통 횟수
    public float speed;     // 근접 = 회전 속도 / 원거리 = 연사 속도
    
    private float timer;     // 원거리 발사 타이머
    private Player player;   // player.Scanner 대상에 접근하기 위함

    private void Awake()
    {
        // 부모 객체의 자식 컴포넌트들을 모두 가져오기 위해서
        player = GetComponentInParent<Player>();
    }

    // 메모리에 올라 갈 때 처음 1회 자동 호출
    void Start()
    {
        // 무기 인스턴스(Weapon 클래스)가 만들어질 때 자동 초기화
        Init();
    }

    // Update()는 매 프레임당 호출되는 함수 - 입력, 회전 등 일반 로직
    // FixedUpadate()는 프레임당 호출되는건 같음, but 물리 갱신 프레임당 호출 - Rigdbody 물리 이동 전용
    // LateUpdate() 업데이트 들이 모두 실행 된 후 호출 - 카메라, 방향 보정 등 후처리
    void Update()
    {
        // 무기 종류별 동작 분기
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.forward * (speed * Time.deltaTime));
                break;
            case 1:
                // 타이머가 연사속도(speed)를 넘으면 발사
                timer += Time.deltaTime;
                if (timer >= speed)
                {
                    timer = 0;
                    Fire();
                }
                break;
            default:
                break;
        }
    }

    public void Init()
    {
        // 무기 종류별 초기 세팅
        switch (id)
        {
            case 0:
                speed = -150; // (음수 = 시계방향)
                Arrange(); // 칼날 원형 배치
                break;
            case 1:
                speed = 0.3f; // 연사속도 (0.3초마다 발사)
                break;
            default:
                break;
        }
    }
    
    // 칼날을 풀에서 꺼내서 플레이어 주위에 원형으로 균등 배치
    void Arrange()
    {
        // 칼날 count개를 풀에서 꺼냄
        for (int idx = 0; idx < count; idx++)
        {
            // 풀에서 총알(Bullet)을 꺼내서, Transform을 확보
            Transform bullet = GameManager.instance.pool.Get(prefavId).transform;
            // Weapon의 자식으로 설정
            bullet.parent = transform;
            // 부모 기준 위치, 회전 초기화(재사용 사 이전 값을 제거)
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;
            
            // idx 마다 균등 각도로 회전 (360/count 간격)
            Vector3 rotVec = Vector3.forward * 360 *idx / count;
            bullet.Rotate(rotVec);
            
            // 회전된 위 방향으로 1.5 바깥으로 위치(월드 기준)
            bullet.Translate(bullet.up * 1.5f, Space.World);
            
            bullet.GetComponent<Bullet>().Init(damage, -1);
        }
    }

    // 가장 가까운 적을 향해서 총을 1개 발사
    void Fire()
    {
        // 조준 대상이 없는 경우 필터링
        if(player.scanner.nearestTarget == null) return;
        // 대상 방향을 계산
        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        
        // 풀 매니저에서 총알을 꺼내 위치, 회전 세팅(대상을 바라보도록)
        Transform bullet = GameManager.instance.pool.Get(prefavId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); // A파라미터의 방향을 B방향으로 최전값 -> 총알의 up위쪽 방향이 적을 향하게 함
        
        // Init 값 주입
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }
}
