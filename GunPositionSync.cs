using UnityEngine;
using UnityEngine.Networking;

public class GunPositionSync : NetworkBehaviour {

    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform handMount;
    [SerializeField] Transform gunPivot;
    [SerializeField] Transform rightHandHold;
    [SerializeField] Transform leftHandHold;
    [SerializeField] float threshold = 10f;     //will be 10 degree off at max
    [SerializeField] float smoothing = 5f;

    [SyncVar] float pitch;
    Vector3 lastOffset;
    float lastSynchedPitch;
    Animator anim;      //local off of pitch

    private void Start()
    {
        anim = GetComponent<Animator>();

        if(isLocalPlayer)
        {
            gunPivot.parent = cameraTransform;
        }
        else
        {
            lastOffset = handMount.position - transform.position;
        }
    }

    private void Update()
    {
        if(isLocalPlayer)
        {
            pitch = cameraTransform.localRotation.eulerAngles.x;        //yields x rotation (up/down) of guys head
            if(Mathf.Abs(lastSynchedPitch-pitch)>=threshold)            //if moved enough--> sync
            {
                //CMD
                CmdUpdatePitch(pitch);
                lastSynchedPitch = pitch;                               //last time u updated
            }
        }
        else
        {
            Quaternion newRotation = Quaternion.Euler(pitch, 0f, 0f);

            Vector3 curretOffset = handMount.position - transform.position;     //bobs
            gunPivot.localPosition += curretOffset - lastOffset;
            lastOffset = curretOffset;

            gunPivot.localRotation = Quaternion.Lerp(gunPivot.localRotation, newRotation, Time.deltaTime * smoothing);      //smoothly lerp
        }
    }

    [Command]
    void CmdUpdatePitch(float newPitch)
    {
        pitch = newPitch;       //tells server to update
    }

    private void OnAnimatorIK(int layerIndex)           //works from hand in
    {
        if (!anim)
            return;
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandHold.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandHold.rotation);

        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandHold.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandHold.rotation);
    }
}
