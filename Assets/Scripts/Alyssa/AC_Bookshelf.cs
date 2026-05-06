using UnityEngine;

public class AC_Bookshelf : MonoBehaviour
{
    public delegate void OnCollisionWormBookshelfFunc(SpriteRenderer spriteRender);
    public static event OnCollisionWormBookshelfFunc OnCollisionWormBookshelf;


    public void OnCollisionEnter2D(Collision2D collision)
    {
        //checks if the object collision is a boookworm
        if (collision.gameObject.tag == "Bookworm")
        {
            //grabs renderer of prefab instance and invokes function
            SpriteRenderer spriteRender = gameObject.GetComponentInChildren<SpriteRenderer>();
            OnCollisionWormBookshelf?.Invoke(spriteRender);
            // Debug.Log("hit");
            // Debug.Log(gameObject.name);
        }
            
    }


    
}
