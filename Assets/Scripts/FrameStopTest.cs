using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameStopTest : MonoBehaviour
{
  [Header("Variables")]
  // public int numberOfRuns = 1;
  [Tooltip("Sound offset [ms]. The delay used in the before and after timings.")]
  public float soundOffset = 150f;
  [Tooltip("The time between each test [s]")]
  public float waitTime = 2;
  [Tooltip("Time to pause at the moment of coincidence [ms]")]
  public float pauseTime = 0f;

  private bool startTest = false;
  private bool doneWithTest = true;
  private bool startNextTest = false;
  private MoveHandler moveHandler;
  private List<string> testOrder = new List<string>();

  void Awake()
  {
    moveHandler = GetComponent<MoveHandler>();
  }

  public IEnumerator StartTest()
  {
    testOrder = RandomizeTests();
    yield return StartCoroutine(RunTest());
  }

  IEnumerator RunTest()
  {
    while (!doneWithTest)
    {
      doneWithTest = moveHandler.MoveDisks();
      yield return null; // Has to return in order for unity to update frame
    }
    yield return StartCoroutine(StartNext());
  }

  // Pause for given time and then continue with the next test
  IEnumerator StartNext()
  {
    if (testOrder.Count == 0) // All tests performed when 0
      yield break;

    yield return new WaitForSeconds(waitTime);

    // Start the next test
    string newTest = testOrder[0];
    testOrder.RemoveAt(0);
    moveHandler.SetTest(newTest, soundOffset, pauseTime * 0.001f);
    doneWithTest = false;
    Debug.Log("Starting Test: " + newTest);

    yield return StartCoroutine(RunTest());
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
}
