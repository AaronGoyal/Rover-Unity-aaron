using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

public class TestReceiver : MonoBehaviour
{
    // Start is called before the first frame update
    private JObject driveMessage;
    void Start()
    {
        StartCoroutine(RunAtInterval());
        UdpController.inst.ConfigureSubscription("joint_states","sensor_msgs/msg/JointState");
    }
    
    IEnumerator RunAtInterval()
    {
        while (true) // Or some condition
        {
            driveMessage = UdpController.inst.GetLatestMessage("joint_states");
            Debug.Log(driveMessage);
            Debug.Log($"UdpController instance ID: {UdpController.inst.GetInstanceID()}");

            
            yield return new WaitForSeconds(1f); // Wait 1 second
        }
    }
}