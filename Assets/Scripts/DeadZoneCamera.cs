using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZoneCamera : MonoBehaviour {

    public GameObject target;
    public float MaxCameraSpeed = 0.8f;
    private Camera _camera;

    public GameObject UpperBound;
    public GameObject LowerBound;
    public GameObject LeftBound;
    public GameObject RightBound;

    private bool cameraIsMoving = false;

    private float deadZoneMaxX = 4;
    private float deadZoneMinX = -4;
    private float deadZoneMaxY = 4;
    private float deadZoneMinY = -4;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // draw debug lines
        //Debug.DrawLine(
        //    new Vector2(_camera.transform.position.x + deadZoneMaxX, _camera.transform.position.y + deadZoneMaxY),
        //    new Vector2(_camera.transform.position.x + deadZoneMaxX, _camera.transform.position.y + deadZoneMinY),
        //    Color.green
        //);

        //Debug.DrawLine(
        //    new Vector2(_camera.transform.position.x + deadZoneMinX, _camera.transform.position.y + deadZoneMinY),
        //    new Vector2(_camera.transform.position.x + deadZoneMinX, _camera.transform.position.y + deadZoneMaxY),
        //    Color.blue
        //);

        //Debug.DrawLine(
        //    new Vector2(_camera.transform.position.x + deadZoneMinX, _camera.transform.position.y + deadZoneMaxY),
        //    new Vector2(_camera.transform.position.x + deadZoneMaxX, _camera.transform.position.y + deadZoneMaxY),
        //    Color.red
        //);

        //Debug.DrawLine(
        //    new Vector2(_camera.transform.position.x + deadZoneMinX, _camera.transform.position.y + deadZoneMinY),
        //    new Vector2(_camera.transform.position.x + deadZoneMaxX, _camera.transform.position.y + deadZoneMinY),
        //    Color.yellow
        //);

        var targetPositionX = target.transform.position.x;
        var targetPositionY = target.transform.position.y;

        // var newCameraPosition = Vector2.Lerp(_camera.transform.position, target.transform.position, Time.deltaTime / MaxCameraSpeed);
        var newCameraPosition = target.transform.position;

        var v3WorldPosUL = _camera.ViewportToWorldPoint(new Vector3(0, 1, _camera.transform.position.z));
        var v3WorldPosLL = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.transform.position.z));

        var v3WorldPosUR = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.transform.position.z));
        var v3WorldPosLR = _camera.ViewportToWorldPoint(new Vector3(1, 0, _camera.transform.position.z));

        //Helpers.DebugValue("_camera.transform.position.x", _camera.transform.position.x);
        //Helpers.DebugValue("v3WorldPosUL.x", v3WorldPosUL.x);

        // calculate distance between camera and viewport
        var lDist = Vector2.Distance(new Vector2(_camera.transform.position.x, 0), new Vector2(v3WorldPosUL.x, 0));
        var newLDist = newCameraPosition.x - lDist - 1.0f;

        Debug.DrawLine(new Vector3(_camera.transform.position.x, _camera.transform.position.y + 2, _camera.transform.position.z), new Vector3(_camera.transform.position.x - lDist, _camera.transform.position.y + 2, _camera.transform.position.z), Color.green);
        Debug.DrawLine(new Vector3(newCameraPosition.x, newCameraPosition.y + 1, _camera.transform.position.z), new Vector3(newLDist, newCameraPosition.y + 1, _camera.transform.position.z), Color.cyan);

        var rDist = Mathf.Abs(_camera.transform.position.x - v3WorldPosUR.x);
        var newRDist = newCameraPosition.x + rDist;


        // DOWN DISTANCE CALCULATIONS

        var dDist = Vector2.Distance(new Vector2(0, _camera.transform.position.y), new Vector2(0, v3WorldPosLL.y));
        var newDDist = newCameraPosition.y - dDist - 1.0f;

        Debug.DrawLine(new Vector3(newCameraPosition.x + 1, newCameraPosition.y, _camera.transform.position.z), new Vector3(newCameraPosition.x + 1, newDDist, _camera.transform.position.z), Color.red);

        bool withinLeftBound = newLDist > LeftBound.transform.position.x;
        bool withinLowerBound = newDDist > LowerBound.transform.position.y;

        bool withinRightBound = newRDist < RightBound.transform.position.x;
        bool withinUpperBound = v3WorldPosUL.y < UpperBound.transform.position.y;

        //Helpers.DebugValue(nameof(withinLeftBound), withinLeftBound);
        //Helpers.DebugValue(nameof(withinRightBound), withinRightBound);
        //Helpers.DebugValue(nameof(withinUpperBound), withinUpperBound);

        if (withinLeftBound && withinRightBound)
        {
            _camera.transform.position = new Vector3(newCameraPosition.x, _camera.transform.position.y, _camera.transform.position.z);
        }

        if (withinUpperBound && withinLowerBound)
        {
            _camera.transform.position = new Vector3(_camera.transform.position.x, newCameraPosition.y, _camera.transform.position.z);
        }
	}
}
