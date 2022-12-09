using UnityEngine;

public abstract class RotatableObject : MonoBehaviour
{
    protected ObjectRotation objectRotation;
    
    protected virtual void Awake()
	{
        switch(transform.localEulerAngles.z)
		{
            case 0:
                objectRotation = ObjectRotation.North;
                break;
            case 90:
                objectRotation = ObjectRotation.West;
                break;
            case 180:
                objectRotation = ObjectRotation.South;
                break;
            case 270:
                objectRotation = ObjectRotation.East;
                break;
            default:
                objectRotation = ObjectRotation.North;
                break;
		}
	}
}

public enum ObjectRotation
{
    North,
    West,
    South,
    East
}