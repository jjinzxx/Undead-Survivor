using UnityEngine;


public class GameResult : MonoBehaviour
{
    public GameObject[] titles;     // [0]=Dead / [1]=Survived

    public void GameOver()
    {
        titles[0].SetActive(true);
    }
    
    public void GameVictory()
    {
        titles[1].SetActive(true);
    }
}
