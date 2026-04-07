using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SetLocaltButtonClick : MonoBehaviour
{
    public bool isModifying = false;
    [SerializeField] TextMeshProUGUI btnText;
    //[SerializeField] Image btnColor;
    [SerializeField] MapController map;
    //[SerializeField] SelectDestination selectScr;

    public void OnClick()
    {	
    	isModifying = !isModifying;
    	
    	if (GpsLocation.inst == null)
	    {
		Debug.LogError("GpsLocation.inst is NULL — no GPS object in scene!");
		return;
	    }
	if (btnText == null)
	{
    		Debug.LogError("btnText is NULL — assign it in the Inspector!");
		return;
	}

	if (LocationAdder.inst == null)
	{
		Debug.LogError("LocationAdder.inst is NULL — no LocationAdder in scene!");
    		return;
	}

        // 1. pull GPS location from a global provider instead of a parent component
        double lat = GpsLocation.inst.latitude;
        double lon = GpsLocation.inst.longitude;

        if (double.IsNaN(lat) || double.IsNaN(lon))
        {
            Debug.LogWarning("GPS location not ready!");
            return;
        }
        else 
        {
            //Debug.Log("Lat: %d\n", lat);
            //Debug.Log($"The value is: {lat}");
        }
        if (isModifying)
        {
            btnText.text = "Finish";
            //btnColor.color = new Color(255 / 255f, 131 / 255f, 136 / 255f);
            GpsLocation locationScript /*= GpsLocation.inst;*/ = transform.GetComponentInParent<GpsLocation>();
            LocationAdder.inst.OpenAddingMode();
            //WaypointAdder.inst.OpenAddingMode(locationScript);
            Debug.Log("Modify");
        }
        else
        {
            btnText.text = "+ Click";
            //btnColor.color = new Color(115 / 255f, 232 / 255f, 255 / 255f);
            LocationAdder.inst.CloseAddingMode();
            //WaypointAdder.inst.CloseAddingMode();
        }

        // 2. add marker
        //map.AddLocationMarker(lat, lon);

        // 3. UI feedback
        //btnText.text = "Added!";
        //btnColor.color = new Color(115/255f, 232/255f, 255/255f);

        //selectScr.SetBgColor();
    }
}

