using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickableItem
{
    void Use(Transform viewTransform);
    void OnPickup();
    
}
