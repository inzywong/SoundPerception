using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MoveHandler : MonoBehaviour
{
  [Header("Objects")]
  public GameObject discL;
  public GameObject discR;
  public GameObject xLookAt;
  [Header("Variables")]
  public float speed = 20f;
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
  private Vector3 startLeft;
  private Vector3 startRight;
  private float startTime;
  private string soundTiming; // "at", "before" or "after"
  private float soundOffset;
  private float distanceOffset;
  private float triggerDist;
  private float journeyLength;
  private bool played;
  private float error; // Apply error since dt isn't infinitly small

  void Awake()
  {
    audioS = GetComponent<AudioSource>();
    diskTransL = discL.GetComponent<Transform>();
    diskTransR = discR.GetComponent<Transform>();
  }

  void Start()
  {
    startLeft = diskTransL.position;
    startRight = diskTransR.position;
    error = 0.1f; // Tested by pausing at coincidense and checking if disks overlap

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

  public void SetColors(Color backgroundColor, Color diskColorL,
    Color diskColorR, Color xColor)
  {
    discL.GetComponent<SpriteRenderer>().color = diskColorL;
    discR.GetComponent<SpriteRenderer>().color = diskColorR;
    xLookAt.GetComponent<SpriteRenderer>().color = xColor;
    Camera.main.backgroundColor = backgroundColor;
  }

  public void SetTest(string soundTiming, float soundOffset)
  {
    this.soundTiming = soundTiming;
    this.soundOffset = soundOffset;
    diskTransL.position = startLeft;
    diskTransR.position = startRight;
    startTime = Time.time;
    played = false;
    journeyLength = Vector3.Distance(diskTransL.position, diskTransR.position);

    // This value is used for playing the sound before coincidence. Multiply with 0.001 to convert from ms to s.
    distanceOffset = speed * (soundOffset * 0.001f) * 2; // Times 2 since they are moving towards each other.
    triggerDist = 0;

    // Set the distance offset depending on our choice
    switch (soundTiming)
    {
      case "at":
      case "after":
        triggerDist = 0 + error;
        break;
      case "before":
        triggerDist = distanceOffset;
        break;
    }
  }

  public bool MoveDisks()
  {
    float distCovered = (Time.time - startTime) * speed; // The distance traveled
    float fracJourney = distCovered / journeyLength;  // How much of the total length has been traveled. 

    // Move the disks towards each other
    diskTransL.position = Vector3.Lerp(startLeft, startRight, fracJourney);
    diskTransR.position = Vector3.Lerp(startRight, startLeft, fracJourney);

    // Distance between the disks
    float dist = Vector3.Distance(diskTransL.position, diskTransR.position);

    // Check if we are within the trigger distance
    if (dist <= triggerDist && !played)
    {
      played = true;
      // If we should play after coincidence, add delay to sound. 
      if (soundTiming == "after")
      {
        audioS.PlayDelayed(soundOffset * 0.001f);
        return false;
      }
      audioS.Play();
    }

    return fracJourney >= 1;
  }
}