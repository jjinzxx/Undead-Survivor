using System;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;   // 이 버튼이 담당할 아이템 데이터
    public int level;       // 현재 아이템 강화 레벨
    public Weapon weapon;   // 이 아이템이 만들거나 관리하고 있는 무기

    private Image icon; // 버튼에 표시할 아이템 아이콘
    Text textLevel;     // 버튼에 표시할 아이템 레벨

    private void Awake()
    {
        // [0] = 버튼 자기 자신(버튼도 이미지라서) / [1] = 자식 아이콘
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon; // 데이터의 아이콘 그림으로 세팅
        
        // 텍스트는 버튼이 하나뿐이라서 [0]
        textLevel = GetComponentsInChildren<Text>()[0];
    }

    private void LateUpdate()
    {
        textLevel.text = "Lv." + (level+1).ToString();
    }
    
    // OnClick Event
    public void OnClick()
    {
        // 아이템 종류별로 분기
        switch (data.itemType)
        {
            // 근접&원거리는 같은 무기 로직으로 처리
            case ItemData.ItemType.Malee:
            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    // 첫 선택: WeaponCmponent를 붙여 무기를 생성해줘야 한다.
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    // 재선택: 다음 레벨 수치 계산후 무기에 반영(강화)
                    // 강화된 데미지 = 기본값 + (기본값 * 레벨별 증가율)
                    float nextDamage = data.baseDamage;
                    nextDamage += data.baseDamage * data.damages[level];
                    
                    // 갯수 = 레벨별 증가
                    int nextCount = 0;
                    nextCount += data.counts[level];
                    
                    weapon.LevelUP(nextDamage, nextCount);
                }
                break;
            case ItemData.ItemType.Glove:
                break;
            case ItemData.ItemType.Shoe:
                break;
            case ItemData.ItemType.Heal:
                break;

        }

        level++; // UI에 표기되고 있는 아이템 레벨 증가
        // 최대 레벨에 도달 시 (레벨 == 데이터 배열의 최대 길이) -> 더 못올리도록 비활성화

        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
