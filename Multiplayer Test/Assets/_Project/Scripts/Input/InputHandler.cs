using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Inputs")]
    public Vector2 move;
    //public bool use;
    //public bool primaryDown;
    //public bool secondaryDown;
    //public bool paused;
    //public bool menu;
    [Space]
    public bool isGamepad;

    public Action OnPrimaryPress = delegate { };
    public Action OnSecondaryPress = delegate { };
    public Action OnUsePress = delegate { };
    public Action OnPausePress = delegate { };
    public Action OnMenuPress = delegate { };

    public Action<bool> OnDeviceChanged = delegate { };

    private PlayerInput playerInput;

    private InputAction moveActon;
    private InputAction primaryAction;
    private InputAction secondaryAction;
    private InputAction useAction;
    private InputAction pauseAction;
    private InputAction menuAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        moveActon = InputSystem.actions.FindAction("Move");
        primaryAction = InputSystem.actions.FindAction("Primary");
        secondaryAction = InputSystem.actions.FindAction("Secondary");
        useAction = InputSystem.actions.FindAction("Use");
        pauseAction = InputSystem.actions.FindAction("Pause");
        menuAction = InputSystem.actions.FindAction("Menu");
    }
    private void OnEnable()
    {
        moveActon.performed += Move;
        moveActon.canceled += StopMove;
        primaryAction.performed += Primary;
        secondaryAction.performed += Secondary;
        useAction.performed += Use;
        pauseAction.performed += Pause;
        menuAction.performed += Menu;
    }
    private void OnDisable()
    {
        moveActon.performed -= Move;
        moveActon.canceled -= StopMove;
        primaryAction.performed -= Primary;
        secondaryAction.performed -= Secondary;
        useAction.performed -= Use;
        pauseAction.performed -= Pause;
        menuAction.performed -= Menu;
    }

    private void CheckControl(string current)
    {
        if (current.Equals("Keyboard") || current.Equals("Mouse"))
        {
            if(isGamepad == true)
            {
                isGamepad = false;
                OnDeviceChanged?.Invoke(isGamepad);
            }
        }
        else
        {
            if(isGamepad == false)
            {
                isGamepad = true;
                OnDeviceChanged?.Invoke(isGamepad);
            }
        }
    }

    private void Move(InputAction.CallbackContext context)
    {
        CheckControl(context.control.device.name);

        MoveInput(context.ReadValue<Vector2>());
        //move = context.ReadValue<Vector2>();
    }
    private void StopMove(InputAction.CallbackContext context)
    {
        CheckControl(context.control.device.name);

        MoveInput(context.ReadValue<Vector2>());
        //move = context.ReadValue<Vector2>();
    }
    private void Primary(InputAction.CallbackContext context)
    {
        CheckControl(context.control.device.name);

        bool good = context.ReadValueAsButton();
        PrimaryInput(good);

        //if (good) OnPrimaryPress?.Invoke();
    }
    private void Secondary(InputAction.CallbackContext context)
    {
        CheckControl(context.control.device.name);

        bool good = context.ReadValueAsButton();
        SecondaryInput(good);

        //if (good) OnSecondaryPress?.Invoke();
    }
    private void Use(InputAction.CallbackContext context)
    {
        CheckControl(context.control.device.name);

        bool good = context.ReadValueAsButton();
        UseInput(good);

        //if (good) OnUsePress?.Invoke();
    }
    private void Pause(InputAction.CallbackContext context)
    {
        CheckControl(context.control.device.name);

        bool good = context.ReadValueAsButton();
        PauseInput(good);

        //if (good) OnPausePress?.Invoke();
    }
    private void Menu(InputAction.CallbackContext context)
    {
        CheckControl(context.control.device.name);

        bool good = context.ReadValueAsButton();
        MenuInput(good);

        //if (good) OnMenuPress?.Invoke();
    }


    public void MoveInput(Vector2 newMove)
    {
        move = newMove;
    }
    public void PrimaryInput(bool newPrimary)
    {
        if (newPrimary) OnPrimaryPress?.Invoke();
    }
    public void SecondaryInput(bool newSecondary)
    {
        if (newSecondary) OnSecondaryPress?.Invoke();
    }
    public void UseInput(bool newUse)
    {
        if (newUse) OnUsePress?.Invoke();
    }
    public void PauseInput(bool newPause)
    {
        if (newPause) OnPausePress?.Invoke();
    }
    public void MenuInput(bool newMenu)
    {
        if (newMenu) OnMenuPress?.Invoke();
    }
}
