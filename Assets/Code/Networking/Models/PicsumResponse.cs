using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PicsumResponse
{
    public string id;
    public string author;
    public int width;
    public int height;
    public string url;
    public string download_url;

    // Helper property to convert id to long if needed
    public long Id => long.Parse(id);
    
    // Helper property for DownloadUrl as Uri
    public Uri DownloadUrl => new Uri(download_url);
}

[Serializable]
public class PicsumResponseList
{
    public PicsumResponse[] items;
}
