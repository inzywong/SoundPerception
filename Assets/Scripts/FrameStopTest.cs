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
  public float pauseTime1, pauseTime2, pauseTime3;

  private bool doneWithTest = true;
  private MoveHandler moveHandler;
  private List<List<string>> testOrder = new List<List<string>>();

  // private string[,] results;
  private List<string[]> results = new List<string[]>();

  void Awake()
  {
    moveHandler = GetComponent<MoveHandler>();
  }

  public IEnumerator StartTest(System.Action<List<string[]>> result)
  {
    testOrder = RandomizeTests();

    yield return new WaitForSeconds(waitTime);
    yield return StartCoroutine(RunTest());
    result(results);
  }

  IEnumerator RunTest()
  {
    while (testOrder.Count != 8)
    {
      // Start the next test
      List<string> newTest = testOrder[0];
      testOrder.RemoveAt(0);
      moveHandler.SetTest(newTest[0], soundOffset, float.Parse(newTest[1]) * 0.001f);
      doneWithTest = false;
      Debug.Log("Starting Test: " + newTest[0] + ". Frame pause: " + newTest[1]);

      while (!doneWithTest)
      {
        doneWithTest = moveHandler.MoveDisks();
        yield return null; // Has to return in order for unity to update frame
      }

      // Wait for user to answer
      yield return StartCoroutine(UserAnswer(newTest));
    }
  }

  IEnumerator UserAnswer(List<string> newTest)
  {
    bool hasAnswered = false;
    string choice = "";
    while (!hasAnswered)
    {
      if (Input.GetMouseButtonDown(0))
      {
        hasAnswered = true;
        choice = "bounce";
      }
      if (Input.GetMouseButtonDown(1))
      {
        hasAnswered = true;
        choice = "stream";
      }
      yield return null;
    }
    string[] answer = new string[] { newTest[0], newTest[1], choice };

    results.Add(answer);

    yield return null;
  }

  // Gives a random order of "at", "before" and "after"
  // TODO: include framestop
  public List<List<string>> RandomizeTests()
  {
    List<string> tests = new List<string> { "at", "before", "after" };
    List<List<string>> randomTests = Scramble(tests);
    return randomTests;
  }

  // Not pretty but it works. Returns random order of the tests.
  public List<List<string>> Scramble(List<string> original)
  {
    List<List<string>> almostScrambled = new List<List<string>>();

    while (original.Count != 0)
    {
      int randomIndex = Random.Range(0, original.Count);
      string timing = original[randomIndex];
      original.RemoveAt(randomIndex);

      // Create 3 different tests, one for each "frame" stop type.
      List<string> first = new List<string> { timing, pauseTime1.ToString() };
      List<string> second = new List<string> { timing, pauseTime2.ToString() };
      List<string> third = new List<string> { timing, pauseTime3.ToString() };

      almostScrambled.Add(first); almostScrambled.Add(second); almostScrambled.Add(third);
    }

    // Repeat once more to make it scrambled
    List<List<string>> scrambled = new List<List<string>>();
    while (almostScrambled.Count != 0)
    {
      int randomIndex = Random.Range(0, almostScrambled.Count);
      List<string> temp = almostScrambled[randomIndex];
      almostScrambled.RemoveAt(randomIndex);
      scrambled.Add(temp);
    }

    return scrambled;
  }
}
