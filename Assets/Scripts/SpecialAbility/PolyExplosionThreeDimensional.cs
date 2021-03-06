﻿using UnityEngine;
using System.Collections.Generic;

public class PolyExplosionThreeDimensional : MonoBehaviour
{
    public GameObject explosion;

    public bool explode = false;
    public bool lowFiExplosion = false;
    public float extrudeFactor;
    public int healthValue = 60;
    
    public bool explodeable = true;
    public bool respawn = false;
    public int timeTillRespawn = 10;

    private string pooledObjectName = "Fragment3DObject";

    private int vertexCount;
    private int step;

    private Vector3 scaleFactor;

    private int health;


    private int matchedIndex;
    private DestructibleRespawn respawnScript;
    public List<Deactivator> deactivators;

    [Header("Power Up")]
    [SerializeField]
    [Tooltip("Power up prefab for line cutting.")]
    protected GameObject powerUpPrefabLineCut;
    [SerializeField]
    [Tooltip("Power up prefab for mango explosion.")]
    protected GameObject powerUpPrefabMango;

    [SerializeField]
    [Tooltip("Probability for a power up spawn (line).")]
    protected float powerUpProbabilityLineCut = 0.05f;
    [SerializeField]
    [Tooltip("Probability for a power up spawn (mango).")]
    protected float powerUpProbabilityMango = 0.05f;

    public struct Tri
    {
        public int x;
        public int y;
        public int z;

        public Tri(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    private int grandStep = 3;
    private List<Tri> triList = new List<Tri>();

    MeshFilter MF;
    MeshRenderer MR;
    Mesh M;
    Vector3[] verts;
    Vector3[] normals;
    Vector2[] uvs;

    GameObject GO;
    Mesh mesh;
    Vector3[] newVerts;
    Vector3[] newNormals;
    Vector2[] newUvs;

    // Use this for initialization
    void Start()
    {
        MF = GetComponent<MeshFilter>();
        MR = GetComponent<MeshRenderer>();
        M = MF.sharedMesh;
        verts = M.vertices;
        normals = M.normals;
        uvs = M.uv;
        vertexCount = M.vertexCount;

        step = vertexCount / 90;
        while (step % 3 != 0)
        {
            step++;
        }

        if(lowFiExplosion){
            grandStep *= 3;
        }

        //scaleFactor = 0.03f * step * 2;
        scaleFactor = transform.localScale;
       
        health = healthValue;

        if (respawn)
        {
            respawnScript = gameObject.AddComponent<DestructibleRespawn>();
        }
        deactivators = new List<Deactivator>();
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (explode)
        {
            explode = false;
            ExplodePartial(0);
        }
    }


    private void GetTriangles(int[] indices)
    {
        triList = new List<Tri>();
        for (int i = 0; i < indices.Length; i += 3)
        {
            triList.Add(new Tri(indices[i], indices[i + 1], indices[i + 2]));
        }
    }


    private bool GetMatchingTri(int x, int y, int z)
    {
        foreach (Tri tri in triList)
        {
            if (((tri.x == x && tri.y == y) || (tri.x == y && tri.y == x)) && tri.z != z) {// || (tri.x == x && tri.z == y) || (tri.x == y && tri.z == x)||(tri.y ==x && tri.z == y) || (tri.y ==y && tri.z == x))

                matchedIndex = tri.z;
                triList.Remove(tri);
                return true;
            }

            if (((tri.x == x && tri.z == y) || (tri.x == y && tri.z == x)) && tri.y != z)
            {
                matchedIndex = tri.y;
                triList.Remove(tri);
                return true;
            }

            if (((tri.y == x && tri.z == y) || (tri.y == y && tri.z == x)) && tri.x != z)
            {
                matchedIndex = tri.x;
                triList.Remove(tri);
                return true;
            }
        }
        return false;
    }

    private void ExplodePartial(int start)
    {
        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {
            int[] indices = M.GetTriangles(submesh);
            GetTriangles(indices);

            for (int i = triList.Count - 1; i >= 0; i -= grandStep)
            {
                Tri tri = triList[i];

                Vector3 direction1 = Vector3.Normalize(-normals[tri.x]) * extrudeFactor;
                Vector3 direction2;
                int[] triangles = new int[0];

                mesh = new Mesh();
                

                if (GetMatchingTri(tri.x, tri.y, tri.z))
                {
                    newVerts = new Vector3[8];
                    newNormals = new Vector3[8];
                    newUvs = new Vector2[8];
                    direction2 = Vector3.Normalize(-normals[matchedIndex]) * extrudeFactor;

                    newVerts[0] = verts[tri.x];
                    newUvs[0] = uvs[tri.x];
                    newNormals[0] = normals[tri.x];

                    newVerts[1] = verts[tri.y];
                    newUvs[1] = uvs[tri.y];
                    newNormals[1] = normals[tri.y];

                    newVerts[2] = verts[tri.z];
                    newUvs[2] = uvs[tri.z];
                    newNormals[2] = normals[tri.z];

                    newVerts[3] = verts[matchedIndex];
                    newUvs[3] = uvs[matchedIndex];
                    newNormals[3] = normals[matchedIndex];

                    newVerts[4] = newVerts[0] + direction1;
                    newUvs[4] = uvs[tri.x];
                    newNormals[4] = newNormals[0];

                    newVerts[5] = newVerts[1] + direction1;
                    newUvs[5] = uvs[tri.y];
                    newNormals[5] = newNormals[1];

                    newVerts[6] = newVerts[2] + direction1;
                    newUvs[6] = uvs[tri.z];
                    newNormals[6] = newNormals[2];


                    newVerts[7] = newVerts[3] + direction2;
                    newUvs[7] = uvs[matchedIndex];
                    newNormals[7] = newNormals[3];

                    triangles = new int[] { 0, 1, 2, 3, 1, 0, /*up tris*/  6, 4, 2, 0, 2, 4,  /*side1 */ 5, 2, 1, 2, 5, 6,  /*side2*/ 0, 4, 7, 7, 3, 0, /*side3*/ 7, 5, 1, 7, 1, 3, /*side4*/ 6, 5, 4, 4, 5, 7  /*down*/};

                }
                else
                {
                    newVerts = new Vector3[6];
                    newNormals = new Vector3[6];
                    newUvs = new Vector2[6];

                    newVerts[0] = verts[tri.x];
                    newUvs[0] = uvs[tri.x];
                    newNormals[0] = normals[tri.x];

                    newVerts[1] = verts[tri.y];
                    newUvs[1] = uvs[tri.y];
                    newNormals[1] = normals[tri.y];

                    newVerts[2] = verts[tri.z];
                    newUvs[2] = uvs[tri.z];
                    newNormals[2] = normals[tri.z];

                    newVerts[3] = newVerts[0] + direction1;
                    newUvs[3] = uvs[tri.x];
                    newNormals[3] = newNormals[0];

                    newVerts[4] = newVerts[1] + direction1;
                    newUvs[4] = uvs[tri.y];
                    newNormals[4] = newNormals[1];

                    newVerts[5] = newVerts[2] + direction1;
                    newUvs[5] = uvs[tri.z];
                    newNormals[5] = newNormals[2];


                    triangles = new int[] { 0, 1, 2, /*up*/   4, 1, 0, 0, 3, 4,  /*side1*/  0, 2, 5, 1, 4, 5, /*side2*/     5, 4, 3, 5, 3, 0,   /*side3*/  5, 2, 1 /*down*/};

                }

                mesh.vertices = newVerts;
                mesh.normals = newNormals;
                mesh.uv = newUvs;
                mesh.triangles = triangles;

                //GO = pool.getPooledObject();
                GO = ObjectsPool.Spawn(pooledObjectName, Vector3.zero, Quaternion.identity);
                if (GO != null)
                {
                    GO.SetActive(true);

                    Deactivator deactivator = GO.GetComponent<Deactivator>();

                    GO.layer = LayerMask.NameToLayer("Fragments");

                    GO.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    GO.transform.rotation = transform.rotation;
                    GO.transform.localScale = new Vector3(scaleFactor.x, scaleFactor.y, scaleFactor.z);

                    deactivator.attachedRenderer.material = MR.materials[submesh];
                    deactivator.attachedFilter.mesh = mesh;
                    deactivators.Add(deactivator);

                    GO.AddComponent<BoxCollider>();
                    
                }
            }
        }

        // Spawn PowerUp
        SpawnPowerUp();

        MR.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        if (GetComponent<NavMeshObstacle>() != null)
        {
            GetComponent<NavMeshObstacle>().enabled = false;
        }
       
        if (respawn)
        {
            Invoke("TriggerRespawn", timeTillRespawn);
        }
        
    }

    void OnTriggerEnter(Collider coll)
    {        
        if (coll.GetComponent<Collider>().tag == "Bullet" || coll.GetComponent<Collider>().tag == "EnemyBullet")
        {
            Bullet bullet = coll.GetComponent<Bullet>();
            if (bullet != null)
                DecrementHealth(bullet.Damage);
        }        
    }

    /// <summary>
    /// Decrements the health and checks if the object should explode.
    /// </summary>
    public void DecrementHealth(int health)
    {
        this.health -= health;

        if (this.health <= 0 && explodeable)
        {
            explode = true;
            explodeable = false;
        }
    }

    private void TriggerRespawn()
    {
        respawnScript.Respawn();       
        explodeable = true;
        health = healthValue;
    }

    /// <summary>
    /// Spawns the line cut power up.
    /// </summary>
    protected void SpawnPowerUp()
    {
        bool instantiated = false;

        // Spawn based on the probability
        if (powerUpPrefabLineCut != null && Random.value < powerUpProbabilityLineCut)
        {
            // Instantiate
            Instantiate(powerUpPrefabLineCut, transform.position, powerUpPrefabLineCut.transform.rotation);
            instantiated = true;
        }

        if (!instantiated)
        {
            // Spawn based on the probability
            if (powerUpPrefabMango != null && Random.value < powerUpProbabilityMango)
            {
                // Instantiate
                Instantiate(powerUpPrefabMango, transform.position, powerUpPrefabMango.transform.rotation);
            }
        }
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}