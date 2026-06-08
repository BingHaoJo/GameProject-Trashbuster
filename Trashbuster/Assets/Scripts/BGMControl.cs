using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class BGMControl : MonoBehaviour
{
    private Image volumeIcon;
    [SerializeField] private Sprite volumeOnSprite;
    [SerializeField] private Sprite volumeOffSprite;
    private InputAction muteAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        muteAction = InputSystem.actions.FindAction("Mute");
        volumeIcon = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        muteAction.started += context =>
        {
            if (context.interaction is TapInteraction)
            {
                GlobalVar.bgmMuted = !GlobalVar.bgmMuted;
            }
        };
        
        volumeIcon.sprite = GlobalVar.bgmMuted ? volumeOffSprite : volumeOnSprite;
    }
}
