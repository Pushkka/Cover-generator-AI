using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Cover : MonoBehaviour
{
    public GameObject LeftM, RightM, DuckM;
    public CoverType TypeOfCover = CoverType.Stand;
    public enum CoverType
    {
        Stand,
        Duck
    }
    public BotController MyBot;
    [SerializeField]
    public bool Right;

    void Start()
    {
        if (TypeOfCover == CoverType.Duck)
        {
            LeftM.SetActive(false);
            RightM.SetActive(false);
            DuckM.SetActive(true);
        }
        if (TypeOfCover == CoverType.Stand && Right)
        {
            LeftM.SetActive(false);
            RightM.SetActive(true);
            DuckM.SetActive(false);
        }
        if (TypeOfCover == CoverType.Stand && !Right)
        {
            LeftM.SetActive(true);
            RightM.SetActive(false);
            DuckM.SetActive(false);
        }
    }

    private void OnValidate()
    {
        if (TypeOfCover == CoverType.Duck)
        {
            LeftM.SetActive(false);
            RightM.SetActive(false);
            DuckM.SetActive(true);
        }
        if (TypeOfCover == CoverType.Stand && Right)
        {
            LeftM.SetActive(false);
            RightM.SetActive(true);
            DuckM.SetActive(false);
        }
        if (TypeOfCover == CoverType.Stand && !Right)
        {
            LeftM.SetActive(true);
            RightM.SetActive(false);
            DuckM.SetActive(false);
        }
    }
}
