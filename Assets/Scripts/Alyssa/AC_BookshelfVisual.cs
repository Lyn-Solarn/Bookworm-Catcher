using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AC_BookshelfVisual : MonoBehaviour
{  
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

    
    
    public void Awake()
    {        
        AC_Bookshelf.OnCollisionWormBookshelf += OnCollisionWormBookshelf;
    }
    

    public void Start()
    {
        SelectRandomBookshelf();
        wormTouchCount = 0;
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
                    Debug.Log("change");
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
