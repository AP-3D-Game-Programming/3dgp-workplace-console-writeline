using System;
using UnityEngine;

public class FireIndicator : MonoBehaviour
{
    public event Action<bool> OnFireStateChange;
    void OnEnable()
    {
        Debug.Log("FIRE INDEACTOR IS ENABLE METHOD WORKS!");
        OnFireStateChange?.Invoke(true);
    }
    void OnDisable()
    {
        OnFireStateChange?.Invoke(false);
    }
}
