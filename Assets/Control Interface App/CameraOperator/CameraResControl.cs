using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class CameraResControl : MonoBehaviour
{
    [SerializeField] private TextAsset camServiceJson;
    private JObject camService;

    private JObject camServiceResponse;
    // Start is called before the first frame update
    void Start()
    {
        

    }


    // Update is called once per frame
    //public IEnumerator changeCamRes()
    //{
        
    //}
    public void setHighResolution()
    {
        camService = JObject.Parse(camServiceJson.text);
        camService["service"]="/cam/service";
        camService["request"]["request"]["preset_level"] = 1;
        camService["request"]["request"]["bitrate"] = 8000000;
        camService["request"]["request"]["stream_width"] = 1920;
        camService["request"]["request"]["stream_height"] = 1080;
        camService["request"]["request"]["fec_percentage"] = 65;
        string msg = camService.ToString();

    }

        
    
 

}
