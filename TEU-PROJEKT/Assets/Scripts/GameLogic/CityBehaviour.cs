using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBehaviour : MonoBehaviour
{

    [SerializeField] float animationSpeed;
    float animationCompleted;
    Vector3 oldPosition;
    Quaternion oldRotation;

    bool shouldAnimate = false;
    bool isAnimating = false;
    // Start is called before the first frame update
    void Start()
    {
        oldPosition = transform.position;
        oldRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldAnimate && !isAnimating)
        {
            animationCompleted = 0f;
            shouldAnimate = false;
            isAnimating = true;
        }
        if (animationCompleted < 1f)
        {
            if (isAnimating)
            {
                AnimatePosition();
                AnimateRotation();
                animationCompleted += animationSpeed * Time.deltaTime;
            }
        }
        else
        {
            transform.position = oldPosition;
            transform.rotation = oldRotation;
            isAnimating = false;
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
    public void SetSHouldAnimate(bool animate)
    {
        shouldAnimate = animate;
    }
}
