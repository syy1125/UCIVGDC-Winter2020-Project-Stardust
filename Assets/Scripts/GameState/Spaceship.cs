using System;

public class Spaceship
{
	private SpaceshipTemplate _template;

	// If the spaceship is in orbit around a planet, OrbitingBody is set and FreeOrbit is null
	// If the spaceship is moving through space, FreeOrbit is set and OrbitingBody is null
	public CelestialBody OrbitingBody { get; private set; }
	public Orbit FreeOrbit { get; private set; }

	public void SetOrbitingBody(CelestialBody body)
	{
		OrbitingBody = body;
		FreeOrbit = null;
	}

	public void SetFreeOrbit(Orbit orbit)
	{
		FreeOrbit = orbit;
		OrbitingBody = null;
	}
}