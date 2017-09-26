using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHandler : MonoBehaviour
{
  // Use this for initialization
  [Header("Colors")]
  public Color backgroundColor;
  public Color diskColor;
  public Color xColor;
  [Header("Objects")]
  public GameObject discL;
  public GameObject discR;
  public GameObject xLookAt;
  [Header("Variables")]
  [Range(-20.0f, 30.0f)]
  public float speed;

  private double scale; // Disk scale
  private double r; // Disk radius
  private Transform diskTransL;
  private Transform diskTransR;
  private bool played; // If the sound has been played
  private AudioSource audioS;

  void Awake()
  {
    audioS = GetComponent<AudioSource>();
    diskTransL = discL.GetComponent<Transform>();
    diskTransR = discR.GetComponent<Transform>();
    scale = discL.GetComponent<Transform>().localScale.x; // The scale of the disks. Assuming that both have the same scale
    played = false;
  }
  void Start()
  {
    r = (discL.GetComponent<CircleCollider2D>().radius) * scale;
    discL.GetComponent<SpriteRenderer>().color = diskColor;
    discR.GetComponent<SpriteRenderer>().color = diskColor;
    xLookAt.GetComponent<SpriteRenderer>().color = xColor;
    Camera.main.backgroundColor = backgroundColor;
  }

  // Update is called once per frame
  void Update()
  {
    if (diskTransL.position.x + r < 50 && diskTransL.position.x - r > -50)
    {
      diskTransL.Translate(Vector2.right * Time.deltaTime * speed);
      diskTransR.Translate(Vector2.left * Time.deltaTime * speed);
      //Debug.DrawLine(diskTransL.position, new Vector3((diskTransL.position.x+r), 0, 0), Color.red);
      if ((diskTransL.position.x + r) >= (diskTransR.position.x - r) && !played)
      {
        audioS.Play();
        Debug.Log("intersection point?: " + diskTransL.position.x + r);
        played = true;
      }
    }

  }
}
