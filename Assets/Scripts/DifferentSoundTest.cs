using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifferentSoundTest : MonoBehaviour
{
  [Header("Variables")]
  // public int numberOfRuns = 1;
  [Tooltip("Sound offset [ms]. The delay used in the before and after timings.")]
  public float soundOffset = 150f;
  [Tooltip("The time between each test [s]")]
  public float waitTime = 0.5f;
  [Tooltip("Time to pause at the moment of coincidence [ms]")]
  public float pauseTime;
  [Header("Sounds")]
  public AudioClip[] bounceSounds;

  private bool doneWithTest = true;
  private MoveHandler moveHandler;
  private List<List<string>> testOrder = new List<List<string>>();
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
    while (testOrder.Count != 0)
    {
      // Start the next test
      List<string> newTest = testOrder[0];
      testOrder.RemoveAt(0);
      // newTest[0] = at, before or after. newTest[1] = index of sound to use
      int soundIndex = Int32.Parse(newTest[1]); // convert string to int
      moveHandler.SetTest(newTest[0], 
        bounceSounds[soundIndex], 
        soundOffset, 
        float.Parse(newTest[1]) * 0.001f, 
        "Horizontal");
      doneWithTest = false;
      Debug.Log("Starting Test: " + newTest[0] + ". Sound nr: " + newTest[1]);

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
        choice = "1"; // 1 represents bounce
      }
      if (Input.GetMouseButtonDown(1))
      {
        hasAnswered = true;
        choice = "0"; // 0 represents no bounce
      }
      yield return null;
    }

    string[] answer = new string[5];
        if(newTest[0] == "before") answer[0] = "0";
    if(newTest[0] == "at") answer[0] = "1";
    if(newTest[0] == "after") answer[0] = "2"; 
    answer[1] = "00"; 
    int soundIndex = Int32.Parse(newTest[1]);
    answer[2] = bounceSounds[soundIndex].name;
    answer[3] = "00";
    answer[4] = choice;

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
      int randomIndex = UnityEngine.Random.Range(0, original.Count);
      string timing = original[randomIndex];
      original.RemoveAt(randomIndex);

      // Create 3 different tests, one for each "sound" to use. "i" is the index in the sounds array.
      for (int i = 0; i < bounceSounds.Length; i++)
      {
        List<string> sound = new List<string> { timing, i.ToString() };
        almostScrambled.Add(sound);
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
