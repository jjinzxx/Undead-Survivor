using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;          // 무기 종류 (0=근접, 1=원거리)
    public int prefavId;    // 풀 매니저에 등록된 무기 인덱스(어떤 무기 외형일지)
    public float damage;    // 데미지
    public int count;       // 칼날 개수
    public float speed;     // 회전 속도
    
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
}
