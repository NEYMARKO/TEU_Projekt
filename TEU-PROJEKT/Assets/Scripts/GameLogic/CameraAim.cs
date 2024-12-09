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

    CityBehaviour activeCity;
    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }
    void Start()
    {
        rayOrigin = transform.position;
        rayDirection = transform.forward;
        camera = GetComponent<Camera>();
        screenDimensions = new Vector2(Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        mousePositionScreen = NormalizeMouseScreenPosition();
        rayOrigin = ConvertScreenToWorldCoordinates(mousePositionScreen) + Vector3.up * rayLength / 2;
        ShootRay(rayOrigin, rayLength, layerMask);

        OnCityHover();
        //Debug.Log("MOUSE POS: " + mousePositionScreen);
        //Debug.Log("WORLD POS: " + rayOrigin);
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
    
    Vector3 ShootRay(Vector3 rayOrigin, float rayLength, LayerMask layerMask)
    {
        Physics.Raycast(rayOrigin, camera.transform.forward, out hit, rayLength, layerMask);
        return hit.point;
    }

    void OnCityHover()
    {
        if (activeCity == hit.collider.gameObject.GetComponent<CityBehaviour>()) return;
        activeCity = hit.collider.gameObject.GetComponent<CityBehaviour>();
        if (activeCity != null) activeCity.SetSHouldAnimate(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(rayOrigin, rayOrigin + rayDirection * rayLength);
        Gizmos.DrawSphere(hit.point, 0.1f);
    }
}
