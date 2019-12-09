using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeaShader : MonoBehaviour
{
    public SeaBehavior seaBehavior;
    public Material mat;
    public Renderer renderer;
    public string sortingLayerName;
    public int sortingOrder;

    void Awake()
    {
        //
        mat.SetFloat("_seaHeight", seaBehavior.transform.position.y);
        // to draw at first frame:
        mat.SetFloat("_quadSizeX", gameObject.transform.localScale.x / 2.0f);
        mat.SetFloat("_quadSizeY", gameObject.transform.localScale.y / 2.0f);
        //
        mat.SetFloatArray("_WaveAmps", new float[25]);
        mat.SetFloatArray("_WaveXPositions", new float[25]);
        mat.SetFloatArray("_WaveYPositions", new float[25]);
        mat.SetFloatArray("_WaveWidths", new float[25]);
        mat.SetFloatArray("_WaveDecays", new float[25]);
    }

    private void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
        renderer.sortingLayerName = "BlahBlah";
        renderer.sortingOrder = 10;
    }

    void Update()
    {
        if (seaBehavior.waves.Count == 0) 
        { 
            return;
        }
        var waveAmps = new float[seaBehavior.waves.Count];
        var waveSigns = new float[seaBehavior.waves.Count];
        var waveWidths = new float[seaBehavior.waves.Count];
        var waveDecays = new float[seaBehavior.waves.Count];
        var wavePosXs = new float[seaBehavior.waves.Count];
        var wavePosYs = new float[seaBehavior.waves.Count];
        // This is probably worth doing (the other way we iterate the waves n+1 times where n is the number shader attributes we end up with
        for (int i = 0; i < seaBehavior.waves.Count; ++i)
        {
            Wave wave = seaBehavior.waves[i];
            waveAmps[i] = wave.amplitude;
            waveSigns[i] = wave.sign;
            waveWidths[i] = seaBehavior.WaveWidth; // one less indirection(this probably won't stay like this so w/e)
            waveDecays[i] = wave.decay;
            var waveWorldPosition = seaBehavior.transform.TransformPoint(wave.position);
            wavePosXs[i] = waveWorldPosition.x;
            wavePosYs[i] = waveWorldPosition.y;

        }

        //
        mat.SetFloat("_quadPosY", gameObject.transform.position.y); // to change origin for correction function
        mat.SetFloat("_quadPosX", gameObject.transform.position.x); // to change origin for correction function
        mat.SetFloat("_quadSizeX", gameObject.transform.localScale.x / 2.0f); // to transform to NDC space
        mat.SetFloat("_quadSizeY", gameObject.transform.localScale.y / 2.0f);
        //
        mat.SetFloat("_NumWaves", seaBehavior.waves.Count);
        mat.SetFloatArray("_WaveWidths", waveWidths);
        mat.SetFloatArray("_WaveAmps", waveAmps);
        mat.SetFloatArray("_WaveSigns", waveSigns);
        mat.SetFloatArray("_WaveDecays", waveDecays);
        mat.SetFloatArray("_WaveXPositions", wavePosXs);
        mat.SetFloatArray("_WaveYPositions", wavePosYs);

        // debug stuff
        //var waveAmpsAndWaveWidths = waveAmps.Zip(waveWidths, (first, second) => first + ":" + second);
        //var waveAmpsAndWaveWidthsAndXPosition = waveAmpsAndWaveWidths.Zip(wavePosXs, (first, second) => first + ":" + second);
        //foreach (var item in waveAmpsAndWaveWidthsAndXPosition)
        //{
        //    Debug.Log(item);
        //}

        //Debug.Log("x:  " + gameObject.transform.localScale.x / 2.0f);
        //Debug.Log("y:  " + gameObject.transform.localScale.y / 2.0f);
    }
}
