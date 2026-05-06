using System.Collections.Generic;
using UnityEngine;

public class AC_BookshelfRefactored : MonoBehaviour
{
    // public delegate void OnCollisionWormBookshelfFunc(SpriteRenderer spriteRender);
    // public static event OnCollisionWormBookshelfFunc OnCollisionWormBookshelf;
    // [SerializeField] private AC_BookshelfVisual bookshelfVisual;

    [SerializeField] private AC_BookshelfListSO bookshelfListSO;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private int minRange;
    [SerializeField] private int maxRange;
    [SerializeField] private ParticleSystem paperBurst;
    private List<Sprite> bookshelfEatenState;
    private State state;
    private int wormTouchCount;

    private enum State
    {
        Full,
        Half,
        Quarter,
        Empty
    }


    public void Start()
    {
        SelectRandomBookshelf();
        wormTouchCount = 0;
        // signOffEvents = false;
    }


    private void SelectRandomBookshelf()
    {
        //randomizes bookshelf base for middle section
        AC_BookshelfSO selectedBookshelf = bookshelfListSO.bookshelfSOList[UnityEngine.Random.Range(minRange, maxRange)];

        //gets sprite base from SO
        selectedSprite = selectedBookshelf.spriteBase;
        gameObject.GetComponent<SpriteRenderer>().sprite = selectedSprite;

        bookshelfEatenState = selectedBookshelf.bookshelfEatenSprites;

        // Debug.Log(selectedSprite);
    }


    public void OnTriggerEnter2D(Collider2D collider)
    {
        // Debug.Log(collider.gameObject.tag);
        //checks if the object collision is a boookworm
        if (collider.gameObject.CompareTag("Bookworm"))
        {
            //grabs renderer of prefab instance and invokes function
            SpriteRenderer spriteRender = gameObject.GetComponent<SpriteRenderer>();
            OnCollisionWormBookshelf(spriteRender);
            // bookshelfVisual.OnCollisionWormBookshelf(spriteRender);
            // OnCollisionWormBookshelf?.Invoke(spriteRender);
            // Debug.Log("hit");
            // Debug.Log(gameObject.name);
        }
            
    }


     public void OnCollisionWormBookshelf(SpriteRenderer spriteRender)
    {
        //makes sure only the prefab touching the worm is changed 
        if (spriteRender != this.gameObject.GetComponent<SpriteRenderer>())
            return;
        
        // Debug.Log("hit by worm");
        //changes sprite after a certain amount of collisions with worm
        //sprites should change to be slowly eaten books; should match with randomized selected sprite from start 
        switch (state)
        {
            case State.Full:
                if (wormTouchCount > UnityEngine.Random.Range(3, 6))
                {
                    wormTouchCount = 0;
                    state = State.Half;
                }
                wormTouchCount++;
                break;
            case State.Half:
                //enclosed the sprite change into an if statement so it only changes the sprite once per state change instead of every collision
                if(selectedSprite != bookshelfEatenState[0])
                {
                    selectedSprite = bookshelfEatenState[0];
                    PlayPaperBurst();
                }
                if (wormTouchCount > UnityEngine.Random.Range(3, 6))
                {
                    wormTouchCount = 0;
                    state = State.Quarter;
                }
                wormTouchCount++;
                break;
            case State.Quarter:
                if(selectedSprite != bookshelfEatenState[1])
                {
                    selectedSprite = bookshelfEatenState[1];
                    PlayPaperBurst();
                }
                if (wormTouchCount > UnityEngine.Random.Range(3, 6))
                {
                    wormTouchCount = 0;
                    state = State.Empty;
                }
                wormTouchCount++;
                break;
            case State.Empty:
                if(selectedSprite != bookshelfEatenState[2])
                {
                    selectedSprite = bookshelfEatenState[2];
                    PlayPaperBurst();
                }
                break;
        }

        spriteRender.GetComponent<SpriteRenderer>().sprite = selectedSprite;
        // Debug.Log(selectedSprite);
    }


    //-----Moises-----
    private void PlayPaperBurst()
    {
        if(paperBurst != null)
        {
            paperBurst.Play();
        }
    }
    //----------------
}
