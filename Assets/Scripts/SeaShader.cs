using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct WaveShaderData
{
    public WaveShaderData(Wave wave)
    {
        amp = wave.amplitude;
        position = wave.seaBehavior.transform.TransformPoint(wave.position);
        width = wave.seaBehavior.WaveWidth;
        decay = wave.decay;
    }
    public float width;
    public float amp;
    public Vector3 position;
    public float decay;
}

public class SeaShader : MonoBehaviour
{
    public SeaBehavior seaBehavior;
    public Material mat;
    
    void Awake()
    {
        mat.SetFloatArray("_WaveAmps", new float[25]);
        mat.SetFloatArray("_WaveXPositions", new float[25]);
        mat.SetFloatArray("_WaveYPositions", new float[25]);
        mat.SetFloatArray("_WaveWidths", new float[25]);
        mat.SetFloatArray("_WaveDecays", new float[25]);
    }

    void Update()
    {
        if (seaBehavior.waves.Count == 0) 
        { 
            return;
        }

        var WaveData = seaBehavior.waves.Select(w => new WaveShaderData(w));
        var waveAmps = WaveData.Select(w => w.amp).ToList();
        var waveWidths = WaveData.Select(w => w.width).ToList();
        var wavePosXs = WaveData.Select(w => w.position.x).ToList();
        var wavePosYs = WaveData.Select(w => w.position.y).ToList();
        var waveDecays = WaveData.Select(w => w.decay).ToList();

        mat.SetFloat("_NumWaves", WaveData.Count());
        mat.SetFloatArray("_WaveWidths", waveWidths);
        mat.SetFloatArray("_WaveAmps", waveAmps);
        mat.SetFloatArray("_WaveDecays", waveDecays);
        mat.SetFloatArray("_WaveXPositions", wavePosXs);
        mat.SetFloatArray("_WaveYPositions", wavePosYs);
    }
}
