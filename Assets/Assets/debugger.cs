using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugger : MonoBehaviour
{

    public enum myEnums { item1, item2, item3 };
    public myEnums a;

    public static debugger deb = new debugger();
    public Rigidbody rbodyBall;

    public bool laserOn;
   void Start()
    {
        deb = this;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayerPrefs.SetInt("savedLevel",1);
        }


        //if (a == myEnums.item1)
        //{
        //    print("item1");
        //}


        LevelComplete();

        ResetSave();

        //brickbreaker.BB.info.text = "" + rbodyBall.velocity.magnitude;


        //if(laserOn)
        //{
        //    //brickbreaker.BB.Laser();
        //}



    }

    public void LevelComplete()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ball.Ball.scored = 0;
            brickbreaker.BB.NextLevel();
            PlayerPrefs.SetInt("highScore", ball.Ball.scored);
        }
    }

    public void ResetSave()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayerPrefs.SetInt("highScore", 0);
            brickbreaker.BB.highScore = PlayerPrefs.GetInt("highScore");
        }
    }


    public void testNA()
    {
        print("just testing");
    }


  

}
