using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public CelestialBody Body;
    public int nanite_count;
    public int nanite_rate;
    public int material_count;
    public int material_rate;

    private void Start()
    {
        nanite_count = 0;
        nanite_rate = 0;
        material_count = 0;
        material_rate = 0;
    }

    private void Update()
    {
        if (Body == null) return;

        this.transform.position = Body.GetGlobalPositionAndVelocityAt(GameController.turn).Item1;
    }

    public Vector3 GetPosition(float turn)
    {
        return Body.GetGlobalPositionAndVelocityAt(turn).Item1;
    }
}
