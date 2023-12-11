using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConsumable
{
    // The "target" game object can be used to search for components necessary to the effect's function such as PlayerCharacter
    void Consume(GameObject target);
}
