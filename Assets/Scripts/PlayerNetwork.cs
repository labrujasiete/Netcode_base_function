using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotationSpeed = 500f;

    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(new MyCustomData{
        _int = 56,
        _bool = true,
    },NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct MyCustomData : INetworkSerializable{
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    public override void OnNetworkSpawn(){
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        };
    }
    



    // Update is called once per frame
    void Update()
    {

        if(!IsOwner) return;

        if(Input.GetKeyDown(KeyCode.T)){
            
            //TestServerRpc(new ServerRpcParams());

            TestClientRpc();

            // randomNumber.Value = new MyCustomData{
            //     _int = 10,
            //     _bool = false,
            //     message = "all your base is ours",
            // };
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        transform.Translate(movementDirection * movementSpeed * Time.deltaTime, Space.World);

        if(movementDirection != Vector3.zero){
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }


    [ServerRpc]
    private void TestServerRpc(ServerRpcParams serverRpcParams){
        Debug.Log("TestServerRpc " + OwnerClientId + "; " + serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void TestClientRpc(){
        Debug.Log("TestClientRpc");
    }
    


}
