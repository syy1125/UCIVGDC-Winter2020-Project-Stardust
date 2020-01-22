using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 7f;
    public float panBorderThickness = 10f;
    public float scrollSpeed = 200f;

    public float minX = -5000;
    public float maxX = 5000;

    public float minY = 500;
    public float maxY = 6000;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        if(Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime * pos.y / 100;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime * pos.y / 100;
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime * pos.y / 100;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime * pos.y / 100;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        scroll = Mathf.Clamp(scroll, -2, 2);
        pos.y -= scroll * scrollSpeed * Time.deltaTime * 100f;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY + VariableMinScroll(transform.position), maxY);
        pos.z = Mathf.Clamp(pos.z, minX - GetZOffset(transform.position), maxX - GetZOffset(transform.position));


        transform.position = pos;
    }

    //dynamically changes the scroll limit of Z depending on how zoomed in the camera is
    public float GetZOffset(Vector3 pos)
    {
        //convert to radians
        float cameraAngle = 30 * Mathf.Deg2Rad;
        float tan = Mathf.Tan(cameraAngle);
        float zOffset = tan * pos.y;
        return zOffset;
    }


    public float VariableMinScroll(Vector3 pos)
    {
        float distance = Mathf.Max(Mathf.Abs(pos.x), Mathf.Abs(pos.z + GetZOffset(pos)));
        float minScroll = distance * 2500 / 5000;
        return minScroll;
    }
}
