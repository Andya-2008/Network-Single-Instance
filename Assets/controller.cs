using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour
{
    public float speed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A)) {
            this.transform.position = new Vector2(this.transform.position.x-speed, this.transform.position.y);
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position = new Vector2(this.transform.position.x + speed, this.transform.position.y);
        }
    }
}
