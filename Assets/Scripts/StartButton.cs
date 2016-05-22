using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	Vector3 buttonOriginScale;
	GameManager gm;

	public GameObject dotsPool;
	public PlayerManager player;

	bool onClick = false;

	// Use this for initialization
	void Start () {
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		buttonOriginScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			if(!onClick){
				Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Collider2D hitCol = Physics2D.OverlapPoint(mousePos,LayerMask.GetMask("UI"));
				if(hitCol != null){
					if(hitCol.transform == transform){
						onClick = true;
						ChangeButtonColor();
					}
				}	
			}
		} 
		if(Input.GetMouseButtonUp(0)){
			if(onClick){
				onClick = false;
				GameStart();	
			}
		}

		if(gm.gameState == GameManager.GameState.end){
			ReturnBackToMenu();
		}
	}

	public void ChangeButtonColor(){
		iTweenExtensions.ColorTo(gameObject,new Color(1,1,1,0.2f),0.1f,0);
		iTweenExtensions.ScaleTo(gameObject,buttonOriginScale * 0.8f,0.1f,0);
	}

	public void GameStart(){
		//duration is 1.3 second
		GetComponent<Collider2D>().enabled =false;
		iTweenExtensions.ColorTo(gameObject,new Color(1,1,1,1f),0.5f,0);
		iTweenExtensions.ScaleTo(gameObject,buttonOriginScale * 1.2f,0.5f,0);
		iTweenExtensions.ScaleTo(gameObject,buttonOriginScale * 0,1f,0.5f,EaseType.easeInOutQuart);
		StartCoroutine(EnterGame(1.5f));
	}

	IEnumerator EnterGame(float time){
		yield return new WaitForSeconds(time);
		gm.CreateDotsMap();
		dotsPool.transform.localScale = Vector3.one * 0.2f;
		dotsPool.ScaleTo(Vector3.one,1,0,EaseType.easeOutBounce);
		yield return new WaitForSeconds(1);
		if(gm.gameState == GameManager.GameState.menu){
			gm.gameState = GameManager.GameState.play;
		}
		player.Reborn();
	}

	void ReturnBackToMenu(){
		gm.gameState = GameManager.GameState.menu;
		gm.ClearAllStuffWhenGameEnd();
		StartCoroutine(ReturnButton(3));
	}

	IEnumerator ReturnButton(float time) {
		float lastTime = Time.realtimeSinceStartup;
		float timer = 0.0f;

		while (timer < time) {
			timer += (Time.realtimeSinceStartup - lastTime);
			lastTime = Time.realtimeSinceStartup;
			yield return null;
		}

		iTweenExtensions.ColorTo(gameObject,new Color(1,1,1,1f),0.45f,0);
		iTweenExtensions.ScaleTo(gameObject,buttonOriginScale,2,0,EaseType.easeInOutBounce);
		GetComponent<Collider2D>().enabled = true;
	}
}
