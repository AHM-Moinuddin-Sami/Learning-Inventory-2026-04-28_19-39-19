using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Core")]
    [SerializeField] public Item item;
    [Header("UI")]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI countText;


    public int count = 1;
    public Transform parentAfterDrag;

    public void InitializeItem(Item newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        RefreshCount();
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }
    public void SetRaycastTarget(bool value)
    {
        image.raycastTarget = value;
    }

    public void SnapToParentAfterDrag()
    {
        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        if (InventoryManager.Instance.HasHeldItem)
        {
            InventorySlot parentSlot = transform.parent.GetComponent<InventorySlot>();

            if (parentSlot != null)
            {
                InventoryManager.Instance.TryPlaceHeldItemInSlot(parentSlot);
            }

            return;
        }

        InventoryManager.Instance.PickUpItemWithCursor(this);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // works
        // transform.position = Pointer.current.position.ReadValue(); // any pointer system -> Mouse, touch etc.
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        SnapToParentAfterDrag();
    }

}
