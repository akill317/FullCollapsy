using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	public Sprite wall0;
	public Sprite wall1;
	public Sprite wall2;
	public Sprite wall3;
	public Sprite wall4;
	public Sprite wall5;


	SpriteRenderer sprtRnd;
	Vector3 originScale;
	public AnimationCurve scaleCurve;
	public float scaleDUration;
	float lastScaleTime;
	// Use this for initialization
	void Start () {
		sprtRnd = GetComponent<SpriteRenderer>();
		originScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		RandomSprt();
		ScaleSelfOnTouch();
	}

	void RandomSprt(){
		// Do this once per 2 frame
		if(Time.frameCount % 2 == 0){
			int randNum = Random.Range(0,6);
			switch(randNum){
			case 0:
				sprtRnd.sprite = wall0;
				break;
			case 1:
				sprtRnd.sprite = wall1;
				break;
			case 2:
				sprtRnd.sprite = wall2;
				break;
			case 3:
				sprtRnd.sprite = wall3;
				break;
			case 4:
				sprtRnd.sprite = wall4;
				break;
			case 5:
				sprtRnd.sprite = wall5;
				break;
			}	
		}
	}

	void ScaleSelfOnTouch(){
		if(Time.time - lastScaleTime <= scaleDUration){
			float scaler = scaleCurve.Evaluate(Time.time - lastScaleTime / scaleDUration);
			transform.localScale = Vector3.one * scaler;
		}else{
			transform.localScale = originScale;
		}
	}

	void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.tag == "Player"){
			lastScaleTime = Time.time;
		}
	}
}
