using System;
using UnityEngine;

public class MngrGame : MonoBehaviour
{

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        UIMonoBehaviour.UIShow<UIMain>();

        // Test Net

        gameObject.AddComponent<Test>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
