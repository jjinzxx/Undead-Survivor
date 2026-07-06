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
    public GameResult uiResult;   // 게임 오버/승리 결과창
    public GameObject enemyCleaner; // 승리시 남은 몬스터를 일괄 제거하기 위함(KillZone)

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
        
        AudioManager.instance.PlayBgm(true); // 배경음 시작
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    // 사망 시 호출
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);  // 묘비 애니메이션이 나타날 시간 확보
        uiResult.gameObject.SetActive(true);    // 결과창 켜기
        uiResult.GameOver();
        Stop();                                 // 시간 정지
        
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose); // 패배 효과음
        AudioManager.instance.PlayBgm(false); // 배경음 정지
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }
    
    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.transform.position = player.transform.position; // 킬존을 플레이어 위치로 옮기기
        enemyCleaner.SetActive(true);           // 몬스터 일괄 제거
        yield return new WaitForSeconds(0.5f);  // 처치 애니메이션 볼 시간 확보
        uiResult.gameObject.SetActive(true);    // 결과 창 보기
        uiResult.GameVictory();                 //
        Stop();                                 // 시간 정지
        
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win); // 패배 효과음
        AudioManager.instance.PlayBgm(false); // 배경음 정지
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
            GameVictory();
        }
        
    }
    
    // 경험치 획득 및 레벨업 로직
    public void GetExp()
    {
        if(!isLive) return;
        
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
