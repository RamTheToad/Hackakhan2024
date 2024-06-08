using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 startPos;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 1;
        startPos = rb.transform.position;
    }

    
    // Update is called once per frame
    
    void Update()
    {
        // reset the ball if it's too far away from the view
        if (transform.position.x > 150 || transform.position.x < -50 || transform.position.y < -20 || transform.position.y > 150) {
            rb.transform.position = startPos;
            rb.velocity = new Vector2(0, 0);
        }
    }

    

}
