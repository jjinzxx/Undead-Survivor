using System;
using UnityEngine;

// 오디오 매니저 - BGM, SFX 한 곳에서 관리하는 싱글톤
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [Header("BGM")]
    public AudioClip bmgClip;   // 배경음 파일
    public float bgmVolume; // 배경음 볼륨
    AudioSource bgmPlayer;  // 배경음 재생기
    AudioHighPassFilter bgmEffect; // 레벨업 시 먹먹하게 하는 효과
    
    [Header("Sound Effects")]
    public AudioClip[] sfxClips; // 효과음 파일
    public float sfxVolume;      // 효과음 볼륨
    public int channel;          // 효과음 채널 개수(동시 재생용)
    AudioSource[] sfxPlayers;    // 효과음 재생기 배열(채널)
    private int channelIndex;    // 마지막에 사용한 채널 인덱스
    
    // 효과음 종류 - sfxClips 배열 인덱스와 일치시켜두기
    // Hit, Melee는 클립이 2개라 배열 건너 뜀.
    public enum Sfx {Dead, Hit, LevelUp = 3, Lose, Melee, Range=7, Select, Win}

    private void Awake()
    {
        instance = this;
        Init();
    }
    
    private void Init()
    {
        // ===== [ 배경음 ] =====
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BGM Player");
        bgmObject.transform.parent = transform; // 오디오 매니저의 자식으로 등록
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false; // 시작하자마자 재생 X (버튼 누를 때 재생되도록)
        bgmPlayer.loop = true; // 배경음은 반복
        bgmPlayer.clip = bmgClip;
        bgmPlayer.volume = bgmVolume;
        
        // BGM 이펙트(하이패스 필터)는 오디오 리스너가 있는 메인 카메라에서 가져옴
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();
        
        // ===== [ 효과음 ] =====
        // 효과음 플레이어 초기화 (채널 갯수만큼)
        GameObject sfxObject = new GameObject("SFX Player");
        sfxObject.transform.parent = transform; // 오디오 매니저의 자식으로 등록
        sfxPlayers = new AudioSource[channel];  // 채널수에 따라 생성

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true; // 효과음은 BGM 필터 영향 받지 않도록
            // sfxPlayers[index].clip = sfxClips[index]; 아래에서 할당ㅋ
            sfxPlayers[index].volume = sfxVolume;
        }
    }
    
    // 배경음 재생/정지
    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }
    
    // 배경음 먹먹효과 켜기/끄기 (레벨업 창 뜰 때 이펙트)
    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }
    
    // 효과음 재생 - 쉬고 있는 채널을 찾아 재생
    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            // 마지막에 쓴 채널 다음부터 순환 검색 (% 나머지로 배열 범위 안넘도록)
            int loopIndex = (index+channelIndex) % sfxPlayers.Length;
            
            // 이미 재생중인 채널 건너뜀
            if (sfxPlayers[loopIndex].isPlaying) continue;
            
            // 비어있는 채널도달
            channelIndex = loopIndex; // 사용 가능한 채널
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx]; // 종류에 맞는 클립 끼움
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
