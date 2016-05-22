using UnityEngine;
using System.Collections;
using PathologicalGames;

public class ScoreCounter : MonoBehaviour {

	GameManager gm;
	public int currentScore = -1;

	public Transform middleHolder;
	public Transform leftHolder;
	public Transform rightHolder;
	public Transform farLeftHolder;
	public Transform farRightHolder;
	// Use this for initialization
	void Start () {
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(gm.gameState == GameManager.GameState.play){
			ChangeScore(gm.totalScore);
		}else if(gm.gameState == GameManager.GameState.menu){
			ChangeScore(gm.highScore);
		}
	}

	void ChangeScore(int score){
		if(currentScore != score){
			PoolManager.Pools["ScoresPool"].DespawnAll();
			currentScore = score;
			if(score > gm.highScore){
				gm.highScore = score;
				PlayerPrefs.SetInt("highScore",score);
			}
			if(currentScore >= 0 && currentScore < 10){
				PoolManager.Pools["ScoresPool"].Spawn(currentScore.ToString(),middleHolder.position,Quaternion.identity);
			}else if(currentScore<100){
				PoolManager.Pools["ScoresPool"].Spawn((Mathf.Floor(currentScore/10)).ToString(),leftHolder.position,Quaternion.identity);
				PoolManager.Pools["ScoresPool"].Spawn((Mathf.Floor(currentScore%10)).ToString(),rightHolder.position,Quaternion.identity);
			}else if(currentScore<1000){
				PoolManager.Pools["ScoresPool"].Spawn((Mathf.Floor(currentScore/100)).ToString(),farLeftHolder.position,Quaternion.identity);
				PoolManager.Pools["ScoresPool"].Spawn((Mathf.Floor((currentScore%100)/10)).ToString(),middleHolder.position,Quaternion.identity);
				PoolManager.Pools["ScoresPool"].Spawn((Mathf.Floor((currentScore%100)%10)).ToString(),farRightHolder.position,Quaternion.identity);
			}	
		}
	}
}
