using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;

public partial class Player : MonoBehaviour
{
    #region Fields
    private bool _isFacingRight;
    private CharacterController2D _controller;
    private float _normalizedHorizontalSpeed;

    public float MaxSpeed = 8;
    public float SpeedAccelerationOnGround = 10f;
    public float SpeedAccelerationInAir = 5f;

    public Transform playerGraphics;

    private Animator anim;

    private GameManager gameManager;

    private HUDManager hudManager;

    #endregion
    #region Properties
    private bool _isInvulnerable = false;
    public bool IsInvulnerable
    {
        get { return _isInvulnerable; }
        private set { _isInvulnerable = value; }
    }
    private float _health = 100;
    public float Health
    {
        get { return _health; }
        private set { _health = value; }
    }

    private bool _hasControl = true;

    private bool HasControl
    {
        get { return _hasControl; }
        set { _hasControl = value; }
    }
    #endregion
    public void Awake()
    {
        playerGraphics = transform.FindChild("Graphics");
        anim = GetComponent<Animator>();
    }

    public void Start()
    {
        _controller = GetComponent<CharacterController2D>();
        _isFacingRight = playerGraphics.localScale.x > 0;

        gameManager = GameManager.instance;
        hudManager = HUDManager.instance;

    }

    public void Update()
    {
        HandleAnim();
        if (HasControl)
        {
            HandleInput();
            float movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;
            _controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));

        }
        JetpackUpdate();
    }

    private void HandleAnim()
    {
        anim.SetBool("Ground", _controller.State.IsGrounded);
        anim.SetFloat("Speed", Mathf.Abs(_controller.Velocity.x));
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _normalizedHorizontalSpeed = 1;
            if (!_isFacingRight)
            {
                Flip();
            }
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _normalizedHorizontalSpeed = -1;
            if (_isFacingRight)
            {
                Flip();
            }
        }
        else
        {
            _normalizedHorizontalSpeed = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_controller.CanJump)
            {
                _controller.Jump();
                GameManager.DelayedFunction(DefaultJetpackParameters.HoldSpaceJetpackDelay, () =>
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        StartJetpack();
                    }
                });
            }
            else
            {
                StartJetpack();
            }
        }
        else if (!Input.GetKey(KeyCode.Space))
        {
            StopJetpack();
        }
    }

    private void Flip()
    {
        playerGraphics.localScale = new Vector3(-playerGraphics.localScale.x, playerGraphics.localScale.y, playerGraphics.localScale.z);
        _isFacingRight = playerGraphics.localScale.x > 0;
    }



    public void TakeDamage(DamageInstance damageInstance)
    {
        if (IsInvulnerable)
        {
            return;
        }
        Health -= damageInstance.Damage;
        if (Health <= 0)
        {
            gameManager.OnPlayerDeath();
            return;
        }
        hudManager.HealthDisplay.SendMessage("SetDisplayedHealth", Health);
        if (damageInstance.TriggersInvulnerability)
        {
            IsInvulnerable = true;
            GameManager.DelayedFunction(gameManager.InvulnerabilityTime, () => IsInvulnerable = false);
            StartCoroutine(BlinkingMode(gameManager.InvulnerabilityTime));
        }


        if (damageInstance.TriggersKnockback)
        {
            if (damageInstance.SpecialKnockbackBehaviour != null)
            {
                damageInstance.SpecialKnockbackBehaviour(this);
                return;
            }
            HasControl = false;

            Action regainControl = () =>
            {
                HasControl = true;
            };

            DefaultKnockback(damageInstance);

            GameManager.DelayedFunction(gameManager.OutOfControlTime, regainControl);
        }
    }

    private void DefaultKnockback(DamageInstance damageInstance)
    {
        float strength = damageInstance.KnockBackStrength;
        _controller.SetHorizontalForce(-strength * 0.5f);
        _controller.SetVerticalForce(strength);
    }

    private IEnumerator BlinkingMode(float blinkingTime)
    {
        float singleBlinkTime = 0.1f;
        bool isVisible = true;
        while (blinkingTime > 0)
        {
            isVisible = !isVisible;
            setVisibilityRecursively(isVisible);

            blinkingTime -= singleBlinkTime;
            yield return new WaitForSeconds(singleBlinkTime);
        }
        setVisibilityRecursively(true);
    }

    private void setVisibilityRecursively(bool isVisible)
    {
        if (GetComponent<Renderer>() != null)
            GetComponent<Renderer>().enabled = isVisible;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Renderer>() != null)
                child.GetComponent<Renderer>().enabled = isVisible;
        }
    }

    public void SetHealth(float newHealth)
    {
        Health = newHealth;
    }


}
