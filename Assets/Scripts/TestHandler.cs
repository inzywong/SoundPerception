using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveHandler))]
public class TestHandler : MonoBehaviour
{
  public int numberOfRuns = 1;
  [Header("Colors")]
  public Color backgroundColor;
  public Color diskColorL;
  public Color diskColorR;
  public Color xColor;
  private MoveHandler moveHandler;

  void Awake()
  {
    moveHandler = GetComponent<MoveHandler>();
  }

  // Use this for initialization
  void Start()
  {
    moveHandler.SetColors(backgroundColor, diskColorL,
      diskColorR, xColor);
  }

  // Update is called once per frame
  void Update()
  {
    moveHandler.MoveDisks();
  }
}
