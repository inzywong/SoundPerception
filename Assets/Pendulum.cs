using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour {

	// Use this for initialization

public GameObject disc1;
public GameObject disc2;
public Transform trans1;
public Transform trans2;
protected double r;
private double scale;
bool played = false;
private AudioSource audioS;


[Range(20f, 25f)]
public float rPendulum;

public Vector3 centerPos;

private float alpha,
							alphaAcc,
							alphaVel,
 							beta,
							betaAcc,
							betaVel,
							gravity;

void Awake(){
	//float scale = 0.5f;//0.5 is the scale of the object, the object is scalled down.
	gravity = 9.82f;
	audioS = GetComponent<AudioSource>();
	scale = 0.5;
	centerPos = new Vector3(0f,0f,0f);
	rPendulum = 23f;
	alpha =Mathf.PI/2f;
	alphaVel=0;
	beta = 2*Mathf.PI-alpha;
	trans1.position = centerPos + (new Vector3(Mathf.Sin(alpha),Mathf.Cos(alpha),0f) *rPendulum);
	trans2.position = centerPos + (new Vector3(Mathf.Sin(beta),Mathf.Cos(beta),0f) *rPendulum);

	//Debug.Log(Mathf.Cos(alpha));
	//Debug.Log(trans1.position);


}
void Start(){
	r=(disc1.GetComponent<CircleCollider2D>().radius)*scale;
}

	// Update is called once per frame
	void Update () {
		//if( trans1.position.x+r <50 && trans1.position.x-r >-50 ){
		//beta -=rotationSpeed;
		//Debug.DrawLine(centerPos, trans2.position, Color.blue);
	//	if( -24f <= trans2.position.x <=20f ){
		if(trans1.position.x > -24 && trans2.position.x< 20){

		trans1.position = centerPos + (new Vector3(Mathf.Sin(alpha),Mathf.Cos(alpha),0f) *rPendulum);
		trans2.position = centerPos + (new Vector3(Mathf.Sin(beta),Mathf.Cos(beta),0f) *rPendulum);
		Debug.DrawLine(centerPos, trans1.position, Color.red);

		alphaAcc = (gravity/rPendulum)*Mathf.Sin(alpha);
		alphaVel+= alphaAcc * Time.time;
		alpha 	+= Time.deltaTime*alphaVel;
		betaAcc  = (gravity/rPendulum)*Mathf.Sin(beta);
		betaVel += betaAcc * Time.time;
		beta    += Time.deltaTime*betaVel;



			Debug.Log("disc1"+ trans1.position.x);
			Debug.Log("disc2"+ trans2.position);
		}


			//trans2.Translate(Vector2.left * Time.deltaTime*speed);
			 //Debug.DrawLine(trans1.position, new Vector3((trans1.position.x+r), 0, 0), Color.red);
		/*	if((trans1.position.x+r) >= (trans2.position.x-r) && !played){
				audioS.Play();
				Debug.Log("intersection point?: "+trans1.position.x+r);
				played= true;
			}*/
		//}

	}



}
