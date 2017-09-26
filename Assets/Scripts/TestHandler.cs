using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
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
  public enum SoundTiming
  {
    at,
    before,
    after
  }
  public SoundTiming soundTiming;
  [Tooltip("Sound offset in ms")]
  public float soundOffset = 150f;
  public enum Sounds
  {
    sound1,
    sound2,
    sound3
  }
  public Sounds sound;

  [Header("Sounds")]
  public AudioClip sound1;
  public AudioClip sound2;
  public AudioClip sound3;

  // // private double scale; // Disk scale
  // private double r; // Disk radius
  private Transform diskTransL;
  private Transform diskTransR;
  private AudioSource audioS;
  private bool played;
  private Vector3 startLeft;
  private Vector3 startRight;
  private float journeyLength;
  private float startTime;
  private float error; // Apply error since dt isn't infinitly small

  void Awake()
  {
    audioS = GetComponent<AudioSource>();
    diskTransL = discL.GetComponent<Transform>();
    diskTransR = discR.GetComponent<Transform>();
    // // // // scale = discL.GetComponent<Transform>().localScale.x; // The scale of the disks. Assuming that both have the same scale
  }

  void Start()
  {
    startLeft = diskTransL.position;
    startRight = diskTransR.position;
    startTime = Time.time;
    error = 0.1f; // Tested by pausing at coincidense and checking if disks overlap
    played = false;
    discL.GetComponent<SpriteRenderer>().color = diskColorL;
    discR.GetComponent<SpriteRenderer>().color = diskColorR;
    xLookAt.GetComponent<SpriteRenderer>().color = xColor;
    Camera.main.backgroundColor = backgroundColor;
    journeyLength = Vector3.Distance(diskTransL.position, diskTransR.position);

    // Attach the selected sound to the audiosource.
    switch (sound)
    {
      case Sounds.sound1:
        audioS.clip = sound1;
        break;
      case Sounds.sound2:
        audioS.clip = sound2;
        break;
      case Sounds.sound3:
        audioS.clip = sound3;
        break;
    }
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
    // This value is used for playing the sound before coincidence. Multiply with 0.001 to convert from ms to s.
    float distanceOffset = speed * (soundOffset * 0.001f) * 2; // Times 2 since they are moving towards each other.
    float triggerDist = 0;

    // Set the distance offset depending on our choice
    switch (soundTiming)
    {
      case SoundTiming.at:
      case SoundTiming.after:
        triggerDist = 0 + error;
        break;
      case SoundTiming.before:
        triggerDist = distanceOffset;
        break;
    }

    if (dist <= triggerDist && !played)
    {
      played = true;
      // If we should play after coincidence, add delay to sound. 
      if (soundTiming == SoundTiming.after)
      {
        audioS.PlayDelayed(0.15f);
        return;
      }
      audioS.Play();
    }
  }
}