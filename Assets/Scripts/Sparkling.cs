using UnityEngine;
using System.Collections;

public class Sparkling : MonoBehaviour {

	SpriteRenderer sptRndr;

	public float maxValue;
	public float minValue;

	float originMaxValue;
	float originMinValue;

	public float lightUpDuration = 1f;
	public float lastLightUpTime;

	public bool overShine = false;

	// Use this for initialization
	void Start () {
		sptRndr = GetComponent<SpriteRenderer>();
		originMaxValue = maxValue;
		originMinValue = minValue;
	}
	
	// Update is called once per frame
	void Update () {
		LightUp();
		Sparkle();
	}

	void Sparkle(){
		float alpha = sptRndr.color.a;
		alpha += (maxValue - minValue) * 0.3f* Random.Range(-1.0f, 1.0f);
		alpha = Mathf.Clamp(alpha, minValue, maxValue);
		sptRndr.color = new Color(sptRndr.color.r,sptRndr.color.g,sptRndr.color.b,alpha);
	}

	void LightUp(){
		float tmpDuration = lightUpDuration;
		if(Time.time - lastLightUpTime < tmpDuration && Time.time >= tmpDuration){
			float invAlpha = (Time.time - lastLightUpTime)/lightUpDuration; //from 0 to 1
			if(!overShine){
				minValue = maxValue = Mathf.Lerp(0.2f, originMinValue,invAlpha);	
			}
			else{
				tmpDuration = 2* lightUpDuration;
				minValue = maxValue = Mathf.Lerp(1, originMinValue, invAlpha);	
			}
		}else{
			ReturnAlphaValue();
		}
	}

	void ReturnAlphaValue(){
		minValue = originMinValue;
		maxValue = originMaxValue;
		overShine = false;
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.tag == "Player"||col.tag == "Bullet"){
			lastLightUpTime = Time.time;
		}
		if(col.tag == "Enemy"&&tag=="Dot"){
			if(col.GetComponent<Enemy>().color == GetComponent<Dot>().color){
				lastLightUpTime = Time.time;
			}
		}
	}

	void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.tag == "Player"){
			lastLightUpTime = Time.time;
			overShine = true;
		}
	}

	void OnDisable(){
//		ReturnAlphaValue();
		lastLightUpTime = Time.time;
		overShine = true;
	}
}
