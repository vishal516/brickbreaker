using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ball : MonoBehaviour
{
    public static ball Ball;
    public Material[] mat;
    private Rigidbody rbody;
    public int scored = 0;
    public ParticleSystem[] particles;
    public GameObject powerDrop;
    public GameObject powerIndicator;
    private int powerTimer = 8;
    public TMP_Text powerIndicatorB;
    public int scoreTotal;
    public AudioSource audioSource;
    public AudioClip[] clip;

    void Start()
    {
        Ball = this;
        rbody = GetComponent<Rigidbody>();
        Ball.GetComponent<MeshRenderer>().material = mat[0];
        scoreTotal = PlayerPrefs.GetInt("scoreTotal");
    }

    

    public void collect( GameObject obj)
    {
        if (obj.transform.localScale.y < 1)
        {
            scoreTotal += 1;
            PlayerPrefs.SetInt("scoreTotal", scoreTotal);
            highscoreUpdater();
            scored += 1;
            obj.SetActive(false);
        }
        else
            obj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        
        if (scored == brickManager.BM.brickCountPerLevel)
        {
            scored = 0;
            brickbreaker.BB.NextLevel();
        }
        
        particles[0].transform.position = transform.position;
        particles[0].Play();
        audioSource.PlayOneShot(clip[0]);
    }

    public void highscoreUpdater()
    {
        if (scoreTotal >= brickbreaker.BB.highScore)
        {
            PlayerPrefs.SetInt("highScore", scoreTotal);
            brickbreaker.BB.highScore = scoreTotal;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "deadzone")
        {
            brickbreaker.BB.RestartGame();
            audioSource.PlayOneShot(clip[1]);
        }
        if (other.gameObject.tag == "collectable")
        {
            collect(other.gameObject); 
        }
        if (other.gameObject.tag == "source")
        {
            dropper();
            brickManager.BM.RandomizeBricks();
        }
        brickbreaker.BB.noise.m_AmplitudeGain = 2;
        LeanTween.delayedCall(0.2f, () => {brickbreaker.BB.noise.m_AmplitudeGain = 0; });
        
    }
    //if fireball power is active
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "collectable")
        {
            collect(other.gameObject);
        }
    }


    #region powers
    private void dropper()
    {
        if (powerDrop.transform.position.z < -14)
        {
            powerDrop.transform.position = transform.position;
            LeanTween.moveZ(powerDrop, -15, 4);
        }
    }

    public void IndicatorBanner()
    {
        Time.timeScale = 0.1f;
        LeanTween.delayedCall(gameObject, 0.2f, () => { Time.timeScale = 1; });
        LeanTween.moveLocalY(powerIndicator, -5, 0.1f);
        LeanTween.delayedCall(gameObject, 0.3f, () => { LeanTween.moveLocalY(powerIndicator, -25, 1f); });
    }

    public void ActivatePower()
    {
        IndicatorBanner();
        int whichPower = Random.Range(1, 9);
        audioSource.PlayOneShot(clip[2]);
        StopAllCoroutines();
        
        if (whichPower == 1)
        {
            Fireball();
            BallScale();
            powerIndicatorB.text = "FIREBALL+";
        }
        if (whichPower == 2)
        {
            PadScale();
            if (brickbreaker.BB.lives < 5)
            {
                brickbreaker.BB.lives += 1;
                powerIndicatorB.text = "LIFE+";
            }
            else
            {
                PadScale();
                powerIndicatorB.text = "<SCALE>";
            }
        }

        if (whichPower == 3)
        {
            Fireball();
            powerIndicatorB.text = "FIREBALL";
        }
            
        if (whichPower == 4)
        {
            BallScale();
            powerIndicatorB.text = "BALL XL";
        }
            
        if (whichPower == 5)
        {
            brickbreaker.BB.laserOn = true;
            LeanTween.delayedCall(gameObject, powerTimer, () => { brickbreaker.BB.Laser(false); });
            powerIndicatorB.text = "LASER";
        }
        if (whichPower == 6)
        {
            brickbreaker.BB.shoot = !brickbreaker.BB.shoot;
            LeanTween.delayedCall(gameObject, powerTimer, () => { brickbreaker.BB.shoot = false; });
            powerIndicatorB.text = "GUN";
        }
        if(whichPower == 7)
        {
            brickbreaker.BB.ballSpeed = brickbreaker.BB.BallSpeedSlider.value-4;
            LeanTween.delayedCall(gameObject, powerTimer, () => { brickbreaker.BB.ResetBallSpeed(); });
            powerIndicatorB.text = "SPEED-";
        }
        if(whichPower == 8)
        {
            brickbreaker.BB.ballSpeed = brickbreaker.BB.BallSpeedSlider.value+4;
            LeanTween.delayedCall(gameObject, powerTimer, () => { brickbreaker.BB.ResetBallSpeed(); });
            powerIndicatorB.text = "SPEED+";
        }
        if(whichPower == 9)
        {
            //aiming ball
        }
    }

    
    private void Fireball()
    {
        brickManager.BM.brickTriggerActivator(true);
        Ball.GetComponent<MeshRenderer>().material = mat[2];
        particles[1].Play();
        StartCoroutine(DisableFireball());
    }

    IEnumerator DisableFireball()
    {
        yield return new WaitForSeconds(powerTimer);
        brickManager.BM.brickTriggerActivator(false);
        Ball.GetComponent<MeshRenderer>().material = mat[0];
        particles[1].Stop();
    }

    private void PadScale()
    {
        brickbreaker.BB.PadScaler(Random.Range(1,3));
        StartCoroutine(RescalePad());
    }
    IEnumerator RescalePad()
    {
        yield return new WaitForSeconds(powerTimer);
        brickbreaker.BB.PadScaler(0);
    }

    private void BallScale()
    {
        LeanTween.scale(gameObject, Vector3.one, 1).setEase(LeanTweenType.easeSpring);
        LeanTween.scale(gameObject, new Vector3(0.42586f, 0.42586f, 0.42586f), 1).setEase(LeanTweenType.easeSpring).setDelay(powerTimer);
    }

    #endregion



}
