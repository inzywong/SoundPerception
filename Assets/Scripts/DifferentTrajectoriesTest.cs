using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifferentTrajectoriesTest : MonoBehaviour
{
  [Tooltip("Time to pause at the moment of coincidence [ms]")]
  [ShowOnly]
  public float pauseTime;
  [Header("Sound")]
  public AudioClip bounceSound;
  [Header("Trajectories to use")]
  public bool horizontal;
  public bool cross;
  public bool pendulum;

  private float soundOffset = 150f;
  private float waitTime = 0.5f;
  private bool doneWithTest = true;
  private MoveHandler moveHandler;
  private List<List<string>> testOrder = new List<List<string>>();
  private List<string[]> results = new List<string[]>();
  private List<string> trajectories = new List<string>(); // 1 = cross, 2 = pendulum

  void Awake()
  {
    moveHandler = GetComponent<MoveHandler>();
  }

  void Start()
  {
    if (horizontal) trajectories.Add("Horizontal");
    if (cross) trajectories.Add("Cross");
    if (pendulum) trajectories.Add("Pendulum");
  }

  public IEnumerator StartTest(System.Action<List<string[]>> result)
  {
    testOrder = RandomizeTests();
    results.Clear();

    yield return new WaitForSeconds(waitTime);
    yield return StartCoroutine(RunTest());
    result(results);
  }

  IEnumerator RunTest()
  {
    while (testOrder.Count != 0)
    {
      // Start the next test
      List<string> newTest = testOrder[0];
      testOrder.RemoveAt(0);
      // newTest[0] = at, before or after. newTest[1] = index of sound to use
      moveHandler.SetTest(newTest[0],
        bounceSound,
        soundOffset,
        pauseTime * 0.001f,
        newTest[1]); // newTest[1] = which trajectory
      doneWithTest = false;
      Debug.Log("Starting Test: " + newTest[0] + ". Trajectory nr: " + newTest[1]);

      while (!doneWithTest)
      {
        if (newTest[1] == "Pendulum")
          doneWithTest = moveHandler.MovePendulum();
        else
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
      if (Input.GetKeyDown(KeyCode.Z))
      {
        hasAnswered = true;
        choice = "1"; // Bounce
      }
      if (Input.GetKeyDown(KeyCode.M))
      {
        hasAnswered = true;
        choice = "0"; // No bounce
      }
      yield return null;
    }

    string[] answer = new string[5];
    if (newTest[0] == "none") answer[0] = "0";
    if (newTest[0] == "before") answer[0] = "1";
    if (newTest[0] == "at") answer[0] = "2";
    if (newTest[0] == "after") answer[0] = "3";
    answer[1] = pauseTime.ToString();
    answer[2] = bounceSound.name;
    if (newTest[1] == "Horizontal") answer[3] = "1";
    if (newTest[1] == "Cross") answer[3] = "2";
    if (newTest[1] == "Pendulum") answer[3] = "3";
    answer[4] = choice;

    results.Add(answer);
    yield return null;
  }

  // Gives a random order of "at", "before" and "after"
  public List<List<string>> RandomizeTests()
  {
    List<string> tests = new List<string> { "at", "before", "after", "none" };
    List<List<string>> randomTests = Scramble(tests);
    return randomTests;
  }

  // Not pretty but it works. Returns random order of the tests.
  public List<List<string>> Scramble(List<string> original)
  {
    List<List<string>> almostScrambled = new List<List<string>>();

    while (original.Count != 0)
    {
      int randomIndex = UnityEngine.Random.Range(0, original.Count);
      string timing = original[randomIndex];
      original.RemoveAt(randomIndex);

      // Create 3 different tests, one for each "sound" to use. "i" is the index in the sounds array.
      for (int i = 0; i < trajectories.Count; i++)
      {
        List<string> trajec = new List<string> { timing, trajectories[i] };
        almostScrambled.Add(trajec);
      }
    }

    // Repeat once more to make it scrambled
    List<List<string>> scrambled = new List<List<string>>();
    while (almostScrambled.Count != 0)
    {
      int randomIndex = UnityEngine.Random.Range(0, almostScrambled.Count);
      List<string> temp = almostScrambled[randomIndex];
      almostScrambled.RemoveAt(randomIndex);
      scrambled.Add(temp);
    }

    return scrambled;
  }
}
