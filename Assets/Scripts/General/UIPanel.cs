using UnityEngine;

public class UIPanel : MonoBehaviour
{
    [SerializeField] CanvasGroup panel;

    public void Show()
    {
        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }
    public void Hide()
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }
}
