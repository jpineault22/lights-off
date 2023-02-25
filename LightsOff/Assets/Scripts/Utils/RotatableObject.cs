using UnityEngine;

public abstract class RotatableObject : MonoBehaviour
{
    protected ObjectOrientation objectRotation;
    
    protected virtual void Awake()
	{
        switch(transform.localEulerAngles.z)
		{
            case 0:
                objectRotation = ObjectOrientation.North;
                break;
            case 90:
                objectRotation = ObjectOrientation.West;
                break;
            case 180:
                objectRotation = ObjectOrientation.South;
                break;
            case 270:
                objectRotation = ObjectOrientation.East;
                break;
            default:
                objectRotation = ObjectOrientation.North;
                break;
		}
	}
}

public enum ObjectOrientation
{
    North,
    West,
    South,
    East
}