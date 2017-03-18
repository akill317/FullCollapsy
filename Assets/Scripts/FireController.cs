using UnityEngine;
using System.Collections;
using PathologicalGames;

public class FireController : MonoBehaviour {

	public string color = "White";

	string bulletColor;
	public int bulletNum = 0;


	public float shootInterval = 0.3f;
	float lastShootTime = 0;

	public float changeBulletInterval = 1;
	float lastChangeBulletTime = 0;

	public float bulletSpeed;
	GameManager gm;
	SpriteRenderer sprtRnd;
	public SpriteRenderer sprintBallSprtRnd;
	public ParticleSystem sprintBallPrtcleSys;

	public Transform playerPieces;

	public int touchDotID = -1;
	Vector2 dotPos = Vector2.zero;

	// Use this for initialization
	void Start () {
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		sprtRnd = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.J)){
			FireBullet();
		}
		if(Input.GetKeyDown(KeyCode.K)){
			GetBullets();
		}
		if(Input.GetMouseButtonDown(0)){
			FireBullet();
		}
		if(Input.GetMouseButtonDown(1)){
			GetBulletsByMouse();
		}
		ChangeColorToWhiteIfOutofBullet();
	}

	void GetBullets(){
		if(Time.time - lastChangeBulletTime >= changeBulletInterval|| Time.time == 0){
			lastChangeBulletTime = Time.time;
//			Vector2 playerPos = transform.position;
			if(touchDotID != -1)
				dotPos = gm.GetTargetPosition(touchDotID);
			Collider2D hitCol = Physics2D.OverlapPoint(dotPos,LayerMask.GetMask("Dots"));
			if(hitCol != null){
				if(hitCol.tag == "Dot"){
					Dot d = hitCol.GetComponent<Dot>();
					//Change player color
					color = d.color;
					Color gmCol = gm.GetColorByString(color);
					ChangeColorToNewBulletColor(gmCol);
					//Change bullet color
					bulletColor = d.color;
					int dotsNum = d.OnClickDots();
					if(dotsNum >= 3){
						bulletNum = dotsNum;
					}
				}
			}	
		}
	}

	void GetBulletsByMouse(){
		if(Time.time - lastChangeBulletTime >= changeBulletInterval|| Time.time == 0){
			lastChangeBulletTime = Time.time;
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Collider2D hitCol = Physics2D.OverlapPoint(mousePos,LayerMask.GetMask("Dots"));
			if(hitCol != null){
				if(hitCol.tag == "Dot"){
					Dot d = hitCol.GetComponent<Dot>();
					//Change player color
					color = d.color;
					Color gmCol = gm.GetColorByString(color);
					ChangeColorToNewBulletColor(gmCol);
					//Change bullet color
					bulletColor = d.color;
					int dotsNum = d.OnClickDots();
					if(dotsNum >= 3){
						bulletNum = dotsNum;
					}
				}
			}	
		}
	}

	void FireBullet(){
		if(bulletNum > 0){
			if(Time.time - lastShootTime >= shootInterval){
				lastShootTime = Time.time;
				bulletNum --;
				Vector3 faceDir = transform.TransformDirection(Vector3.left) + transform.position;
				Vector2 fireDir = faceDir - transform.position;
				fireDir.Normalize();
				Transform bullet = PoolManager.Pools["BulletsPool"].Spawn("Bullet",transform.position,Quaternion.Euler(new Vector3(0,0,transform.rotation.eulerAngles.z - 90)));
				bullet.GetComponent<Bullet>().color = bulletColor;
				bullet.GetComponent<SpriteRenderer>().color = gm.GetColorByString(bulletColor);
				bullet.GetComponent<Rigidbody2D>().velocity = bulletSpeed * fireDir;
			}
		}
	}

	public void ChangeColorToNewBulletColor(Color gmCol){
		sprtRnd.color = new Color(gmCol.r,gmCol.g,gmCol.b,sprtRnd.color.a);
		sprintBallSprtRnd.color = new Color(gmCol.r,gmCol.g,gmCol.b,sprintBallSprtRnd.color.a);
		sprintBallPrtcleSys.startColor = new Color(gmCol.r,gmCol.g,gmCol.b,sprintBallPrtcleSys.startColor.a);
		//Player pieces color
		SpriteRenderer[] childsSprtRnds = playerPieces.GetComponentsInChildren<SpriteRenderer>();
		foreach(SpriteRenderer piece in childsSprtRnds){
			piece.color = new Color(gmCol.r,gmCol.g,gmCol.b,piece.color.a);
		}
		ParticleSystem pieceParticle = playerPieces.GetComponentInChildren<ParticleSystem>();
		pieceParticle.startColor = new Color(gmCol.r,gmCol.g,gmCol.b,pieceParticle.startColor.a);
	}

	void ChangeColorToWhiteIfOutofBullet(){
		if(bulletNum == 0 && color != "White"){
			color = "White";
			ChangeColorToNewBulletColor(Color.white);
		}
	}
}
