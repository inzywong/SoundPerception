using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;

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
  [Header("File name for saving data")]
  [Tooltip("Recomended filetype is .csv. No path will default to project folder")]
  public string @filePath;


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
    List<string[]> results = new List<string[]>();
    yield return StartCoroutine(frameStopTest.StartTest(value => results = value));
    SaveResults(results);

    theCanvas.SetActive(true);
  }

  void SaveResults(List<string[]> results)
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("New Test1 entry");
    for (int i = 0; i < results.Count; i++)
    {
      sb.AppendLine(string.Join(",", results[i]));
    }
    File.AppendAllText(filePath, sb.ToString());
    Debug.Log("Data saved!");
  }

  void SetColors()
  {
    discL.GetComponent<SpriteRenderer>().color = diskColorL;
    discR.GetComponent<SpriteRenderer>().color = diskColorR;
    xLookAt.GetComponent<SpriteRenderer>().color = xColor;
    Camera.main.backgroundColor = backgroundColor;
  }
}
