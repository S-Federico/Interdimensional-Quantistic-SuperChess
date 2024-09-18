using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public TextMeshProUGUI Text;
    // Start is called before the first frame update
    // Reference to the popup Panel
    // Action to be confirmed
    private System.Action onConfirmAction;
    private System.Action onCancelAction;
    private System.Action onFinish;

    void Start()
    {
    }

    // Function to display the popup and pass the action to confirm
    public void ShowPopup(string message, System.Action confirmAction, System.Action cancelAction, System.Action onFinish)
    {
        Text.text = message;
        onConfirmAction = confirmAction;
        onCancelAction = cancelAction;
        this.onFinish = onFinish;
    }

    // Called when the confirm button is pressed
    public void OnConfirm()
    {
        onConfirmAction?.Invoke();  // Invoke the action passed in
        onFinish?.Invoke();
    }

    // Called when the cancel button is pressed
    public void OnCancel()
    {
        onCancelAction?.Invoke();
        onFinish?.Invoke();
    }
}
