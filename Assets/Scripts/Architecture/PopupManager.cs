using UnityEngine;
using UnityEngine.UI;

public class PopupManager : Singleton<PopupManager>
{
    private GameObject popupPrefab;

    void Awake() {
        popupPrefab = AssetsManager.Instance.PopupPrefab;
    }

    public void ShowPopup(string message, System.Action confirmAction, System.Action cancelAction)
    {
        GameObject popup = Instantiate(popupPrefab);
        popup.GetComponent<PopupController>().ShowPopup(message, confirmAction, cancelAction, () => Destroy(popup));
    }
}
