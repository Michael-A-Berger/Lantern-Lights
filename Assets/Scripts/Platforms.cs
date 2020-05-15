using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    public GameObject mSprite;
    [SerializeField]
    Vector3 mScale;
    // Start is called before the first frame update
    void Start()
    {
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        mScale = transform.localScale;
        mSprite.transform.localScale = new Vector3(1/mScale.x, 1/mScale.y, 1);
        SpriteRenderer mSpriteRenderer = mSprite.transform.GetComponent<SpriteRenderer>();
        mSpriteRenderer.drawMode = SpriteDrawMode.Tiled;
        mSpriteRenderer.size = new Vector2(mScale.x, mScale.y);
    }
    private void OnDrawGizmos() {
        UpdateSprite();
    }
}
