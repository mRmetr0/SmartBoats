using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class PirateLogic : AgentLogic
{
    #region Static Variables
    private static float _boxPoints = 0.1f;
    private static float _boatPoints = 3.0f;
    private static float _omnivorePoints = 5.0f;
    #endregion
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Box"))
        {
            points += _boxPoints;
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag.Equals("Boat"))
        {
            points += _boatPoints;
            Destroy(other.gameObject);
        } else if(other.gameObject.tag.Equals("Omnivore"))
        {
            points += _omnivorePoints;
            Destroy(other.gameObject);
        }
    }

}
