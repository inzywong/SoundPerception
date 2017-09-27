using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour {

	// Use this for initialization

public GameObject disc1;
public GameObject disc2;
public Transform trans1;
public Transform trans2;
protected double r;
private double scale;
bool played = false;
private AudioSource audioS;


[Range(-20.0f, 30.0f)]
public float speed;

void Awake(){
	//float scale = 0.5f;//0.5 is the scale of the object, the object is scalled down.
	audioS = GetComponent<AudioSource>();
	scale = 0.5;

}
void Start(){
	r=(disc1.GetComponent<CircleCollider2D>().radius)*scale;

}

	// Update is called once per frame
	void Update () {
		if( trans1.position.x+r <50 && trans1.position.x-r >-50 ){
			trans1.Translate(Vector2.right * Time.deltaTime*speed);
			trans2.Translate(Vector2.left * Time.deltaTime*speed);
			 //Debug.DrawLine(trans1.position, new Vector3((trans1.position.x+r), 0, 0), Color.red);
			if((trans1.position.x+r) >= (trans2.position.x-r) && !played){
				audioS.Play();
				Debug.Log("intersection point?: "+trans1.position.x+r);
				played= true;
			}
		}

	}



}
