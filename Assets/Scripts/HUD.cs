using System;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType
    {
        Exp,
        Level,
        Kill,
        Time,
        Health
    }
    public InfoType type; // 인스펙터에서 HUD 스크립트 컴포넌트가 어떤 정보를 처리할지 지정
    
    Text myText;
    Slider mySlider;

    private void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                // 슬라이더 값 = 현재 경험치 / 목표 경험치 (0~1)
                float curExp = GameManager.instance.exp;
                // 무한 레벨업으로 레벨이 배열 길이를 넘는 경우, 인덱스 초과 IndexOutOfRange 예외가 발생 할 수 있음
                // Mathf.Min 으로 마지막 인덱스를 고정하기 위함 -> level이 배열 범위 안이면 그대로, 초과하면 마지막 인덱스를 사용.
                int expIndex = Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length - 1);
                float maxExp = GameManager.instance.nextExp[expIndex];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                // string.Format: {0:F0} -> 변수의 값을 0인자에 소수점 없는 숫자로 대입
                myText.text = string.Format("Lv. {0:F0}", GameManager.instance.level);
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                // 남은 시간 = 최대 시간 - 경과 시간
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                // 소수점 버림(정수화) Mathf.FloorToInt(분단위)
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec); // 00:00 으로 나타내기 위한 포맷
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
    
}
