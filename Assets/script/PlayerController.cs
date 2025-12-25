using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{
    public bool isZombie = false;
    public SpriteRenderer mySprite;

    [Header("UI Hiển thị")]
    public Slider staminaBar;
    public Text nameText; // <--- BIẾN MỚI: Để hiển thị tên

    public float maxStamina = 100f;
    public float currentStamina;

    void Start()
    {
        currentStamina = maxStamina;
        mySprite.color = Color.blue;

        // --- CODE MỚI: HIỂN THỊ TÊN ---
        if (photonView.Owner != null)
        {
            // Lấy tên từ mạng và gán vào Text
            nameText.text = photonView.Owner.NickName;
        }
        // ------------------------------

        if (!photonView.IsMine)
        {
            staminaBar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // Xử lý di chuyển
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0)
        {
            movement.moveSpeed = 10f;
            currentStamina -= 30f * Time.deltaTime;
        }
        else
        {
            movement.moveSpeed = 5f;
            if (currentStamina < maxStamina)
                currentStamina += 10f * Time.deltaTime;
        }
        staminaBar.value = currentStamina / maxStamina;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.isZombie == true && photonView.IsMine)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PhotonView targetView = collision.gameObject.GetComponent<PhotonView>();
                targetView.RPC("BienHinh", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void BienHinh()
    {
        isZombie = true;
        mySprite.color = Color.red;

        if (GameLogic.Instance != null)
        {
            GameLogic.Instance.KiemTraKetThuc();
        }
    }
}