using System;
using UnityEngine;

public class TreeManager : MonoBehaviour
{

    private FireIndicator fireIndicator;
    public Transform fireIndicatorTransform;
    private int amountSecOnFire;
    public bool Burned;

    public event Action<TreeManager> OnBurnedStateChange;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        amountSecOnFire = 0;
        Burned = false;
        fireIndicator = fireIndicatorTransform.GetComponent<FireIndicator>();
        fireIndicator.OnFireStateChange += HandleFireChange;
        Debug.Log(fireIndicator);
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
            if (endTimer() >= 3 && !Burned)
            {
                Burned = true;
                OnBurnedStateChange.Invoke(this);
            }
            
        }
    }
    void StartTimer()
    {
        InvokeRepeating("AddOneSec", 1f, 1f);
        Debug.Log("timer started");
    }
    int endTimer()
    {
        CancelInvoke("AddOneSec");
        var time = amountSecOnFire;
        amountSecOnFire = 0;
        Debug.Log("TIMER ENDED");
        return time;
    }
    void AddOneSec()
    {
        amountSecOnFire++;
        if (amountSecOnFire >= 3)
            fireIndicator.gameObject.SetActive(false);
    }
    
}
