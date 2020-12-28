using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public CoverType TypeOfCover = CoverType.Stand;
    public enum CoverType
    {
        Stand,
        Duck
    }
    public BotController MyBot;
    [SerializeField]
    public bool Right;
}
