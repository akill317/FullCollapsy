using UnityEngine;
using System.Collections;
using PathologicalGames;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public enum GameState{
		menu,
		play,
		end
	}

	public int mapHoriNum;
	public int mapVertiNum;
	int totalDots;

	public Dictionary<int, Dot> dotsDict;
	public List<int> sameColorSurroundDotsList;

	public List<Transform> enemiesInFieldList;
	public Transform enemyLeader;

	public GameObject dotSprite;

	public GameState gameState = GameState.menu;

	Vector2 bottomLeft;
	float spriteWidth;
	float spriteHeight;

	public Color redDotCol;
	public Color blueDotCol;
	public Color yellowDotCol;
	public Color greenDotCol;

	public Vector3 mapTopRightPos;

	public int totalScore = 0;
	public int highScore = 0;

	public GameObject playerDieGObj;
	public Transform player;


	// Use this for initialization
	void Start () {
		totalDots = mapHoriNum * mapVertiNum;
		spriteWidth = dotSprite.GetComponent<SpriteRenderer>().bounds.size.x * 0.6f;
		spriteHeight= dotSprite.GetComponent<SpriteRenderer>().bounds.size.y * 0.6f;
		bottomLeft = new Vector2((-(mapHoriNum * spriteWidth) + spriteWidth)/ 2.0f, (-(mapVertiNum * spriteHeight) + spriteHeight)/ 2.0f);
		dotsDict = new Dictionary<int, Dot>();
		sameColorSurroundDotsList = new List<int>();
		highScore = PlayerPrefs.GetInt("highScore");
	}

		
	public void CreateDotsMap(){
		dotsDict.Clear();
		PoolManager.Pools["DotsPool"].DespawnAll();
		for (int i=0; i<totalDots; i++){

			Vector2 dotPos = bottomLeft;
			dotPos.x += (i % mapHoriNum) * spriteWidth;
			dotPos.y += Mathf.FloorToInt(i / mapHoriNum) * spriteHeight;
			SpawnDot(i,dotPos);
			SpawnWall(i,dotPos);
		}
		mapTopRightPos = dotsDict[totalDots-1].transform.position;
	}

	void SpawnWall(int id, Vector2 pos){
		if(Mathf.Floor(id/mapHoriNum) == mapVertiNum-1){
			PoolManager.Pools["BoundriesPool"].Spawn("wallPiece", pos * 1.05f, Quaternion.identity);
		}
		if(Mathf.Floor(id/mapHoriNum) == 0){
			PoolManager.Pools["BoundriesPool"].Spawn("wallPiece", pos * 1.05f, Quaternion.Euler(new Vector3(0,0,180)));
		}
		if(id % mapHoriNum == 0){
			PoolManager.Pools["BoundriesPool"].Spawn("wallPiece", pos * 1.05f, Quaternion.Euler(new Vector3(0,0,90)));
		}
		if(id % mapHoriNum == mapHoriNum-1){
			PoolManager.Pools["BoundriesPool"].Spawn("wallPiece", pos * 1.05f, Quaternion.Euler(new Vector3(0,0,270)));
		}
	}

	string RandColor(){
		int rand = Random.Range(0,3);
		string dotcolor = "";
		if(rand == 0){
			dotcolor = "Red";
		}else if(rand == 1){
			dotcolor = "Blue";
		}else if(rand == 2){
			dotcolor = "Yellow";
		}else if(rand == 3){
			dotcolor = "Green";
		}
		return dotcolor;
	}

	public void RandomEnemyLeader(){
		int randLeadIndex = Random.Range(0, enemiesInFieldList.Count);
		enemyLeader = enemiesInFieldList[randLeadIndex];
	}

	public Color GetColorByString(string col){
		if(col == "Red"){
			return redDotCol;
		}else if(col == "Blue"){
			return blueDotCol;
		}else if(col == "Yellow"){
			return yellowDotCol;
		}else if(col == "Green"){
			return greenDotCol;
		}else
			return Color.white;
	}

	public void GetSurroundDots(int id){
		//Added self
		if(!sameColorSurroundDotsList.Contains(id)){
			sameColorSurroundDotsList.Add(id);
		}
		//CheckUp
		if(dotsDict[id].up != null){
			if(dotsDict[id].color == dotsDict[id].up.color){
				if(!sameColorSurroundDotsList.Contains(dotsDict[id].up.id)){
					sameColorSurroundDotsList.Add(dotsDict[id].up.id);
					GetSurroundDots(dotsDict[id].up.id);
				}
			}
		}
		//CheckDown
		if(dotsDict[id].down != null){
			if(dotsDict[id].color == dotsDict[id].down.color){
				if(!sameColorSurroundDotsList.Contains(dotsDict[id].down.id)){
					sameColorSurroundDotsList.Add(dotsDict[id].down.id);
					GetSurroundDots(dotsDict[id].down.id);
				}
			}
		}
		//CheckLeft
		if(dotsDict[id].left != null){
			if(dotsDict[id].color == dotsDict[id].left.color){
				if(!sameColorSurroundDotsList.Contains(dotsDict[id].left.id)){
					sameColorSurroundDotsList.Add(dotsDict[id].left.id);
					GetSurroundDots(dotsDict[id].left.id);
				}
			}
		}
		//CheckRight
		if(dotsDict[id].right != null){
			if(dotsDict[id].color == dotsDict[id].right.color){
				if(!sameColorSurroundDotsList.Contains(dotsDict[id].right.id)){
					sameColorSurroundDotsList.Add(dotsDict[id].right.id);
					GetSurroundDots(dotsDict[id].right.id);
				}
			}
		}

	}

	public void SpawnDot(int id, Vector2 pos){
		string clr = RandColor();
		Transform dot = PoolManager.Pools["DotsPool"].Spawn("Dot", pos, Quaternion.identity);
		Dot d = dot.GetComponent<Dot>();
		dotsDict.Add(id,d);
		d.name = id.ToString();
		d.id = id;
		d.color = clr;
		SpriteRenderer dotSptRnd = dot.GetComponent<SpriteRenderer>();
		dotSptRnd.color = new Color(GetColorByString(clr).r,GetColorByString(clr).g,GetColorByString(clr).b,dotSptRnd.color.a);

		LinkDot(id);
	}

	void LinkDot(int id){
		//LinkUp
		if(dotsDict.ContainsKey(id+mapHoriNum)){
			dotsDict[id+mapHoriNum].down = dotsDict[id];
			dotsDict[id].up = dotsDict[id+mapHoriNum];
		}else{
			dotsDict[id].up = null;
		}
		//LinkDown
		if(dotsDict.ContainsKey(id-mapHoriNum)){
			dotsDict[id-mapHoriNum].up = dotsDict[id];
			dotsDict[id].down = dotsDict[id-mapHoriNum];
		}else{
			dotsDict[id].down = null;
		}
		//LinkLeft
		if(dotsDict.ContainsKey(id-1) && id%mapHoriNum != 0){
			dotsDict[id-1].right = dotsDict[id];
			dotsDict[id].left = dotsDict[id-1];
		}else{
			dotsDict[id].left = null;
		}
		//LinkRight
		if(dotsDict.ContainsKey(id+1) && id%mapHoriNum != mapHoriNum-1){
			dotsDict[id+1].left = dotsDict[id];
			dotsDict[id].right = dotsDict[id+1];
		}else{
			dotsDict[id].right = null;
		}
	}

	public void LightUpSurroundDots(){
		if(sameColorSurroundDotsList.Count >= 3){
			foreach(int dot in sameColorSurroundDotsList){
				Sparkling sprk = dotsDict[dot].GetComponent<Sparkling>();
				sprk.lastLightUpTime = Time.time;
				sprk.overShine = true;
			}
		}
	}

	//Return whether destroied the dot or not
	public void DestroySurroundDots(){
		if(sameColorSurroundDotsList.Count >= 3){
			foreach(int dot in sameColorSurroundDotsList){
				DestroyADot(dot);
			}
		}
		CreateNewDotRelinkSurroundDotsRows();
	}

	public void CreateNewDotRelinkSurroundDotsRows(){
		List<int> DestoryRows = new List<int>();
		foreach(int dot in sameColorSurroundDotsList){
			if(!DestoryRows.Contains(dot%mapHoriNum)){
				DestoryRows.Add(dot%mapHoriNum);
			}
		}
		foreach(int row in DestoryRows){
			CreateNewDotRelinkARow(row);
		}
	}

	public void DestroyADot(int id){
		if(dotsDict.ContainsKey(id)){
			PoolManager.Pools["DotsPool"].Despawn(dotsDict[id].transform);
			dotsDict.Remove(id);	
		}
	}

	public void CreateNewDotRelinkARow(int row){
		//Count how many dots are missing
		int lackDot = 0;

		for(int i=0; i<mapVertiNum; i++){
			//If there is a null then spanw on top, otherwise rename the dot
			int currentID = i*mapHoriNum + row;
			if(!dotsDict.ContainsKey(currentID)){
				lackDot += 1;
			}else{
				dotsDict[currentID].id = currentID - (lackDot*mapHoriNum);
				dotsDict[currentID].name = dotsDict[currentID].id.ToString();
				if(dotsDict.ContainsKey(currentID - (lackDot*mapHoriNum)))
					dotsDict[currentID - (lackDot*mapHoriNum)] = dotsDict[currentID];
				else
					dotsDict.Add(currentID - (lackDot*mapHoriNum),dotsDict[currentID]);
			}
		}
		for(int i=0; i<lackDot; i++){
			Vector2 dotPos = bottomLeft;
			dotPos.x += row * spriteWidth;
			dotPos.y += (i + mapVertiNum) * spriteHeight;
			if(dotsDict.ContainsKey(((i+mapVertiNum-lackDot)*mapHoriNum)+row))
				dotsDict.Remove(((i+mapVertiNum-lackDot)*mapHoriNum)+row);
			SpawnDot(((i+mapVertiNum-lackDot)*mapHoriNum)+row,dotPos);
		}

		for(int i=0; i<mapVertiNum; i++){
			int currentID = i*mapHoriNum + row;
			LinkDot(currentID);
			dotsDict[currentID].MoveDot();
		}
	}

	public Vector2 GetTargetPosition(int id){
		Vector2 pos = bottomLeft;
		pos.x += (id % mapHoriNum) * spriteWidth;
		pos.y += Mathf.FloorToInt(id / mapHoriNum) * spriteHeight;
		return pos;
	}

	public bool EnemyDotColorMatch(int dotId, string enemyColor){
		if(dotsDict.ContainsKey(dotId)){
			if(dotsDict[dotId].color == enemyColor)
				return true;
			else
				return false;
		}else
			return false;
	}

	public void KillPlayer(){
		playerDieGObj.transform.position = player.position;
		Camera.main.GetComponent<CameraShake>().shakeDuration = 1f;
		Camera.main.GetComponent<CameraShake>().shakeAmount = 0.5f;
		playerDieGObj.GetComponent<Animator>().Play("playerDie");
		playerDieGObj.GetComponentInChildren<ParticleSystem>().Play();
	}

	public void ClearAllStuffWhenGameEnd(){
		foreach(KeyValuePair<int,Dot> dot in dotsDict){
			Dot d = dot.Value;
			d.GetComponent<Sparkling>().lastLightUpTime = Time.time;
			d.targetPos = Random.onUnitSphere * 100;
			d.smoothing = 2;
			d.MoveDot();
			PoolManager.Pools["DotsPool"].Despawn(d.transform,2);
		}
		dotsDict.Clear();
		PoolManager.Pools["BoundriesPool"].DespawnAll();
		foreach(Transform enemy in enemiesInFieldList){
			enemy.GetComponent<Enemy>().Die();
		}
		totalScore = 0;
	}
}
