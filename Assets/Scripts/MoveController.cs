using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour {

	public Transform sprintBall;
	Vector3 sprintBallOriginSize;
	SpriteRenderer sprintBallSprtRnd;
	ParticleSystem sprintBallPrtSys;

	Rigidbody2D rigid;
	Vector2 inputDir;
	float currentAngle;
	public float forcePower;

	public float sprintInterval = 1f;
	public float lastSprintTime;

	float stopTime;

	// Use this for initialization
	void Start () {
		forcePower = 10;
		rigid = GetComponent<Rigidbody2D>();
		sprintBallOriginSize = sprintBall.localScale;
		sprintBallSprtRnd = sprintBall.GetComponent<SpriteRenderer>();
		sprintBallPrtSys = sprintBall.GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		MoveAndRotate();
		Sprint();
		Break();
	}

	void MoveAndRotate() {
		inputDir = Vector2.zero;
		inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		if(inputDir.magnitude<=0.6f)	inputDir = Vector2.zero;
		if(inputDir.magnitude != 0){
			inputDir.Normalize();
			if(inputDir.y<0)
				currentAngle = Vector3.Angle(Vector3.left,inputDir);
			else
				currentAngle = -Vector3.Angle(Vector3.left,inputDir);
			rigid.MoveRotation(currentAngle);
			rigid.AddForce(inputDir * forcePower * 1);
		}
	}

	void Sprint() {
		Color sprintBallOriginColor = Color.white;
		if(Time.time - lastSprintTime >= sprintInterval){
			if(Input.GetKeyDown(KeyCode.Space)){
				lastSprintTime = Time.time;
				rigid.AddForce(transform.TransformDirection(Vector2.left) * forcePower * 0.5f,ForceMode2D.Impulse);
				sprintBallOriginColor = sprintBallSprtRnd.color;
				sprintBallPrtSys.Play();
			}
		}

		if(Time.time - lastSprintTime < sprintInterval){
			float scaler = (Time.time - lastSprintTime)/sprintInterval;
			sprintBall.localScale = sprintBallOriginSize * scaler;
			if(scaler > 0.95f){
				sprintBallSprtRnd.color = new Color(sprintBallSprtRnd.color.r,sprintBallSprtRnd.color.g,sprintBallSprtRnd.color.b,sprintBallOriginColor.a * scaler);
			}else{
				sprintBallSprtRnd.color = new Color(sprintBallSprtRnd.color.r,sprintBallSprtRnd.color.g,sprintBallSprtRnd.color.b,sprintBallOriginColor.a * 0.2f * scaler);
			}
		}
	}

	void Break() {
		rigid.drag = Mathf.Clamp(Vector3.Angle(-transform.right, rigid.velocity) / 180 * 5, 0.5f, 1);
	}
}
