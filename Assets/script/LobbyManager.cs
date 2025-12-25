using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI; // Để dùng InputField

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Giao diện Lobby")]
    public InputField nameInput; // Ô nhập tên
    public GameObject lobbyPanel; // Cái bảng chứa UI để tắt đi khi vào game

    void Start()
    {
        // Không tự động kết nối nữa. Chờ người chơi bấm nút.
        Debug.Log("Vui lòng nhập tên và bấm nút Vào Game.");
    }

    // Hàm này sẽ gán vào nút bấm "VÀO GAME"
    public void BamNutVaoGame()
    {
        string tenNguoiChoi = nameInput.text;

        // Nếu người chơi lười không nhập tên -> Tự đặt tên ngẫu nhiên
        if (string.IsNullOrEmpty(tenNguoiChoi))
        {
            tenNguoiChoi = "Player " + Random.Range(100, 999);
        }

        // 1. Lưu tên vào hệ thống mạng Photon
        PhotonNetwork.NickName = tenNguoiChoi;

        // 2. Bắt đầu kết nối
        Debug.Log("Đang kết nối với tên: " + tenNguoiChoi);
        PhotonNetwork.ConnectUsingSettings();

        // Khóa nút lại để không bấm nhiều lần (Optional)
        nameInput.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Đã vào phòng thành công!");

        // 3. Tắt giao diện Lobby đi để hiện Map game
        if (lobbyPanel != null)
        {
            lobbyPanel.SetActive(false);
        }

        // Sinh nhân vật
        float randomX = Random.Range(-2f, 2f);
        float randomY = Random.Range(-2f, 2f);
        Vector2 spawnPos = new Vector2(randomX, randomY);
        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }
}