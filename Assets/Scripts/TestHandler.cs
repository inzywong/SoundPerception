using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHandler : MonoBehaviour
{
  // Use this for initialization
  public Color backgroundColor;
  public Color diskColor;
  public Color xColor;
  public GameObject disc1;
  public GameObject disc2;
  public GameObject xLookAt;
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
    audioS = GetComponent<AudioSource>();
    trans1 = disc1.GetComponent<Transform>();
    trans2 = disc2.GetComponent<Transform>();
    scale = disc1.GetComponent<Transform>().localScale.x; // The scale of the disks. Assuming that both have the same scale
    played = false;
  }
  void Start()
  {
    r = (disc1.GetComponent<CircleCollider2D>().radius) * scale;
    disc1.GetComponent<SpriteRenderer>().color = diskColor;
    disc2.GetComponent<SpriteRenderer>().color = diskColor;
    xLookAt.GetComponent<SpriteRenderer>().color = xColor;
    Camera.main.backgroundColor = backgroundColor;
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
