using System.Collections;
using FishNet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerState PlayerState {get; set;}
    public InventorySlots[] WeaponSlots = new InventorySlots[3];
    public ItemManager ItemManager {get; set;}
    private GameObject draggedObject;
    private InventorySlots LastSlotPosition;
    private InventorySlots currentSlot;
    private GameObject selectedSlotIcon;
    private ObjectSpawner objectSpawner;
    private DescriptionManager descriptionManager => GetComponent<DescriptionManager>();
    [SerializeField] private Image DropPanel;
    [SerializeField] private Image inventoryPanel;
    public int EquipedSlotIndex;


    void Start(){
        objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<ObjectSpawner>();
        EquipedSlotIndex = 0;
    }
    void Update()
    {
        //fais suivre l'objet clique par la souris, sur la souris.
        if(draggedObject != null){
            draggedObject.transform.position = Input.mousePosition;
        }

    
        if(Input.GetKeyDown("m")){
            SwapEquipedSlot();
        }

    }


    private IEnumerator DropWeapon(GameObject weapon)
    {
        while (weapon == null)
        {
            yield return null;
        }
        weapon.GetComponent<WeaponScript>().Drop();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        //change the raycast state needed for the descriptionManager
        DropPanel.raycastTarget = true;
        inventoryPanel.raycastTarget = true;

        //Hide selected icon when dragging object
        if(selectedSlotIcon != null) selectedSlotIcon.SetActive(false);

        if(eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlots>().heldItem != null){
            //Hide descprition panels if the item is dragged
            if(descriptionManager.GetInCD()) descriptionManager.StopTheCoroutine();
            else descriptionManager.GetDescriptionWindow().SetActive(false);
        }

        if(eventData.button == PointerEventData.InputButton.Left){

            //recupere le raycast du slot clique.
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlots slot = clickedObject.GetComponent<InventorySlots>();

            if (slot != null && slot.heldItem != null){
                draggedObject = slot.heldItem;
                slot.heldItem = null;
                LastSlotPosition = slot;
            }
        }
        //Drop the inventory Item whith right clicking on it 
        if(eventData.button == PointerEventData.InputButton.Right){
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlots slot = clickedObject.GetComponent<InventorySlots>();

            if(slot != null && slot.heldItem != null) {

                if(slot == WeaponSlots[EquipedSlotIndex])
                {
                    PlayerState.WeaponScript.Drop();
                    PlayerState.HoldingWeapon = false;
                    PlayerState.PointerScript.SpriteNumber = 0;
                }
                else
                {
                    GameObject weapon = SpawnInventoryItem(slot.heldItem);
                    StartCoroutine(DropWeapon(weapon));
                }

                Destroy(slot.heldItem);
                slot.heldItem = null;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(draggedObject != null && eventData.button == PointerEventData.InputButton.Left){
            
            if (eventData.pointerCurrentRaycast.gameObject != null){

                GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
                currentSlot = clickedObject.GetComponent<InventorySlots>();

                //if the slot is empty - place item
                if(currentSlot != null && currentSlot.heldItem == null){
                    if(Swapable(currentSlot, draggedObject)){
                        RefreshScale();
                        PlaceItem();
                    }
                    else{
                        PlaceLastSlotPosition();
                    }

                }

                //if the slot is not empty - swap item
                else if (currentSlot != null && currentSlot.heldItem != null){
                    if(Swapable(currentSlot, draggedObject) && Swapable(LastSlotPosition, currentSlot.heldItem)){
                        RefreshSwapScale();
                        SwapItemPosition();
                    }
                    else{
                        PlaceLastSlotPosition();
                    }
                }

                //if there is not slot and no drop - replace item at last pos
                else if (clickedObject.name != "DropItem"){
                    PlaceLastSlotPosition();
                }

                //drop the item on the map
                else
                {
                    if (LastSlotPosition == WeaponSlots[EquipedSlotIndex])
                    {
                        PlayerState.WeaponScript.Drop();
                        PlayerState.HoldingWeapon = false;
                        PlayerState.PointerScript.SpriteNumber = 0;
                    }
                    else
                    {
                        GameObject weapon = SpawnInventoryItem(draggedObject);
                        StartCoroutine(DropWeapon(weapon));
                    }

                    Destroy(draggedObject);
                }

                draggedObject = null;
                DropPanel.raycastTarget = false;
                inventoryPanel.raycastTarget = false;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData){
        if(eventData.pointerCurrentRaycast.gameObject.tag == "InventorySlot"){
            selectedSlotIcon = eventData.pointerCurrentRaycast.gameObject.transform.GetChild(0).gameObject;
            selectedSlotIcon.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        if(selectedSlotIcon != null){
            selectedSlotIcon.SetActive(false);
            selectedSlotIcon = null;
        }
    }

    public void SwapEquipedSlot(){
        GameObject currentStoredObject = WeaponSlots[EquipedSlotIndex].heldItem; 

        if (currentStoredObject != null){
            WeaponSlots[EquipedSlotIndex].transform.localScale = new Vector3(1.5f,1.5f,1f);

            if (currentStoredObject != null) {
                currentStoredObject.transform.localScale = new Vector3(1.5f,1.5f,1f);
                do {
                    EquipedSlotIndex = (EquipedSlotIndex + 1)%3;
                }while(WeaponSlots[EquipedSlotIndex].heldItem == null);
            }

            GameObject nextStoredObject = WeaponSlots[EquipedSlotIndex].heldItem;
            WeaponSlots[EquipedSlotIndex].transform.localScale = new Vector3(2f,2f,1f);
            nextStoredObject.transform.localScale = WeaponSlots[EquipedSlotIndex].transform.localScale;
            
            GameObject equipedObject = nextStoredObject.GetComponent<InventoryItem>().displayedItem.prefab;
            GameObject destroyedGameObject = PlayerState.WeaponScript.gameObject;
            destroyedGameObject.GetComponent<WeaponScript>().Drop();
            destroyedGameObject.SetActive(false);
            objectSpawner.SpawnObjectAndUpdateRpc(equipedObject, PlayerState.gameObject.transform.position, new Quaternion(), InstanceFinder.ClientManager.Connection, ItemManager.gameObject);
            Destroy(destroyedGameObject, 0.15f);
        }
    }

//----Auxilaries Functions that work as their name says.---------------------------------------------------------

    private bool Swapable(InventorySlots selectedSlot, GameObject selectedItem){
        bool res = false;
        string placedItemName = selectedItem.GetComponent<InventoryItem>().displayedItem.prefab.tag;
        string currentSlotName = selectedSlot.transform.parent.name;

        if(currentSlotName == "InventorySlots") res = true;
        else{
            if(placedItemName == "Weapon" && currentSlotName == "WeaponSlots") res = true;
            else if(placedItemName == "Implant" && currentSlotName == "ImplantSlot") res = true;
            else if(placedItemName == "Item" && currentSlotName == "ConsumabelSlots") res = true;
        }
        return res;
    }

    private void SwapItemPosition(){
        LastSlotPosition.GetComponent<InventorySlots>().SetHeldItem(currentSlot.heldItem);
        currentSlot.SetHeldItem(draggedObject);
    }

    private void PlaceItem(){
        currentSlot.SetHeldItem(draggedObject);
    }

    private void PlaceLastSlotPosition(){
        LastSlotPosition.SetHeldItem(draggedObject);
    }

    private void RefreshScale(){
        Vector3 draggedObjectScale = draggedObject.transform.localScale;
        Vector3 currentSlotScale = currentSlot.transform.localScale;

        if(draggedObjectScale != currentSlotScale){
            draggedObject.transform.localScale = currentSlotScale;
        }
    }

    private void RefreshSwapScale(){
        Vector3 draggedObjectScale = draggedObject.transform.localScale;
        Vector3 currentSlotScale = currentSlot.transform.localScale;

        if(draggedObjectScale != currentSlotScale){
            draggedObject.transform.localScale = currentSlotScale;
            currentSlot.heldItem.transform.localScale = draggedObjectScale;
        }
    }

    private GameObject SpawnInventoryItem(GameObject spawnedItem){
        GameObject spawnedItemPrefab = spawnedItem.GetComponent<InventoryItem>().displayedItem.prefab;
        Vector3 spawnPosition = GameObject.FindWithTag("Player").transform.position;
        objectSpawner.SpawnObjectRpc(spawnedItemPrefab, spawnPosition, new Quaternion());
        return spawnedItemPrefab;
    }
}
