using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static float turn;
    public float next_turn;
    public float turn_speed;

    public GameObject[] planets;

    void Start()
    {
        turn = 1.0F;
        turn_speed = 4f;
    }

    void Update()
    {
        turn = Mathf.Lerp(turn, next_turn, turn_speed * 0.01f);
    }

    public void AdvanceTurn()
    {
        next_turn++;
    }
}