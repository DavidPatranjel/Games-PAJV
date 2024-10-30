using UnityEngine;

public class LapTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the car
        if (other.CompareTag("Car"))
        {
            Car car = other.GetComponent<Car>();
            if (car != null)
            {
                if(car.CurrentSection.sectionId == 8)
                    car.IncrementLaps();
            }
            
        }
    }
}
