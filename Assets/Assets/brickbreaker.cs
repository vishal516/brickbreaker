using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using Cinemachine;


public class brickbreaker : MonoBehaviour
{
    public static brickbreaker BB;
    private Vector3 Myinput;
    private Rigidbody rbodyBall;
    private bool launched;

    public AudioMixer mixer;
    public TMP_Text info;
    public TMP_Text levelText;
    public TMP_Text scoreText;
    public TMP_Text pauseText;
    public GameObject pauseMenu;
    public float NoiseAmplitude;
    public CinemachineVirtualCamera actionCam;
    public CinemachineVirtualCamera menuCam;
    public CinemachineBasicMultiChannelPerlin noise;
    public GameObject quickMenuPanel;
    public GameObject mainMenuPanel;
    public GameObject ballbody;
    public GameObject[] backgroundColorObj;
    public LineRenderer laser;
    private Image varJoyImage;
    public LayerMask layers;
    public GameObject bullets;
    public GameObject camTarget;
    public Slider volumeSlider;
    public Slider BallSpeedSlider;
    public VariableJoystick varJoy;

    [Range(-80, 20)]
    public float vol;
    public int lives = 2;
    public bool laserOn;
    public bool shoot;


    private float volume;
    [HideInInspector]
    public float ballSpeed;
    private float padSpeed;
    public int highScore;
    public int level;
    private float padScale = 1;


    void Awake()
    {
        BB = this;
    }

    void Start()
    {
        BallSpeedSlider.value = PlayerPrefs.GetFloat("ballSpeed");
        volumeSlider.value = PlayerPrefs.GetFloat("masterAll");
        level = PlayerPrefs.GetInt("savedLevel");
        highScore = PlayerPrefs.GetInt("highScore");
        ResetBallSpeed();
        padSpeed = 5f;
        rbodyBall = ballbody.GetComponent<Rigidbody>();
        varJoyImage = varJoy.GetComponent<Image>();
        actionCam.enabled = false;
        menuCam.enabled = true;
        levelText.enabled = false;
        MainMenu();
        LeanTween.move(ballbody, new Vector3(0, 0, 0.5f), 1).setEase(LeanTweenType.easeSpring).setDelay(1);
        if (!actionCam.GetComponent<CinemachineBasicMultiChannelPerlin>())
        {
            actionCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            print("reset");
        }
        noise = actionCam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        Myinput.x = Input.GetAxis("Horizontal") + varJoy.Horizontal;
        Myinput.y = Input.GetAxis("Vertical") + varJoy.Vertical;
        movement();

        levelText.text = "level\n" + level + "\n" + ball.Ball.scored + "/" + brickManager.BM.brickCountPerLevel + "\n Life:" + lives + "\nScore:"+ball.Ball.scoreTotal+ "\n HighScore:" + highScore;//❤

        volume = Mathf.Lerp(volume, vol, Time.deltaTime);
        AudioManager(volume);

        if (laserOn)
        {
            Laser(true);
        }
        Shoot();
    }


    void FixedUpdate()
    {
        if (launched)
        {
            if (rbodyBall.velocity.magnitude < ballSpeed)
            {
                rbodyBall.velocity = rbodyBall.velocity * 1.1f;
            }
            else if (rbodyBall.velocity.magnitude > ballSpeed)
            {
                rbodyBall.velocity = rbodyBall.velocity / 1.1f;
            }
        }
    }

    public void movement()
    {
        float xpos = transform.position.x + Myinput.x * padSpeed*Time.deltaTime;
        transform.position = new Vector3(Mathf.Clamp(xpos, -(4.8f - padScale / 2), (4.8f - padScale / 2)), 0.23f, -7.15f);
        camTarget.transform.position = transform.position / 3;

        if (Input.GetKeyDown(KeyCode.E) && !launched)
        {
            Launch();
        }
        if (Myinput.y != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(transform.rotation.x, Myinput.y * 2, transform.rotation.z, 9), 2f );
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(0, 0, 0, 1), 90 );
        }

    }


    public void ResetBallSpeed()
    {
        ballSpeed = BallSpeedSlider.value;
    }

    public void Launch()
    {
        rbodyBall.isKinematic = false;
        rbodyBall.AddForce(new Vector3(0, 0, ballSpeed) * 90 * Time.deltaTime);
        launched = true;
        pauseText.text = "pause";
    }

    public void NextLevel()
    {
        if (level <= 199)
        {
            ball.Ball.IndicatorBanner();
            ball.Ball.powerIndicatorB.text = "NEXT LEVEL";
            level += 1;
            save();
            PlayGame();
        }
        else
        {
            lives = 0;
            RestartGame();
            scoreText.text = "win";
        }
        ResetBall();
        camEffect();
        ColorRandomizer();
        pauseText.text = "launch";
    }

    public void RestartGame()
    {
        if (lives > 0)
        {
            ResetBall();
            camEffect();
            lives--;
            pauseText.text = "launch";
        }
        else
        {
            brickManager.BM.disableAllbricks();
            save();
            lives = 2;
            actionCam.enabled = false;
            menuCam.enabled = true;
            rbodyBall.isKinematic = true;
            brickManager.BM.ValuesReset();
            scoreText.text = "Game Over";
            LeanTween.moveLocalZ(quickMenuPanel, 0, 2f).setEase(LeanTweenType.easeOutCirc).setDelay(0.0f);
            vol = -13;
            varJoyImage.enabled = false;
            pauseText.text = "better luck next time!";
        }
        ColorRandomizer();
    }

   

    public void PlayGame()
    {
        actionCam.enabled = true;
        menuCam.enabled = false;
        ResetBall();
        ball.Ball.scored = 0;
        scoreText.text = "";

        LeanTween.moveLocalZ(mainMenuPanel, 30.5f, 1f).setEase(LeanTweenType.easeInOutBack).setDelay(0.2f);
        LeanTween.moveLocalZ(quickMenuPanel, 30.5f, 1f).setEase(LeanTweenType.easeInOutBack).setDelay(0.2f);
        levelText.enabled = true;
        levelText.CrossFadeAlpha(1, 1, true);
        vol = -80;
        ColorRandomizer();
        varJoyImage.enabled = true;
        pauseText.text = "launch";
        LeanTween.delayedCall(gameObject, 2f, () => { brickManager.BM.brickPatternMaker(level); });
        
    }

    public void MainMenu()
    {
        brickManager.BM.disableAllbricks();
        mainMenuPanel.SetActive(true);
        levelText.CrossFadeAlpha(0, 1, true);
        LeanTween.moveLocalZ(quickMenuPanel, 30.5f, 1f).setEase(LeanTweenType.easeInOutBack).setDelay(0.2f);
        LeanTween.moveLocalZ(mainMenuPanel, 0, 1f).setEase(LeanTweenType.easeOutCirc).setDelay(1.1f);
        vol = -13;
        ColorRandomizer();
        varJoyImage.enabled = false;
        pauseText.text = "settings";
    }


    public void ResetBall()
    {
        rbodyBall.isKinematic = true;
        LeanTween.move(ballbody, new Vector3(0, -20, transform.position.z + 2), 1f).setEase(LeanTweenType.linear);
        LeanTween.move(ballbody, new Vector3(0, transform.position.y, transform.position.z + 2), 1).setEase(LeanTweenType.easeSpring).setDelay(1);
        launched = false;
    }

    public void camEffect()
    {
        actionCam.enabled = false;
        menuCam.enabled = true;
        LeanTween.delayedCall(1, () => { actionCam.enabled = true; menuCam.enabled = false; });
    }



    public void ColorRandomizer()
    {
        byte r = (byte)Random.Range(0, 255);
        byte g = (byte)Random.Range(0, 255);
        byte b = (byte)Random.Range(0, 255);
        for (int i = 0; i < backgroundColorObj.Length; i++)
        {
            LeanTween.color(backgroundColorObj[i], new Color32(r, g, b, 255), 4);
        }
    }

 
    public void PauseGame()
    {
        if (pauseText.text == "pause")//(Time.timeScale != 0 && launched)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            pauseText.text = "resume";
            varJoyImage.enabled = false;
        }

        else if (pauseText.text == "launch")//(!launched && Time.timeScale != 0)
        {
            Launch();
        }

        else if (pauseText.text == "settings")
        {
            pauseMenu.SetActive(true);
            pauseText.text = "back";
        }
        else if (pauseText.text == "back")
        {
            pauseMenu.SetActive(false);
            pauseText.text = "settings";
        }
            
        else if (pauseText.text == "resume")//(Time.timeScale == 0)
        {
            
            Time.timeScale = 1f;
            pauseText.text = "pause";
            varJoyImage.enabled = true;
            pauseMenu.SetActive(false);
        }
    }


    public void ExitGame()
    {
        Application.Quit();
    }


    public void PadScaler(int value)
    {
        if (value == 0)
            padScale = 1;
        else if (value == 1)
            padScale = 0.6f;
        else if (value == 2)
            padScale = 1.5f;
        LeanTween.scaleX(gameObject, padScale, 1).setEase(LeanTweenType.easeSpring);
    }

    public void SpeedModifier(GameObject a)
    {
        ball.Ball.collect(a);
    }

    public void Laser(bool on)
    {
        if (on)
        {
            laser.enabled = true;

            laser.SetPosition(0, transform.position);
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, layers))
            {

                ParticleSystem p = ball.Ball.particles[2];

                if (hit.collider.gameObject.tag == "collectable")
                {
                    p.transform.position = hit.point;
                    p.Play();
                    GameObject obj = hit.collider.gameObject;
                    obj.GetComponent<properties>().resist -= 5;
                    if (obj.GetComponent<properties>().resist < 1)
                    {
                        ball.Ball.collect(obj);
                    }
                }
                laser.SetPosition(1, hit.point);
            }
            else
                laser.SetPosition(1, ray.GetPoint(100));
        }
        else
        {
            laserOn = false;
            laser.enabled = false;
        }
    }

    public void Shoot()
    {
        if (shoot)
        {
            Vector3 posz = bullets.transform.localPosition;
            bullets.transform.localPosition = posz + new Vector3(0, 0, 20 * Time.deltaTime);
        }
        else
            LeanTween.moveLocalZ(bullets, -15, 0);
    }

    public void AudioManager(float vol)
    {
        mixer.SetFloat("masterVol", vol);
    }
    public void visitUrl()
    {
        Application.OpenURL("https://itch.io/");
    }

    public void save()
    {
        PlayerPrefs.SetInt("savedLevel", level);
    }

    public void ClearSave()
    {
        PlayerPrefs.SetInt("savedLevel", 1);
        level = 1;
        ball.Ball.scoreTotal = 0;
        PlayerPrefs.SetInt("scoreTotal", 0);
        Back2MainMenu();
    }

    public void Back2MainMenu()
    {
        Time.timeScale = 1f;
        actionCam.enabled = false;
        menuCam.enabled = true;
        pauseMenu.SetActive(false);
        MainMenu();
        ResetBall();
        LeanTween.move(ballbody, new Vector3(0,0,0.5f), 1).setEase(LeanTweenType.easeSpring).setDelay(1);
    }
    
    public void AudioController()
    {
        PlayerPrefs.SetFloat("masterAll", volumeSlider.value);
        mixer.SetFloat("masterAll", volumeSlider.value);
    }
    public void ballSpeedController()
    {
        PlayerPrefs.SetFloat("ballSpeed", BallSpeedSlider.value);
        ResetBallSpeed();
    }

}