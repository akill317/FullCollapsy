using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	MoveController moveControl;
	FireController fireControl;
	public GameObject sprintBall;

	GameManager gm;

	public AnimationCurve animCurve;

	public bool alive;

	void Start(){
		moveControl = GetComponent<MoveController>();
		fireControl = GetComponent<FireController>();
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		alive = true;
		transform.position = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
		if(gm.gameState == GameManager.GameState.menu){
			DisableController();
		}else if(gm.gameState == GameManager.GameState.play){
			if(!alive){
				DisableController();
			}else{
				EnableController();
			}	
		}
	}

	void EnableController(){
		moveControl.enabled = true;
		fireControl.enabled = true;
		GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<Collider2D>().enabled = true;
		sprintBall.SetActive(true);
	}

	void DisableController(){
		moveControl.enabled = false;
		fireControl.enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
		sprintBall.SetActive(false);
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.tag == "Enemy"){
			if(col.GetComponent<Enemy>().color != fireControl.color){
				if(alive){
					alive = false;
					PlayerDie();	
				}	
			}
		}
	}

	public void Reborn(){
		EnableController();
		alive = true;
		transform.position = Vector2.zero;
		moveControl.lastSprintTime = Time.time;
		fireControl.color = "White";
		fireControl.bulletNum = 0;
		fireControl.ChangeColorToNewBulletColor(Color.white);
		fireControl.touchDotID = -1;
	}

	void PlayerDie(){
		gm.KillPlayer();
		StartCoroutine(ReturnBackToMenu(1));
		StartCoroutine(ScaleCameraAndBack(6f,4));
		StartCoroutine(ScaleTimeAndBackAndChangeGameState(1,4));
	}

	IEnumerator ScaleTimeAndBackAndChangeGameState(float start, float time) {
		float lastTime = Time.realtimeSinceStartup;
		float timer = 0.0f;

		while (timer < time) {
			Time.timeScale = start - animCurve.Evaluate(timer/time);
			timer += (Time.realtimeSinceStartup - lastTime);
			lastTime = Time.realtimeSinceStartup;
			yield return null;
		}

		Time.timeScale = start; 
	}

	IEnumerator ScaleCameraAndBack(float start, float time){
		float lastTime = Time.realtimeSinceStartup;
		float timer = 0;
		while(timer < time){
			Camera.main.orthographicSize = start - animCurve.Evaluate(timer/time);
			timer += (Time.realtimeSinceStartup - lastTime);
			lastTime = Time.realtimeSinceStartup;
			yield return null;
		}
		Camera.main.orthographicSize = start;
	}

	IEnumerator ReturnBackToMenu(float time) {
		float lastTime = Time.realtimeSinceStartup;
		float timer = 0.0f;

		while (timer < time) {
			timer += (Time.realtimeSinceStartup - lastTime);
			lastTime = Time.realtimeSinceStartup;
			yield return null;
		}
		gm.gameState = GameManager.GameState.end;
	}
}
