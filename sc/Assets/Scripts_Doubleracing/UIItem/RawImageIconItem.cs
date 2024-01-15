using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageIconItem : MonoBehaviour
{

    private RawImage rawImage;
    private float UpdateValue = 1;
    // Start is called before the first frame update


    private void OnEnable()
    {
        rawImage = transform.GetComponent<RawImage>();
        UpdateValue = Random.Range(0.8f, 1.5f);

        int aaa = Random.Range(1, 16);
        rawImage.texture = Resources.Load("UIRes/Texture/Icon/" + aaa, typeof(Texture)) as Texture;
    }

    // Update is called once per frame
    void Update()
    {
        if (rawImage)
        {
            if (UpdateValue > 0)
            {
                UpdateValue -= Time.deltaTime;
                if (UpdateValue <= 0)
                {
                    UpdateValue = Random.Range(0.8f, 1.5f);
                    int aaa = Random.Range(1, 16);
                    rawImage.texture = Resources.Load("UIRes/Texture/Icon/" + aaa, typeof(Texture)) as Texture;
                }
            }
        }
    }
}
