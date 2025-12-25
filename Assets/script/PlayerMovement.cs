using UnityEngine;
using Photon.Pun; // <--- THÊM DÒNG NÀY

// Sửa MonoBehaviour thành MonoBehaviourPun
public class PlayerMovement : MonoBehaviourPun
{
    [Header("Cài đặt tốc độ")]
    public float moveSpeed = 10f;
    public float maxSpeed = 7f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
       
    }

    void Update()
    {
        // --- ĐOẠN QUAN TRỌNG NHẤT ---
        // Nếu đây KHÔNG phải là nhân vật của tôi -> Thì không làm gì cả
        if (photonView.IsMine == false) return;
        // -----------------------------

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void FixedUpdate()
    {
        // Cũng kiểm tra ở đây cho chắc, không phải của mình thì không đẩy lực
        if (photonView.IsMine == false) return;

        rb.AddForce(moveInput * moveSpeed, ForceMode2D.Force);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }
}