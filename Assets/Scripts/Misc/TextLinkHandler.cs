using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextLinkHandler : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI textMeshPro;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Get the index of the link clicked
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, Input.mousePosition, null);

        if (linkIndex != -1) // If a link was clicked
        {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            string url = linkInfo.GetLinkID(); // This gets the href value of the clicked link

            // Open the URL in the default web browser
            Application.OpenURL(url);
            Debug.Log("Opened URL: " + url);
        }
    }
}