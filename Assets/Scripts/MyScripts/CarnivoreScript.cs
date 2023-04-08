using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarnivoreScript : AgentLogic
{
    #region Static Variables
    private static float _boxPoints = 2.0f;
    private static float _boatPoints = 5.0f;
    private static float _piratePoints = -100.0f;
    #endregion
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Plant") || other.gameObject.tag.Equals("Box"))
        {
            points += _boxPoints;
            Destroy(other.gameObject);
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag.Equals("Herbivore") || other.gameObject.tag.Equals("Boat"))
        {
            points += _boatPoints;
            Destroy(other.gameObject);
        }
        if(other.gameObject.tag.Equals("Carnivore") || other.gameObject.tag.Equals("Enemy"))
        {
            //This is a safe-fail mechanism. In case something goes wrong and the Boat is not destroyed after touching
            //a pirate, it also gets a massive negative number of points.
            points += _piratePoints;
        }
    }
}
