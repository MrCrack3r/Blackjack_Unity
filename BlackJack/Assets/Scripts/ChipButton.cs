using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class ChipButton : MonoBehaviour
{
    public int chipValue;

    [Header("Animacijos nustatymai")]
    public float bounceScale = 1.3f;
    public float bounceDuration = 0.1f;

    [Header("Popup tekstas")]
    public GameObject popupTextPrefab;
    public Transform popupSpawnPoint;

    private Vector3 originalScale;
    private bool isAnimating = false;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnChipClicked()
    {
        GameManager.Instance.AddBet(chipValue);

        if (!isAnimating)
            StartCoroutine(BounceAnimation());

        SpawnPopupText();
    }

    private void SpawnPopupText()
    {
        if (popupTextPrefab == null) return;

        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas == null) return;

        GameObject popup = Instantiate(popupTextPrefab);
        popup.transform.SetParent(rootCanvas.transform, false);

        TextMeshProUGUI tmp = popup.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = "+" + chipValue + "$";

        RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            mouseScreenPos,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out mousePos
        );

        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.anchoredPosition = mousePos;

        StartCoroutine(AnimatePopup(popup));
    }

    private IEnumerator AnimatePopup(GameObject popup)
    {
        RectTransform rt = popup.GetComponent<RectTransform>();
        TextMeshProUGUI tmp = popup.GetComponent<TextMeshProUGUI>();

        if (rt == null || tmp == null) yield break;

        Material mat = new Material(tmp.fontMaterial);
        mat.EnableKeyword("OUTLINE_ON");
        mat.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.25f);
        mat.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);
        mat.EnableKeyword("UNDERLAY_ON");
        mat.SetColor(ShaderUtilities.ID_UnderlayColor, new Color(0f, 0f, 0f, 0.8f));
        mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0.5f);
        mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -0.5f);
        tmp.fontMaterial = mat;

        float duration = 0.8f;
        float t = 0f;

        Vector3 startPos = rt.position;
        Vector3 endPos = startPos + new Vector3(0f, 80f, 0f);

        Color startColor = tmp.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        rt.localScale = Vector3.one * 0.5f;

        float popIn = 0.15f;
        while (t < popIn)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 1.2f, t / popIn);
            yield return null;
        }

        t = 0f;
        float scaleBack = 0.1f;
        while (t < scaleBack)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, t / scaleBack);
            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            rt.position = Vector3.Lerp(startPos, endPos, progress);
            tmp.color = Color.Lerp(startColor, endColor, progress);
            yield return null;
        }

        Destroy(popup);
    }

    private IEnumerator BounceAnimation()
    {
        isAnimating = true;

        float t = 0f;
        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * bounceScale, t / bounceDuration);
            yield return null;
        }

        t = 0f;
        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale * bounceScale, originalScale, t / bounceDuration);
            yield return null;
        }

        t = 0f;
        while (t < bounceDuration * 0.5f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.1f, t / (bounceDuration * 0.5f));
            yield return null;
        }

        t = 0f;
        while (t < bounceDuration * 0.5f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale * 1.1f, originalScale, t / (bounceDuration * 0.5f));
            yield return null;
        }

        transform.localScale = originalScale;
        isAnimating = false;
    }
}