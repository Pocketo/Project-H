using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    private float borde = 8;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Spawn(3);
    }

    public Vector3 PosAle()
    {
        float Xpos = Random.Range(-borde, borde);
        float Zpos = Random.Range(-borde, borde);
        Vector3 posAle = new Vector3(Xpos, 2, Zpos);
        return posAle;
    }

    private void Spawn(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            Instantiate(enemy, PosAle(),enemy.transform.rotation);
        }
            
    }


// Update is called once per frame
    void Update()
    {
        
    }
}
