using UnityEngine;
using UnityEngine.Events;
//Gustavo
using UnityEngine.InputSystem;
//Gustavo

[RequireComponent(typeof(PlayerInput))]
public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 16f;							// Amount of force added when the player jumps.
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	public Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;


	//Gustavo
	private static CharacterController2D instance;

    public float runSpeed = 160f;
    private float horizontalMove = 0f;

	public Animator flipAnim;
	public Animator playerAnim;

    private bool interactPressed = false;
	private bool sitPressed = false;
    private bool talkToCafePressed = false;

	private bool pausePressed = false;

	[SerializeField] private GameObject pottedPlant01;
	[SerializeField] private GameObject pottedPlant02;

	//Gustavo

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		if (instance != null)
        {
            Debug.LogError("Found more than one Character Controller 2D in the scene.");
        }
        instance = this;

		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

	}

	public static CharacterController2D GetInstance() 
    {
        return instance;
    }

	private void FixedUpdate()
	{
		if(DialogueManager.GetInstance().dialogueIsPlaying || PauseMenuManager.GetInstance().isPaused){
			return;
		}

		UpdateIsGrounded();
		HandleHorizontalMovement();

	}


	private void UpdateIsGrounded(){
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}

	private void HandleHorizontalMovement()
    {
		playerAnim.SetFloat("Speed", Mathf.Abs(horizontalMove));
		//Aqui movemos a Novelo
		m_Rigidbody2D.velocity = new Vector2(horizontalMove * runSpeed * Time.fixedDeltaTime, m_Rigidbody2D.velocity.y);

		if (horizontalMove > 0 && !m_FacingRight)
		{
			// ... flip the player.
			Flip();
			
		}
		// Otherwise if the input is moving the player left and the player is facing right...
		else if (horizontalMove < 0 && m_FacingRight)
		{
			// ... flip the player.
			Flip();
			
		}

    }

	public void Move(InputAction.CallbackContext context)
	{
		if (context.performed){
			horizontalMove = context.ReadValue<Vector2>().x;
		}
        else if (context.canceled)
        {
            horizontalMove = context.ReadValue<Vector2>().x;
        }

	}

	//Gustavo
	public void Jump(InputAction.CallbackContext context){

		if(DialogueManager.GetInstance().dialogueIsPlaying || PauseMenuManager.GetInstance().isPaused){
			return;
		}

		if (context.performed && m_Grounded){
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpForce);
			Debug.Log("Pulou!");
		}
		if(context.canceled && m_Rigidbody2D.velocity.y > 0f){
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_Rigidbody2D.velocity.y * 0.5f);
		}

	}
	//Gustavo

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		//Gustavo
		flipAnim.SetTrigger("Flip");
		//Gustavo

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;

	}

	//Gustavo
	public void Interact(InputAction.CallbackContext context){

		// If the player should interact...
		if (context.performed && m_Grounded && !PauseMenuManager.GetInstance().isPaused){
			interactPressed = true;
			//Debug.Log("Interact.");
		}
        else if (context.canceled)
        {
            interactPressed = false;
        } 
	}
	//Gustavo

	//Gustavo
	public void Sit(InputAction.CallbackContext context){
		if(DialogueManager.GetInstance().dialogueIsPlaying){
			return;
		}
		// If the player should sit... or skip plant stage
		if (context.performed && m_Grounded && !PauseMenuManager.GetInstance().isPaused){
			sitPressed = true;
			Debug.Log("Sit.");
			pottedPlant01.GetComponent<PottedPlant>().SkipTimer();
			pottedPlant02.GetComponent<PottedPlant>().SkipTimer();
		}
		else if (context.canceled)
        {
            sitPressed = false;
        } 
	}
	//Gustavo

	//Gustavo
	public void TalkToCafe(InputAction.CallbackContext context){

		// If the player should talk to Café...
		if (context.performed && m_Grounded && !PauseMenuManager.GetInstance().isPaused){
			talkToCafePressed = true;
			Debug.Log("Talk to Café.");
		}
		else if (context.canceled)
        {
            talkToCafePressed = false;
        } 
	}
	//Gustavo

		//Gustavo
	public void Pause(InputAction.CallbackContext context){

		// If the player should pause...
		if (context.performed && !DialogueManager.GetInstance().dialogueIsPlaying){
			pausePressed = true;
			//Debug.Log("Pause.");
		}
        else if (context.canceled)
        {
            pausePressed = false;
        } 
	}
	//Gustavo
	
    public bool GetInteractPressed() 
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }

    public bool GetSitPressed() 
    {
        bool result = sitPressed;
        sitPressed = false;
        return result;
    }

	public bool GetTalkToCafePressed() 
    {
        bool result = talkToCafePressed;
        talkToCafePressed = false;
        return result;
    }

	public bool GetPausePressed() 
    {
        bool result = pausePressed;
        pausePressed = false;
        return result;
    }

	public void StopMoving(){

		playerAnim.SetFloat("Speed", 0f);
		m_Rigidbody2D.velocity = new Vector2(0f, 0f);

	}

}
