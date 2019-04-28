using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class UIImage : UIObject
{
	Image _image;

	public void SetAlpha(float Value) {
		Color c = Image.color;
		c.a = Value;
		Image.color = c;
	}

	protected override void OnGameReload() {
	}

	public Color GetColor {
		get { return Image.color; }
	}

	public Image Image {
		get {
			if (_image == null)
				_image = GetComponent<Image>();
			return _image;
		}
		set {
			_image = value;
		}
	}
}
