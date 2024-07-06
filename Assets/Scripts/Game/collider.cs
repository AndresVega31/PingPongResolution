using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupPhysicsMaterials2D : MonoBehaviour
{
    void Start()
    {
        // Crear y configurar el material de físicas 2D
        PhysicsMaterial2D bouncyMaterial2D = new PhysicsMaterial2D();
        bouncyMaterial2D.friction = 0;
        bouncyMaterial2D.bounciness = 1;

        // Asignar el material a los colliders relevantes
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.sharedMaterial = bouncyMaterial2D;
        }
    }
}