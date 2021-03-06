﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BossIdle : FSMState
{
    #region Private Members
    // Specifies if the change of a phase is allowed.
    protected bool phaseChangeAllowed;

    // Current phase change timer.
    protected float phaseChangeTimer;

    // Reference to boss enemy
    protected BossEnemy bossEnemy;

    // Index of the actual mob spawn percentage array.
    protected int mobSpawnIndex;
    #endregion

    
    public BossIdle(BossEnemy e)
    {
        this.stateID = StateID.BossIdle;

        this.phaseChangeAllowed = false;
        this.phaseChangeTimer = 0f;
        this.bossEnemy = e;
        this.mobSpawnIndex = 0;
    }

    /// <summary>
    /// Actions of the State.
    /// </summary>
    /// <param name="player">Player reference</param>
    /// <param name="npc">NPC reference</param>
    public override void Act(GameObject player, GameObject npc)
    {
        //Play animation.
        Animator anim = bossEnemy.GetComponent<Animator>();

        if (anim != null)
            anim.SetTrigger("Idle");
    }

    /// <summary>
    /// Reason for a transition to another state.
    /// </summary>
    /// <param name="player">Player reference</param>
    /// <param name="npc">NPC reference</param>
    public override void Reason(GameObject player, GameObject npc)
    {
        DecideNextState(player, npc);
    }

    /// <summary>
    /// Set entering conditions.
    /// </summary>
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();

        // Calculate new target player.
        bossEnemy.CalculateTargetPlayer();

        //Debug.Log("Boss: Idle State");
    }

    /// <summary>
    /// Set leaving conditions.
    /// </summary>
    public override void DoBeforeLeaving()
    {
        base.DoBeforeLeaving();

        phaseChangeAllowed = false;
        phaseChangeTimer = 0f;
        
    }

    /// <summary>
    /// Decide which State should be chosen.
    /// </summary>
    private void DecideNextState(GameObject player, GameObject npc)
    {
        MonoBehaviour m = npc.GetComponent<MonoBehaviour>();

        if (m is BossEnemy)
        {
            BossEnemy e = (BossEnemy) m;

            // Only make a transition if the npc has an target player.
            if (e.TargetPlayer != null)
            {
                if (phaseChangeAllowed)
                {
                    ChooseState(e);
                    phaseChangeAllowed = false;
                }
            }

            // Handle phase change timer logic.
            IncreasePhaseChangeTimer(e);       
        }
    }

    /// <summary>
    /// Calculates the probability for the State change based on the boss enemy values and changes to this state.
    /// </summary>
    /// <param name="e">Boss enemy</param>
    private void ChooseState(BossEnemy e)
    {
        Animator anim = bossEnemy.GetComponent<Animator>();

        // Total probability of all enabled Phases.
        float totalProb = 0f;

        //Random Value between 0 and 1.
        float randomValue = 0f;

        // Arrays for the total probability calculation.
        bool[] phaseEnabled = new bool[] { e.MeleePhase.phaseEnabled, e.RangedPhase.phaseEnabled, e.SpecialPhase.phaseEnabled };
        float[] probabilities = new float[] { e.MeleePhase.phaseProbability, e.RangedPhase.phaseProbability, e.SpecialPhase.phaseProbability };

        // Index of the probability based calculation.
        int indexFoundElement = -1;

        // Key Value list
        List<KeyValuePair<int, float>> indexProbability = new List<KeyValuePair<int, float>>();
        
        for (int i = 0; i < phaseEnabled.Length; i++)
        {
            if (!phaseEnabled[i])
                probabilities[i] = 0f;
            else
                totalProb += probabilities[i];

            // Fill List.
            indexProbability.Add(new KeyValuePair<int, float>(i, probabilities[i]));
        }

        // Sort List descending
        indexProbability.Sort((first, next) =>
        {
            return next.Value.CompareTo(first.Value);
        });
        
        // Map the range of the Random value to the total probability.
        randomValue = UnityEngine.Random.value * totalProb;

        // Search for the list element based on its probability.
        for (int i = 0; i < indexProbability.Count; i++)
        {
            if (randomValue < indexProbability[i].Value)    // Element found.
            {
                //Debug.Log("Index: " + indexProbability[i].Key + ", Value: " + indexProbability[i].Value);

                //Save the found index.
                indexFoundElement = indexProbability[i].Key;
            }
            else
            {
                randomValue -= indexProbability[i].Value;
            }
        }

        // Check if the boss should make a sprint attack
        if (e.SprintPhase.enabled && e.SprintPhase.currentDamage >= e.SprintPhase.sprintTriggerDamage)
        {
            // Sprint attack
            e.SetTransition(Transition.DecisionSprint);

            // Reset the damage counter
            e.SprintPhase.currentDamage = 0;
        }
        else
        {
            #region MobSpawn and Phase selection block
            // Current health percentage.
            float healthPercentage = (float)e.Health / e.MaxHealth;

            if (e.MobSpawnPhase.spawnPercentage.Length > 0
                && mobSpawnIndex < e.MobSpawnPhase.spawnPercentage.Length
                && healthPercentage <= e.MobSpawnPhase.spawnPercentage[mobSpawnIndex])
            {
                mobSpawnIndex++;

                e.SetTransition(Transition.DecisionMobSpawn);
            }
            else
            {
                // Switch state based on the calculatet probability.
                if (indexFoundElement == 0)
                {
                    anim.SetTrigger("Announce");
                    e.SetTransition(Transition.DecisionMelee);
                }
                else if (indexFoundElement == 1)
                {
                    anim.SetTrigger("Announce");
                    e.SetTransition(Transition.DecisionRanged);
                }
                else if (indexFoundElement == 2)
                {
                    anim.SetTrigger("Announce");
                    e.SetTransition(Transition.DecisionSpecial);
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// Increases the phase change timer and allows the phase change if interval is reached.
    /// </summary>
    /// <param name="e"></param>
    private void IncreasePhaseChangeTimer(BossEnemy e)
    {
        phaseChangeTimer += Time.deltaTime;

        if (phaseChangeTimer >= e.IdleTime)
        {
            phaseChangeAllowed = true;
            phaseChangeTimer = 0f;
        }
    }
}
