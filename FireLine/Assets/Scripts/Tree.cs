using System.Diagnostics;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public GameObject palyer;
    private GameObject fireIndicator;
    private GameObject bucket;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireIndicator = transform.Find("Fire Indicator").gameObject;
        bucket = palyer.transform.Find("Bucket").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
