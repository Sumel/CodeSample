using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;

public class BasicAI : MonoBehaviour
{
    #region fields
    private Animator anim;
    private Dictionary<int, AIState> AIStateHash = new Dictionary<int, AIState>();
    protected Transform player;
    [SerializeField]
    protected EnemyParameters parameters;
    [SerializeField]
    protected LayerMask PlatformLayer;
    [SerializeField]
    protected float Health = 100;

    private CharacterController2D _controller;
    private bool lastDirWasRight = false;

    protected bool isCollidingWithPlayer = false;

    private BoxCollider2D _collider;
    protected AIState currentState = AIState.Idle;

    private bool ContactDamageOnCooldown = false;



    #endregion
    #region properties
    protected CharacterController2D Controller
    {
        get { return _controller; }
        set { _controller = value; }
    }

    protected bool IsDirectlyBelowPlayer
    {
        get
        {
            if (player == null)
                return false;
            float top = transform.position.y + (_collider.size.y * transform.localScale.y / 2.0f);
            if (player.position.y < top)
                return false;
            float center = transform.position.x;
            float halfSizeX = (_collider.size.x * transform.localScale.x) / 2.0f;
            float left = center - halfSizeX;
            float right = center + halfSizeX;
            if (player.position.x > left && player.position.x < right)
                return true;
            return false;
        }
    }

    protected Vector2 DirectionToPlayer
    {
        get
        {
            return (player.position - transform.position).normalized;
        }
    }

    protected float DistanceToPlayer
    {
        get
        {
            return (player.position - transform.position).magnitude;
        }
    }

    protected bool HasClearLineToPlayer
    {
        get
        {
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, DirectionToPlayer, DistanceToPlayer, PlatformLayer);
            return !rayHit;
        }

    }


    #endregion
    void Awake()
    {
        Controller = GetComponent<CharacterController2D>();
        _collider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        foreach (AIState state in (AIState[])System.Enum.GetValues(typeof(AIState)))
        {
            AIStateHash.Add(Animator.StringToHash("Base Layer." + state.ToString()), state);
        }
        anim.SetBool("chaseUntilDead", parameters.ChaseUntilDead);
    }
    protected virtual void Start()
    {
        player = GameManager.instance.PlayerGameObject.transform;
    }
    // Update is called once per frame
    public virtual void Update()
    {
        currentState = AIStateHash[anim.GetCurrentAnimatorStateInfo(0).nameHash];
        switch (currentState)
        {
            case AIState.Idle:
                StayIdle();
                break;
            case AIState.Chasing:
                TryChasing();
                break;
            case AIState.Fighting:
                TryFighting();
                break;
        }
    }

    #region health
    public virtual void TakeDamage(DamageInstance damageInstance)
    {
        Health -= damageInstance.Damage;
        onTakeDamage();
        if (Health <= 0)
        {
            OnDestroy();
            Destroy(gameObject);
            return;
        }
    }

    protected virtual void OnDestroy()
    {
        StopCoroutine("resetShotBoolean");
    }

    #endregion

    #region behaviour

    protected virtual void onTakeDamage()
    {
        anim.SetBool("shotByPlayer", true);
        StartCoroutine("resetShotBoolean");
    }

    private IEnumerator resetShotBoolean()
    {
        yield return new WaitForSeconds(parameters.AggroTimeAfterShot);
        anim.SetBool("shotByPlayer", false);
    }

    protected void turnToPlayer()
    {
        lastDirWasRight = player.transform.position.x > transform.position.x;
    }

    protected virtual void StayIdle()
    {
        float movementFactor = Controller.State.IsGrounded ? parameters.SpeedAccelerationOnGround : parameters.SpeedAccelerationInAir;
        Controller.SetHorizontalForce(Mathf.Lerp(Controller.Velocity.x, 0, Time.deltaTime * movementFactor));
    }

    protected virtual void TryChasing()
    {
        MoveHorizontallyTowardsPlayer(parameters.MaxSpeedChasing);
        if (Controller.CanEverJump)
        {
            bool isRight = player.transform.position.x > transform.position.y;
            if ((Controller.State.IsCollidingLeft && !isRight) || (Controller.State.IsCollidingRight && isRight) || IsDirectlyBelowPlayer)
            {
                Controller.TryJump();
            }
        }
    }

    protected virtual void TryFighting()
    {
        Attack();

        MoveHorizontallyTowardsPlayer(parameters.MaxSpeedFighting);

        if (Controller.CanEverJump)
        {
            bool isRight = player.transform.position.x > transform.position.y;
            if ((Controller.State.IsCollidingLeft && !isRight) || (Controller.State.IsCollidingRight && isRight) || IsDirectlyBelowPlayer)
            {
                Controller.TryJump();
            }
        }
    }

    protected virtual void Attack()
    {

    }

    protected virtual void TryDealingContactDamage()
    {
        if (!ContactDamageOnCooldown)
        {
            ContactDamageOnCooldown = true;
            DealContactDamage();
            StartCoroutine(ResetDamageCooldown());
        }
    }

    protected virtual void DealContactDamage()
    {
        player.SendMessage("TakeDamage", new DamageInstance(parameters.Damage, gameObject));
    }

    protected IEnumerator ResetDamageCooldown()
    {
        yield return (new WaitForSeconds(parameters.ContactDamageCooldown));
        ContactDamageOnCooldown = false;
    }

    #endregion
    #region  movement
    protected void MoveInLastDirection(float maxSpeed)
    {
        float normalizedHorizontalSpeed = lastDirWasRight ? 1 : -1;
        float movementFactor = Controller.State.IsGrounded ? parameters.SpeedAccelerationOnGround : parameters.SpeedAccelerationInAir;
        Controller.SetHorizontalForce(Mathf.Lerp(Controller.Velocity.x, normalizedHorizontalSpeed * maxSpeed, Time.deltaTime * movementFactor));
    }

    protected void MoveHorizontallyTowardsPlayer(float maxSpeed)
    {
        float movementFactor = Controller.State.IsGrounded ? parameters.SpeedAccelerationOnGround : parameters.SpeedAccelerationInAir;
        float normalizedHorizontalSpeed = (player.transform.position.x > transform.position.x) ? 1 : -1;
        Controller.SetHorizontalForce(Mathf.Lerp(Controller.Velocity.x, normalizedHorizontalSpeed * maxSpeed, Time.deltaTime * movementFactor));
        lastDirWasRight = player.transform.position.x > transform.position.x;
    }

    protected void StopMovementGradually()
    {
        float movementFactor = Controller.State.IsGrounded ? parameters.SpeedAccelerationOnGround : parameters.SpeedAccelerationInAir;
        Controller.SetHorizontalForce(Mathf.Lerp(Controller.Velocity.x, 0, Time.deltaTime * movementFactor));
    }

    #endregion
    #region utility
    protected bool isBetween(float numberToCheck, float lowerNumber, float higherNumber)
    {
        return numberToCheck >= lowerNumber && numberToCheck <= higherNumber;
    }

    #endregion


    #region trigger_responders
    public virtual void OnPlayerEnterAggro(Transform _player)
    {
        anim.SetBool("playerIsInsideTrigger", true);
    }

    public virtual void OnPlayerExitAggro(Transform _player)
    {
        anim.SetBool("playerIsInsideTrigger", false);
    }

    public virtual void OnPlayerEnterFighting(Transform _player)
    {
        anim.SetBool("playerInFightingRange", true);
    }

    public virtual void OnPlayerExitFighting(Transform _player)
    {
        anim.SetBool("playerInFightingRange", false);
    }

    public virtual void OnPlayerEnterDamage(Transform _player)
    {
        TryDealingContactDamage();
        isCollidingWithPlayer = true;
    }

    public virtual void OnPlayerExitDamage(Transform _player)
    {
        isCollidingWithPlayer = false;
    }

    public virtual void OnPlayerStayDamage(Transform _player)
    {
        TryDealingContactDamage();
    }

    #endregion
}
