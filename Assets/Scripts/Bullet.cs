using UnityEngine;
using System.Collections;
using PathologicalGames;

public class Bullet : MonoBehaviour {

	public string color;

	void OnEnable(){
		GetComponent<SpriteRenderer>().enabled = true;
		GetComponent<Collider2D>().enabled = true;
		Invoke("DespawnSelf",3);
	}

	void OnCollisionEnter2D(Collision2D col){
		DisableSelfAndPlayParticle();
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.GetComponent<Enemy>()){
			if(col.GetComponent<Enemy>().color != color){
				Camera.main.GetComponent<CameraShake>().shakeDuration = 0.2f;
				Camera.main.GetComponent<CameraShake>().shakeAmount = 0.2f;
				DisableSelfAndPlayParticle();
			}
		}
	}

	void DisableSelfAndPlayParticle(){
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		GetComponentInChildren<ParticleSystem>().startColor = GameObject.Find("GameManager").GetComponent<GameManager>().GetColorByString(color);
		GetComponentInChildren<ParticleSystem>().Play();
		Invoke("DespawnSelf",0.5f);
	}

	void DespawnSelf(){
		if(gameObject.activeSelf)
			PoolManager.Pools["BulletsPool"].Despawn(transform);
	}

}
