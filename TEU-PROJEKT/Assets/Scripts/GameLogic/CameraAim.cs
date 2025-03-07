using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Text;
using System;
using Mapbox.Directions;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using UnityEngine.EventSystems;

public class CameraAim : MonoBehaviour
{
    [Header("Player Controls")]
    [SerializeField] PlayerInputActions playerControls;
    private InputAction move;
    private InputAction fire;
    private InputAction toggleUI;

    [Header("Map settings")]
    [SerializeField] Transform map;
    [SerializeField] GameObject _realMap;
    private AbstractMap _abstractMap;
    [SerializeField] LayerMask layerMask;
    //[SerializeField] float mapDimensions = 15f;
    [SerializeField] float increment = 0.1f;
    float mapWidth;
    float mapHeight;

    [Header("RaySettings")]
    [SerializeField] float rayLength;
    [SerializeField] Color rayColor;

    public event EventHandler<string> OnCitySelected;

    Quaternion rotAroundZAxis = Quaternion.identity;
    Quaternion rotAroundYAxis = Quaternion.identity;
    
    RaycastHit hit;

    Vector3 rayOrigin;
    Vector3 rayDirection;
    Camera camera;

    Vector2 mousePositionScreen;
    Vector2 screenDimensions;

    GameObject hitGameObject;
    CityBehaviour activeCity, newCity;

    Vector3 widthRight;
    Vector3 widthLeft;
    Vector3 heightUp;
    Vector3 heightDown;
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
        rotAroundZAxis = Quaternion.Euler(Vector3.right * camera.fieldOfView / 2f);
        rotAroundYAxis = Quaternion.Euler(Vector3.up * camera.fieldOfView / 2f);
        RecalculateBoundingRays();
        _abstractMap = _realMap.GetComponent<AbstractMap>();
        rayOrigin = transform.position;
        rayDirection = transform.forward;
        screenDimensions = new Vector2(Screen.width, Screen.height);

        StartCoroutine("AlignCameraToMapDimensions");
    }

    // Update is called once per frame
    void Update()
    {
        if (_abstractMap == null)
        {
            _abstractMap = _realMap.GetComponent<AbstractMap>();
        }
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
        if (activeCity && fire.triggered) 
        {
            OnCitySelected?.Invoke(this, activeCity.gameObject.name);
        }
    }

    public IEnumerator AlignCameraToMapDimensions()
    {
        Transform tile = null;
        Vector3 centerPosition = Vector3.zero;
        while (centerPosition == Vector3.zero && tile == null)
        {
            if (_realMap.transform.childCount >= 9)
            {
                foreach(Transform child in _realMap.transform)
                {
                    if (child.gameObject.name != "TileProvider")
                    {
                        centerPosition += child.position;
                    }
                }
                tile = _realMap.transform.GetChild(1);
                centerPosition /= _realMap.transform.childCount;
            }
            else Debug.Log("CENTER TILE STIL NOT AVAILABLE");
            yield return null;
        }
        


        Vector2d mapWidthHeight = tile.gameObject.GetComponent<MeshRenderer>().bounds.size.ToVector2d();
        mapWidth = (float)mapWidthHeight.x * 3;
        
        mapHeight = (float)mapWidthHeight.y * 3;

        transform.position = centerPosition + Vector3.up * transform.position.y;
        
        RecalculateBoundingRays();
        //direction: 0 -> left_right, 1 -> up_down
        RepositionCamera(widthLeft, widthRight, 0);
        //RepositionCamera(heightDown, heightUp, 1);
    }

    private void RecalculateBoundingRays()
    {
        widthRight = rotAroundZAxis * rotAroundYAxis * camera.transform.forward;
        widthLeft = Quaternion.Euler(Vector3.up * 180f) * widthRight;

        heightUp = Quaternion.Euler(Vector3.up * 90f) * widthRight;
        heightDown = Quaternion.Euler(Vector3.up * 270f) * widthRight;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hit.point, 0.1f);

        if (camera)
        {
            Vector3 intersectionWidthRight = PlaneRayIntersection(widthRight);
            Vector3 intersectionWidthLeft = PlaneRayIntersection(widthLeft);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(intersectionWidthRight, 0.25f);
            Gizmos.DrawSphere(intersectionWidthLeft, 0.25f);
        }  
    }

    private void RepositionCamera(Vector3 rayDirection1, Vector3 rayDirection2, short direction)
    {
        Vector3 intersect1 = PlaneRayIntersection(rayDirection1);
        Vector3 intersect2 = PlaneRayIntersection(rayDirection2);

        float shrinkFactor = 1;
        //if camera width is larger than map width at the beginning, camera needs to shrink
        if (Vector3.Distance(intersect1, intersect2) > mapWidth) shrinkFactor = -1;

        //if camera bounds are wider than map width, multiply with -1 to shrink it => also multiply condition to reverse inequation meaning (a > b) => (-a > -b)
        //this way there is no need for writing if statements
        float cameraMovedDistance = 0f;
        while(Vector3.Distance(intersect1, intersect2) * shrinkFactor < mapWidth * shrinkFactor)
        {
            camera.farClipPlane += increment * shrinkFactor;

            cameraMovedDistance += increment * shrinkFactor;
            intersect1 = PlaneRayIntersection(rayDirection1);
            intersect2 = PlaneRayIntersection(rayDirection2);

        }

        Debug.Log($"Camera moved distance: {cameraMovedDistance}");
        camera.transform.position = camera.transform.position + Vector3.up * cameraMovedDistance;
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
