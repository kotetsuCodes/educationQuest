using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour
{

    public float ScrollSpeed;
    private Vector2 savedOffset;
    Camera mainCamera;
    Renderer rendererReference;

    // Use this for initialization
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rendererReference = GetComponent<Renderer>();
        savedOffset = rendererReference.sharedMaterial.GetTextureOffset("_MainTex");
    }

    // Update is called once per frame
    void Update()
    {
        // lock position to camera
        transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, transform.position.z);


        float x = Mathf.Repeat(Time.time * ScrollSpeed, 1);
        Vector2 offset = new Vector2(x, savedOffset.y);

        rendererReference.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }

    private void OnDisable()
    {
        rendererReference.sharedMaterial.SetTextureOffset("_MainTex", savedOffset);
    }
}
