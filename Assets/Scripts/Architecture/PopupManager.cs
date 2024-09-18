using UnityEngine;
using UnityEngine.UI;

public class PopupManager : Singleton<PopupManager>
{
    private GameObject popupPrefab;

    void Start() {

    }

    public void ShowPopup(string message, System.Action confirmAction, System.Action cancelAction)
    {
        if (popupPrefab == null) popupPrefab = Resources.Load<GameObject>("PopupPrefab");
        GameObject popup = Instantiate(popupPrefab);
        popup.GetComponent<PopupController>().ShowPopup(message, confirmAction, cancelAction, () => Destroy(popup));
    }
}
