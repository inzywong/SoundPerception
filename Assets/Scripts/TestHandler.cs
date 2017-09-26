using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHandler : MonoBehaviour
{
  // Use this for initialization
  [Header("Colors")]
  public Color backgroundColor;
  public Color diskColorL;
  public Color diskColorR;
  public Color xColor;
  [Header("Objects")]
  public GameObject discL;
  public GameObject discR;
  public GameObject xLookAt;
  [Header("Variables")]
  [Range(0f, 20.0f)]
  public float speed;

  private double scale; // Disk scale
  // private double r; // Disk radius
  private Transform diskTransL;
  private Transform diskTransR;
  private bool played; // If the sound has been played
  private AudioSource audioS;
  private Vector3 startLeft;
  private Vector3 startRight;
  private float journeyLength;
  private float startTime;

  private float error;

  void Awake()
  {
    audioS = GetComponent<AudioSource>();
    diskTransL = discL.GetComponent<Transform>();
    diskTransR = discR.GetComponent<Transform>();
    scale = discL.GetComponent<Transform>().localScale.x; // The scale of the disks. Assuming that both have the same scale
  }

  void Start()
  {
    played = false;
    startLeft = diskTransL.position;
    startRight = diskTransR.position;
    startTime = Time.time;
    error = 0.1f;
    // r = (discL.GetComponent<CircleCollider2D>().radius) * scale;
    discL.GetComponent<SpriteRenderer>().color = diskColorL;
    discR.GetComponent<SpriteRenderer>().color = diskColorR;
    xLookAt.GetComponent<SpriteRenderer>().color = xColor;
    Camera.main.backgroundColor = backgroundColor;
    journeyLength = Vector3.Distance(diskTransL.position, diskTransR.position);
  }

  // Update is called once per frame
  void Update()
  {
    float distCovered = (Time.time - startTime) * speed; // The distance traveled
    float fracJourney = distCovered / journeyLength;  // How much of the total length has been traveled. 

    // Move the disks towards each other
    diskTransL.position = Vector3.Lerp(startLeft, startRight, fracJourney);
    diskTransR.position = Vector3.Lerp(startRight, startLeft, fracJourney);

    float dist = Vector3.Distance(diskTransL.position, diskTransR.position); // Distance between the disks
    float distanceDelay = speed * 0.15f; // Distance traveled at 150ms
    // Debug.Log(dist);
    if ((Mathf.Abs(dist) <= 0 + error) && !played)
    {
      played = true;
      // Time.timeScale = 0;
      audioS.Play();
      Debug.Log("Distance from each other: " + dist);
      Debug.Log("Position DiskL: " + diskTransL.position);
      Debug.Log("Position DiskR: " + diskTransR.position);
    }
  }
}

// if (diskTransL.position.x + r < 50 && diskTransL.position.x - r > -50 && !played)
// {
// diskTransL.Translate(Vector2.right * Time.deltaTime * speed);
// diskTransR.Translate(Vector2.left * Time.deltaTime * speed);
//Debug.DrawLine(diskTransL.position, new Vector3((diskTransL.position.x+r), 0, 0), Color.red);
//if ((diskTransL.position.x + r) >= (diskTransR.position.x - r) && !played)

