using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSection : MonoBehaviour
{
    public int sectionId; // Unique ID for the section
    public Vector3 referencePoint; // Reference point for comparison   
    private void Start()
    {
        referencePoint = transform.position;
    }    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            // Assume you have a method to update the car's current section
            Car car = other.GetComponent<Car>();
            Debug.Log("Touched car");
            if (car != null)
            {
                if(car.CurrentSection == null || sectionId == 1 + car.CurrentSection.sectionId)
                    car.SetCurrentSection(this);
            }
        }
    }
}
