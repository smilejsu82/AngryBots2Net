//This script detects mouse clicks on a plane using Plane.Raycast.
//In this example, the plane is set to the Camera's x and y position, but you can set the z position so the plane is in front of your Camera.
//The normal of the plane is set to facing forward so it is facing the Camera, but you can change this to suit your own needs.

//In your GameObject's Inspector, set your clickable distance and attach a cube GameObject in the appropriate fields

using UnityEngine;

public class PlaneRayExample : MonoBehaviour
{
    //Attach a cube GameObject in the Inspector before entering Play Mode
    public GameObject m_Cube;

    //This is the distance the clickable plane is from the camera. Set it in the Inspector before running.
    public float m_DistanceZ;

    Plane m_Plane;
    Vector3 m_DistanceFromCamera;

    void Start()
    {
        //This is how far away from the Camera the plane is placed
        m_DistanceFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - m_DistanceZ);

        Debug.LogFormat("m_DistanceFromCamera: {0}", m_DistanceFromCamera);
        
        //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector.
        //This is the plane that you can click so make sure it is reachable.
        m_Plane = new Plane(Vector3.forward, m_DistanceFromCamera);
    }

    void Update()
    {
        //Detect when there is a mouse click
        if (Input.GetMouseButton(0))
        {
            //Create a ray from the Mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Initialise the enter variable
            float enter = 0.0f;
            
            Debug.DrawRay(ray.origin, ray.direction * 11, Color.red, 10f);

            if (m_Plane.Raycast(ray, out enter))
            {
                //Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);

                Debug.LogFormat("hitPoint : {0}", hitPoint);

                //Move your cube GameObject to the point where you clicked
                m_Cube.transform.position = hitPoint;
            }
        }
    }
}