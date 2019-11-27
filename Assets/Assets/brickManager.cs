using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brickManager : MonoBehaviour
{
    public static brickManager BM;
    [HideInInspector]
    public Vector3[,] pos;
    public int totalbricks;
    
    private GameObject[] bricksOneD;
    private float spacing;

    public GameObject brick;

    private int row;
    private int clmn;
    [HideInInspector]
    public int brickCountPerLevel;
    private int brickIndex = 0;
    private bool checkRC;
    void Start()
    {
        BM = this;
        totalbricks = 900;
        brickCountPerLevel = 1;
        bricksOneD = new GameObject[totalbricks];
        clmn = 0; row = 1;
        LoadAllBricksInPool();
    }

    public void disableLeft()
    {
        for (int i = 0; i < totalbricks; i++)
        {
            if (transform.GetChild(i))
                transform.GetChild(i).transform.gameObject.SetActive(false);
        }
    }

    public void Values()
    {  
        clmn += 1;
        if (clmn>10)
        {
            row += 1;
            clmn -= 10;
        }
        brickCountPerLevel = (clmn * 2 + 1) * (row * 2 + 1);//36;
        spacing = 0.7f;
    }

    public void ValuesReset()
    {
        clmn = 0;
        row = 1;
    }



    private void LoadAllBricksInPool()
    {
        for (int i = 0; i < totalbricks; i++)
        {
            bricksOneD[i] = Instantiate(brick);
            bricksOneD[i].transform.parent = transform;
            bricksOneD[i].transform.localPosition = Vector3.zero;
        }
    }



    public void brickPatternMaker(int a)
    {
        ValuesReset();
        Leveled();
        brickIndex = 0;
        disableAllbricks();
        for (int j = -clmn; j <= clmn; j++)
        {
            for (int k = -row; k <= row; k++)
            {
                if (brickIndex < brickCountPerLevel)
                {
                    bricksOneD[brickIndex].transform.localPosition = new Vector3(0, 0, 0) * spacing;
                    LeanTween.moveLocal(bricksOneD[brickIndex], new Vector3(j, 0, k) * spacing, 2).setEase(LeanTweenType.easeInOutBack);
                    bricksOneD[brickIndex].SetActive(true);
                    brickIndex++;
                }
                else
                {
                    break;
                }
            }
        }

        void Leveled()
        {
            for (int i = 1; i <= a; i++)
            {
                Values();
            }
        }
    }

    public void RandomizeBricks()
    {
        for (int i = 0; i < totalbricks; i++)
        {
            if (bricksOneD[i].activeSelf)
                LeanTween.scaleY(bricksOneD[i],Random.Range(1,9),1).setEase(LeanTweenType.easeInOutBack);
        }
    }


    public void disableAllbricks()
    {
        for (int i = 0; i < totalbricks; i++)
        {
            bricksOneD[i].SetActive(false);
        }
    }
    public void brickTriggerActivator(bool value)
    {
        for (int i = 0; i < totalbricks; i++)
        {
            bricksOneD[i].GetComponent<Collider>().isTrigger = value;
        }
    }


}
