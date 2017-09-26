using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MoveHandler))]
public class TestHandler : MonoBehaviour
{
  [Header("Variables")]
  // public int numberOfRuns = 1;
  [Tooltip("Sound offset [ms]. The delay used in the before and after timings.")]
  public float soundOffset = 150f;
  [Tooltip("The time between each test [s]")]
  public float waitTime = 2;
  [Tooltip("Time to pause at the moment of coincidence [ms]")]
  public float pauseTime = 0f;
  [Header("Colors")]
  public Color backgroundColor;
  public Color diskColorL;
  public Color diskColorR;
  public Color xColor;
  // [Header("UI components")]
  // public Button nextButton;
  private MoveHandler moveHandler;
  private bool doneWithTest = true;
  private bool startNextTest = true;
  private List<string> testOrder = new List<string>();

  void Awake()
  {
    moveHandler = GetComponent<MoveHandler>();
  }

  // Use this for initialization
  void Start()
  {
    moveHandler.SetColors(backgroundColor, diskColorL,
      diskColorR, xColor);
    testOrder = RandomizeTests();
  }

  // Update is called once per frame
  void Update()
  {
    // If test animation isn't done yet, continue.
    if (!doneWithTest)
    {
      doneWithTest = moveHandler.MoveDisks();
      // When we are done, allow the next test to start.
      if (doneWithTest) startNextTest = true;
    }

    // If we are done with the test and should play the next one. 
    if (doneWithTest && startNextTest)
    {
      StartCoroutine(AutoStartNext());
      startNextTest = false;
    }
  }

  public void RunNextTest()
  {
    if (testOrder.Count == 0) // All tests performed when 0
      return;
    string newTest = testOrder[0];
    testOrder.RemoveAt(0);
    moveHandler.SetTest(newTest, soundOffset, pauseTime * 0.001f);
    doneWithTest = false;
    Debug.Log("Starting Test: " + newTest);
  }

  // Gives a random order of "at", "before" and "after"
  // TODO: include framestop
  public List<string> RandomizeTests()
  {
    List<string> randomTest = new List<string>();
    List<string> tests = new List<string> { "at", "before", "after" };
    while (tests.Count != 0)
    {
      int randomIndex = Random.Range(0, tests.Count);
      string val = tests[randomIndex];
      tests.RemoveAt(randomIndex);
      randomTest.Add(val);
    }
    return randomTest;
  }

  // Pause for given time and then continue with the next test
  IEnumerator AutoStartNext()
  {
    yield return new WaitForSeconds(waitTime);
    RunNextTest();
    yield return null;
  }
}
