using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// why not have this code in the GpsLocation script? idk -matthew

public class SelectDestination : MonoBehaviour
{
    [SerializeField] Image bg;
    public bool isSelected = false;
    public void SetBgColor()
    {
        bg.color = new Color(0, 134/255f, 1f, 33 / 255f);
        isSelected = true;
        SetIconGreen();

        Transform listParent = transform.parent;
        foreach(Transform child in listParent)
        {
            if (child.gameObject.name == "Button") continue;
            var scr = child.GetComponent<SelectDestination>();
            if (scr == this) continue;
            scr.ResetBackgroundColor();
            scr.UnhighlightIcon();
        }
    }

    private void Update()
	{
	    if (!isSelected) return;

	    var gps = GetComponent<GpsLocation>();
	    if (gps == null) return; // prevents crashes!

	    if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.R))
	    {
		Debug.Log("Ctrl + R detected");
		gps.ClearWaypoints();
	    }
	}
    public void ResetBackgroundColor()
    {
        bg.color = new Color(0, 1f, 14 / 255f, 0);
        isSelected = false;
    }

    public void UnhighlightIcon()
	{
	    var scr = GetComponent<GpsLocation>();
	    if (scr == null || scr.iconObject == null) return;

	    scr.iconObject.transform.GetChild(0)
		.GetComponent<SpriteRenderer>().color = Color.white;
	    scr.SetHighlightStatus(false);
	}

	public void SetIconGreen()
	{
	    var scr = GetComponent<GpsLocation>();
	    if (scr == null || scr.iconObject == null) return;

	    scr.iconObject.transform.GetChild(0)
		.GetComponent<SpriteRenderer>().color = new Color(0, 1f, 2 / 255f);

	    scr.SetHighlightStatus(true);
	}

}
