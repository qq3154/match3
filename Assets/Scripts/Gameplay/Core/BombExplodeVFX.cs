using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplodeVFX : MonoBehaviour
{    
    [SerializeField] Sprite[] spriteBomb4;
    [SerializeField] Sprite[] spriteBomb3x3;    
    SpriteRenderer _spriteRenderer;
    Animator _animator;
    

    #region GetComponents
    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _animator = gameObject.GetComponent<Animator>();
    }
    #endregion

    #region Public Methods
    
    public void Destroy3x3(Vector2 position, Board.DotId id, Board.BombId bombId)
    {
        gameObject.transform.position = position;
        _spriteRenderer.sprite = spriteBomb3x3[(int)id];
        _animator.Play("Bomb3x3Explode");
        Destroy(gameObject, 0.4f);                                                          
    }
    
    public void Destroy5x5(Vector2 position, Board.DotId id, Board.BombId bombId)
    {
        gameObject.transform.position = position;
        _spriteRenderer.sprite = spriteBomb3x3[(int)id];
        _animator.Play("Bomb5x5Explode");
        Destroy(gameObject, 0.4f);
    }

    public void DestroyTile(Vector2 position, Board.DotId id, Board.BombId bombId)
    {
        gameObject.transform.position = position;
        _spriteRenderer.sprite = spriteBomb4[(int)id];
        if (bombId == Board.BombId.Bomb4Vertical)
        {
            gameObject.transform.Rotate(0, 0, 90);
        }
        _animator.Play("Bomb4Explode");
        Destroy(gameObject, 0.4f);
    }

    public void DestroyTile3X(Vector2 position, Board.DotId id, Board.BombId bombId)
    {
        gameObject.transform.position = position;
        _spriteRenderer.sprite = spriteBomb4[(int)id];        
        if (bombId == Board.BombId.Bomb4Vertical)
        {
            gameObject.transform.Rotate(0, 0, 90);
        }
        _animator.Play("DestroyTile3X");
        Destroy(gameObject, 0.4f);
    }
    #endregion
}
