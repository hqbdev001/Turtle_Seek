using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrtIngameSettings : MonoBehaviour
{
    [Serializable]
    public struct TurtleSettings
    {
        public Vector2 startPosition;
        public Vector2 startSize;
    }

    [SerializeField]
    private int turtleCount = 4;

    [SerializeField]
    private float turtleShowingTime = 5.0f;

    [SerializeField]
    private bool hidingTurtle = true;

    [SerializeField]
    private float guessingTime = 30.0f;

    [SerializeField]
    private List<TurtleSettings> turtleSettings;


    public int TurtleCount => turtleCount;
    public int SettingCount => turtleSettings.Count;
    public float TurtleShowingTime => turtleShowingTime;
    public bool HidingTurtle => hidingTurtle;
    public float GuessingTime => guessingTime;

    public TurtleSettings GetTurtleSettings(int index)
    {
        if (index < 0 || index >= turtleSettings.Count)
        {
            Debug.LogError("Turtle index out of range: " + index);
            return default;
        }
        return turtleSettings[index];
    }
}
