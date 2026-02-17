using UnityEngine;
using System.Diagnostics;

public class GStreamerLauncher : MonoBehaviour
{
    public string portNum;
    public string sourcePort; 
    private Process gStreamerProcess;
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

	    UnityEngine.Debug.Log("GStreamer launched.");
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

