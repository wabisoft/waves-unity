using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sortingOrderTest : MonoBehaviour
{
    public Renderer renderer;
    public string sortingLayerName;
    public int sortingOrder;

    void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
        renderer.sortingLayerName = "BlahBlah";
        renderer.sortingOrder = 8;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
