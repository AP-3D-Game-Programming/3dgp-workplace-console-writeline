using System;
using UnityEngine;

public class FireIndicator : MonoBehaviour
{
    public event Action<bool> OnFireStateChange;
    void OnEnable()
    {
        OnFireStateChange?.Invoke(true);
    }
    void OnDisable()
    {
        OnFireStateChange?.Invoke(false);
    }
}
