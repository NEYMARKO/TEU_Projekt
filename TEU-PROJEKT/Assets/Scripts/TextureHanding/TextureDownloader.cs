using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class TextureDownloader : MonoBehaviour
{
    public RawImage rawImage;
    public GameObject map;
    private string imgURL = "https://www.shutterstock.com/shutterstock/photos/2311697527/display_1500/stock-photo-saga-prefecture-of-japan-low-resolution-satellite-map-2311697527.jpg";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DownloadImage");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator DownloadImage()
    {
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(imgURL))
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(req);
                rawImage.texture = texture;
                Renderer renderer = map.GetComponent<Renderer>();
                renderer.material.mainTexture = texture;
            }
            else
            {
                Debug.Log("PROBLEMS WITH DOWNLOADING");
            }
        }
    }
}
