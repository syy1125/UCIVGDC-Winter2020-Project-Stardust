using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public CelestialBody Body;

    private void Start()
    {
        if (Body == null) return;

        transform.position = Body.GetGlobalPositionAndVelocityAt(0).Item1;
    }
}
