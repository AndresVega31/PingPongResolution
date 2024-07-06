using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;



public class NetworkUIManager : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private InputField ipInputField;
    [SerializeField] private Text hostIPText;  // Referencia al texto UI para mostrar la IP
    [SerializeField] private Text portText;  // Referencia al texto UI para mostrar el puerto

    void Start()
    {
        //hostButton.onClick.AddListener(StartHost);
        //clientButton.onClick.AddListener(StartClient);
    }

    void StartHost()
    {
        string localIP = GetLocalIPAddress();
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            localIP,  // The IP address is a string
            (ushort)9000, // The port number is an unsigned short
            "0.0.0.0" // The server listen address is a string.
        );

        NetworkManager.Singleton.StartHost();
        PrintHostIP();  // Llamar al método para imprimir la IP del host
    }

    void StartClient()
    {
        // Obtener la IP ingresada por el usuario
        string ipAddress = ipInputField.text;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            ipAddress,  // The IP address is a string
            (ushort)9000, // The port number is an unsigned short
            "0.0.0.0" // The server listen address is a string.
        );

        NetworkManager.Singleton.StartClient();
    }
    
    void PrintHostIP()
    {
        // Obtener el componente de transporte
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // Obtener la IP del host
        string hostIP = transport.ConnectionData.Address;
        ushort port = transport.ConnectionData.Port;

        // Mostrar la IP en la UI
        hostIPText.text = "Host IP: " + hostIP;
        portText.text = $"Port: {port}";
    }

    string GetLocalIPAddress()
    {
        List<string> ipAddresses = new List<string>();

        foreach (var netInterface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
        {
            if (netInterface.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
            {
                foreach (var address in netInterface.GetIPProperties().UnicastAddresses)
                {
                    if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddresses.Add(address.Address.ToString());
                    }
                }
            }
        }

        // Return the first valid IP address found, or null if none were found
        return ipAddresses.Count > 0 ? ipAddresses[0] : null;
    }
}
