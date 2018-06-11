using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageSwapper : UIObject
{
    [SerializeField] bool _awakeAtSecondImage;
    [SerializeField] Image _image1, _image2;


    public void SwapImages() {
        _image1.gameObject.SetActive(!_image1.gameObject.activeInHierarchy);
        _image2.gameObject.SetActive(!_image2.gameObject.activeInHierarchy);
    }

    public void ResetImages() {
        _image1.gameObject.SetActive(true);
        _image2.gameObject.SetActive(false);
    }
}
