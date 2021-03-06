﻿using UnityEngine;
using UnityEngine.Events;
using Scriptables.GameEvents;

[RequireComponent(typeof(Selectable))]
public class ItemContainerTalker : MonoBehaviour {
    public bool instantlyRespawn = false;
    [SerializeField]
    private GameEvent invalidSelection;
    /**
     * <summary>
     * Read only. Use AddItem, DestroyItem, and ActivateItem
     * </summary>
     */
    public bool hasItem;
    [Range(0f, 1f)]
    public float chanceToPlayAntSound = 0.1f;
    public SpriteRenderer poof;
    public ItemDescriptionViewer descriptionViewer;
    public float globalCooldownTime = 1f;
    public Timer globalCooldown;
    private CharacterCreatorItem item;
    private Selectable selectable;
    public UnityEvent onItemActivated;

    public void ActivateItem() {
        // transform.BroadcastMessage("Apply");
        if(hasItem) {
            // Check whether the cooldown is finished
            if (globalCooldown.timeRemaining <= 0)
            {
                poof.color = item.gameObject.GetComponentInChildren<SpriteRenderer>().color;
                poof.GetComponent<Animator>().SetTrigger("Poof");
                SfxManager.Instance.PlayPoof();

                if (Random.value <= chanceToPlayAntSound)
                {
                    SfxManager.Instance.PlayAntSound();
                }
                item.OnApply.Invoke();
                onItemActivated.Invoke();
                // if (item == null)
                // {
                hasItem = false;
                // }
                if (instantlyRespawn)
                {
                    SendMessageUpwards("ItemUsed", this, SendMessageOptions.RequireReceiver);
                }

                // Start the cooldown
                globalCooldown.BeginTimer(globalCooldownTime);
            } else {
                if( invalidSelection )
                    invalidSelection.Raise();
            }
            
        } else {
            if( invalidSelection )
                invalidSelection.Raise();
            Debug.LogWarning("No CharacterCreatorItem inside container");
        }
    }
    public void AddItem( CharacterCreatorItem newItem ) {
        Debug.Log("Item " + newItem.itemName + " was refreshed!");
        hasItem = true;
        item = newItem;
        if( selectable.isHovered ) {
            StartHoverItem();
        }
    }

    public void StartHoverItem() {
        if(hasItem) {
            item.OnHover.Invoke();
        }
    }

    public void StopHoverItem() {
        if(hasItem) {
            item.OnStopHover.Invoke();
        }
    }

    public void DestroyItem() {
        if(hasItem) {
            // item.OnDiscard.Invoke();
            Destroy(item);
            hasItem = false;
        } else {
            Debug.LogWarning("Couldn't destroy non-existant item");
        }
    }

    // private bool UpdateItem() {
    //     if( item != null && item.isActiveAndEnabled ) {
    //         return true;
    //     } else {
    //         item = transform.GetComponentInChildren<ApplyEffect>();
    //         return item != null && item.isActiveAndEnabled;
    //     }
    // }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        selectable = GetComponent<Selectable>();
    }
}
