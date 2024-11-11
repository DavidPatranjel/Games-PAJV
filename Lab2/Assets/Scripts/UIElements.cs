using System;
using UnityEngine;
using UnityEngine.UI;

public class UIElements : MonoBehaviour
{
    // Reference to the Panel GameObject
    public GameObject panel;

    // Reference to the Arrow Image component (child of the Panel)
    public Image arrow;
    public Image pengs;
    public Image otters;
    // Method to activate the Panel
    public void ActivateElement()
    {
        if (panel != null)
        {
            panel.SetActive(true);
            arrow.transform.rotation = Quaternion.Euler(0, 0, 90);
        }

    }

    // Method to deactivate the Panel
    public void DeactivateElement()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    // Method to rotate the Arrow element by a specified angle
    public void RotateElement(float angle)
    {
        if (arrow != null)
        {
            float currentRotation = arrow.transform.eulerAngles.z;
            arrow.transform.rotation = Quaternion.Euler(0, 0, currentRotation - angle);
            //Debug.Log(angle.ToString());
        }
    }

    public void ToggleImages()
    {
        if (pengs != null && otters != null)
        {
            bool isImageOneActive = pengs.gameObject.activeSelf;

            // Turn one image on and the other off
            pengs.gameObject.SetActive(!isImageOneActive);
            otters.gameObject.SetActive(isImageOneActive);
        }
    }
}
