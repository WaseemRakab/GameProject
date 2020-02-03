using System;

/**
 * Vector3 For Each Position,Scale of An Interactable Object in-Game
 */
[Serializable]
public class Vector
{
    public float X;
    public float Y;
    public float Z;
    public Vector(float X, float Y, float Z)
    {
        this.X = X;
        this.Y = Y;
        this.Z = Z;
    }
}