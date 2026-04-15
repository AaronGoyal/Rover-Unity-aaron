using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip clip;      // Assign in Inspector
    private AudioSource source;

    void Start()
    {
        source = gameObject.AddComponent<AudioSource>(); // Add AudioSource at runtime
        source.clip = clip;
        source.loop = true;
        StartCoroutine(monitor_ros());        

        UdpController.inst.ConfigureSubscription("servo_node/status","std_msgs/msg/Int8");
    }

    IEnumerator monitor_ros()
    {
        int zeroCount = 0;
        const int zeroThreshold = 5;

        while (true)
        {
            JObject msg = UdpController.inst.GetLatestMessage("servo_node/status");

            if (msg != null)
            {
                int state = (int)msg["data"]["data"];

                if (state > 0)
                {
                    // reset zero counter
                    zeroCount = 0;

                    // play only if not already playing
                    if (!source.isPlaying)
                    {
                        source.Play();
                        Debug.Log("Audio playing");
                    }
                }
                else // state == 0
                {
                    zeroCount++;
                    Debug.Log("Zero count: " + zeroCount);

                    if (zeroCount >= zeroThreshold && source.isPlaying)
                    {
                        source.Stop();
                        Debug.Log("Audio stopped");
                    }
                }
            }

            yield return new WaitForSeconds(0.03f);
        }
    }


    // Or trigger via method
    public void PlaySound()
    {
        source.PlayOneShot(clip); // Plays once, without interrupting
    }
}
