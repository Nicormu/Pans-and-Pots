using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DropCountdown : MonoBehaviour
{
    [SerializeField] private int from = 3;
    [SerializeField] private TMP_Text text;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip tickClip;
    [SerializeField] private AudioClip goClip;
    [SerializeField] private string goText = "GO!";

    private Coroutine routine;

    public void Begin(Action onDone)
    {
        Cancel();
        routine = StartCoroutine(Run(onDone));
    }

    public void Cancel()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;
        if (text != null) text.gameObject.SetActive(false);
    }

    private IEnumerator Run(Action onDone)
    {
        text.gameObject.SetActive(true);

        for (int i = from; i > 0; i--)
        {
            text.text = i.ToString();
            if (tickClip != null) source.PlayOneShot(tickClip);
            yield return Wait(1f);
        }

        text.text = goText;
        if (goClip != null) source.PlayOneShot(goClip);
        onDone?.Invoke();          

        yield return Wait(0.4f);    
        text.gameObject.SetActive(false);
        routine = null;
    }

    private IEnumerator Wait(float seconds)
    {
        float t = 0f;
        while (t < seconds) { t += Time.deltaTime; yield return null; }
    }
}