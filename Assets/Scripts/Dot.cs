using UnityEngine;
using System.Collections;

public class Dot : MonoBehaviour {

	public int id;
	public string color;

	public Dot up;
	public Dot down;
	public Dot left;
	public Dot right;

	GameManager gm;

	public float smoothing = 0.5f;
	float xVel;
	float yVel;

	bool canMove;
	public Vector2 targetPos;

	void OnEnable(){
		id = -1;
		up = null;
		down = null;
		left = null;
		right = null;
		smoothing = Random.Range(0.1f,0.5f);
	}

	void Start(){
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
	}

	void Update(){
		if(canMove){
			MoveToPlace();	
		}
	}

	void MoveToPlace(){
		if(gm.gameState == GameManager.GameState.play){
			targetPos = gm.GetTargetPosition(id);	
		}
		if(targetPos.y == transform.position.y){
			canMove = false;
			return;
		}
		float xPos = transform.position.x;
		float yPos = transform.position.y;
		xPos = Mathf.SmoothDamp(xPos, targetPos.x, ref xVel, smoothing);
		yPos = Mathf.SmoothDamp(yPos, targetPos.y, ref yVel, smoothing);

		transform.position = new Vector3(xPos,yPos,transform.position.z);

	}

	public int OnClickDots(){
		int surroundDots;
		gm.sameColorSurroundDotsList.Clear();
		gm.GetSurroundDots(id);
		gm.LightUpSurroundDots();
		Invoke("DestroySurroundDotsInGM",1);
		surroundDots = gm.sameColorSurroundDotsList.Count;
		return surroundDots;
	}

	void DestroySurroundDotsInGM(){
		gm.DestroySurroundDots();
	}

	public void MoveDot(){
		canMove = true;
	}
}
