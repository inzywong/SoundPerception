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
  // [Header("File name for saving data")]
  // [Tooltip("Recomended filetype is .csv. No path will default to project folder")]
  // public string filePath = "gathered_data.csv";

  private FrameStopTest frameStopTest;
  private DifferentSoundTest diffSoundTest;
  private DifferentTrajectoriesTest diffTrajTest;

  void Awake()
  {
    frameStopTest = GetComponent<FrameStopTest>();
    diffSoundTest = GetComponent<DifferentSoundTest>();
    diffTrajTest = GetComponent<DifferentTrajectoriesTest>();
  }

  void Start()
  {
    SetColors();
  }

  // Start test one. Called from a button in the scene
  public void StartTest1()
  {
    StartCoroutine(RunTest1());
    theCanvas.SetActive(false);
  }
  IEnumerator RunTest1()
  {
    List<string[]> results = new List<string[]>();
    yield return StartCoroutine(frameStopTest.StartTest(value => results = value));
    SaveResults(results, "Test1_data.csv");

    theCanvas.SetActive(true);
  }

  // Start test two. Called from a button in the scene
  public void StartTest2()
  {
    StartCoroutine(RunTest2());
    theCanvas.SetActive(false);
  }
  IEnumerator RunTest2()
  {
    List<string[]> results = new List<string[]>();
    yield return StartCoroutine(diffSoundTest.StartTest(value => results = value));
    SaveResults(results, "Test2_data.csv");

    theCanvas.SetActive(true);
  }

  public void StartTest3()
  {
    StartCoroutine(RunTest3());
    theCanvas.SetActive(false);
  }
  IEnumerator RunTest3()
  {
    List<string[]> results = new List<string[]>();
    yield return StartCoroutine(diffTrajTest.StartTest(value => results = value));
    SaveResults(results, "Test3_data.csv");

    theCanvas.SetActive(true);
  }


  // Save the results to a file
  void SaveResults(List<string[]> results, string fileName)
  {
    StringBuilder sb = new StringBuilder();
    //sb.AppendLine(title);
    for (int i = 0; i < results.Count; i++)
    {
      sb.AppendLine(string.Join(",", results[i]));
    }
    File.AppendAllText(fileName, sb.ToString());
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
