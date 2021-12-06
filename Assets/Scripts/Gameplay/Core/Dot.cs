using System.Collections;
using System.Collections.Generic;
using Observer;
using UnityEngine;
using DG.Tweening;

public class Dot : MonoBehaviour
{
    public Board.DotId id;
    [SerializeField] Sprite[] bomb;           

    SpriteRenderer _spriteRenderer;
    Animator _animator;

    Vector3 _destination;
    float _speed = 20;        
    
    private void Awake()
    {
        _destination = transform.position;
        _animator = gameObject.GetComponent<Animator>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _animator.Play("Idle");
    }

    private void Start()
    {
        

    }

    private void Update()
    {
        float step = _speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, _destination, step);
    }

    public void Move(Vector2 destination, float speed) 
    {
        this._destination = destination;
        this._speed = speed;              
    }

    public void ChangeSprite(Board.BombId x)
    {
        
        _spriteRenderer.sprite = bomb[(int)x];
        _animator.Play("CreateBomb");
        this.PostEvent(EventID.OnDestroyDot, this.id);
    }

    public void MoveWhenCreateBomb(float X, float Y)
    {
        _animator.Play("Rolling");
        _spriteRenderer.sprite = bomb[0];
        this._destination = new Vector2(X + 0.5f, Y + 0.5f);
        this.PostEvent(EventID.OnDestroyDot, this.id);       
        Object.Destroy(gameObject, 0.4f);
    }

    public void DestroyThisDot()
    {
        this.PostEvent(EventID.OnDestroyDot, this.id);

        _spriteRenderer.sprite = bomb[0];
        _animator.Play("OnDestroy");        
        StartCoroutine(IE_DestroyDot());                
        
    }

    IEnumerator IE_DestroyDot()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        Move(new Vector2(5, 11), 15);
        Object.Destroy(gameObject, 0.5f);
    }

}
