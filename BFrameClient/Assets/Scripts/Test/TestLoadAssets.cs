using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoadAssets : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string pathAB = Application.streamingAssetsPath + "/Assets/Res/lua";
        AssetBundle asset = AssetBundle.LoadFromFile(pathAB);

        //Instantiate(asset.LoadAsset<GameObject>("CanvasA"));
        //Instantiate(asset.LoadAsset<GameObject>("CanvasB"));
        Debug.Log(asset.LoadAsset<TextAsset>("Main").text);
        Debug.Log(asset.LoadAsset<TextAsset>("UIBase").text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
