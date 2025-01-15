using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
public class TextureDownloader : MonoBehaviour
{
    public DBManager dbManager;
    public RawImage rawImage;
    public GameObject map;
    public ChangeRegionDropdown pauseRegionDropdown;
    public ChangeRegionDropdown endGameRegionDropdown;
    private List<string> regions;
    private string textureSavePath = Application.dataPath + "/SavedTextures";
    private string imgURL = "";
    private string croatiaTexture = "https://i.etsystatic.com/6545793/r/il/b035c6/6394639346/il_fullxfull.6394639346_3qnp.jpg";
    private string franceTexture = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/7d/France_OlympusMons_Size.svg/1024px-France_OlympusMons_Size.svg.png";
    private string spainTexture = "https://media.istockphoto.com/id/834400736/photo/spain-country-3d-render-topographic-map-neutral.jpg?s=1024x1024&w=is&k=20&c=IN6qs7-7tn-s-lWtcKSjyqVgBFmjkQACqw8B4AI73LA=";
    private string japanTexture = "https://pathikworld.com/wp-content/uploads/japan-map-1024x1024.png";
    private string hungaryTexture = "https://media.istockphoto.com/id/873810576/photo/hungary-country-3d-render-topographic-map-border.jpg?s=1024x1024&w=is&k=20&c=OMLf5SNtNTUEp_JAWV_N1L3qJvjo-f15iuZNddtz5MA=";
    void Start()
    {
        //Debug.Log("DATA PATH: " + textureSavePath);
        HandleRegionsLoaded(dbManager, (dbManager.GetRegions() != null));
        FetchTextureForCurrentRegion(dbManager, dbManager.GetActiveRegionID());
        //StartCoroutine("DownloadImage");
    }

    private void OnEnable()
    {
        dbManager.OnRegionsLoaded += HandleRegionsLoaded;
        pauseRegionDropdown.OnRegionChange += FetchTextureForCurrentRegion;
        endGameRegionDropdown.OnRegionChange += FetchTextureForCurrentRegion;
    }

    private void OnDisable()
    {
        dbManager.OnRegionsLoaded -= HandleRegionsLoaded;
        pauseRegionDropdown.OnRegionChange -= FetchTextureForCurrentRegion;
        endGameRegionDropdown.OnRegionChange -= FetchTextureForCurrentRegion;
    }

    private void HandleRegionsLoaded(object sender, bool loaded)
    {
        if (loaded) regions = dbManager.GetRegions();
    }

    private void FetchTextureForCurrentRegion(object sender, int regionID)
    {
        string regionName = "";
        if (sender == null)
        {
            regionName = regions[dbManager.GetActiveRegionID()].ToUpper();
        }
        else
        {
            regionName = regions[regionID].ToUpper();
        }

        //Debug.Log("REGION NAME: " + regionName);
        string texturePath = $"{textureSavePath}/{regionName}.png";
        if (!File.Exists(texturePath))
        {
            //Debug.Log("Texture doesn't exist in folder");
            imgURL = GetUrlForCountry(regionName);
            StartCoroutine("DownloadAndSaveTextureToFolder");
        }
        else
        {
            //Debug.Log("TEXTURE ALREADY EXIST ABOUT TO LOAD IT");
            ApplyTextureToMap(regionName);
        }
    }

    private string GetUrlForCountry(string regionName)
    {
        string url = "";
        switch(regionName)
        {
            case "CROATIA":
                url = croatiaTexture;
                break;
            case "FRANCE":
                url = franceTexture;
                break;
            case "JAPAN":
                url = japanTexture;
                break;
            case "SPAIN":
                url = spainTexture;
                break;
            case "HUNGARY":
                url = hungaryTexture;
                break;
            default:
                break;
        }

        return url;
    }
    private void ApplyTextureToMap(string regionName)
    {
        byte[] fileData = File.ReadAllBytes($"{textureSavePath}/{regionName}.png");
        Texture2D loadedTexture = new Texture2D(1024, 1024);
        loadedTexture.LoadImage(fileData);

        map.GetComponent<Renderer>().material.mainTexture = loadedTexture;
    }

    private IEnumerator DownloadAndSaveTextureToFolder()
    {
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(imgURL))
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                //rawImage.GetComponent<Renderer>().material.renderQueue = 2000;
                Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(req);
                //texture.filterMode = FilterMode.Trilinear; 
                //rawImage.texture = texture;
                byte[] textureBytes = downloadedTexture.EncodeToPNG();
                File.WriteAllBytes($"{textureSavePath}/{regions[dbManager.GetActiveRegionID()].ToUpper()}.png", textureBytes);
                //rawImage.texture = downloadedTexture;
                //Debug.Log($"DOWNLOADED TEXTURE AT: {textureSavePath}/{regions[dbManager.GetActiveRegionID()].ToUpper()}.png");
                map.GetComponent<Renderer>().material.mainTexture = downloadedTexture;
            }
            else
            {
                Debug.Log("Failed to load texture");
            }
        }
    }
}
