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

  private Transform discTransL;
  private Transform discTransR;
  private AudioSource audioS;
  private Vector3 startLeft;
  private Vector3 startRight;
  private float startTime;
  private string soundTiming; // "at", "before" or "after"
  private float soundOffset;
  private float distanceOffset;
  private float triggerDist;
  private float journeyLength;
  private bool hasPlayedSound;
  private float pauseTime;
  private bool hasPausedTime;
  private float error; // Apply error since dt isn't infinitly small

  void Awake()
  {
    audioS = GetComponent<AudioSource>();
    discTransL = discL.GetComponent<Transform>();
    discTransR = discR.GetComponent<Transform>();
  }

  void Start()
  {
    startLeft = discTransL.position;
    startRight = discTransR.position;
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

  public void SetTest(string soundTiming, float soundOffset, float pauseTime)
  {
    this.soundTiming = soundTiming;
    this.soundOffset = soundOffset;
    this.pauseTime = pauseTime;
    discTransL.position = startLeft;
    discTransR.position = startRight;
    startTime = Time.time;
    hasPlayedSound = false;
    hasPausedTime = pauseTime == 0;

    journeyLength = Vector3.Distance(discTransL.position, discTransR.position);

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
    discTransL.position = Vector3.Lerp(startLeft, startRight, fracJourney);
    discTransR.position = Vector3.Lerp(startRight, startLeft, fracJourney);

    // Distance between the disks
    float dist = Vector3.Distance(discTransL.position, discTransR.position);

    if (dist <= 0 + error && !hasPausedTime)
    {
      Debug.Log("Pause");
      StartCoroutine(FramePause());
      hasPausedTime = true;
    }

    // Check if we are within the trigger distance
    if (dist <= triggerDist && !hasPlayedSound)
    {
      hasPlayedSound = true;
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

  // Function for showing and hiding the disks
  public void ShowDisks(bool status)
  {
    discL.SetActive(status);
    discR.SetActive(status);
  }

  // Pause the movement for the given time
  IEnumerator FramePause()
  {
    Time.timeScale = 0f;
    float pauseEndTime = Time.realtimeSinceStartup + pauseTime;
    while (Time.realtimeSinceStartup < pauseEndTime)
    {
      yield return 0;
    }
    Time.timeScale = 1;
  }
}