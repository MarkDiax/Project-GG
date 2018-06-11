using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerEquipmentUI : UIObject
{
    [SerializeField] ImageSwapper _bowIconParent, _swordIconParent;

    GameObject _activeIconSet;
    bool _hasBowEquipped, _hasSwordEquipped;

    private void Start() {
        EventManager.PlayerEvent.OnEquipBow.AddListener((Equipped) => { SwitchImages(_bowIconParent, Equipped); });
        EventManager.PlayerEvent.OnEquipSword.AddListener((Equipped) => { SwitchImages(_swordIconParent, Equipped); });
    }

    void SwitchImages(ImageSwapper IconParent, bool Equipped) {
        if (_activeIconSet != IconParent.gameObject) {
            if (_activeIconSet != null)
                _activeIconSet.SetActive(false);

            _activeIconSet = IconParent.gameObject;
            _activeIconSet.SetActive(true);
        }

        if (Equipped)
            IconParent.ResetImages();
        else
            IconParent.SwapImages();
    }
}