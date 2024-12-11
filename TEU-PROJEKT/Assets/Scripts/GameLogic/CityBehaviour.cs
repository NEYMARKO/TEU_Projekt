using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class CityBehaviour : MonoBehaviour
{

    [SerializeField] float animationSpeed;
    float animationCompleted;
    Vector3 oldPosition;
    Quaternion oldRotation;

    bool shouldAnimate = false;
    bool isAnimating = false;
    bool isSelected = false;

    Outline outlineScript;
    // Start is called before the first frame update
    void Start()
    {
        oldPosition = transform.position;
        oldRotation = transform.rotation;
        outlineScript = GetComponent<Outline>();
        outlineScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (shouldAnimate && !isAnimating)
        //{
        //    animationCompleted = 0f;
        //    shouldAnimate = false;
        //    isAnimating = true;
        //}
        //if (animationCompleted < 1f)
        //{
        //    if (isAnimating)
        //    {
        //        AnimatePosition();
        //        AnimateRotation();
        //        animationCompleted += animationSpeed * Time.deltaTime;
        //    }
        //}
        //else
        //{
        //    transform.position = oldPosition;
        //    transform.rotation = oldRotation;
        //    isAnimating = false;
        //}

        if (isSelected)
        {
            Debug.Log("SELECTED " + gameObject.name);
            isSelected = false;
        }
    }

    void AnimatePosition()
    {
        Vector3 newPosition = oldPosition + Vector3.up * Mathf.Sin(animationCompleted * Mathf.PI);
        transform.position = Vector3.Lerp(oldPosition, newPosition, animationCompleted);
    }

    void AnimateRotation()
    {
        Quaternion newRotation = oldRotation * Quaternion.Euler(0f, 720f * animationCompleted, 0f); ;
        transform.rotation = Quaternion.Slerp(oldRotation, newRotation, animationCompleted);
    }

    public void ToggleOutline()
    {
        outlineScript.enabled = !outlineScript.enabled;
    }    
    public void SetShouldAnimate(bool animate)
    {
        shouldAnimate = animate;
    }
    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }
}
