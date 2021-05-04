using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Sprite hoverSprite;
    private Sprite defaultSprite;
    private Image image;

    public Sprite HoverSprite { get => hoverSprite; set => hoverSprite = value; }
    public Sprite DefaultSprite { get => defaultSprite; set => defaultSprite = value; }
    public Image Image => image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        defaultSprite = image.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        image.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData pointerEventData) => image.sprite = defaultSprite;
}
