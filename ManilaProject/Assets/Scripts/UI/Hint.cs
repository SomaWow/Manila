using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour {
    private Text hintText;
    private Color color;
    public float hintShowTime = 5f;

    private void Awake()
    {
        EventCenter.AddListener<string>(EventType.Hint, Show);
        hintText = GetComponent<Text>();
        color = hintText.color;
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<string>(EventType.Hint, Show);
    }
    private void Show(string text)
    {
        hintText.text = text;
        hintText.DOColor(new Color(color.r, color.g, color.b, 1), 0.3f).OnComplete(()=> {
            StartCoroutine(Delay());
        });
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(hintShowTime);
        hintText.DOColor(new Color(color.r, color.g, color.b, 0), 0.3f);
    }
}
