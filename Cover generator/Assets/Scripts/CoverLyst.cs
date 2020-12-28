using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverLyst : MonoBehaviour
{
    public List<Cover> allCovers;
    
    void Start()
    {
        allCovers.AddRange(GetComponentsInChildren<Cover>());
    }
}
