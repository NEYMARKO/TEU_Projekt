using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Text;
using System;
using Mapbox.Directions;

public class CameraAim : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] PlayerInputActions playerControls;
    private InputAction move;
    private InputAction fire;
    private InputAction toggleUI;
    [SerializeField] Transform map;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float rayLength;
    [SerializeField] Color rayColor;

    [SerializeField] float mapDimensions = 15f;
    [SerializeField] float increment = 0.1f;
    RaycastHit hit;

    Vector3 rayOrigin;
    Vector3 rayDirection;
    Camera camera;

    Vector2 mousePositionScreen;
    Vector2 screenDimensions;

    GameObject hitGameObject;
    CityBehaviour activeCity, newCity;

    struct Plane
    {
        public Vector3 normal;
        public float D;
        public Plane(Vector3 normal, float D)
        {
            this.normal = normal;
            this.D = D;
        }
    }
    private void Awake()
    {
        playerControls = new PlayerInputActions();
        camera = GetComponent<Camera>();
        camera.transform.position = new Vector3(-0.0900000036f, 9.28683662f, 0.239999995f);
        camera.transform.rotation = new Quaternion(0.707106829f, 0f, 0f, 0.707106829f);
    }
    void Start()
    {
        rayOrigin = transform.position;
        rayDirection = transform.forward;
        screenDimensions = new Vector2(Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRayOrigin();
        ShootRay(rayOrigin, rayLength, layerMask);
        OnCityHover();
        OnCitySelect();

    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        fire = playerControls.Player.Fire;
        fire.Enable();

        toggleUI = playerControls.UI.Toggle;
        toggleUI.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
        toggleUI.Disable();
    }

    Vector2 NormalizeMouseScreenPosition()
    {
        return move.ReadValue<Vector2>() / screenDimensions;
    }
    Vector3 ConvertScreenToWorldCoordinates(Vector2 mouseScreenCoordinates)
    {
        Vector3 worldPoint = camera.ViewportToWorldPoint(new Vector3(mouseScreenCoordinates.x, mouseScreenCoordinates.y, 0.99f * Vector3.Distance(camera.transform.position, map.transform.position)));
        return worldPoint;
    }
    
    private void UpdateRayOrigin()
    {
        mousePositionScreen = NormalizeMouseScreenPosition();
        rayOrigin = ConvertScreenToWorldCoordinates(mousePositionScreen) + Vector3.up * rayLength / 2;
    }
    Vector3 ShootRay(Vector3 rayOrigin, float rayLength, LayerMask layerMask)
    {
        Physics.Raycast(rayOrigin, camera.transform.forward, out hit, rayLength, layerMask);
        return hit.point;
    }

    void OnCityHover()
    {
        hitGameObject = hit.collider ? hit.collider.gameObject : null;
        newCity = hitGameObject ? hitGameObject.GetComponent<CityBehaviour>() : null;

        if (newCity == null && activeCity != null)
        {
            activeCity.ToggleOutline();
            activeCity = null;
            return;
        }
        else if (activeCity == newCity) return;        
        
        if (activeCity != null) activeCity.ToggleOutline();
        newCity.ToggleOutline();
        activeCity = newCity;
    }

    void OnCitySelect()
    {
        if (activeCity && fire.triggered) activeCity.SetSelected(true);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, 0.1f);

        if (camera)
        {
            Quaternion rotation1 = Quaternion.Euler(Vector3.right * camera.fieldOfView / 2f);
            Quaternion rotation2 = Quaternion.Euler(Vector3.up * camera.fieldOfView / 2f);
            Vector3 cameraBoundRay =  rotation1 * rotation2 * camera.transform.forward;
            Gizmos.DrawLine(camera.transform.position, camera.transform.position + cameraBoundRay * rayLength * 5);
            Vector3 cameraBoundRay2 = Quaternion.Euler(-Vector3.right * camera.fieldOfView / 2f) * Quaternion.Euler(Vector3.up * camera.fieldOfView / 2f) * camera.transform.forward;
            Gizmos.DrawLine(camera.transform.position, camera.transform.position + cameraBoundRay2 * rayLength * 5);

            Vector3 intersection1 = PlaneRayIntersection(cameraBoundRay);
            Vector3 intersection2 = PlaneRayIntersection(cameraBoundRay2);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(intersection1, 0.25f);
            Gizmos.DrawSphere(intersection2, 0.25f);
            //Debug.Log("Distance in gizmos: " + Vector3.Distance(intersection1, intersection2));
            RepositionCamera(cameraBoundRay, cameraBoundRay2);
        }  
    }

    private void RepositionCamera(Vector3 rayDirection1, Vector3 rayDirection2)
    {
        Vector3 intersect1 = PlaneRayIntersection(rayDirection1);
        Vector3 intersect2 = PlaneRayIntersection(rayDirection2);

        while(Vector3.Distance(intersect1, intersect2) < mapDimensions)
        {
            camera.transform.position = camera.transform.position + Vector3.up * increment;
            camera.farClipPlane += increment;

            intersect1 = PlaneRayIntersection(rayDirection1);
            intersect2 = PlaneRayIntersection(rayDirection2);

        }
    }
    private Vector3 PlaneRayIntersection(Vector3 rayDirection)
    {
        Vector3 intersection = rayDirection;
        Vector3 pointOnPlane = camera.transform.position + camera.transform.forward * camera.farClipPlane;
        Plane plane = new Plane(-camera.transform.forward, 0f);
        plane.D = -plane.normal.x * pointOnPlane.x - plane.normal.y * pointOnPlane.y - plane.normal.z * pointOnPlane.z;

        float t = -(Vector3.Dot(plane.normal, camera.transform.position) + plane.D) / (Vector3.Dot(plane.normal, rayDirection));

        return camera.transform.position + rayDirection * t;
    }
}
