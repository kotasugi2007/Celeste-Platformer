using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DashEffect : MonoBehaviour
{
    private Movement move;
    private AnimationScript anim;
    private SpriteRenderer sr;
    public Transform ghostsParent;
    public Color trailColor;
    public Color fadeColor;
    public float ghostInterval;
    public float fadeTime;

    public GameObject[] dashEffectors;

    private void Start()
    {
        anim = FindObjectOfType<AnimationScript>();
        move = FindObjectOfType<Movement>();
        sr = GetComponent<SpriteRenderer>();
        
    }

    public void ShowGhost()
    {   
        Vector3 scaler = transform.localScale;
        scaler.x = move.side;
        scaler.y = 1;
        dashEffectors[0].transform.localScale = scaler;
        dashEffectors[1].transform.localScale = scaler;
        dashEffectors[2].transform.localScale = scaler;

        Sequence s = DOTween.Sequence();

        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            Transform currentGhost = ghostsParent.GetChild(i);
            s.AppendCallback(()=> currentGhost.position = new Vector2(move.transform.position.x, move.transform.position.y + 0.34f));
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().flipX = anim.sr.flipX);
            s.AppendCallback(()=>currentGhost.GetComponent<SpriteRenderer>().sprite = anim.sr.sprite);
            s.Append(currentGhost.GetComponent<SpriteRenderer>().material.DOColor(trailColor, 0));
            s.AppendCallback(() => FadeSprite(currentGhost));
            s.AppendInterval(ghostInterval);
        }
    }

    public void FadeSprite(Transform current)
    {
        current.GetComponent<SpriteRenderer>().material.DOKill();
        current.GetComponent<SpriteRenderer>().material.DOColor(fadeColor, fadeTime);
    }
}
