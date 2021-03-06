﻿using UnityEngine;
using System.Collections;

public class Deactivator : MonoBehaviour
{

    public Rigidbody attachedRigid;
    public MeshRenderer attachedRenderer;
    public BoxCollider attachedCollider;
    public MeshFilter attachedFilter;
 


    public void TriggerDeactivation(float timeTillDeactivation)
    {        
        Invoke("Deactivate", timeTillDeactivation);
    }

    void Deactivate()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Destroy(box);
        }

        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            Destroy(renderer);
            attachedRenderer = gameObject.AddComponent<MeshRenderer>();

            attachedFilter = gameObject.AddComponent<MeshFilter>();
        }

        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter == null)
        {
            attachedFilter = gameObject.AddComponent<MeshFilter>();
        }

        if (attachedRigid != null)
        {
            attachedRigid.angularDrag = 0.0f;
            attachedRigid.drag = 0.0f;
            attachedRigid.useGravity = true;
        }

        if (gameObject != null)
        {
            ObjectsPool.Despawn(gameObject);
        }
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}
