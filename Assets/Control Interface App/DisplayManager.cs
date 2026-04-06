using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    [SerializeField] GameObject canvasDisplay1;
    [SerializeField] GameObject canvasDisplay2;
    [SerializeField] Camera cameraDisplay1;
    [SerializeField] Camera cameraDisplay2;

    void Start()
    {
        // Display 1 always active
        cameraDisplay1.enabled = true;
        canvasDisplay1.SetActive(true);

        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            cameraDisplay2.enabled = true;
            canvasDisplay2.SetActive(true);
            Debug.Log("[DisplayManager] Second monitor activated.");
        }
        else
        {
            // No second monitor, disable second canvas and camera
            cameraDisplay2.enabled = false;
            canvasDisplay2.SetActive(false);
            Debug.Log("[DisplayManager] No second monitor found.");
        }
    }
}