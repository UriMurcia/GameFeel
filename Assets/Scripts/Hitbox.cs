using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderState
{

    Closed,

    Open,

    Colliding

}

public class Hitbox : MonoBehaviour
{
    public LayerMask mask;

    public bool useSphere = false;

    public Vector3 hitboxSize = Vector3.one;

    public float radius = 0.5f;

    public Color inactiveColor;

    public Color collisionOpenColor;

    public Color collidingColor;


    private ColliderState _state;
    public void startCheckingCollision()
    {
        _state = ColliderState.Open;

    }

    public void stopCheckingCollision()
    {
        _state = ColliderState.Closed;

    }

    private void Update()
    {
        if (_state == ColliderState.Closed) { return; }

        //Collider[] colliders = Physics.OverlapBox(position, boxSize, rotation, mask);


        //if (colliders.Length > 0)
        {

            _state = ColliderState.Colliding;

            // We should do something with the colliders

        }
        //else
        {

            _state = ColliderState.Open;

        }
    }

    private void checkGizmoColor()
    {
        switch (_state)
        {

            case ColliderState.Closed:

                Gizmos.color = inactiveColor;

                break;

            case ColliderState.Open:

                Gizmos.color = collisionOpenColor;

                break;

            case ColliderState.Colliding:

                Gizmos.color = collidingColor;

                break;

        }

    }
}
