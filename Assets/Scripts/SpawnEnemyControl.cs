using UnityEngine;
using System.Collections;
using PathologicalGames;

public class SpawnEnemyControl : MonoBehaviour {

	GameManager gm;

	public float spawnEnemyInterval = 10f;
	public float lastSpawnEnemyTime = 0;

	// Use this for initialization
	void Start () {
		gm = GetComponent<GameManager>();

	}
	
	// Update is called once per frame
	void Update () {
		if(gm.gameState == GameManager.GameState.menu){
			ClearAllEnemies();
		}else if(gm.gameState == GameManager.GameState.play){
			SpawnEnemies();	
		}
	}

	void ClearAllEnemies(){
		if(gm.enemiesInFieldList.Count != 0){
			gm.enemiesInFieldList.Clear();
			PoolManager.Pools["EnemiesPool"].DespawnAll();
		}
	}


	void SpawnEnemies(){
		if(Time.time - lastSpawnEnemyTime >= spawnEnemyInterval){
			lastSpawnEnemyTime = Time.time;
			Transform enemy = PoolManager.Pools["EnemiesPool"].Spawn("Enemy",RandPos(),Quaternion.identity);
			gm.enemiesInFieldList.Add(enemy);
			gm.RandomEnemyLeader();
		}
	}

	Vector3 RandPos(){
		Vector3 pos = Vector3.zero;
		float rndX = Random.Range(-gm.mapTopRightPos.x,gm.mapTopRightPos.x);
		float rndY = Random.Range(-gm.mapTopRightPos.y,gm.mapTopRightPos.y);
		pos.x = rndX;
		pos.y = rndY;
		return pos;
	}

}
