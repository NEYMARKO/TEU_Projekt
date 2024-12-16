using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Text;

public class CameraAim : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] PlayerInputActions playerControls;
    private InputAction move;
    private InputAction fire;
    [SerializeField] Transform map;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float rayLength;
    [SerializeField] Color rayColor;

    RaycastHit hit;

    Vector3 rayOrigin;
    Vector3 rayDirection;
    Camera camera;

    Vector2 mousePositionScreen;
    Vector2 screenDimensions;

    GameObject hitGameObject;
    CityBehaviour activeCity, newCity;
    private void Awake()
    {
        playerControls = new PlayerInputActions();
        camera = GetComponent<Camera>();
        camera.transform.position = new Vector3(-0.0900000036f, 9.28683662f, 0.239999995f);
        camera.transform.rotation = new Quaternion(0.707106829f, 4.4555054e-05f, -4.45553851e-05f, 0.707106829f);
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
    }

    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
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
    }
}
