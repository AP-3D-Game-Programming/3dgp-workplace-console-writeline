using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameObject bucket;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bucket = transform.Find("Bucket").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

            if (bucket.active)
                bucket.SetActive(false);
            else
            {
                bucket.transform.Find("water_splash").gameObject.SetActive(false);
                bucket.SetActive(true);
            }
        }
    }
}
