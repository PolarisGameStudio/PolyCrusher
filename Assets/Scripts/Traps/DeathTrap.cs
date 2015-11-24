﻿using UnityEngine;
using System.Collections;

public class DeathTrap : Trap, ITriggerable
{

    // Enum of the Death Traps to show it as a dropdown menu
    public enum DeathTrapEnum
    {
        bearTrap, floorTrap, hlTrap
    }

    [SerializeField]
    private DeathTrapEnum type;

    #region Class Methods

    public override void Trigger(Collider other)
    {
        //play the animation asscociated with the trap type
        if (type == DeathTrapEnum.bearTrap)
        {
            GetComponent<Animation>().Play("strike");
        }
        else if (type == DeathTrapEnum.floorTrap)
        {
            GetComponent<Animation>().Play("spike");
        }

        if (other.tag == "Player")
        {
            // get the BasePlayer of the Game Object
            BasePlayer player = other.GetComponent<BasePlayer>();
            Vector3 tmpPosition = other.GetComponent<Transform>().position;
            Quaternion tmpRotation = other.GetComponent<Transform>().rotation;
            player.CurrentDeathTime = 0;
            player.InstantKill();

            //create playerMesh to destroy it without destroying the real player
            GameObject destroyMesh = null;
            switch (player.name)
            {
                case "Birdman":
                    destroyMesh = this.playerMeshes[0];
                    break;
                case "Charger":
                    destroyMesh = this.playerMeshes[1];
                    break;
                case "Fatman":
                    destroyMesh = this.playerMeshes[2];
                    break;
                case "Timeshifter":
                    destroyMesh = this.playerMeshes[3];
                    break;
            }
            if (destroyMesh != null)
            {
                GameObject toDestroy = Instantiate(destroyMesh, tmpPosition, tmpRotation) as GameObject;
                toDestroy.gameObject.AddComponent<PolyExplosion>();
            }
        }

        if (other.tag == "Enemy")
        {
            // get the BaseEnemy of the Game Object
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();

            if (enemy is BossEnemy)
            {
                if (bossDamage != 0)
                {
                    enemy.TakeDamage(42, other.GetComponent<MonoBehaviour>());
                }

            }
            else
            {
                enemy.InstantKill();
                enemy.gameObject.AddComponent<PolyExplosion>();
            }
        }

        //trigger.onTriggerExit is never called, so trap is reset manually
        StartCoroutine(WaitForReset());
    }

    #endregion
}