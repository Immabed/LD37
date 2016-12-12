using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    Rigidbody2D rb;
    public float speed;
    bool[] isWall = new bool[4];
    public Transform imageTransform;

    [SerializeField]
    private Resource spareParts;
    [SerializeField]
    private Resource computer;
    [SerializeField]
    private Resource powerCell;

    public Animator animator;


    [SerializeField] private Resource item = null;

    public Resource Item { get { return item; } }

    public bool HasItem { get { return item != null; } }
    //public bool IsIdle {  get { return isIdle; } }

	// Use this for initialization
	void Start () {
        //rb = GetComponent<Rigidbody2D>();
	}
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Rotation
        if (horizontal != 0f || vertical != 0f)
        {
            animator.SetBool("IsIdle", false);
            if (horizontal > 0f && vertical > 0f)
                imageTransform.rotation = Quaternion.Euler(0, 0, 45);
            else if (horizontal > 0f && vertical == 0f)
                imageTransform.rotation = Quaternion.Euler(0, 0, 0);
            else if (horizontal < 0f && vertical == 0f)
                imageTransform.rotation = Quaternion.Euler(0, 0, 180);
            else if (horizontal < 0f && vertical > 0f)
                imageTransform.rotation = Quaternion.Euler(0, 0, 135);
            else if (horizontal == 0f && vertical > 0f)
                imageTransform.rotation = Quaternion.Euler(0, 0, 90);
            else if (horizontal < 0f && vertical < 0f)
                imageTransform.rotation = Quaternion.Euler(0, 0, 225);
            else if (horizontal == 0f && vertical < 0f)
                imageTransform.rotation = Quaternion.Euler(0, 0, 270);
            else if (horizontal > 0f && vertical < 0f)
                imageTransform.rotation = Quaternion.Euler(0, 0, 315);
        }
        else
        {
            animator.SetBool("IsIdle", true);
        }

        if (isWall[(int)Direction.RIGHT] && horizontal > 0)
            horizontal = 0;
        if (isWall[(int)Direction.LEFT] && horizontal < 0)
            horizontal = 0;
        if (isWall[(int)Direction.UP] && vertical > 0)
            vertical = 0;
        if (isWall[(int)Direction.DOWN] && vertical < 0)
            vertical = 0;

        rb.MovePosition(rb.position + new Vector2(horizontal, vertical).normalized * speed * Time.fixedDeltaTime);


    }

    // Set resource, DOESN'T account
    public void SetResource (ResourceType res)
    {
        if (item != null)
        {
            item.MakeInvisible();
        }
        if (res == ResourceType.SPAREPARTS)
        {
            item = spareParts;
            item.MakeVisible();
        }
        else if (res == ResourceType.COMPUTER)
        {
            item = computer;
            item.MakeVisible();
        }
        else if (res == ResourceType.POWERCELL)
        {
            item = powerCell;
            item.MakeVisible();
        }
        animator.SetBool("IsCarry", true);
    }

    public void RemoveResource()
    {
        item.MakeInvisible();
        item = null;
        animator.SetBool("IsCarry", false);
    }

    public void Collision(Direction dir, bool isTrigger )
    {
        isWall[(int)dir] = isTrigger;
    }
}

public enum Direction { RIGHT, UP, LEFT, DOWN }