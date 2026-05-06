using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] ParticleSystem paperTrail;
    [SerializeField] ParticleSystem paperBurst;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Remove trail as child, let particle system finish animation/lifetime
        paperTrail.transform.SetParent(null);
        paperTrail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Destroy(paperTrail.gameObject, paperTrail.main.duration + paperTrail.main.startLifetime.constantMax);

        //Intantiate paper burst effect at projetile position and let it play through once.
        ParticleSystem paperBurstTemp = Instantiate(paperBurst, gameObject.transform.position, Quaternion.identity);
        Destroy(paperBurstTemp.gameObject, paperBurstTemp.main.duration + paperBurstTemp.main.startLifetime.constantMax);

        Destroy(gameObject);
    }
}