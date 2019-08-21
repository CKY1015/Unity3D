using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    // 一個 GameObject，用於保存當前與之碰撞的觸發器（trigger），這樣你才能抓住這個對象
    private GameObject collidingObject;
    // 一個 GameObject，用於保存玩家當前抓住的對象。
    private GameObject objectInHand;

    private void SetCollidingObject(Collider col)
    {
        // 如果玩家已经抓著某些東西了，或者這個對象没有一個剛體，則不要將這個 GameObject 作為可以抓取目標。
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        // 可以抓取的目標
        collidingObject = col.gameObject;
    }

    // 當觸發器碰撞體進入另一個碰撞體時，將另一個碰撞體作為可以抓取的目標。
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // 和上面那段類似，但不同的是玩家已經將手柄放在一個對象上並持續一段時間。如果没有這段，碰撞會失敗或者會導致異常。
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // 當碰撞體退出一個對象，放棄目標，這段代碼會將 collidingObject 設為 null 以刪除目標對象。
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void GrabObject()
    {
        // 在玩家手中的 GameObject 轉移到 objectInHand 中，將 collidingObject 中保存的 GameObject 移除。
        objectInHand = collidingObject;
        collidingObject = null;
        // 添加一個連接對象，調用下面的 FixedJoint 方法將手柄和 GameObject 連接起来。
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    // 創建一個固定連接並加到手柄中，並設置連接屬性，使它堅固，不那麼容易斷裂。最後返回這個連接。
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        // 確定控制器上一定有一個固定連接
        if (GetComponent<FixedJoint>())
        {
            // 刪除這個連接上所連的對象，然後銷毀這個連接。
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // 將玩家放開物體時手柄的速度和角度給這個物體，這樣才會形成一個完美的拋物線，不會直直墜落。
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        // 將 objectInHand 變數置空。
        objectInHand = null;
    }
    // Update is called once per frame
    void Update () {
        // 當玩家按下板機，同時手上有一個可以抓取的對象，則將對象抓住。
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }

        // 當玩家鬆開板機，同時手柄上連接著一個物體，則放開這個物體。
        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
    }
}
