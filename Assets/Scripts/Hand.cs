using System;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;             // 왼손 여부 (왼손=근접 / 오른손=원거리)
    public SpriteRenderer spriter;  // 이 손의 스프라이트
    private SpriteRenderer player;  // 플레이어 스프라이트(반전 여부 flipX 확인용)
    
    // 오른손(원거리)는 위치로 표현 = 정상/반전 두가지
    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0);
    Vector3 rightPosReverse = new Vector3(-0.15f, -0.15f, 0);
    
    // 왼손(근접)은 회전으로 표현 - 정상/반전 두가지
    Quaternion leftRot = Quaternion.Euler(0, 0, -35);
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -130);

    private void Awake()
    {
        // 부모 방향으로 SpriteRenderer를 가져오면 [0]=내손, [1]=부모(플레이어)
        // 플레이어의 반전 상태를 가져와야 함
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }

    private void LateUpdate()
    {
        // 플레이어 캐릭터의 이동, 반전 처리가 끝나면 손의 위치, 회전을 보정
        bool isReverse = player.flipX; // 왼쪽=true

        if (isLeft)
        {
            // 왼손 = 근접 무기;
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            spriter.flipX = isReverse; // 플레이어 반전에 따라서 y축 반전
            spriter.sortingOrder = isReverse ? 9 : 11; // 화면에 보이는 우선순위(반전상태 캐릭터뒤(9), 정상시(11) / 기본 캐릭터(10))
        }
        else
        {
            // 오른손 = 원거리 무기: 위치로 자세 표현
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            spriter.flipX = isReverse;
            spriter.sortingOrder = isReverse ? 11 : 9;
        }
    }
}
