using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    private float borde = 8;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(enemy, PosAle(),enemy.transform.rotation);
        
    
    }

    public Vector3 PosAle()
    {
        float Xpos = Random.Range(-borde, borde);
        float Zpos = Random.Range(-borde, borde);
        Vector3 posAle = new Vector3(Xpos, 2, Zpos);
        return posAle;
    }


// Update is called once per frame
    void Update()
    {
        
    }
}
