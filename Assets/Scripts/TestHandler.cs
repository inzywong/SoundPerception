using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHandler : MonoBehaviour
{
  // Use this for initialization
  public Color diskColor;
  public GameObject disc1;
  public GameObject disc2;
  [Range(-20.0f, 30.0f)]
  public float speed;

  private double scale;
  private double r;
  private Transform trans1;
  private Transform trans2;
  private bool played;
  private AudioSource audioS;

  void Awake()
  {
    //float scale = 0.5f;//0.5 is the scale of the object, the object is scalled down.
    audioS = GetComponent<AudioSource>();
    trans1 = disc1.GetComponent<Transform>();
    trans2 = disc2.GetComponent<Transform>();
    scale = 0.5;
    played = false;
  }
  void Start()
  {
    r = (disc1.GetComponent<CircleCollider2D>().radius) * scale;
    disc1.GetComponent<SpriteRenderer>().color = diskColor;
    disc2.GetComponent<SpriteRenderer>().color = diskColor;
  }

  // Update is called once per frame
  void Update()
  {
    if (trans1.position.x + r < 50 && trans1.position.x - r > -50)
    {
      trans1.Translate(Vector2.right * Time.deltaTime * speed);
      trans2.Translate(Vector2.left * Time.deltaTime * speed);
      //Debug.DrawLine(trans1.position, new Vector3((trans1.position.x+r), 0, 0), Color.red);
      if ((trans1.position.x + r) >= (trans2.position.x - r) && !played)
      {
        audioS.Play();
        Debug.Log("intersection point?: " + trans1.position.x + r);
        played = true;
      }
    }

  }



}
