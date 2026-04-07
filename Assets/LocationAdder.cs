using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocationAdder : MonoBehaviour
{
    public static LocationAdder inst;
    public static bool inAddingMode = false;
    public GpsLocation currentLocationBeingModified = null;
    public Texture2D cursorTexture; // Assign your cursor image in the Inspector
    public Vector2 hotspot = Vector2.zero; // Adjust if needed
    public float t;
    //public Camera cam;
    [SerializeField] RectTransform mapBox;
    [SerializeField] Camera cam;
    [SerializeField] GameObject waypointIcon;
    [SerializeField] MapController mapController;
    //const int zoom = 19
    
    public bool isOverMap = false;
    private void Awake()
    {
        inst = this;
    }
    public void OpenAddingMode()
    {
        inAddingMode = true;
        //currentLocationBeingModified = location;
    }

    public void CloseAddingMode()
    {
        inAddingMode = false;
        currentLocationBeingModified = null;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void ChangeCursor(bool isNowOverMap)
    {
        if (isNowOverMap == isOverMap) return;
        isOverMap = isNowOverMap;
        if (isOverMap)
        {
            //Cursor.SetCursor(cursorTexture, hotspot, CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }
    }
    
    private void Update()
	{
	    if (!inAddingMode) return;

	    bool isNowOverMap = CameraControl.inst.IsMouseOverMap();
	    ChangeCursor(isNowOverMap);

	    // GetMouseButtonDown returns true ONLY on the frame the user presses the button
	    if(Input.GetMouseButtonDown(0)) 
	    {
		if (isOverMap) // Ensure we only add points when actually over the map
		{
		    AddPointAtMouse();
		}
	    }
	}
	
	

    double LonToTileX(double lon, int zoom) {
        return (lon + 180.0) / 360.0 * (1 << zoom);
    }

    double LatToTileY(double lat, int zoom) {
        double latRad = lat * Math.PI / 180.0;
        return (1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * (1 << zoom);
    }

    // Use Math.Pow to avoid 32-bit integer overflow at high zoom levels
/*public double TileXToLon(double x, int zoom) {
    return x / Math.Pow(2.0, zoom) * 360.0 - 180.0;
}

public double TileYToLat(double y, int zoom) {
    double n = Math.PI - 2.0 * Math.PI * y / Math.Pow(2.0, zoom);
    // 180.0 / PI is approx 57.29577
    return 57.295779513082323 * Math.Atan(Math.Sinh(n));
}*/
    
    public Vector2 GetWorldPos()
    {
        Vector2 mouseScreenPosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapBox, mouseScreenPosition, Camera.main, out Vector2 localPoint);
        localPoint /= mapBox.rect.width; // convert to percentage
        Vector2 offsetFromCam = cam.orthographicSize * 2 * localPoint;

        Vector2 camPos = new Vector2(cam.transform.position.x, cam.transform.position.y);
        var worldPos = camPos + offsetFromCam;
        return worldPos;
    }
    
    double TileXToLon(double x, int zoom) {
        return x / (1 << zoom) * 360.0 - 180.0;
    }

    double TileYToLat(double y, int zoom) {
        double n = Math.PI - 2.0 * Math.PI * y / (1 << zoom);
        return 180.0 / Math.PI * Math.Atan(Math.Sinh(n));
    }


void AddPointAtMouse()
{
    if (!isOverMap) return;
    
    var worldPos = GetWorldPos();
    
    // 1. USE DYNAMIC VALUES FROM YOUR MAP DATA
    // Access the current active map's real-world center
    double centerLat = MapController.instance.GetCurrMapLat(); // Add a getter for currMap.lat
    double centerLon = MapController.instance.GetCurrMapLon(); // Add a getter for currMap.lon
    
    // Use the zoom level you are currently viewing
    int zoom = 19; 
    float unityUnitsPerTile = 1.0f; 
    
    // 2. CONVERT CENTER TO TILE SPACE
    double centerTileX = LonToTileX(centerLon, zoom);
    double centerTileY = LatToTileY(centerLat, zoom);

    // 3. CALCULATE CLICKED TILE
    // We remove the magic number subtractions here.
    double iconTileX = (worldPos.x / unityUnitsPerTile) + centerTileX;
    double iconTileY = -(worldPos.y / unityUnitsPerTile) + centerTileY;

    // 4. CONVERT BACK TO LAT/LON
    double lon = (TileXToLon(iconTileX, zoom));
    double lat = TileYToLat(iconTileY, zoom);

    // 5. SPAWN AND REGISTER
    GameObject newObject = Instantiate(waypointIcon, worldPos, Quaternion.identity);
    newObject.transform.SetParent(MapController.instance.iconsParent, true);
    
    // This now sends the accurate Oregon coordinates!
    MapController.instance.AddCoordinateClick(lat, lon, "Click", 1);
    Debug.Log($"The long is: {lon}");
    Debug.Log($"The lat is: {lat}");

}
}
