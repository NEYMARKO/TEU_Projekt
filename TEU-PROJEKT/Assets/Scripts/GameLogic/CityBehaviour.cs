using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class CityBehaviour : MonoBehaviour
{
    [SerializeField] GameLoop gameLoop;

    Color defaultColor;
    [SerializeField] Color correctAnswerEndColor;
    [SerializeField] Color wrongAnswerEndColor;
    [SerializeField] float animationSpeed;
    [SerializeField] float colorAnimationSpeed;

    float hue;
    float saturation;
    float brightness;
    bool answeredCorrectly = false;
    float animationCompleted;
    float colorAnimationCompleted;

    #region CorrectAnswerHSVComponents
    float ca_start_h;
    float ca_end_h;
    float ca_start_s;
    float ca_end_s;
    float ca_start_v;
    float ca_end_v;
    #endregion

    #region WrongAnswerHSVComponents
    float wa_start_h;
    float wa_end_h;
    float wa_start_s;
    float wa_end_s;
    float wa_start_v;
    float wa_end_v;
    #endregion

    Color currentColor;
    Material material;
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
        material = gameObject.GetComponent<Renderer>().material;

        defaultColor = material.color;
        Color.RGBToHSV(defaultColor, out ca_start_h, out ca_start_s, out ca_start_v);
        Color.RGBToHSV(correctAnswerEndColor, out ca_end_h, out ca_end_s, out ca_end_v);
        Color.RGBToHSV(defaultColor, out wa_start_h, out wa_start_s, out wa_start_v);
        Color.RGBToHSV(wrongAnswerEndColor, out wa_end_h, out wa_end_s, out wa_end_v);
        colorAnimationCompleted = 0f;
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
            //Debug.Log("SELECTED " + gameObject.name);
            answeredCorrectly = gameLoop.CheckCityGuess(gameObject.name);
            shouldAnimate = true;
            isSelected = false;
            //material.color = correctAnswerEndColor;
        }

        if (shouldAnimate && colorAnimationCompleted < 1) AnimateColorChange();
    }

    private void AnimateColorChange()
    {
        Color.RGBToHSV(material.color, out hue, out saturation, out brightness);
        hue = answeredCorrectly ? Mathf.Lerp(ca_start_h, ca_end_h, colorAnimationCompleted) : Mathf.Lerp(wa_start_h, wa_end_h, colorAnimationCompleted);
        saturation = answeredCorrectly ? Mathf.Lerp(ca_start_s, ca_end_s, colorAnimationCompleted) : Mathf.Lerp(wa_start_s, wa_end_s, colorAnimationCompleted);
        brightness = answeredCorrectly ? Mathf.Lerp(ca_start_v, ca_end_v, colorAnimationCompleted) : Mathf.Lerp(wa_start_v, wa_end_v, colorAnimationCompleted);

        material.color = Color.HSVToRGB(hue, saturation, brightness);
        colorAnimationCompleted += colorAnimationSpeed * Time.deltaTime;
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

    private void ResetColor()
    {
        material.color = defaultColor;
        colorAnimationCompleted = 0f;
    }

    public void ResetCity()
    {
        ResetColor();
        shouldAnimate = false;
        isSelected = false;
        answeredCorrectly = false;
    }
}
