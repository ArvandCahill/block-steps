using System.Collections;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, Iinteractable
{
    public abstract void Interact(Vector3 position);

    [SerializeField] private float duration = 0.2f;

    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private Coroutine fadeRoutine;
    private bool isTransparent;

    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
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
        rend.GetPropertyBlock(mpb);

        Color color = mpb.HasColor(ColorID)
            ? mpb.GetColor(ColorID)
            : rend.sharedMaterial.color;

        float startAlpha = color.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);

            mpb.SetColor(ColorID, color);
            rend.SetPropertyBlock(mpb);

            yield return null;
        }

        color.a = targetAlpha;
        mpb.SetColor(ColorID, color);
        rend.SetPropertyBlock(mpb);

        fadeRoutine = null;
    }
}