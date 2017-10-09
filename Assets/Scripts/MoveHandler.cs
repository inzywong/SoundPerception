using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MoveHandler : MonoBehaviour
{
  [Header("Objects")]
  public Transform discTransL;
  public Transform discTransR;
  [Header("Variables")]
  public float speed = 20f;
  // Apply error since dt isn't infinitly small in real time
  // Tested by pausing at coincidense and checking if disks overlap
  // public float error = 0.1f;
  // [Header("End positions Cross Move")]
  private Vector3 endLeft;
  private Vector3 endRight;

  private AudioSource audioS;
  private AudioClip bounceSound;
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

  // [Header("Pendulum variables (Keep gravity at 8!)")]
  private float gravity = 8f;
  public Transform centerPos;
  private float rPendulum = 40;
  // private float alpha = -45 * Mathf.PI / 180;
  private float alpha;
  private float alphaVel;
  private float alphaAcc;
  private float beta;
  private float betaVel;
  private float betaAcc;
  private Vector2 startLP;
  private Vector2 startRP;

  private float startAlpha;
  private float startBeta;
  private float eclipseTime = 0.74f;
  private float pendulumSoundOffset = 0;
  private float travelDistPendulum = 0;
  private float halfWayTime = 0;

  private float linearTrajOffset = 0;

  void Awake()
  {
    audioS = GetComponent<AudioSource>();
  }

  void Start()
  {
    startLeft = discTransL.position;
    startRight = discTransR.position;
    // Angle from hangpoint to starting position
    alpha = Vector3.Angle(-transform.up, centerPos.position - discTransR.position) * Mathf.PI / 180;
    beta = 2 * Mathf.PI - alpha;
    // startRP = centerPos.position + (new Vector3(Mathf.Sin(alpha), Mathf.Cos(alpha), 0f) * rPendulum);
    // startLP = centerPos.position + (new Vector3(Mathf.Sin(beta), Mathf.Cos(beta), 0f) * rPendulum);
    rPendulum = (discTransR.position - centerPos.position).magnitude;
    startAlpha = alpha;
    startBeta = beta;
    travelDistPendulum = rPendulum * ((Mathf.PI - alpha) * 2);
    // Debug.Log("Horizontal distance: " + Vector3.Distance(startLeft, startRight));
    // Debug.Log("Pendulum distance: " + travelDistPendulum);
  }

  public void SetTest(string soundTiming, AudioClip bounceSound, float soundOffset, float pauseTime, string traject)
  {
    this.soundTiming = soundTiming;
    this.soundOffset = soundOffset;
    this.pauseTime = pauseTime;
    this.bounceSound = bounceSound;
    audioS.clip = this.bounceSound;
    // discTransL.position = startLeft;
    // discTransR.position = startRight;
    startTime = Time.time;
    hasPlayedSound = false;
    hasPausedTime = pauseTime == 0;

    if (traject == "Horizontal" || traject == "Cross")
    {
      if (traject == "Horizontal")
      {
        endLeft = new Vector2(startRight.x, startRight.y);
        endRight = new Vector2(startLeft.x, startLeft.y);
        // Adjustments to make them coincide exactly
        startLeft = new Vector3(-70, 0, 0);
        startRight = new Vector3(70, 0, 0);
        linearTrajOffset = -0.5f;
      }
      else if (traject == "Cross")
      {
        endLeft = new Vector3(startRight.x, startRight.y - startRight.x * 0.5f);
        endRight = new Vector3(startLeft.x, startLeft.y - startRight.x * 0.5f);
        // Adjustments to make them coincide exactly
        startLeft = new Vector3(-69.1f, 0, 0);
        startRight = new Vector3(70, 0, 0);
        linearTrajOffset = -0.3f;
      }

      journeyLength = Vector3.Distance(startRight, endRight);
      float halfD = journeyLength / 2;

      float timeToHalfD = halfD / speed;
      // Need seperate for this since we freeze time at coincidense.
      float distanceForBefore = speed * (timeToHalfD - (soundOffset * 0.001f));
      // Set the distance offset depending on our choice
      switch (soundTiming)
      {
        case "at":
        case "after":
          triggerDist = halfD;
          break;
        case "before":
          triggerDist = distanceForBefore;
          break;
      }
    }

    //TODO: Pendulum at, before and after
    else if (traject == "Pendulum")
    {
      endLeft = new Vector3(startRight.x, startRight.y);
      endRight = new Vector3(startLeft.x, startLeft.y);
      alpha = startAlpha;
      beta = startBeta;
      alphaVel = 0;
      betaVel = 0;
      float offset = -2.5f; // Tested to get this.

      switch (soundTiming)
      {
        case "at":
        case "after":
          pendulumSoundOffset = (travelDistPendulum / 2) + offset;
          break;
        case "before":
          pendulumSoundOffset = 70.34f; // Tested by printing time and distance
          break;
      }
    }
  }

  public bool MoveDisks()
  {
    float distCovered = (Time.time - startTime) * speed; // The distance traveled
    float fracJourney = distCovered / journeyLength;  // How much of the total length has been traveled.

    // Move the disks towards each other
    discTransL.position = Vector3.Lerp(startLeft, endLeft, fracJourney);
    discTransR.position = Vector3.Lerp(startRight, endRight, fracJourney);

    // Check if we are within the sound trigger distance
    if (distCovered >= triggerDist && !hasPlayedSound && soundTiming != "none")
    {
      hasPlayedSound = true;
      // If we should play after coincidence, add delay to sound, since we might freeze time.
      if (soundTiming == "after")
      {
        audioS.PlayDelayed(soundOffset * 0.001f);
      }
      else
      {
        audioS.Play();
      }
    }

    // If we should pause at the moment of coincidence
    if (distCovered >= journeyLength / 2 + linearTrajOffset && !hasPausedTime)
    {
      // Time.timeScale = 0;
      StartCoroutine(FramePause());
      hasPausedTime = true;
    }

    return fracJourney >= 1; // Return true when whole distance is traveled
  }

  public bool MovePendulum()
  {
    float distCovered = Mathf.Abs(alpha - startAlpha) * rPendulum;
    // Move the disks towards each other
    discTransR.position = centerPos.position + (new Vector3(Mathf.Sin(alpha), Mathf.Cos(alpha), 0f) * rPendulum);
    discTransL.position = centerPos.position + (new Vector3(Mathf.Sin(beta), Mathf.Cos(beta), 0f) * rPendulum);
    alphaAcc = (gravity / rPendulum) * Mathf.Sin(alpha);
    alphaVel += alphaAcc * (Time.time - startTime);
    alpha += Time.deltaTime * alphaVel;
    betaAcc = (gravity / rPendulum) * Mathf.Sin(beta);
    betaVel += betaAcc * (Time.time - startTime);
    beta += Time.deltaTime * betaVel;

    // Subtract 5 since dt isn't small enough in real time
    if (distCovered >= pendulumSoundOffset && !hasPlayedSound && soundTiming != "none")
    {
      hasPlayedSound = true;
      if (soundTiming == "after")
      {
        audioS.PlayDelayed(soundOffset * 0.001f);
      }
      else
      {
        audioS.Play();
      }
    }

    // Debug.Log("Dist travel: " + distCovered + " " + (Time.time - startTime));
    // Always at coincidence!
    float offset = -2.5f;
    if (distCovered >= (travelDistPendulum / 2) + offset && !hasPausedTime)
    {
      // Debug.Log("Dist travel: " + distCovered + " " + (Time.time - startTime) + " FREEZE!!!!!!!!!!!!!!!!!!!!!!! ");
      // Time.timeScale = 0;
      StartCoroutine(FramePause());
      hasPausedTime = true;
    }
    return distCovered >= travelDistPendulum * 0.8;
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
