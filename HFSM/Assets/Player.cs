using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public TMPro.TMP_Text infoText;
    public Image hpImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetProgress(float value)
    {
        hpImage.fillAmount = value;
        hpImage.color = Color.Lerp(Color.red, Color.green, value);
    }

    public void SetInfo(string value,float progress)
    {
        infoText.text = value;
        SetProgress(progress);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
