using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keytest : MonoBehaviour
{
    public GameObject canvasGO;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        canvasGO.SetActive(true);
    }
}
