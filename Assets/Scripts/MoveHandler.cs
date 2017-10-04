﻿using System.Collections;
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
  public float error = 0.1f;
  [Header("End positions Cross Move")]
  public Vector2 endLeft;
  public Vector2 endRight;

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

  [Header("Pendulum variables")]
  public float gravity = 1f;
  public Vector3 centerPos = new Vector3(0f, 0f, 0f);
  public float rPendulum;
  public float alpha = Mathf.PI / 2f;
  public float alphaVel = 0;
  public float alphaAcc;
  public float beta;
  public float betaVel;
  public float betaAcc;
  public Vector2 startLP;
  public Vector2 startRP;

  private float startAlpha;
  private float startBeta;
  private float eclipseTime = 0.74f;
  private float pendulumTimeSound = 0;
  private float travelDistPendulum = 0;
  private float halfWayTime = 0;

  void Awake()
  {
    audioS = GetComponent<AudioSource>();
  }

  void Start()
  {
    startLeft = discTransL.position;
    startRight = discTransR.position;
    startRP = centerPos + (new Vector3(Mathf.Sin(alpha), Mathf.Cos(alpha), 0f) * rPendulum);
    startLP = centerPos + (new Vector3(Mathf.Sin(beta), Mathf.Cos(beta), 0f) * rPendulum);
    beta = 2 * Mathf.PI - alpha;
    rPendulum = discTransR.position.x - centerPos.x;
    startAlpha = alpha;
    startBeta = beta;
    travelDistPendulum = Mathf.PI * Vector3.Distance(startLeft, startRight) / 2;
  }

  public void SetTest(string soundTiming, AudioClip bounceSound, float soundOffset, float pauseTime, string traject)
  {
    this.soundTiming = soundTiming;
    this.soundOffset = soundOffset;
    this.pauseTime = pauseTime;
    this.bounceSound = bounceSound;
    audioS.clip = this.bounceSound;
    discTransL.position = startLeft;
    discTransR.position = startRight;
    startTime = Time.time;
    hasPlayedSound = false;
    hasPausedTime = pauseTime == 0;
    endLeft = new Vector2(startRight.x, startRight.y);
    endRight = new Vector2(startLeft.x, startLeft.y);

    if (traject == "Horizontal")
    {
      journeyLength = Vector3.Distance(discTransL.position, discTransR.position);
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

    //TODO: Pendulum at, before and after. Cross
    else if (traject == "Pendulum" || traject == "Cross")
    {
      journeyLength = Vector3.Distance(discTransL.position, discTransR.position) * Mathf.PI / 2f;
      switch (soundTiming)
      {
        case "at":
          pendulumTimeSound = eclipseTime;
          // audioS.PlayDelayed(eclipseTime);
          break;
        case "after":
          pendulumTimeSound = eclipseTime + (soundOffset * 0.001f);
          // audioS.PlayDelayed(eclipseTime + (soundOffset * 0.001f) + GetComponent<DifferentTrajectoriesTest>().waitTime);
          break;
        case "before":
          pendulumTimeSound = eclipseTime - (soundOffset * 0.001f);
          // audioS.PlayDelayed(eclipseTime - (soundOffset * 0.001f));
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
    if (distCovered >= triggerDist && !hasPlayedSound)
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
    if (distCovered >= Vector3.Distance(startLeft, startRight) / 2 && !hasPausedTime)
    {
      StartCoroutine(FramePause());
      hasPausedTime = true;
    }
    return fracJourney >= 1; // Return true when whole distance is traveled
  }

  public bool MovePendulum()
  {
    float l = Mathf.Abs(alpha - startAlpha) * rPendulum;
    // Move the disks towards each other
    discTransL.position = centerPos + (new Vector3(Mathf.Sin(alpha), Mathf.Cos(alpha), 0f) * rPendulum);
    discTransR.position = centerPos + (new Vector3(Mathf.Sin(beta), Mathf.Cos(beta), 0f) * rPendulum);
    alphaAcc = (gravity / rPendulum) * Mathf.Sin(alpha);
    alphaVel += alphaAcc * (Time.time - startTime);
    alpha += Time.deltaTime * alphaVel;
    betaAcc = (gravity / rPendulum) * Mathf.Sin(beta);
    betaVel += betaAcc * (Time.time - startTime);
    beta += Time.deltaTime * betaVel;

    // Distance between the disks
    float dist = Vector3.Distance(discTransL.position, discTransR.position);

    // If we should pause at the moment of coincidence
    if (dist <= 0 + error && !hasPausedTime)
    {
      StartCoroutine(FramePause());
      hasPausedTime = true;
    }

    if (l >= travelDistPendulum / 2 && !hasPlayedSound)
    {
      audioS.Play();
      hasPlayedSound = true;
    }

    return l >= 105;
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
