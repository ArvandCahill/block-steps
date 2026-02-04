using System.Collections;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, Iinteractable
{
    public abstract void Interact(Vector3 position);

    [SerializeField] private float duration = 0.2f;

    private Material material;
    private Coroutine fadeRoutine;
    private bool isTransparent;

    private void Awake()
    {
        material = GetComponentInChildren<Renderer>().material;
    }

    public Vector3Int GetPosition()
    {
        Vector3 pos = transform.position;
        return new Vector3Int(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(pos.y),
            Mathf.RoundToInt(pos.z)
        );
    }

    public void FadeOut()
    {
        if (isTransparent) return;

        StartFadeTo(0.2f);
        isTransparent = true;
    }

    public void FadeIn()
    {
        if (!isTransparent) return;

        StartFadeTo(1f);
        isTransparent = false;
    }

    private void StartFadeTo(float targetAlpha)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        Color color = material.color;
        float startAlpha = color.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            material.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        material.color = new Color(color.r, color.g, color.b, targetAlpha);
        fadeRoutine = null;
    }
}