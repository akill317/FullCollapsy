using UnityEngine;
using System.Collections;

public class ScoreNumber : MonoBehaviour {

	GameManager gm;
	SpriteRenderer sptRnd;
	string color;

	public float changeColorInterval = 2;
	float lastChangeTime;

	void OnEnable(){
		sptRnd = GetComponent<SpriteRenderer>();
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
	}

	void Update(){
		if(gm.gameState == GameManager.GameState.menu){
			sptRnd.material.color = Color.white;
		}else{
			if(Time.time - lastChangeTime >= changeColorInterval){
				lastChangeTime = Time.time;
				iTweenExtensions.ColorTo(gameObject,RandColor(),changeColorInterval,0);
			}
		}
	}

	Color RandColor(){
		int randNum = Random.Range(0,3);
		string col = "White";
		if(randNum == 0)	col = "Red";
		else if(randNum == 1)	col = "Blue";
		else if(randNum == 2)	col = "Yellow";
		return gm.GetColorByString(col);
	}
}
