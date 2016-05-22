using UnityEngine;
using System.Collections;
using PathologicalGames;

public class Enemy : MonoBehaviour {

	public string color = "White";
	public float enemySpeed;

	public float minPlayerEnemyDist = 10;
	public float randomMoveInterval = 2;
	float lastRandomMove;


	public Sprite beginningSprite;
	SpriteRenderer sptRndr;
	Rigidbody2D rigid;
	Animator anim;
	Collider2D clldr;

	Transform player;
	GameManager gm;

	int touchedDotId = -1;

	bool alive = false;

	public float shakeDuration = 2f;
	public float shakeAmount = 0.5f;
	float currentShakeTime = 0;
	Vector2 shakePos;

	//For moving toward the target on player circle
	Vector3 targetPosOnPlayerUnitCircle;
	Vector2 steerDirAmount;

	FireController playerFireControl;
	PlayerManager playerManager;

	void OnEnable(){
		color = "White";
		touchedDotId = -1;
		sptRndr = GetComponent<SpriteRenderer>();
		clldr = GetComponent<Collider2D>();
		sptRndr.color = Color.white;
		sptRndr.sprite = beginningSprite;
		alive = true;
		clldr.enabled = true;
		targetPosOnPlayerUnitCircle = new Vector2(Random.value,Random.value);
		targetPosOnPlayerUnitCircle.Normalize();
		steerDirAmount = new Vector2(Random.Range(1,6),Random.Range(1,6));
	}

	void OnDisable(){
		sptRndr.sprite = beginningSprite;
	}

	// Use this for initialization
	void Start () {
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		player = GameObject.Find("Player").transform;
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		playerFireControl = player.GetComponent<FireController>();
		playerManager = player.GetComponent<PlayerManager>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(alive){
			Move();
			ConstraintWithinWall();
		}else{
			DisableColliderAndRemoveFromEnemyList();
			ShakeSelf();
		}
	}


	void Move(){
		if(color != playerFireControl.color && playerManager.alive){
		//If the player have different color with the enemy then the enemy should chase player as well as surrounded enemy
			//Chase the player
			if(gm.enemyLeader == transform){
				//Move toward player
				Vector2 dir = player.position - transform.position;
				rigid.velocity = dir.normalized * enemySpeed;
			}else{
				//Move toward other places on the player range circle
				float r = (gm.enemyLeader.position - player.position).magnitude / 2;
				Vector3 targetPos = player.position + targetPosOnPlayerUnitCircle * r;
				Vector2 dir = targetPos - transform.position;
				Vector2 steerDir = new Vector2(dir.x * steerDirAmount.x,dir.y * steerDirAmount.y);
				steerDir.Normalize();
				rigid.velocity = steerDir * enemySpeed;
			}
		}else{
		//If the player have the same color with the enemy then the enemy should random move and flee player as well as surround enemy
			if(gm.enemyLeader == transform){
				//If the enemy is leader change another leader
				gm.RandomEnemyLeader();
			}else{
				//Flee the player and Randomly move	
				if((player.position - transform.position).magnitude <= minPlayerEnemyDist){
					Vector2 dir = transform.position - player.position;
					rigid.velocity = dir.normalized * enemySpeed;
				}else{
					if(Time.time - lastRandomMove > randomMoveInterval){
						lastRandomMove = Time.time;
						Vector2 randomDir = new Vector2(Random.value, Random.value);
						randomDir.Normalize();
						rigid.velocity = randomDir * enemySpeed;
					}
				}
			}	
		}
	}

	void ConstraintWithinWall(){
		Vector2 boundrySize = 1.1f * gm.mapTopRightPos;
		if(transform.position.x > boundrySize.x){
			transform.position = new Vector2(boundrySize.x,transform.position.y);
			rigid.velocity = -transform.position.normalized * enemySpeed;
		}else if(transform.position.x < -boundrySize.x){
			transform.position = new Vector2(-boundrySize.x,transform.position.y);
			rigid.velocity = -transform.position.normalized * enemySpeed;
		}
		if(transform.position.y > boundrySize.y){
			transform.position = new Vector2(transform.position.x,boundrySize.y);
			rigid.velocity = -transform.position.normalized * enemySpeed;
		}else if(transform.position.y < -boundrySize.y){
			transform.position = new Vector2(transform.position.x,-boundrySize.y);
			rigid.velocity = -transform.position.normalized * enemySpeed;
		}
	}


	void DisableColliderAndRemoveFromEnemyList(){
		if(clldr.enabled){
			clldr.enabled = false;
		}
		if(gm.enemiesInFieldList.Contains(transform)){
			gm.enemiesInFieldList.Remove(transform);
		}
	}

	void ShakeSelf(){
		if(Time.time - currentShakeTime <= shakeDuration){
			rigid.MovePosition(shakePos + Random.insideUnitCircle * shakeAmount);
		}	
	}

	void LightUpDotWhenTouchDot(Collider2D col){
		int.TryParse(col.name,out touchedDotId);
		if(touchedDotId == -1)	return;
		if(gm.EnemyDotColorMatch(touchedDotId, color)){
			if(alive){
				alive = false;
				Sparkling dotSpark = col.GetComponent<Sparkling>();
				dotSpark.lastLightUpTime = Time.time;
				dotSpark.overShine = true;
				Die();
			}
		}
	}

	public void Die(){
		currentShakeTime = Time.time;
		shakePos = transform.position;
		anim.Play("die");
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.gameObject.tag == "Bullet"){
			color = col.gameObject.GetComponent<Bullet>().color;
			sptRndr.color = gm.GetColorByString(color);
		}

		if(color != "White"){
			if(col.tag == "Dot"){
				LightUpDotWhenTouchDot(col);
			}
		}
	}

	void OnTriggerStay2D(Collider2D col){
		if(color != "White"){
			if(col.tag == "Dot"){
				LightUpDotWhenTouchDot(col);	
			}
		}
	}

	public void DespawnSelf(){
		if(gameObject.activeSelf)
			PoolManager.Pools["EnemiesPool"].Despawn(transform);
		gm.totalScore += 1;
		gm.DestroyADot(touchedDotId);
		gm.CreateNewDotRelinkARow(touchedDotId%gm.mapHoriNum);
	}
}
