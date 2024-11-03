using Mirror;
using Steamworks;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
    public GameObject hostBtn;

    // Calback����Steamworks�Ƿ񴴽�����
    protected Callback<LobbyCreated_t> lobbyCreated;
    // ��Steam�����������Ϸ��ֱ�Ӽ�����Ϸʱ���ᴥ����Callback
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    // ������ҽ������
    protected Callback<LobbyEnter_t> lobbyEnter;

    private NetworkManager networkManager;
    private const string HostAddressKey = "HostAddress";

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();

        if (!SteamManager.Initialized) { return; }

        // 2. ����������������˴�������Callback�¼���Ҫ����ʲô����
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    // 1. ��ť�������ʼ��������
    public void HostLobby()
    {
        hostBtn.SetActive(false);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    // 3. ���۴����ɹ�������񣬶�����ô˺���
    // LobbyCreated_t����Ϊ��Ҫ�����ȫ�����ڴ���������
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        // �����������ʧ�ܣ�������ʾ��ť
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            hostBtn.SetActive(true);
            Debug.LogError("Lobby create failed.");
            return;
        }

        // �ɹ�ʱ����Mirror�ϳ�ΪHost
        // ʹ��Mirror������ͨ���䣬IP��ַ����Ϊ�����ַ
        // ʹ��Steam����ʱ���������Steam ID
        networkManager.StartHost();

        // �κν���������ˣ�������ͨ����ȡHostAddressKey������ȡ���ǵ�SteamID
        // ����һ����֪Steam������ID (CSteamID����)
        // ������������һ��Key��ָ��Value�������ֵ������
        // ������������һ��Value�����������ߵ�SteamID
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey,
            SteamUser.GetSteamID().ToString());

        Debug.LogError("Lobby created.");
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        // ����Lobby ID
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby); 
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        // �����������ֱ�ӷ���
        if (NetworkServer.active) { return; }

        string playerName = SteamFriends.GetPersonaName();
        string playerID = SteamUser.GetSteamID().ToString();
        Debug.LogError($"Player {playerName}({playerID}) joined the lobby!");

        // ����ǿͻ�����Lobby�����л�ȡHostAddress
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey);

        // ��Mirror������HostAddress�������ͻ��ˣ��ͻ��˽�ʹ��Steam�������ӵ��õ�ַ
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        hostBtn.SetActive(false);
    }
}
