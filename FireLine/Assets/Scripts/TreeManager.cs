using System;
using Unity.VisualScripting;
using UnityEngine;

public class TreeManager : MonoBehaviour
{

    private FireIndicator fireIndicator;
    public Transform fireIndicatorTransform;
    private int amountSecOnFire;
    public bool Burned;
    
    private int deadTimer;
    private TutorialManager tutorialManager;
    private DayManager dayManager;

    public event Action<TreeManager> OnBurnedStateChange;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        fireIndicator = fireIndicatorTransform.GetComponent<FireIndicator>();
        fireIndicator.OnFireStateChange += HandleFireChange;
        
        Burned = false;
        amountSecOnFire = 0;
        deadTimer = 25;
    }
    void Start()
    {
        tutorialManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
        dayManager = GameObject.Find("MangerDayManager").GetComponent<DayManager>(); 
    }
    void OnDestroy()
    {
        fireIndicator.OnFireStateChange -= HandleFireChange;
    }
    private void HandleFireChange(bool onFire)
    {
        if (onFire)
        {
            StartTimer();
        } else
        {
            if (endTimer() >= deadTimer && !Burned)
            {
                Burned = true;
                OnBurnedStateChange.Invoke(this);
            }
            
        }
    }
    void StartTimer()
    {
        InvokeRepeating("AddOneSec", 1f, 1f);
    }
    int endTimer()
    {
        CancelInvoke("AddOneSec");
        var time = amountSecOnFire;
        amountSecOnFire = 0;
        return time;
    }
    void AddOneSec()
    {
        amountSecOnFire++;
        if (amountSecOnFire >= deadTimer)
            fireIndicator.gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("waterCollider"))
        {
            if (fireIndicatorTransform.gameObject.activeSelf)
            {
                tutorialManager.waterBucketSubtask.UpdateProgress();
            }
            fireIndicatorTransform.gameObject.SetActive(false);   
        }
    }
}
