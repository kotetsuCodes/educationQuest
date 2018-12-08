using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{

    public float ScrollSpeed;
    private Vector2 savedOffset;
    public Camera target;
    // public Camera targetCamera;
    Renderer rendererReference;
    float lastOffsetX = 0.0f;

    public bool FollowX;
    public bool FollowY;

    // Use this for initialization
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rendererReference = GetComponent<Renderer>();
        savedOffset = rendererReference.sharedMaterial.GetTextureOffset("_MainTex");
        // camera = target.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // lock position to camera

        if (FollowX && FollowY)
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
        else if (FollowX)
            transform.position = new Vector3(target.transform.position.x, transform.position.y, transform.position.z);
        else if (FollowY)
            transform.position = new Vector3(transform.position.x, target.transform.position.y, transform.position.z);

        var inputAxis = Input.GetAxis("Horizontal");

        // Debug.Log($"inputAxis: {inputAxis}");

        // character is moving right
        if (Vector3.Distance(target.velocity, Vector3.zero) >= 0.01f)
        {
            if (inputAxis > 0.0)
            {
                float x = Mathf.Repeat(lastOffsetX, 1);
                Vector2 offset = new Vector2(x, savedOffset.y);

                rendererReference.sharedMaterial.SetTextureOffset("_MainTex", offset);

                lastOffsetX += ScrollSpeed;
            }
            // character is moving left
            else if (inputAxis < 0.0)
            {
                float x = Mathf.Repeat(lastOffsetX, 1);
                Vector2 offset = new Vector2(x, savedOffset.y);

                rendererReference.sharedMaterial.SetTextureOffset("_MainTex", offset);

                lastOffsetX -= ScrollSpeed;
            }
        }

    }

    private void OnDisable()
    {
        rendererReference.sharedMaterial.SetTextureOffset("_MainTex", savedOffset);
    }
}
