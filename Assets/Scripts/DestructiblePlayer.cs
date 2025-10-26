using System;
using UnityEngine;

public class DestructiblePlayer : DestructibleObject
{

    public override void ReactTo(ElementType other)
    {
        if (IsOpposingElement(other))
        {
            // put fade out / respawn logic here
        }
        base.ReactTo(other);
    }
}
