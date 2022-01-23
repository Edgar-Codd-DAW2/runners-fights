using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGrounded : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private LayerMask platformLayerMask;
    public static bool isGrounded;
    private static BoxCollider2D boxCollider2;

    private void Start()
    {
        boxCollider2 = transform.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        Debug.Log(platformLayerMask);
        /*RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2.bounds.center, boxCollider2.bounds.size, 0f, Vector2.down * 1f);
        Debug.Log(raycastHit2D.collider);
        if (Physics2D.BoxCast(boxCollider2.bounds.center, boxCollider2.bounds.size, 0f, Vector2.down, 1f).collider != null)
        {
            isGrounded = true;
        }
        else
        {

            isGrounded = false;
        }*/

    }

    public static bool IsGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(boxCollider2.bounds.center, boxCollider2.bounds.size, 0f, Vector2.down, .1f, 64);
        Debug.Log(raycastHit2D.collider);
        return raycastHit2D.collider != null;
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
            isGrounded = true;
            Debug.Log("Enter " + collision.name + " " + gameObject.name);
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
            isGrounded = true;
            Debug.Log("Stay");
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
            isGrounded = false;
            Debug.Log("Out");
        
    }*/
}
