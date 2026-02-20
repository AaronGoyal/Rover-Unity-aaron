using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

public class RobotArmController : MonoBehaviour
{
    public static RobotArmController inst;
    public Transform[] joints; 
    public Transform[] vis_joints;

    [SerializeField] float angle;
    [SerializeField] List<float> offsets;
    [SerializeField] public List<int> multipliers;

    public float[] trueAngles = {0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};

    public GameObject rover_arm_vis;

    
    void Start()
    {
        inst = this;
        StartCoroutine(joint_state_subscription());        
        UdpController.inst.ConfigureSubscription("joint_states","sensor_msgs/msg/JointState");
        Debug.Log("Starting joint controller");

    }
 

    IEnumerator joint_state_subscription()
    {
        while (true)
        {
            JObject jointMessage = UdpController.inst.GetLatestMessage("joint_states");
            if(jointMessage != null)
            {
                if (joints == null || joints.Length < 6)
                {
                    Debug.LogError("[RobotArmController] Joints array not set or incomplete!");
                    yield break;
                }
                JArray positions = (JArray)jointMessage["data"]["position"];
                // Map each joint
                // Multiply by multiplier, convert rad → deg, add offset
                joints[0].localRotation = Quaternion.Euler(
                    0,
                    positions[0].Value<float>() * Mathf.Rad2Deg * multipliers[0] + offsets[0],
                    0
                );

                joints[1].localRotation = Quaternion.Euler(
                    0,
                    0,
                    positions[1].Value<float>() * Mathf.Rad2Deg * multipliers[1] + offsets[1]
                );

                joints[2].localRotation = Quaternion.Euler(
                    0,
                    positions[2].Value<float>() * Mathf.Rad2Deg * multipliers[2] + offsets[2],
                    0
                );

                joints[3].localRotation = Quaternion.Euler(
                    0,
                    0,
                    positions[3].Value<float>() * Mathf.Rad2Deg * multipliers[3] + offsets[3]
                );

                joints[4].localRotation = Quaternion.Euler(
                    0,
                    0,
                    positions[4].Value<float>() * Mathf.Rad2Deg * multipliers[4] + offsets[4]
                );

                joints[5].localRotation = Quaternion.Euler(
                    0,
                    positions[5].Value<float>() * Mathf.Rad2Deg * multipliers[5] + offsets[5],
                    0
                );

                // Update trueAngles array in radians
                trueAngles[0] = positions[0].Value<float>();
                trueAngles[1] = positions[1].Value<float>();
                trueAngles[2] = positions[2].Value<float>();
                trueAngles[3] = positions[3].Value<float>();
                trueAngles[4] = positions[4].Value<float>();
                trueAngles[5] = positions[5].Value<float>();

            }
            yield return new WaitForSeconds(0.03f);
        }
    }
    public void Receive(string message){
        if(message.Contains("nan")) return;

        var parts = message.Split(";");

        try {


        }
        catch (Exception e){
            Debug.LogError("[RobotArmController]: " + e.Message + "\nOriginal msg: " + message);
            return;
        }
    }

    public void visualize_goal(List<float> angles)

    {   
        
        Vector3[] axes = new Vector3[] {Vector3.up, Vector3.forward, Vector3.forward, Vector3.up, Vector3.forward, Vector3.up};
        for(int i = 0; i < 6; i++)
        {
            vis_joints[i].localRotation = Quaternion.AngleAxis(angles[i]* 57.2958f * multipliers[i], axes[i]);
        }
        rover_arm_vis.SetActive(true);
        
    }
    public void remove_vis(){
        rover_arm_vis.SetActive(false);
    }
   

}
