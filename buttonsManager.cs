using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class buttonsManager : MonoBehaviour {
    public Camera cameraMenu;
    public CanvasScaler canvScale;
    public menuManager scriptMenu;
    public Vector3 menuPos;
    public SpriteRenderer[] mainBtns;
    public SpriteRenderer[] creditBtn;
    public SpriteRenderer[] buttons;
    public Transform[] bounds;
    Vector2 screenSize;
    Vector2[] bnds;
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    enum Inputs {mouse, other };
    Inputs inputMenu;

    int buttonSelect;
    int menuSelect;

    Vector3 mousePos;
    bool axisDown;

    void Awake() {
        inputMenu = Inputs.mouse;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        buttonSelect = 0;
        menuSelect = 0;
        mousePos = Input.mousePosition;
        transform.position = cameraMenu.ViewportToWorldPoint(menuPos);
        //menuPos = cameraMenu.WorldToViewportPoint(transform.position);
        bnds = new Vector2[bounds.Length];    
        canvScale.referenceResolution = new Vector2(Screen.width,Screen.height);    
    }

    void Update() {
        //Scale Bounds Button
        if(screenSize.x != Screen.width || screenSize.y != Screen.height) {
            screenSize = new Vector2(Screen.width,Screen.height);
            for (int i = 0; i < bounds.Length; i++){
                bnds[i] = cameraMenu.WorldToScreenPoint(bounds[i].position);
            }
        }

        //Input Detect
        if (Input.anyKeyDown) {
            foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode))){
                if (Input.GetKeyDown(kcode)) {
                    if(kcode.ToString().Contains("Mouse")) {
                        inputMenu = Inputs.mouse;
                        if(!Cursor.visible) {
                            Cursor.visible = true;
                        }
                        
                    }
                    else {
                        inputMenu = Inputs.other;
                        if(Cursor.visible) {
                            Cursor.visible = false;
                        }
                    }
                }
            }
            mousePos = Input.mousePosition;
        }
        else if(Input.GetAxisRaw("Vertical") != 0) {
            inputMenu = Inputs.other;
            mousePos = Input.mousePosition;
            if(Cursor.visible) {
                Cursor.visible = false;
            }
        }
        else if(Input.GetAxisRaw("Vertical Dpad") != 0) {
            inputMenu = Inputs.other;
            mousePos = Input.mousePosition;
            if(Cursor.visible) {
                Cursor.visible = false;
            }
        }
        else if(mousePos != Input.mousePosition && inputMenu != Inputs.mouse) {
            inputMenu = Inputs.mouse;
            mousePos = Input.mousePosition;
            if(!Cursor.visible) {
                Cursor.visible = true;
            }
        }

        //Mouse Input
        if(inputMenu == Inputs.mouse) {
            if(menuSelect == 0) {
                if(Input.mousePosition.y > bnds[1].y && Input.mousePosition.y < bnds[0].y && Input.mousePosition.x < bnds[1].x && Input.mousePosition.x > bnds[0].x) {
                    if(buttonSelect != 1) {
                        buttonSelect = 1;
                        Audio_ButtonSelect(0);
                    }
                }
                else if(Input.mousePosition.y > bnds[3].y && Input.mousePosition.y < bnds[2].y && Input.mousePosition.x < bnds[3].x && Input.mousePosition.x > bnds[2].x) {
                    if(buttonSelect != 2) {
                        buttonSelect = 2;
                        Audio_ButtonSelect(0);
                    }
                }
                else if(Input.mousePosition.y > bnds[5].y && Input.mousePosition.y < bnds[4].y && Input.mousePosition.x < bnds[5].x && Input.mousePosition.x > bnds[4].x) {
                    if(buttonSelect != 3) {
                        buttonSelect = 3;
                        Audio_ButtonSelect(0);
                    }
                }
                else if(buttonSelect != 0) {
                    buttonSelect = 0;
                }
            }
            else if(menuSelect == 1) {
                if(Input.mousePosition.y > bnds[7].y && Input.mousePosition.y < bnds[6].y && Input.mousePosition.x < bnds[7].x && Input.mousePosition.x > bnds[6].x) {
                    if(buttonSelect != 4) {
                        buttonSelect = 4;
                        Audio_ButtonSelect(0);
                    }
                }
                else if(buttonSelect != 0) {
                    buttonSelect = 0;
                }
            }
            

            if (Input.GetMouseButtonDown(0)) {
                if(buttonSelect == 1) {
                    Audio_ButtonSelect(1);
                    scriptMenu.OnStartButton();
                }
                else if(buttonSelect == 2) {
                    Audio_ButtonSelect(1);
                    scriptMenu.OnCreditsButton();
                    menuSelect = 1;
                }
                else if(buttonSelect == 3) {
                    Audio_ButtonSelect(1);
                    scriptMenu.OnExitButton();
                }
                else if(buttonSelect == 4) {
                    Audio_ButtonSelect(1);
                    scriptMenu.OnBackButton();
                    menuSelect = 0;
                }
            }
        }
        //Keyboard/Controller Input
        else if(inputMenu == Inputs.other) {
            if(menuSelect == 0) {
                if((Input.GetAxisRaw("Vertical") < 0 && !axisDown)) {
                    axisDown = true;
                    buttonSelect++;
                    if(buttonSelect > 3) {
                        buttonSelect = 1;
                    }
                    Audio_ButtonSelect(0);
                }
                else if((Input.GetAxisRaw("Vertical") > 0 && !axisDown)) {
                    axisDown = true;
                    buttonSelect--;
                    if(buttonSelect < 1) {
                        buttonSelect = 3;
                    }
                    Audio_ButtonSelect(0);
                }
                else if((Input.GetAxisRaw("Vertical Dpad") < 0 && !axisDown)) {
                    axisDown = true;
                    buttonSelect++;
                    if(buttonSelect > 3) {
                        buttonSelect = 1;
                    }
                    Audio_ButtonSelect(0);
                }
                else if((Input.GetAxisRaw("Vertical Dpad") > 0 && !axisDown)) {
                    axisDown = true;
                    buttonSelect--;
                    if(buttonSelect < 1) {
                        buttonSelect = 3;
                    }
                    Audio_ButtonSelect(0);
                }
                else if((Input.GetAxisRaw("Vertical") == 0) && (Input.GetAxisRaw("Vertical Dpad") == 0)) {
                    axisDown = false;
                }
            }
            

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Joystick1Button7) || Input.GetKeyDown(KeyCode.Joystick1Button0)) {
                if(buttonSelect == 1) {
                    Audio_ButtonSelect(1);
                    scriptMenu.OnStartButton();
                }
                else if(buttonSelect == 2) {
                    Audio_ButtonSelect(1);
                    scriptMenu.OnCreditsButton();
                    menuSelect = 1;
                }
                else if(buttonSelect == 3) {
                    Audio_ButtonSelect(1);
                    scriptMenu.OnExitButton();
                }
                else if(buttonSelect == 4) {
                    Audio_ButtonSelect(1);
                    scriptMenu.OnBackButton();
                    menuSelect = 0;
                }
            }
            
            if(buttonSelect == 0 && menuSelect == 0) {
                buttonSelect = 1;
            }
            else if(buttonSelect == 4 && menuSelect == 0) {
                buttonSelect = 2;
            }
            else if(buttonSelect != 4 && menuSelect == 1) {
                buttonSelect = 4;
            }
        }

        //Fade Buttons
        if(buttonSelect == 0) {
            for (int i = 0; i < buttons.Length; i++){
                if(buttons[i].color.a > 0) {
                    buttons[i].color -= new Color(0,0,0,4f*Time.deltaTime);
                }
            }
        }
        else if(buttonSelect != 0) {
            for (int i = 0; i < buttons.Length; i++){
                if(i == buttonSelect-1) {
                    if(buttons[i].color.a < 1) {
                        buttons[i].color += new Color(0,0,0,2f*Time.deltaTime);
                    }
                }
                else {
                    if(buttons[i].color.a > 0) {
                        buttons[i].color -= new Color(0,0,0,4f*Time.deltaTime);
                    }
                }
            }
        }

        //Hide Buttons Menu Select
        if(menuSelect == 0) {
            if(!mainBtns[0].enabled || creditBtn[0].enabled) {
                for (int i = 0; i < mainBtns.Length; i++){
                    mainBtns[i].enabled = true;
                }
                for (int i = 0; i < creditBtn.Length; i++){
                    creditBtn[i].enabled = false;
                }
            }
        }
        else if(menuSelect == 1) {
            if(mainBtns[0].enabled || !creditBtn[0].enabled) {
                for (int i = 0; i < mainBtns.Length; i++){
                    mainBtns[i].enabled = false;
                }
                for (int i = 0; i < creditBtn.Length; i++){
                    creditBtn[i].enabled = true;
                }
            }
        }
    }

    void Audio_ButtonSelect(int aux) {
        if (aux == 0) {
            audioSource.clip = audioClips[0];
            audioSource.pitch = UnityEngine.Random.Range(1f,1.5f);
        }
        else if(aux == 1) {
            audioSource.clip = audioClips[1];
            audioSource.pitch = UnityEngine.Random.Range(1f,1.5f);
        }
        audioSource.Play();
    }
}

public static class RectTransformExtension {
   
    public static Rect GetScreenRect(this RectTransform rectTransform, Canvas canvas) {
        
        Vector3[] corners = new Vector3[4];
        Vector3[] screenCorners = new Vector3[2];
 
        rectTransform.GetWorldCorners(corners);
 
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
        {
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
        }
        else
        {
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
        }
 
        screenCorners[0].y = Screen.height - screenCorners[0].y;
        screenCorners[1].y = Screen.height - screenCorners[1].y;
 
        return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
    }
 
}
