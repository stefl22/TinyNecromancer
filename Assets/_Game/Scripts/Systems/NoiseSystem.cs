using System;
using UnityEngine;

public static class NoiseSystem
{
    public static event Action<Vector3, float, GameObject> NoiseEmitted;

    public static void EmitNoise(
        Vector3 position,
        float radius,
        GameObject source)
    {
        NoiseEmitted?.Invoke(
            position,
            radius,
            source
        );
    }
}