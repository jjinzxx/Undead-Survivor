using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Game Objects")]
    public Player player;
    public PoolManager pool;
    public LevelUp uiLevelUp;       // 레벨업 선택 UI
    public GameObject uiGameOver;   // 게임 오버/승리 결과창

    [Header("Play Time")] 
    public bool isLive;         // 일시정지용
    public float gameTime;      // 흐르는 게임 시간 - 난이도 계산 용도/서바이벌 타임 계산 용도
    public float maxGameTime;   // 최대 게임 시간 - 난이도 증가 기준
    
    [Header("Game Player Data")]
    public int level;
    public int kill;
    public int exp;
    public List<int> nextExp = new List<int> { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 }; // 동적 배열을 위해 일반 배열 대신 리스트 자료구조 변경
    public float health;
    public float maxHealth = 100;

    private void Awake()
    {
        instance = this;
    }

    // 기본적으로 아무것도 안쓰면 private.
    // public으로 바꾸는 이유는 인스펙터에 나타나고, OnClick Event 목록에 보여주기 위함.
    public void GameStart()
    {
        Resume();
        health = maxHealth;         // 게임 시작 시 체력을 최대 체력으로 초기화
        uiLevelUp.Select(0);   // 기본무기 (0: 삽) 제공
    }

    // 사망 시 호출
    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);  // 묘비 애니메이션이 나타날 시간 확보
        uiGameOver.SetActive(true);             // 결과창 켜기
        Stop();                                 // 시간 정지
    }

    // Back 버튼이 호출
    public void BackMenu()
    {
        SceneManager.LoadScene(0); // 0번에 등록된 씬 롣,
    }

    private void Update()
    {
        if (!isLive) return; // 일시정지 상태에서는 시간 누적 중단
        
        // 매 프레임마다 실제 흐른 시간을 누적
        gameTime += Time.deltaTime;
        
        // 최대 시간을 넘지 않도록 고정 (게임 종료 등 처리에 활용)
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }
        
    }
    
    // 경험치 획득 및 레벨업 로직
    public void GetExp()
    {
        exp++;
        if (exp == nextExp[level])
        {
            level++;
            exp = 0;
            uiLevelUp.Show(); // 레벨업 -> 아이템 선택 UI 표시
            
            // 초기 레벨 이상 초과시, 경험치 테이블을 추가하면서 최대 경험치를 복사
            if (level >= nextExp.Count)
            {
                nextExp.Add(nextExp[nextExp.Count-1]);
            }
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0; // 시간 흐름 비율, 속도 = 0으로
    }
    
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
}
