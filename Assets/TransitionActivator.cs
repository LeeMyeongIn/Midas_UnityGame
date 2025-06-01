using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionActivator : MonoBehaviour
{
    [SerializeField] private Transition transitionToTrigger;

    private void Update()
    {
        if (IsRightClickedThis())
        {
            if (EndingPanel.Instance != null)
            {
                EndingPanel.Instance.ShowConfirmation(() =>
                {
                    if (transitionToTrigger != null)
                    {
                        transitionToTrigger.gameObject.SetActive(true);
                        var player = GameObject.FindGameObjectWithTag("Player");
                        if (player != null)
                        {
                            transitionToTrigger.InitiateTransition(player.transform);
                        }
                        else
                        {
                            Debug.LogWarning("[TransitionActivator] Player�� ã�� �� �����ϴ�.");
                        }
                    }
                });
            }
            else
            {
                Debug.LogWarning("[TransitionActivator] EndingPanel.Instance�� null�Դϴ�.");
            }
        }
    }

    private bool IsRightClickedThis()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            return hit.collider != null && hit.collider.gameObject == this.gameObject;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
    }
}
