using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationMovementController : MonoBehaviour
{
	// declare reference variables
	PlayerInput playerInput;
	CharacterController characterController;
	Animator animator;

	//variables to store optimized setter
	int isWalkingHash;
	int isRunningHash;

	//variable to store player input values
	Vector2 currentMovementInput;
	Vector3 currentMovement;
	Vector3 currentRunMovement;
	bool isMovementPressed;
	bool isRunPressed;
	float rotationFactorPerFrame  = 15.0f;
	float runMultiplier = 3.0f;

    // Awake is calledd earlier than Start in Unity's event life cycle
    void Awake()
    {
        //initialize set reference variables
	playerInput = new PlayerInput();
	characterController = GetComponent<CharacterController>();
	animator = GetComponent<Animator>();

	isWalkingHash = Animator.StringToHash("isWalking");
	isRunningHash = Animator.StringToHash("isRunning");
	// set the player input callbacks
	playerInput.CharacterControls.Move.started += onMovementInput;
	playerInput.CharacterControls.Move.canceled += onMovementInput;
	playerInput.CharacterControls.Move.performed += onMovementInput;
	playerInput.CharacterControls.Run.started += onRun;
	playerInput.CharacterControls.Run.canceled += onRun;
	 
	
    }

	void onRun (InputAction.CallbackContext context) {
	 isRunPressed = context.ReadValueAsButton();
	
	}

	void handleRotation(){
	
	Vector3 positionToLookAt;
	//the change in position our character should point to
	positionToLookAt.x = currentMovement.x;
	positionToLookAt.y = 0.0f;
	positionToLookAt.z = currentMovement.z;
	// the current rotation of our character
	Quaternion currentRotation = transform.rotation;

	if(isMovementPressed){
	//creates anew rotation based on where the player is currently pressing
	Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
	transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
}
	
	}

	// handler function to set the player input values
	void onMovementInput (InputAction.CallbackContext context){

		currentMovementInput = context.ReadValue<Vector2>();
		currentMovement.x = currentMovementInput.x;
		currentMovement.z = currentMovementInput.y;
		currentRunMovement.x = currentMovementInput.x * runMultiplier;
		currentRunMovement.z = currentMovementInput.y* runMultiplier;
		isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y !=0;


}

	void handleAnimations(){
	// get parameter values from animator
	bool isWalking = animator.GetBool(isWalkingHash);
	bool isRunning = animator.GetBool(isRunningHash);

	//start walking if movement pressed is true and not already running
	if (isMovementPressed && !isWalking){
	animator.SetBool(isWalkingHash, true);
	}

	//stop walking if isMovementPressed is false and not already walking
	else if (!isMovementPressed && isWalking){
	animator.SetBool(isWalkingHash, false);
	}

	if ((isMovementPressed && isRunPressed) && !isRunning){
	animator.SetBool(isRunningHash, true);
		}
	else if ((!isMovementPressed || !isRunPressed) && isRunning){
	animator.SetBool(isRunningHash, false);

		}

	}
	void handleGravity(){
	//apply proper gravity depending on if the character is grounded or not
	if (characterController.isGrounded){
		float groundedGravity = -.05f;
		currentMovement.y = groundedGravity;
		currentRunMovement.y = groundedGravity;
	} else {

	float gravity = -9.8f;
	currentMovement.y  += gravity;
	currentRunMovement.y += gravity; 
	}	

}

    // Update is called once per frame
    void Update()
    {
	handleRotation();
	handleAnimations();

	if (isRunPressed){
	characterController.Move(currentRunMovement * Time.deltaTime);

	}else{
        characterController.Move(currentMovement * Time.deltaTime);
	}
    }

	void OnEnable () {
	//enable the character controls action map
	playerInput.CharacterControls.Enable();			
	
	}

	 void OnDisable() {
	//disable the character controls action map
	playerInput.CharacterControls.Disable();

	}
}
