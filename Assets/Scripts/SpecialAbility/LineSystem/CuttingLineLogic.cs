﻿using UnityEngine;
using System.Collections;

public class CuttingLineLogic : MonoBehaviour {

    [SerializeField]
    private bool activateCutting;

    [SerializeField]
    private int bossCuttingDamage = 5;

    private Vector3 bufferVectorA = new Vector3();

    private Vector3 bufferVectorB = new Vector3();

    private float timeActive = -0.1f;

    private WaitForSeconds bossDamageCoolDown = new WaitForSeconds(0.5f);
    private bool bossTakesDamage = true;

    //private GameObject[] players = new GameObject[0];
    //private LineShaderUtility[] lineShaderUtilities = new LineShaderUtility[0];
    private LineSystem lineSystem;
    private float lineStartOffset = 0.0f;
    private GameObject laserParticles;
    private GameObject lightSabrePrefab;
    private GameObject lightSabreGameObject;
    private LineTweens lineTweens;

    private int[] linesNeeded = new int[] { 0, 1, 3, 6 };
    private int[] firstVertex = new int[] { 0, 1, 2, 0, 1, 2 };
    private int[] secondVertex = new int[] { 1, 2, 0, 3, 3, 3 };

    #region properties
    public float TimeActive
    {
        get { return timeActive; }
        set { timeActive = value; }
    }

    public float LineStartOffset
    {
        set { lineStartOffset = value; }
    }

    public GameObject LaserParticles
    {
        set { laserParticles = value; }
    }

    public GameObject LighSabrePrefab
    {
        set { lightSabrePrefab = value; }
    }

    public LineTweens LineTweens
    {
        set { lineTweens = value; }
    }

    public LineSystem LineSystem
    {
        set { lineSystem = value; }
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        UpdateCuttingStatus();

        if (activateCutting)
        {
            if (lineSystem.Players.Length > 1)
            {
                CuttingLinesPowerUp();
            }
            if (lineSystem.Players.Length == 1)
            {
                PrepareLightSabre();
            }
        }
    }



    private void UpdateCuttingStatus()
    {
        if (timeActive >= 0.0f)
        {
            if (!activateCutting)
            {
                activateCutting = true;
            }

            timeActive -= Time.deltaTime;

            if (timeActive <= 0.0f)
            {
                activateCutting = false;
                for (int i = 0; i < lineSystem.LineShaderUtilities.Length; i++)
                {
                    lineTweens.TweenColor(i, -1, true);
                }
            }
        }
    }


    private void CuttingLinesPowerUp()
    {
        for (int i = 0; i < lineSystem.LineShaderUtilities.Length; i++)
        {
            RaycastHit[] hits;
            bufferVectorA = lineSystem.Players[firstVertex[i]].transform.position;
            bufferVectorA.y = lineStartOffset;

            bufferVectorB = lineSystem.Players[secondVertex[i]].transform.position;
            bufferVectorB.y = lineStartOffset;

            hits = Physics.RaycastAll(new Ray(bufferVectorB, Vector3.Normalize(bufferVectorA - bufferVectorB)), Vector3.Distance(bufferVectorB, bufferVectorA), (1 << 9));

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.GetComponent<MonoBehaviour>())
                {
                    MonoBehaviour gotHit = hit.transform.GetComponent<MonoBehaviour>();
                    if (gotHit is BaseEnemy)
                    {
                        BaseEnemy enemy = hit.transform.GetComponent<BaseEnemy>();
                        if (gotHit is BossEnemy)
                        {
                            Destroy(Instantiate(laserParticles, hit.point, hit.transform.rotation), 2);
                            if (bossTakesDamage)
                            {
                                bossTakesDamage = false;
                                StartCoroutine(StartBossDamageCoolDown());
                                enemy.TakeDamage(bossCuttingDamage, this);
                            }
                        }
                        else
                        {
                            enemy.InstantKill(this);
                            enemy.gameObject.AddComponent<CutUpMesh>();
                            Destroy(Instantiate(laserParticles, hit.point, hit.transform.rotation), 2);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator StartBossDamageCoolDown()
    {
        yield return bossDamageCoolDown;
        bossTakesDamage = true;
    }

    private void PrepareLightSabre()
    {
        if (lightSabreGameObject == null)
        {
            lightSabreGameObject = Instantiate(lightSabrePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            lightSabreGameObject.transform.parent = lineSystem.Players[0].transform;
            lightSabreGameObject.transform.position = Vector3.zero;
        }

    }
}