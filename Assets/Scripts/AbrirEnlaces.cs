/* Script original de Jonathan Hopkins para abrir enlaces de un texto
 * https://gitlab.com/jonnohopkins/tmp-hyperlinks/blob/master/Assets/OpenHyperlinks.cs
 */
 
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class AbrirEnlaces : MonoBehaviour, IPointerClickHandler {

    private TextMeshProUGUI pTextMeshPro;
    private Canvas pCanvas;
    private Camera pCamera;

    void Awake () {
        pTextMeshPro = GetComponent<TextMeshProUGUI>();

        pCanvas = GetComponentInParent<Canvas>();

        // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
        if (pCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            pCamera = null;
        else
            pCamera = pCanvas.worldCamera;
    }

    public void OnPointerClick(PointerEventData eventData) {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, Input.mousePosition, pCamera);
        if (linkIndex != -1) { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

            // open the link id as a url, which is the metadata we added in the text field
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}