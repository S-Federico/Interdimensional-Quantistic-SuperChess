using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;
    public Vector2 offset; // Offset to avoid the tooltip covering the mouse cursor

    private void Start()
    {
    }

    private void Update()
    {
        FollowMouse();
        AdjustLayout();
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
        int contentLength = contentField != null ? contentField.text.Length : 0;

        // Abilita o disabilita il layoutElement in base alla lunghezza del testo
        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit);
    }

    private void FollowMouse()
    {
        // Get the current mouse position and add the offset to prevent overlap
        Vector2 mousePosition = Input.mousePosition;
        Vector2 adjustedPosition = mousePosition + offset;
        transform.position = adjustedPosition;
    }

}
