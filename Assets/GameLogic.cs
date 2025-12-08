using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI; // Thư viện để chỉnh sửa Text UI
using System.Collections;

public class GameLogic : MonoBehaviourPunCallbacks
{
    // Singleton: Giúp các script khác gọi được GameLogic dễ dàng
    public static GameLogic Instance;

    [Header("Gán UI vào đây")]
    public Text thongBaoText; // Ô chứa cái Text bạn vừa tạo

    void Awake()
    {
        Instance = this;
    }

    // --- LOGIC 1: BẮT ĐẦU GAME ---
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(DemNguocChonMa());
            }
        }
    }

    IEnumerator DemNguocChonMa()
    {
        photonView.RPC("CapNhatThongBao", RpcTarget.All, "Sắp có Ma xuất hiện...");
        yield return new WaitForSeconds(5f); // Chờ 5 giây
        photonView.RPC("CapNhatThongBao", RpcTarget.All, ""); // Tắt thông báo

        // Chọn Ma ngẫu nhiên
        int randomActorNumber = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
        Player zombieDuocChon = PhotonNetwork.PlayerList[randomActorNumber];

        // Gửi lệnh chọn Ma
        photonView.RPC("ChonMaDauTien", RpcTarget.All, zombieDuocChon);
    }

    [PunRPC]
    void CapNhatThongBao(string noiDung)
    {
        if (thongBaoText != null) thongBaoText.text = noiDung;
    }

    [PunRPC]
    void ChonMaDauTien(Player zombiePlayer)
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in allPlayers)
        {
            PhotonView pView = p.GetComponent<PhotonView>();
            PlayerController pCtrl = p.GetComponent<PlayerController>();

            if (pView == null || pCtrl == null) continue;

            if (pView.Owner == zombiePlayer)
            {
                pCtrl.BienHinh(); // Biến thành Ma
                break;
            }
        }
    }

    // --- LOGIC 2: KIỂM TRA THẮNG THUA (MỚI) ---
    public void KiemTraKetThuc()
    {
        // Chỉ chủ phòng mới có quyền kiểm tra để tránh xung đột
        if (!PhotonNetwork.IsMasterClient) return;

        int tongSoNguoiChoi = PhotonNetwork.CurrentRoom.PlayerCount;
        int soLuongMa = 0;

        // Tìm tất cả Player trong game để đếm Ma
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            PlayerController ctrl = p.GetComponent<PlayerController>();
            if (ctrl != null && ctrl.isZombie == true)
            {
                soLuongMa++;
            }
        }

        Debug.Log("Tổng: " + tongSoNguoiChoi + " - Số Ma: " + soLuongMa);

        // ĐIỀU KIỆN THẮNG: Số Ma = Tổng số người chơi - 1 (Nghĩa là chỉ còn 1 người sót lại)
        // (Hoặc nếu lỡ bị bắt hết thì Reset luôn)
        if (soLuongMa >= tongSoNguoiChoi - 1 && tongSoNguoiChoi > 1)
        {
            photonView.RPC("KetThucGame", RpcTarget.All);
        }
    }

    [PunRPC]
    void KetThucGame()
    {
        Debug.Log("Game Over!");
        // Hiện thông báo
        if (thongBaoText != null)
            thongBaoText.text = "GAME OVER!\nĐang chơi lại...";

        // Chờ 3 giây rồi Reset
        StartCoroutine(ResetGameDelay());
    }

    IEnumerator ResetGameDelay()
    {
        yield return new WaitForSeconds(3f);

        // Chỉ chủ phòng mới được lệnh load lại màn
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(0); // Load lại Scene đầu tiên
        }
    }
}