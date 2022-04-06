using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string content, header;
    private Coroutine lastRoutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        lastRoutine = StartCoroutine(ShowToolTip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(lastRoutine);
        TooltipSystem.Hide();
    }
    private IEnumerator ShowToolTip()
    {
        yield return new WaitForSeconds(1.5f);
        TooltipSystem.Show(content, header);
    }

}
