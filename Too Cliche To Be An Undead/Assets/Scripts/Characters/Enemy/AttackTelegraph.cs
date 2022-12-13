using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTelegraph : MonoBehaviour
{
    [SerializeField] private Transform pivot;

    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private SpriteRenderer fillSprite;

    private float rad;

    public struct TelegraphData
    {
        public Vector2 telegraphSize;
        public Vector2 offset;
        public Quaternion telegraphRotation;
        public Sprite telegraphSprite;

        public TelegraphData(Vector2 _size, Vector2 _offset, Quaternion _rotation, Sprite _sprite)
        { 
            telegraphSize = _size;
            offset = _offset;
            telegraphRotation = _rotation;
            telegraphSprite = _sprite;
        }
        public TelegraphData(float _size, Vector2 _offset, Quaternion _rotation, Sprite _sprite)
        {
            telegraphSize = new Vector2(_size, _size);
            offset = _offset;
            telegraphRotation = _rotation;
            telegraphSprite = _sprite;
        }
    }

    public void Setup(TelegraphData newData, float time)
    {
        Setup(newData.telegraphSize, newData.offset, newData.telegraphRotation, newData.telegraphSprite, time);
    }
    public void Setup(float _size, Vector2 _offset, Quaternion _rotation, Sprite _sprite, float time)
    {
        Setup(new Vector2(_size, _size), _offset, _rotation, _sprite, time);
    }
    public void Setup(Vector2 _size, Vector2 _offset, Quaternion _rotation, Sprite _sprite, float time)
    {
        rad = _size.x;
        backgroundSprite.sprite = _sprite;
        fillSprite.sprite = _sprite;

        backgroundSprite.transform.localPosition = _offset;

        fillSprite.gameObject.transform.localScale = Vector3.zero;
        backgroundSprite.transform.localScale = _size * 2;

        SetSpriteAlpha(backgroundSprite, .25f);
        SetSpriteAlpha(fillSprite, .25f);

        pivot.transform.rotation = _rotation;

        fillSprite.transform.LeanScale(Vector3.one, time).setOnComplete(
        () =>
        {
            SetSpriteAlpha(backgroundSprite, 0);
            SetSpriteAlpha(fillSprite, 0);
        });
    }

    public void CancelTelegraph()
    {
        LeanTween.cancel(this.gameObject);
        SetSpriteAlpha(backgroundSprite, 0);
        SetSpriteAlpha(fillSprite, 0);
    }

    private void SetSpriteAlpha(SpriteRenderer sprite, float value)
    {
        Color c = sprite.color;
        c.a = value;
        sprite.color = c;
    }
}
