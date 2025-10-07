using System;
using System.Collections;
using UnityEngine;

public class MoverAdelnate : MonoBehaviour
{
    [SerializeField] float velocidad =5;
    [SerializeField] private bool activo;

    private GameObject focal;
    private Rigidbody rb;
    private float fuerza;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        focal = GameObject.Find("Focal");
    }

    // Update is called once per frame
    void Update()
    {
        float adelante = Input.GetAxis("Vertical");
        rb.AddForce(focal.transform.forward * velocidad*adelante);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("pop"))
        {
            activo = true;
            Destroy(other.gameObject);
            StartCoroutine(Temporizador());
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Death"))
        {
           Rigidbody fisicas=other.gameObject.GetComponent<Rigidbody>();
           Vector3 alejar =(other.gameObject.transform.position-transform.position);
           fisicas.AddForce(alejar*fuerza,ForceMode.Impulse);
        }
    }

    IEnumerator Temporizador()
    {
        yield return new WaitForSeconds(5);
        
    }
}
