using UnityEngine;

public class waterCollider : MonoBehaviour
{
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(Vector3.forward * 4f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    // void OnTriggerEnter(Collider other)
    // {

    //     if (other != null) ;
    //     {
    //         other.transform.Find("Fire Indicator").gameObject.SetActive(false);
    //     }
    //     Destroy(gameObject);
    // }
    void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Tree"))
            {
                collision.gameObject.transform.Find("Fire Indicator").gameObject.SetActive(false);
            }
        }
        Destroy(gameObject);
    }
}
