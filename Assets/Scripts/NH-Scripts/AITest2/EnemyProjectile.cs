using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    //-----------Moises-------------
    [SerializeField] ParticleSystem projectileCollision;
    //------------------------------
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //-----------Moises-------------
        //Instantaites particle system at collision location and then destroys particle system after done playing its final animation
        ParticleSystem particleSystem = Instantiate(projectileCollision, gameObject.transform.position, Quaternion.identity);
        Destroy(particleSystem.gameObject, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);
        //------------------------------

        Destroy(gameObject);
    }
}