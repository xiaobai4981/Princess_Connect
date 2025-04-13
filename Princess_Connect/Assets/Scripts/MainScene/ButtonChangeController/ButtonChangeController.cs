using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonChangeController : MonoBehaviour
{
    private Button btn;
    private Animator animator;
    public AnimationClip idleClip;
    public AnimationClip moveClip;
    // Start is called before the first frame update
    void Start()
    {
        btn = this.transform.parent.gameObject.GetComponent<Button>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var eventSystem = EventSystem.current;

        // ≈–∂œ «∑Ò»‘±ª Û±Í–¸Õ£
        bool isHovered = eventSystem.currentSelectedGameObject == btn.gameObject;
        if (!isHovered)
        {
            animator.SetBool("isPressed", false);
        }
        // ≈–∂œ Û±Í «∑Ò»‘∞¥œ¬£®∞¸¿®¥•√˛£©
        bool isPointerDown = Input.GetMouseButton(0) && isHovered;
        if (isPointerDown)
        {
            animator.SetBool("isPressed", true);
        }
    }
}
