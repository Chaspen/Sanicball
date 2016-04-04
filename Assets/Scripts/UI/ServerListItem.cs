﻿using UnityEngine;
using UnityEngine.UI;

namespace Sanicball.UI
{
    public class ServerListItem : MonoBehaviour
    {
        public Text serverNameText;
        public Text serverStatusText;
        public Text playerCountText;
        public Text pingText;
        public Image pingLoadingImage;

        [HideInInspector]
        public ServerInfo info;

        private bool pingDone = false;
        private float pingTimeout = 8f;

        public void SetData(HostData data)
        {
            info = new ServerInfo(data);
            serverNameText.text = info.GameName;
            serverStatusText.text = info.Status;
            if (info.UseNAT)
            {
                serverStatusText.text += " - uses NAT punching";
            }
            playerCountText.text = info.ConnectedPlayers + "/" + info.PlayerLimit;
        }

        private void Update()
        {
            if (info == null || pingDone) return;

            if (pingTimeout > 0f)
            {
                pingTimeout -= Time.deltaTime;
                if (pingTimeout <= 0f)
                {
                    pingLoadingImage.enabled = false;
                    pingText.text = "?";
                    pingDone = true;
                }
            }

            if (info.ping.isDone)
            {
                pingLoadingImage.enabled = false;
                pingText.text = info.ping.time + "ms";
                pingDone = true;
            }
        }
    }

    public class ServerInfo
    {
        public string Status;
        public float VersionFloat;
        public string VersionString;
        public bool UseNAT;
        public int RealPort;
        public Ping ping;
        private HostData rawHostData;

        //The port recieved via host data is sometimes incorrect
        public ServerInfo(HostData data)
        {
            string[] comment = data.comment.Split(';');
            if (comment.Length != 5) { Debug.LogWarning("Server '" + data.gameName + "' has the wrong number of comment arguments"); }
            else
            {
                Status = comment[0];
                if (!float.TryParse(comment[1], out VersionFloat))
                {
                    Debug.LogWarning("Failed to parse server version float for server '" + data.gameName + "'.");
                }
                VersionString = comment[2];
                UseNAT = comment[3] == "useNat";
                if (!int.TryParse(comment[4], out RealPort))
                {
                    Debug.LogWarning("Failed to parse real port for server '" + data.gameName + "'");
                }
            }

            ping = new Ping(data.ip[0]);

            rawHostData = data;
        }

        public string GameName { get { return rawHostData.gameName; } }
        public int ConnectedPlayers { get { return rawHostData.connectedPlayers; } }
        public int PlayerLimit { get { return rawHostData.playerLimit; } }
    }
}