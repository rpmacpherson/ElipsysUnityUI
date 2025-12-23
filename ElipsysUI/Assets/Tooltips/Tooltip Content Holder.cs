using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Tooltips.UI
{
    public class TooltipContentHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private TooltipPopup tooltipPopup;
        [SerializeField]
        private TooltipContent tooltip;

        public void OnPointerEnter(PointerEventData eventData)
        {
            //tooltipPopup.DisplayInfo(tooltip);
            StartCoroutine(tooltipPopup.DisplayInfo(tooltip));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipPopup.HideInfo();
            StopAllCoroutines();
        }
    }
}