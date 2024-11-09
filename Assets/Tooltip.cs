using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;
    public Vector2 offset; // Offset to avoid the tooltip covering the mouse cursor
    public CanvasGroup group;
    public float targetalpha;
    public const float defaultDelay = 0.7f;
    private Coroutine showCoroutine;
    private int toShowInstanceId;

    RectTransform rectTransform;
    private void Start()
    {
        group = transform.parent.GetComponent<CanvasGroup>();
        group.alpha = 0.0f;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0, 0); // Pivot in basso a sinistra
    }

    private void Update()
    {
        // Do nothing if game paused
        if (GameManager.Instance.IsPaused) return;

        FollowMouse();
        AdjustLayout();
        AdjustAlpha();
    }

    public void AdjustAlpha()
    {
        group.alpha = Mathf.Lerp(group.alpha, targetalpha, 0.05f);
    }

    public void SetText(string headerText, string contentText)
    {
        // Assicurati che i parametri non siano null, altrimenti imposta a stringa vuota
        headerText = headerText ?? string.Empty;
        contentText = contentText ?? string.Empty;

        // Imposta il testo solo se i campi di testo non sono nulli
        if (headerField != null)
        {
            headerField.text = headerText;
        }

        if (contentField != null)
        {
            contentField.text = contentText;
        }

        // Aggiorna il layout in base alla lunghezza dei nuovi testi
        AdjustLayout();
    }

private void AdjustLayout()
{
    // Controlla che i campi di testo non siano nulli prima di leggere la loro lunghezza
    int headerLength = headerField != null ? headerField.text.Length : 0;

    int firstLineContentLength = 0;
    if (contentField != null && !string.IsNullOrEmpty(contentField.text))
    {
        // Ottieni la prima riga del testo della descrizione
        string firstLine = contentField.text.Split('\n')[0];
        firstLineContentLength = firstLine.Length;
    }

    // Abilita o disabilita il layoutElement in base alla lunghezza della prima riga del testo
    layoutElement.enabled = (headerLength > characterWrapLimit || firstLineContentLength > characterWrapLimit);
}


    private void FollowMouse()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 adjustedPosition = mousePosition + offset;
        transform.position = adjustedPosition;
    }

    public void Show()
    {
        targetalpha = 1.0f;
    }
    public void Hide()
    {
        targetalpha = 0.0f;
        group.alpha = 0.0f;

        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }
    }

    public void ShowAfterDelay(int instanceId, float delay = defaultDelay)
    {
        toShowInstanceId = instanceId;

        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
        }

        showCoroutine = StartCoroutine(ShowWithDelay(instanceId, delay));
    }

    private IEnumerator ShowWithDelay(int instanceId, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (instanceId == toShowInstanceId)
        {
            Show();
        }
    }

}
