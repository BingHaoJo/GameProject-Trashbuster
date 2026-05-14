using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VacuumGunController : MonoBehaviour
{
    #region ForceValue
    private float vacuumForce = 7f;
    private float pushForce = 13f;
    private float shootForce = 2f;
    #endregion
    
    #region Audio stuff
    [Header("Audio")]
    // Audio that works with the audio manager
    [SerializeField] private AudioClip suckSound;
    [SerializeField] private AudioClip collectSound;
    // Audio that don't work with the audio manager because its used in Update/FixedUpdate
    [SerializeField] private AudioSource shootAudio;
    [SerializeField] private AudioSource pushAudio;
    #endregion

    #region Gun stuff
    [Header("Gun")]
    [SerializeField] private VacuumBarrel vacuumBarrel;
    [SerializeField] private GameObject Slot1;
    [SerializeField] private GameObject Slot2;
    [SerializeField] private GameObject Slot3;
    [SerializeField] private GameObject Slot4;
    [SerializeField] public GameObject AirPushBar;
    private Queue<TrashBase> trashSlot1 = new Queue<TrashBase>();
    private Queue<TrashBase> trashSlot2 = new Queue<TrashBase>();
    private Queue<TrashBase> trashSlot3 = new Queue<TrashBase>();
    private Queue<TrashBase> trashSlot4 = new Queue<TrashBase>();
    private List<Queue<TrashBase>> trashSlots;
    private Queue<TrashBase> currentSlot;
    #endregion

    #region Cooldown
    private bool canPush = true;
    private float pushCooldown = 2f;
    private float countingCooldown = 0;
    private bool canShoot = true;
    private float shootCooldown = 0.1f;
    #endregion

    public event Action<Vector2> mousePositionUpdated;
    private Camera mainCamera;
    private PlayerController player;
    private Vector2 mousePos;
    public Vector3 worldPos;
    private Vector2 gunDir;

    private void OnEnable()
    {
        if (vacuumBarrel != null)
        {
            vacuumBarrel.onTrashCollected += OnTrashCollected; // signal connected
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    
        player = transform.parent.GetComponent<PlayerController>();

        trashSlots = new List<Queue<TrashBase>> { trashSlot1, trashSlot2, trashSlot3, trashSlot4 };

        currentSlot = trashSlots[0]; // default to first slot
    }

    // Update is called once per frame
    void Update()
    {
        // UI stuff
        SwitchHotBar();
        if (SceneStateManager.InLevelScene)
        {
            UIHotBar();
            UIAirPush();
            if (!canPush)
            {
                AirPushBar.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                countingCooldown = pushCooldown;
                AirPushBar.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }


    void FixedUpdate()
    {   
        // Get mouse position in world space
        mousePos = Mouse.current.position.ReadValue();
        worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        gunDir = (worldPos - transform.position).normalized;
        // Rotate gun to look at mouse
        float angleDeg = Mathf.Atan2(gunDir.y, gunDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
        mousePositionUpdated?.Invoke(worldPos); // signal emitted


        if (canShoot && currentSlot.Count > 0 && InputSystem.actions.FindAction("Shoot").IsPressed())
        {
            TriggerShoot();
        }


        if (InputSystem.actions.FindAction("Push").IsPressed() && canPush && worldPos.y < player.transform.position.y - 1f
        && worldPos.x < player.transform.position.x + 6f && worldPos.x > player.transform.position.x - 6f)
        {
            TriggerPush();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TrashBase trash = collision.GetComponent<TrashBase>();
        Vector2 forceDir = ((Vector2)transform.position - (Vector2)collision.transform.position).normalized;
        AudioSource suckAUSrc = AudioManager.Instance.GetAudioSource(suckSound);
        // Apply Vacuum Force on trash
        if (InputSystem.actions.FindAction("Suck").IsPressed())
        {
            if (trash != null)
            {
                trash.ApplyVacuumForce(vacuumForce, forceDir);
            }
            if (!suckAUSrc.isPlaying)
            {
                AudioManager.Instance.PlaySfx(suckSound, 0.3f);
            }
        }
        else
        {
            if (suckAUSrc.isPlaying)
            {
                suckAUSrc.Stop();
            }
            
        }

        // Apply Push Force on trash
        // if (InputSystem.actions.FindAction("Push").IsPressed() && canPush)
        // {
        //     if (trash != null)
        //     {
        //         trash.ApplyVacuumForce(-pushForce + 100f, forceDir);
        //     }
        //     canPush = false;
        //     gunAUSrc.clip = pushSound;
        //     gunAUSrc.Play();
        //     StartCoroutine(PushCooldown());
        // }
    }

    private void OnTrashCollected(TrashBase trash)
    {
        // Object pooling trash collection into hotbar slots

        // Check if there are a slot with the same type of trash in it already
        foreach (Queue<TrashBase> slot in trashSlots)
        {
            if (slot.Count > 0 && slot.Peek().trashType == trash.trashType)
            {
                slot.Enqueue(trash);
                trash.gameObject.SetActive(false);
                AudioManager.Instance.PlaySfx(collectSound);
                return;
            }
        }

        // If don't found any same trash type, find an empty slot to enqueue
        foreach (Queue<TrashBase> slot in trashSlots)
        {
            if (slot.Count < 1)
            {
                slot.Enqueue(trash);
                trash.gameObject.SetActive(false);
                AudioManager.Instance.PlaySfx(collectSound, 0.3f);
                return;
            }
        }
}

    private void TriggerPush()
    {
        // Apply Push Force to push player  
        print("Pushed up!");        
        Vector2 pushDir = (worldPos - player.transform.position).normalized;
        player.ApplyPushForce(-pushForce, pushDir);
        canPush = false;
        pushAudio.Play();
        StartCoroutine(PushCooldown());
    }

    private void TriggerShoot()
    {
        GameObject newTrash = currentSlot.Dequeue().gameObject;
        newTrash.transform.position = vacuumBarrel.transform.position;
        newTrash.SetActive(true); // reuse collected trash
        Vector2 shootDir = (worldPos - player.transform.position).normalized;
        newTrash.GetComponent<TrashBase>().ApplyShootForce(shootForce, shootDir);
        
       // Apply force to trash if the gun not aim down
        // if (worldPos.y > player.transform.position.y - 1f)
        // {
        //     newTrash.GetComponent<TrashBase>().ApplyShootForce(shootForce, shootDir);
        // }

        canShoot = false;
        shootAudio.Play();
        StartCoroutine(ShootCooldown());
    }

    private void SwitchHotBar()
    {
        if (InputSystem.actions.FindAction("Hotbar1").IsPressed())
        {
            currentSlot = trashSlots[0];
        }
        else if (InputSystem.actions.FindAction("Hotbar2").IsPressed())
        {
            currentSlot = trashSlots[1];
        }
        else if (InputSystem.actions.FindAction("Hotbar3").IsPressed())
        {
            currentSlot = trashSlots[2];
        }
        else if (InputSystem.actions.FindAction("Hotbar4").IsPressed())
        {
            currentSlot = trashSlots[3];
        }
    }

    private void UIHotBar()
    {
        // Update hotbar UI images based on the top item in each slot
        Slot1.transform.GetChild(0).GetComponent<Image>().sprite = (trashSlot1.Count > 0) ? trashSlot1.Peek().GetComponent<SpriteRenderer>().sprite : null;
        Slot2.transform.GetChild(0).GetComponent<Image>().sprite = (trashSlot2.Count > 0) ? trashSlot2.Peek().GetComponent<SpriteRenderer>().sprite : null;
        Slot3.transform.GetChild(0).GetComponent<Image>().sprite = (trashSlot3.Count > 0) ? trashSlot3.Peek().GetComponent<SpriteRenderer>().sprite : null;
        Slot4.transform.GetChild(0).GetComponent<Image>().sprite = (trashSlot4.Count > 0) ? trashSlot4.Peek().GetComponent<SpriteRenderer>().sprite : null;

        // Update hotbar UI based on currentSlot
        Slot1.GetComponent<Image>().color = (currentSlot == trashSlots[0]) ? Color.yellow : Color.purple;
        Slot2.GetComponent<Image>().color = (currentSlot == trashSlots[1]) ? Color.yellow : Color.purple;
        Slot3.GetComponent<Image>().color = (currentSlot == trashSlots[2]) ? Color.yellow : Color.purple;
        Slot4.GetComponent<Image>().color = (currentSlot == trashSlots[3]) ? Color.yellow : Color.purple;

        // Update hotbar UI numbers based on the count of items in each slot
        Slot1.transform.GetChild(1).GetComponent<TMP_Text>().text = (trashSlot1.Count > 0) ? trashSlot1.Count.ToString() : "";
        Slot2.transform.GetChild(1).GetComponent<TMP_Text>().text = (trashSlot2.Count > 0) ? trashSlot2.Count.ToString() : "";
        Slot3.transform.GetChild(1).GetComponent<TMP_Text>().text = (trashSlot3.Count > 0) ? trashSlot3.Count.ToString() : "";
        Slot4.transform.GetChild(1).GetComponent<TMP_Text>().text = (trashSlot4.Count > 0) ? trashSlot4.Count.ToString() : "";
    }

    private void UIAirPush()
    {
        AirPushBar.GetComponent<Image>().color = (canPush) ? Color.blue : Color.gray;
        AirPushBar.transform.GetChild(0).GetComponent<Image>().color = (canPush) ? Color.white : Color.darkGray;

        if (!canPush)
        {
            countingCooldown -= Time.deltaTime;
            AirPushBar.transform.GetChild(1).GetComponent<TMP_Text>().text = ((int)countingCooldown).ToString(); 
        }
    }

    private IEnumerator PushCooldown()
    {
        yield return new WaitForSeconds(pushCooldown);
        print("Push ready!");
        canPush = true;
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        print("Shoot ready!");
        canShoot = true;
    }

    private void OnDisable()
    {
        if (vacuumBarrel != null)
        {
            vacuumBarrel.onTrashCollected -= OnTrashCollected; // signal disconnected
        }
    }
    
}
