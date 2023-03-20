using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotationSpeed = 500f;


    // Update is called once per frame
    void Update()
    {

        Debug.Log(OwnerClientId + "; RandomNumber: " + randomNumber.Value);

        if(!IsOwner) return;

        if(Input.GetKeyDown(KeyCode.T)){
            randomNumber.Value = Random.Range(0, 100);
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

}
