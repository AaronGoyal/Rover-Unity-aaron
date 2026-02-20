using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using TMPro;

public class GStreamerLauncher : MonoBehaviour
{
    public string portNum;
    public string sourcePort;
    private Process gStreamerProcess;
    public TMP_Dropdown dropdown;

    private Dictionary<int, string> cameraPortMap = new Dictionary<int, string>()
    {
        { 0, "42067" },
        { 1, "42068" },
        { 2, "42069" },
        { 3, "42070" },
        { 4, "42071" },

    };

    public int defaultCameraIndex = 3;

    void Start()
    {

        dropdown.value = defaultCameraIndex;
        dropdown.RefreshShownValue();

        sourcePort = cameraPortMap[defaultCameraIndex];
        UnityEngine.Debug.Log("Default camera set to index " + defaultCameraIndex + ", port: " + sourcePort);

        dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    public void OnDropdownChanged(int index)
    {
        if (cameraPortMap.ContainsKey(index))
        {
            StopGStreamer();

            sourcePort = cameraPortMap[index];
            UnityEngine.Debug.Log("Camera " + index + " selected. Source port set to: " + sourcePort);

            LaunchGStreamer();
        }
        else
        {
            UnityEngine.Debug.LogWarning("No port mapping found for dropdown index: " + index);
        }
    }

    public void LaunchGStreamer()
    {
        if (gStreamerProcess == null)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "/bin/bash";
            startInfo.Arguments = "-c \"gst-launch-1.0 udpsrc port=" + sourcePort +
                " caps=\\\"application/x-rtp, media=(string)video, clock-rate=(int)90000, encoding-name=(string)H265\\\" " +
                "! rtpulpfecdec ! rtpjitterbuffer latency=200 ! rtph265depay ! h265parse ! " +
                "queue max-size-buffers=3000 max-size-time=0 max-size-bytes=0 ! " +
                "avdec_h265 ! videoconvert ! videorate ! video/x-raw,format=RGB,framerate=30/1 ! " +
                "tcpclientsink host=127.0.0.1 port=" + portNum + " sync=false\"";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            gStreamerProcess = new Process();
            gStreamerProcess.StartInfo = startInfo;
            gStreamerProcess.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);
            gStreamerProcess.ErrorDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);
            gStreamerProcess.Start();
            gStreamerProcess.BeginOutputReadLine();
            gStreamerProcess.BeginErrorReadLine();
            UnityEngine.Debug.Log("GStreamer launched on port: " + sourcePort);
        }
    }

    public void StopGStreamer()
    {
        if (gStreamerProcess != null && !gStreamerProcess.HasExited)
        {
            gStreamerProcess.Kill();
            gStreamerProcess.WaitForExit();
            UnityEngine.Debug.Log("GStreamer process stopped.");
            gStreamerProcess = null;
        }
    }
}