using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class TestSender : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private TextAsset testServiceJson;

    private JObject testService;

    private JObject response;
    async void Start()
    {
        StartCoroutine(RunAtInterval());
        testService = JObject.Parse(testServiceJson.text);

        testService["service"]="test_srv";
        testService["request"]["data"]=true;
        string msg = testService.ToString();
        response = await UdpController.inst.PublishClientReq(msg);


    }
    
    IEnumerator RunAtInterval()
    {
        
        while (true) // Or some condition
        {
            
            Debug.Log(response);
            yield return new WaitForSeconds(1f); // Wait 1 second
        }
        
    }
}