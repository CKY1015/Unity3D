using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    // 這個變量用於引用 Laser prefab
    public GameObject laserPrefab;
    // 這個變量用於引用一個 Laser 實例
    private GameObject laser;
    // 一個 Transform 組件，方便後面使用。
    private Transform laserTransform;
    // Laser 擊中的位置
    private Vector3 hitPoint;

    // 這是 [CameraRig] 的 transform 組件
    public Transform cameraRigTransform;
    // 一個對傳送標記Prefab的引用
    public GameObject teleportReticlePrefab;
    // 一個傳送標記實力的引用
    private GameObject reticle;
    // 一個傳送標記的 transform 的引用
    private Transform teleportReticleTransform;
    // 玩家的頭（攝影機）的引用
    public Transform headTransform;
    // 標記距離地板的偏移，以防止和其他平面發生“z-緩衝衝突”
    public Vector3 teleportReticleOffset;
    // 一個遮罩，用於過濾這個地方允許什麼東西傳送
    public LayerMask teleportMask;
    // 如果為 true，表明找到一個有效的傳送點
    private bool shouldTeleport;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    void Start()
    {
        // 製造出一束新的激光，然後保存一個它的引用
        laser = Instantiate(laserPrefab);
        // 保存激光的 transform 組件
        laserTransform = laser.transform;

        // 創建一個標記點，並儲存到reticle的變量中
        reticle = Instantiate(teleportReticlePrefab);
        // 保存reticle的 transform 組件
        teleportReticleTransform = reticle.transform;
    }
    void Update()
    {
        // 如果板機被按下
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad) )
        {
            RaycastHit hit;
            // 如果laser照到某個東西，儲存位置和顯示laser
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                //碰撞點
                hitPoint = hit.point;
                ShowLaser(hit);
                // 顯示傳送標記
                reticle.SetActive(true);
                // 移動標記到laser的地方，並添加一個偏移以免 z 緩衝衝突
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                // 表明找到了一個有效的瞬移位置
                shouldTeleport = true;
            }
        }
        else // 放開板機laser和標記就消失
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }

        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
        {
            Teleport();
        }

    }

    private void ShowLaser(RaycastHit hit)
    {
        // 顯示laser
        laser.SetActive(true);
        // laser位於手柄和投射點之間
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        // 將laser照到hitPoint的位置
        laserTransform.LookAt(hitPoint);
        // 調整laser的大小
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    private void Teleport()
    {
        // 表明傳送進行中
        shouldTeleport = false;
        // 隱藏傳送標記
        reticle.SetActive(false);
        // 計算距離
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        // 頭部高度不算
        difference.y = 0;
        // 如果不加上這個偏移，玩家會傳送到一個錯誤的地方
        cameraRigTransform.position = hitPoint + difference;
    }
}
