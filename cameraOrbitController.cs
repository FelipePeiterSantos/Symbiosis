using UnityEngine;
using System.Collections;

public class cameraOrbitController : MonoBehaviour 
{
	public Transform player;
    public Camera cameraTool;
    public RectTransform lockTargetFeed;
	
	public Vector3 pivotOffset = new Vector3(0.0f, 1.0f,  0.0f);
	public Vector3 camOffset   = new Vector3(0.0f, 0.7f, -3.0f);

	public float smooth = 10f;

	public Vector3 aimPivotOffset = new Vector3(0.0f, 1.7f,  -0.3f);
	public Vector3 aimCamOffset   = new Vector3(0.8f, 0.0f, -1.0f);
    public Vector3 aimGunPivotOffset = new Vector3(0.0f, 1.7f, -0.3f);
    public Vector3 aimGunCamOffset = new Vector3(0.8f, 0.0f, -1.0f);

    public float horizontalAimingSpeed = 400f;
	public float verticalAimingSpeed = 400f;
	public float maxVerticalAngle = 30f;
	public float flyMaxVerticalAngle = 60f;
	public float minVerticalAngle = -60f;
	
	public float mouseSensitivity = 0.3f;

	public float aimFov = 100f;
	
	private bool playerControl;
    private bool playerAim;
	private float angleH = 0;
	private float angleV = 0;
	private Transform cam;
    private Transform lockEnemy;

	private float relCameraPosMag;
	
    Quaternion aimRotation;
	Quaternion camYRotation;
    Vector3 baseTempPosition;
	Vector3 tempOffset;
	private Vector3 smoothPivotOffset;
	private Vector3 smoothCamOffset;
	private Vector3 targetPivotOffset;
	private Vector3 targetCamOffset;

	private float defaultFOV;
	private float targetFOV;
    private float distanceTargetAngleCam;

	void Start(){
        angleH = transform.eulerAngles.y;
        angleV = -transform.eulerAngles.x;

		cam = transform;
        playerControl = true;
        playerAim = false;

		smoothPivotOffset = pivotOffset;
		smoothCamOffset = camOffset;

		defaultFOV = cameraTool.fieldOfView;
    }

	void FixedUpdate()
	{

        if (playerAim){
            playerControl = false;
            angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * (horizontalAimingSpeed / 2) * Time.deltaTime;
            angleV += Mathf.Clamp(-Input.GetAxis("Mouse Y"), -1, 1) * (verticalAimingSpeed / 2) * Time.deltaTime;
            angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);
            angleH = angleH % 360;
            if (angleH < 0){
                angleH += 360;
            }

            aimRotation = Quaternion.Euler(-angleV, angleH, 0);
            camYRotation = Quaternion.Euler(0, angleH, 0);
            cam.rotation = aimRotation;

            targetPivotOffset = aimGunPivotOffset;
            targetCamOffset = aimGunCamOffset;
            targetFOV = aimFov;

            if(relCameraPosMag != 1.431f) {
                relCameraPosMag = 1.431f;
            }
            if(lockEnemy) {
                lockEnemy = null;
            }
        }
        else if (playerControl) {
            playerAim = false;
            angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * (horizontalAimingSpeed / 2) * Time.deltaTime;
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) {
                angleH += Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1) * horizontalAimingSpeed / 5 * Time.deltaTime;
            }
            angleV += Mathf.Clamp(-Input.GetAxis("Mouse Y"), -1, 1) * (verticalAimingSpeed / 4) * Time.deltaTime;
            angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);
            angleH = angleH % 360;
            if (angleH < 0) {
                angleH += 360;
            }

            aimRotation = Quaternion.Euler(-angleV, angleH, 0);
            camYRotation = Quaternion.Euler(0, angleH, 0);
            cam.rotation = aimRotation;

            targetPivotOffset = pivotOffset;
            targetCamOffset = camOffset;
            targetFOV = defaultFOV;

            if(relCameraPosMag != 6.396f) {
                relCameraPosMag = 6.396f;
            }

        }
        else if (lockEnemy != null) {
            distanceTargetAngleCam = Mathf.Clamp(Vector3.Distance(player.position, lockEnemy.position), 3, 8);
            distanceTargetAngleCam = (distanceTargetAngleCam - 3) / 5;
            Quaternion lockEnemyCamera = Quaternion.LookRotation(lockEnemy.position - transform.position);
            Quaternion LEC_Lerp = Quaternion.Slerp(aimRotation, lockEnemyCamera, 10f * Time.deltaTime);

            if (-LEC_Lerp.eulerAngles.x >= minVerticalAngle) {
                angleV = -LEC_Lerp.eulerAngles.x;
            }
            else {
                angleV = -LEC_Lerp.eulerAngles.x + 360;
            }
            angleV = Mathf.Clamp(angleV, minVerticalAngle * distanceTargetAngleCam, maxVerticalAngle);
            angleH = LEC_Lerp.eulerAngles.y;
            angleH = angleH % 360;
            if (angleH < 0) {
                angleH += 360;
            }

            aimRotation = Quaternion.Euler(-angleV, angleH, 0);
            camYRotation = Quaternion.Euler(0, angleH, 0);
            cam.rotation = aimRotation;

            targetPivotOffset = aimPivotOffset;
            targetCamOffset = aimCamOffset;
            targetFOV = defaultFOV;
            if(relCameraPosMag != 6.396f) {
                relCameraPosMag = 6.396f;
            }
            
        }
        else {
            playerControl = true;
            playerAim = false;
        }
        
        cameraTool.fieldOfView = Mathf.Lerp (cameraTool.fieldOfView, targetFOV,  smooth * Time.deltaTime);
		smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
		smoothCamOffset = Vector3.Lerp(smoothCamOffset, targetCamOffset, smooth * Time.deltaTime);
        baseTempPosition = (Vector3.zero - (Vector3.up * pivotOffset.y)) + camYRotation * targetPivotOffset + aimRotation * targetCamOffset;
        cam.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
        RaycastHit hit;
        if (Physics.Raycast(player.position + (Vector3.up * pivotOffset.y), baseTempPosition,out hit ,relCameraPosMag, 1 << LayerMask.NameToLayer("Default"))) {
            cam.position = hit.point - ((Vector3.zero-Vector3.up*5f) + camYRotation * targetPivotOffset + aimRotation * targetCamOffset)/10f;
        }      
	}

    void LateUpdate() {
        if (lockEnemy != null && !playerControl && !playerAim) {
            if(!lockTargetFeed.gameObject.activeSelf) {
                lockTargetFeed.gameObject.SetActive(true);
            }
            Vector3 lockFeed = cameraTool.WorldToScreenPoint(lockEnemy.position);
            lockTargetFeed.localPosition = new Vector3(lockFeed.x - (Screen.width/2), lockFeed.y - (Screen.height/2), 0);
        }
        else if(lockTargetFeed.gameObject.activeSelf) {
            lockTargetFeed.gameObject.SetActive(false);
        }
        
    }

    public void LockCamera(bool active, Transform target) {
        playerControl = active;
        Transform[] children = target.GetComponentsInChildren<Transform>();
        foreach (Transform item in children){
            if (item.name == "target") {
                lockEnemy = item;
            }
        }
    }

    public void AimCamera(bool active){
        playerAim = active;
    }
}
