using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 방향 키 입력 받는 벡터 (x = 좌우, y = 상하)
    public Vector2 inputVec;
    // 이동속도
    public float speed;
    
    // 객체 물리 컴포넌트 
    Rigidbody2D rb;
    // 스프라이트 렌더러 컴포넌트
    SpriteRenderer sr;
    // 애니메이션 상태 제어 컴포넌트
    Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    
    // FixedUpdate: 물리 이동은 이곳에서
    private void FixedUpdate()
    {
        // 다음에 이동할 양 = 방향 * 속도 * 프레임 시간
        Vector2 nextVec = inputVec.normalized * (speed * Time.fixedDeltaTime);
        // 현재 위치 + 이동량
        rb.MovePosition(rb.position + nextVec);
    }
    
    // LateUpdate: 모든 업데이트가 끝난 뒤 실행. 방향 반전같은 것은 후처리에 적합
    private void LateUpdate()
    {
        if (inputVec.x != 0)
        {
            sr.flipX = inputVec.x < 0;
        }
        
        // 애니메이션도 후처리
        anim.SetFloat("Speed", Mathf.Abs(inputVec.x));
        //anim.SetBool("Speed", inputVec.magnitude); // 입력 벡터의 크기(magnitude)를 애니메이터의 변수로 전달.
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), inputVec.ToString());
        
    }
}
