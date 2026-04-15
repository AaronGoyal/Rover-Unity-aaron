using UnityEngine;

public class NetworkMonitor : MonoBehaviour
{
    public RealtimeChart nanostationNoise; // Drag the chart here in Inspector
    public RealtimeChart nanostationSignal; // Drag the chart here in Inspector
    public RealtimeChart powerbeamNoise; // Drag the chart here in Inspector
    public RealtimeChart powerbeamSignal; // Drag the chart here in Inspector
    public RealtimeChart rocketNoise; // Drag the chart here in Inspector
    public RealtimeChart rocketSignal; // Drag the chart here in Inspector
    public RealtimeChart rxRate; // Drag the chart here in Inspector
    public RealtimeChart txRate; // Drag the chart here in Inspector


    
    
    private float updateInterval = 0.1f;
    private float timer;
    
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= updateInterval)
        {
            timer = 0f;
            
            float throughput = GetNetworkThroughput();
            nanostationNoise.AddDataPoint(throughput);

            throughput = GetNetworkThroughput();
            nanostationSignal.AddDataPoint(throughput);

            throughput = GetNetworkThroughput();
            powerbeamNoise.AddDataPoint(throughput);

            throughput = GetNetworkThroughput();
            powerbeamSignal.AddDataPoint(throughput);

            throughput = GetNetworkThroughput();
            rocketNoise.AddDataPoint(throughput);

            throughput = GetNetworkThroughput();
            rocketSignal.AddDataPoint(throughput);
            
            throughput = GetNetworkThroughput();
            rxRate.AddDataPoint(throughput);

            throughput = GetNetworkThroughput();
            txRate.AddDataPoint(throughput);
        }
    }
    
    float GetNetworkThroughput()
    {
        
        return Random.Range(0f, 100f);
    }
}