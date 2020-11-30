using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Controller2D : RaycastController {
	
	float maxClimbAngle = 80;
	float maxDescendAngle = 80;
	public float upwardY = 0;
	
	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;
	private GameObject player;
	private GameObject cam;
	private GameObject soundManager;
	private GameObject playerVarsObj;

	private GameObject turnIndicatorObj;

	private GameObject explodeObj;

    private bool beltTest = false;

	public override void Start() {
		base.Start ();
		collisions.faceDir = 1;
		cam = GameObject.Find("Main Camera");
		soundManager = GameObject.Find("SoundManager");
		explodeObj = GameObject.Find("ExplosionAnimation");
		playerVarsObj = GameObject.Find("PlayerVars");
		turnIndicatorObj = GameObject.Find("TurnIndicatorSprite");
	}
	
	public void Move(Vector3 velocity, bool standingOnPlatform) {
		Move (velocity, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform = false) {
        UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.velocityOld = velocity;
		playerInput = input;

		if (velocity.x != 0) {
			collisions.faceDir = (int)Mathf.Sign(velocity.x);
		}

		if (velocity.y < 0) {
			DescendSlope(ref velocity);
		}

        CustomizedCollisions(ref velocity);
        HorizontalCollisions(ref velocity);
        if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}


        transform.Translate (velocity);

		if (standingOnPlatform) {
			collisions.below = true;
		}
	}

    void CustomizedCollisions(ref Vector3 velocity)
    {
        beltTest = false;
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {

            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "ConveyorBelt")
                {
                    beltTest = true;
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    float conveyorBeltSpeed = hit.collider.GetComponent<ConveyorBelt>().speed;
                    float conveyorBeltDirectionX = hit.collider.GetComponent<ConveyorBelt>().directionX;
                    float conveyorBeltDirectionY = hit.collider.GetComponent<ConveyorBelt>().directionY;
                    velocity.x += Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * conveyorBeltSpeed * conveyorBeltDirectionX * Time.deltaTime;
                    //velocity.y += Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * conveyorBeltSpeed * conveyorBeltDirectionY * Time.deltaTime;
                }
            }
        }
    }


    void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		if (Mathf.Abs(velocity.x) < skinWidth) {
			rayLength = 2*skinWidth;
		}
		
		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);

			if (hit) {

				if (hit.distance == 0) {
					continue;
				}
			
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (hit.collider.tag == "Player") {
					gameObject.GetComponent<Player>().playerHitHorizontally = true;
				} else {
					gameObject.GetComponent<Player>().playerHitHorizontally = false;
				}
				if (i == 0 && slopeAngle <= maxClimbAngle) {
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance-skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}

				
			}
		}
	}

	void VerticalCollisions(ref Vector3 velocity) {
        float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++) {

			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit) {
				//Fall through
				
				if (hit.collider.tag == "Through") {
					if (directionY == 1 || hit.distance == 0) {
						continue;
					}
					if (collisions.fallingThroughPlatform) {
						continue;
					}
					if (playerInput.y == -1) {
						//collisions.fallingThroughPlatform = true;
						//Invoke("ResetFallingThroughPlatform", .5f);
						//continue;
					}
				}
				

                //Oilfloor
                if (hit.collider.tag == "OilFloor")
                {
                    gameObject.GetComponent<Player>().onOilFloor = true;
                }
                else
                {
                    gameObject.GetComponent<Player>().onOilFloor = false;
                }

				bool goThroughPipe = false;
                if (hit.collider.name == "TubeIn1") {
					gameObject.transform.position = GameObject.Find("TubeOut1").transform.position;
					goThroughPipe = true;
				}
				if (hit.collider.name == "TubeIn2") {
					gameObject.transform.position = GameObject.Find("TubeOut2").transform.position;
					goThroughPipe = true;
				}
				if (hit.collider.name == "TubeIn3") {
					gameObject.transform.position = GameObject.Find("TubeOut3").transform.position;
					goThroughPipe = true;
				}
				if (hit.collider.name == "TubeIn4") {
					gameObject.transform.position = GameObject.Find("TubeOut4").transform.position;
					goThroughPipe = true;
				}
				if (hit.collider.name == "TubeIn5") {
					gameObject.transform.position = GameObject.Find("TubeOut5").transform.position;
					goThroughPipe = true;
				}
				if (hit.collider.name == "TubeIn6") {
					gameObject.transform.position = GameObject.Find("TubeOut6").transform.position;
					goThroughPipe = true;
				}
				if (goThroughPipe && gameObject.GetComponent<Player>().myTurn) {
					//Debug.Log("GoThroughPipe");
					cam.GetComponent<CameraFollow>().caughtUpPipe = false;
					cam.GetComponent<CameraFollow>().canCatchUpPipe = false;
					cam.GetComponent<CameraFollow>().Invoke("ResetCatchUpPipe", 0.1f);
					turnIndicatorObj.GetComponent<TurnIndicatorNew>().subspriteLen = (turnIndicatorObj.GetComponent<TurnIndicatorNew>().subspriteLenOut * 0.3f);
					gameObject.transform.position = new Vector3(
						gameObject.transform.position.x + 1f,
						gameObject.transform.position.y,
						gameObject.transform.position.z
					);
					// play  sound
					if (soundManager != null) {
						soundManager.GetComponent<SoundManager>().PlayPipeSound();
					}
				}
				
		
				

                
				// collide with spring
				if (hit.collider.tag == "Spring") {
					if (hit.collider.transform.position.y < gameObject.transform.position.y) {
						gameObject.GetComponent<Player>().isSpring = true;
						gameObject.GetComponent<Player>().PlayQuickStompFallParticle();
						hit.collider.GetComponent<Spring>().TriggerSpringAnimation();
						//print("Stepped on the spring");
					}
				} else {
					gameObject.GetComponent<Player>().isSpring = false;
				}
				
                if (hit.collider.tag == "Player")
                {
                    //store the collider in playerBelow
                    if (hit.collider.transform.position.y < gameObject.transform.position.y)
                        gameObject.GetComponent<Player>().playerBelow = hit.collider.gameObject;
                    // collide with another player while they in the air
                    if (gameObject.GetComponent<Player>().isInAir)
                    {
                        if (gameObject.transform.position.y > hit.collider.transform.position.y)
                        {
                            if (hit.collider.GetComponent<Player>().isInAir)
                            {
                                gameObject.GetComponent<Player>().hitByAnotherPlayerInTheAir = true;
                            }
                        }
                    }
                    // Death effect and Score
                    if (gameObject.GetComponent<Player>().myTurn)
                    {
                        if (hit.collider.gameObject.transform.position.y < gameObject.transform.position.y && gameObject.GetComponent<Player>().isInAir && !cam.GetComponent<TurnSwitch>().nextTurnProcess)
                        {
                            cam.GetComponent<TurnSwitch>().killingHappening = true;
                            //Debug.Log("Killing is happening" + cam.GetComponent<TurnSwitch>().killingHappening + Random.value);
                            //	if (gameObject.GetComponent<Player>().isStomping) {
                            List<GameObject> deadPlayers = new List<GameObject>();
                            GameObject _playerBelow = hit.collider.gameObject;

                            int kills = 0;

                            while (_playerBelow.GetComponent<Player>().playerBelow)
                            {
                                deadPlayers.Add(_playerBelow);
                                _playerBelow = _playerBelow.GetComponent<Player>().playerBelow;
                                kills++;
                            }

                            deadPlayers.Add(_playerBelow);
                            kills++;
                            foreach (GameObject deadPlayer in deadPlayers)
                            {
                                //Debug.Log("Killed player being teleported" + Random.value);
                                deadPlayer.GetComponent<Player>().imDead = true;
                                deadPlayer.transform.position = new Vector3(9999f, 9999f);
                            }
                            gameObject.GetComponent<Player>().ShakeScore();
                            while (kills != 0)
                            {
                                gameObject.GetComponent<Player>().score++;
                                //Vibration
                                if (gameObject.GetComponent<Player>().playerInputObj.GetComponent<PlayerInput>().currentControlScheme == "Gamepads")
                                    gameObject.GetComponent<Player>().vibration(); 
                                kills--;
								Animator explodeAnimator = explodeObj.GetComponent<Animator>();
								explodeAnimator.SetTrigger("TriggerExplode");
								explodeObj.transform.position = gameObject.transform.position;
                            }
                            // Decrease turn length for player who scored
                            int playerIndex = gameObject.GetComponent<Player>().playerIndex;
							int charIndex = playerVarsObj.GetComponent<PlayerVars>().charSelect[playerIndex];
                            float timeLimitDecrement = cam.GetComponent<TurnSwitch>().timeLimitDecrement;
                            cam.GetComponent<TurnSwitch>().playerTimeLimit[playerIndex] -= timeLimitDecrement;

                            // screen shake effec0t
                            cam.GetComponent<CameraFollow>().shakePosAmount = 3;
                            cam.GetComponent<CameraFollow>().shakeRotAmount = 13;

                            // play stomp sound
                            if (soundManager != null)
                            {
                                soundManager.GetComponent<SoundManager>().PlayStompSound(charIndex);
                            }

                            //print(gameObject.name + "score: " + gameObject.GetComponent<Player>().score);
                            //}
                            cam.GetComponent<TurnSwitch>().killingHappening = false;
                            //Debug.Log("Killing is over" + cam.GetComponent<TurnSwitch>().killingHappening + Random.value);
                        }
                    }
                    if (hit.collider.GetComponent<Player>().onMovingPlatform)
                    {
                        if (hit.collider.gameObject.transform.position.y < gameObject.transform.position.y)
                        {
                            gameObject.GetComponent<Player>().onMovingPlatform = true;
                        }
                    }
                    else
                    {
                        gameObject.GetComponent<Player>().onMovingPlatform = false;
                    }
                    // maybe recursion here????
                    if (gameObject.GetComponent<Player>().onMovingPlatform)
                    {
                        if (hit.collider.GetComponent<Player>())
                        {
                            Player _playerBelow = hit.collider.GetComponent<Player>();
                            while (!_playerBelow.movingPlatform && _playerBelow.playerBelow)
                            {
                                _playerBelow = _playerBelow.playerBelow.GetComponent<Player>();
                            }
                            if (_playerBelow.movingPlatform != null)
                            {
                                velocity.x = _playerBelow.movingPlatform.GetComponent<PlatformController>().passengerMovement[0].velocity.x;
                                velocity.y = _playerBelow.movingPlatform.GetComponent<PlatformController>().passengerMovement[0].velocity.y + skinWidth;
                            }

                        }
                    }
                }
                //default collsion
                velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;
				// when hit something, that means stomp finished
				if (gameObject.GetComponent<Player>().isStomping) {
					gameObject.GetComponent<Player>().PlayQuickStompSlamParticle();
					soundManager.GetComponent<SoundManager>().PlayQuickStompSound();
				}
				gameObject.GetComponent<Player>().isStomping = false;
				if (collisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;

            }

		}
        // slope climbing
		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * directionX,rayLength,collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
        gameObject.GetComponent<Player>().onBelt = beltTest;
    }

	void ClimbSlope(ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	void ResetFallingThroughPlatform() {
		collisions.fallingThroughPlatform = false;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}
