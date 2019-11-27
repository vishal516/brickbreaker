using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObserver : MonoBehaviour
{

    public enum selectMode
    {
        powerDrop,
        bullet
    };
    public selectMode WhosTrigger;

    private void OnTriggerEnter(Collider other)
    {

        print("trigger.tag is ::" + other.tag);
        if (WhosTrigger == selectMode.powerDrop)
        {
            if (other.tag == "Player")
            {
                ball.Ball.ActivatePower();
                transform.position = new Vector2(-15, 4);
                ball.Ball.particles[3].Play();
            }
        }

        if (WhosTrigger == selectMode.bullet)
        {
            if (other.tag == "collectable")
            {

                ball.Ball.collect(other.gameObject);
                ball.Ball.particles[0].transform.position = other.transform.position;
            }


            gameObject.transform.position = brickbreaker.BB.gameObject.transform.position;
        }


    }
}
