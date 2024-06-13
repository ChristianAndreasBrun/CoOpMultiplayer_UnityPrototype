using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.SceneManagement;


public enum AnimatorValueType { Float, Int, Bool, Trigger }

namespace Photon.Pun.Demo.PunBasics
{
    public class PlayerControl: MonoBehaviourPunCallbacks, IPunObservable
    {
        public CharacterController control;
        public float speed, rotationSpeed, gravity, jumpForce;

        [Header("Values")]
        public int life;
        int initLife;
        int deadCount;

        [Header("UI")]
        public GameObject otherCanvas;
        public GameObject mineCanvas;
        public Image otherLifebar;
        public Image mineLifebar;
        public TextMeshProUGUI otherUsername;
        public TextMeshProUGUI mineLifeValue;
        public List<WeaponManager> allWeapons;
        int currentWeapon;

        Vector3 moveDir;

        Vector3 currentPos;
        Quaternion currentRot;

        Vector3 initSpawnPos;
        Quaternion initSpawnRot;

        bool setRespawn = false;
        public Transform character;
        public Animator anim;
        bool jump;

        int maxDeaths = 3;
        int currentDeaths = 0;
        public TextMeshProUGUI deathsText;


        void Start()
        {
            string getSkin = (string)photonView.Owner.CustomProperties["Skin"];
            GameObject getCharacter = Resources.Load<GameObject>(getSkin);
            if (getCharacter != null)
            {
                GameObject newCharacter = Instantiate(getCharacter, character);
                SetAnimator(newCharacter.GetComponent<Animator>());
            }

            if (photonView.IsMine)
            {
                // ! - Es mi PLAYER

                initLife = life;
                otherCanvas.SetActive(false);
                Camera.main.GetComponent<CameraControl>().SetTarget(transform);

                initSpawnPos = transform.position;
                initSpawnRot = transform.rotation;
            }
            else
            {
                // ! - Es el otro PLAYER

                otherUsername.text = photonView.Owner.NickName;
                mineCanvas.SetActive(false);
            }
        }

        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                SceneManager.LoadScene(2);
            }

            if (photonView.IsMine)
            {
                // ! - Es mi PLAYER

                if (Input.GetKeyDown(KeyCode.Alpha1) && allWeapons.Count >= 1) currentWeapon = 0;
                else if (Input.GetKeyDown(KeyCode.Alpha1) && allWeapons.Count >= 2) currentWeapon = 1;
                else if (Input.GetKeyDown(KeyCode.Alpha1) && allWeapons.Count >= 3) currentWeapon = 2;
                else if (Input.GetKeyDown(KeyCode.Alpha1) && allWeapons.Count >= 4) currentWeapon = 3;

                if (allWeapons.Count > currentWeapon)
                {
                    allWeapons[currentWeapon].UpdateWeapon();
                }
                
                transform.Rotate(Vector3.up * rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime);

                moveDir = new Vector3(Input.GetAxis("Horizontal") * speed, moveDir.y, Input.GetAxis("Vertical") * speed);
                Vector3 finalSpeedAnim = moveDir;
                finalSpeedAnim.y = 0;
                float finalSpeed = (finalSpeedAnim.magnitude > 0) ? 1 : 0;

                SetValueAnimator("Speed", finalSpeed);
                SetValueAnimator("isGrounded", control.isGrounded);

                moveDir = transform.TransformDirection(moveDir);    //-> Transforma de global a local

                if (control.isGrounded)
                {
                    if (Input.GetButton("Jump"))
                    {
                        moveDir.y = jumpForce;
                        SetValueAnimator("Jump");
                        jump = true;
                    }
                }
                else
                {
                    moveDir.y -= gravity * Time.deltaTime;
                }
            }
            else
            {
                // ! - Es el otro PLAYER

                transform.position = Vector3.Lerp(transform.position, currentPos, 4 * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, currentRot, 4 * Time.deltaTime);

                if (setRespawn)
                {
                    transform.position = currentPos;
                    transform.rotation = currentRot;
                }
            }
        }

        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                // ! - Es mi PLAYER

                control.Move(moveDir * Time.deltaTime);  //-> manda el movimiento al controlador
            }
            else
            {
                // ! - Es el otro PLAYER

            }
        }


        void AddWeapon(WeaponManager weapon)
        {
            if (allWeapons.Count >= 4)
            {
                allWeapons.RemoveAt(0);
            }

            WeaponManager newWeapon = Instantiate(weapon);
            newWeapon.InitWeapon(transform);
            allWeapons.Add(newWeapon);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Weapon"))
            {
                if (photonView.IsMine)
                {
                    WeaponManager getWeapon = other.GetComponent<WeaponItem>().weapon;
                    if (allWeapons.Find(x => x.id == getWeapon.id) != null) return;
                    AddWeapon(getWeapon);
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(other.gameObject);
                }
            }
        }

 
        public void SetDamage(GameObject objective, int damage)
        {
            objective.GetComponent<PhotonView>().RPC(nameof(RPC_GetDamage), RpcTarget.Others, damage);
        }

        [PunRPC]
        public void RPC_GetDamage(int damage)
        {
            GetDamage(damage);
        }

        public void GetDamage(int damage)
        {
            life -= damage;
            if (life <= 0)
            {
                life = 0;
                deadCount++;
                setRespawn = true;
                transform.position = initSpawnPos;
                transform.rotation = initSpawnRot;
                life = initLife;

                currentDeaths++;

                int remainDeaths = maxDeaths - currentDeaths;
                deathsText.text = remainDeaths.ToString();

                if (currentDeaths >= maxDeaths)
                {
                    PhotonNetwork.LoadLevel(2);
                }
            }

            float percentLife = (float)life / (float)initLife;
            otherLifebar.fillAmount = percentLife;
            mineLifebar.fillAmount = percentLife;
            mineLifeValue.text = life.ToString();
        }

      

        public void SetAnimator(Animator anim)
        {
            this.anim = anim;
        }

        public void SetValueAnimator(string propertyName, float value)
        {
            if (anim != null) anim.SetFloat(propertyName, value);
        }
        public void SetValueAnimator(string propertyName, int value)
        {
            if (anim != null) anim.SetInteger(propertyName, value);
        }
        public void SetValueAnimator(string propertyName, bool value)
        {
            if (anim != null) anim.SetBool(propertyName, value);
        }
        public void SetValueAnimator(string propertyName)
        {
            if (anim != null) anim.SetTrigger(propertyName);
        }

        public float GetFlaotAnimator(string propertyName)
        {
            if (anim != null) return anim.GetFloat(propertyName);
            return 0;
        }
        public int GetIntAnimator(string propertyName)
        {
            if (anim != null) return anim.GetInteger(propertyName);
            return 0;
        }
        public bool GetBoolAnimator(string propertyName)
        {
            if (anim != null) return anim.GetBool(propertyName);
            return false;
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // ! ENVIO

                stream.SendNext(life);
                stream.SendNext(otherLifebar.fillAmount);
                stream.SendNext(setRespawn);
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(deadCount);
                setRespawn = false;

                stream.SendNext(GetFlaotAnimator("Speed"));
                stream.SendNext(GetBoolAnimator("isGrounded"));
                stream.SendNext(jump);
                jump = false;
            }
            else
            {
                // ! RECIBO

                life = (int)stream.ReceiveNext();
                otherLifebar.fillAmount = (float)stream.ReceiveNext();
                setRespawn = (bool)stream.ReceiveNext();
                currentPos = (Vector3)stream.ReceiveNext();
                currentRot = (Quaternion)stream.ReceiveNext();
                deadCount = (int)stream.ReceiveNext();

                float speedAnim = (float)stream.ReceiveNext();
                SetValueAnimator("Speed", speedAnim);
                bool isGroundedAnim = (bool)stream.ReceiveNext();
                SetValueAnimator("isGrounded", isGroundedAnim);
                bool isJumping = (bool)stream.ReceiveNext();
                if (isJumping) SetValueAnimator("Jump");
            }
        }
    }
}

