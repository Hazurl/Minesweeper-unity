using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;

    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private SpriteRenderer objectSpriteRenderer;

    [SerializeField] private List<Sprite> digits0to8Sprite; 
    [SerializeField] private Sprite bombSprite;

    public bool IsRevealed => !backgroundSpriteRenderer.enabled;
    public int X;
    public int Y;

    public int Content = 0;
    public bool IsBomb => Content == -1;

    public bool IsFlag { get; private set; } = false;

    private void Awake() {
        backgroundSpriteRenderer.sprite = upSprite;
        objectSpriteRenderer.enabled = false;
    }

    public void HandleClickDown() {
        backgroundSpriteRenderer.sprite = downSprite;
    }

    public void HandleClickUp() {
        backgroundSpriteRenderer.sprite = upSprite;
    }

    public void Reveal() {
        Sprite sprite = IsBomb ? bombSprite : digits0to8Sprite[Content]; 

        backgroundSpriteRenderer.enabled = false;
        objectSpriteRenderer.sprite = sprite;
        objectSpriteRenderer.enabled = true;
    }

    public void ToggleFlag() {
        if (!IsRevealed) {
            IsFlag = !IsFlag;
            objectSpriteRenderer.enabled = !objectSpriteRenderer.enabled;
        }
    }
}
