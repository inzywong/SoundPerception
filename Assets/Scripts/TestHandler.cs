using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MoveHandler))]
public class TestHandler : MonoBehaviour
{
  [Header("Colors")]
  public Color backgroundColor;
  public Color diskColorL;
  public Color diskColorR;
  public Color xColor;
  [Header("Objects")]
  public GameObject discL;
  public GameObject discR;
  public GameObject xLookAt;
  [Header("UI components")]
  public GameObject theCanvas;
  private FrameStopTest frameStopTest;

  private List<string[]> results;

  void Awake()
  {
    frameStopTest = GetComponent<FrameStopTest>();
  }

  // Use this for initialization
  void Start()
  {
    SetColors();
  }

  public void StartTest1()
  {
    StartCoroutine(RunTest1());
    theCanvas.SetActive(false);
  }

  IEnumerator RunTest1()
  {
    yield return StartCoroutine(frameStopTest.StartTest(value => results = value));

    for (int i = 0; i < results.Count; i++)
    {
      Debug.Log(results[i][0] + " " + results[i][1] + " " + results[i][2]);
    }
    theCanvas.SetActive(true);
  }

  void SaveResults() { }

  void SetColors()
  {
    discL.GetComponent<SpriteRenderer>().color = diskColorL;
    discR.GetComponent<SpriteRenderer>().color = diskColorR;
    xLookAt.GetComponent<SpriteRenderer>().color = xColor;
    Camera.main.backgroundColor = backgroundColor;
  }
}
