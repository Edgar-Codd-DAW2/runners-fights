using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Equip : MonoBehaviour
{
    public GameObject weapon;
    private float horizontal;

    // Update is called once per frame
    void Update()
    {
        if (IsWeaponSet())
        {
            /*horizontal = Input.GetAxisRaw("Horizontal");

            if (horizontal < 0.0f) weapon.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            else if (horizontal > 0.0f) weapon.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);*/

            if (Input.GetKeyDown(KeyCode.Q))
            {
                weapon.GetComponent<Weapon>().Drop();
                weapon = null;
            }

        }
    }

    [PunRPC]
    public bool IsWeaponSet()
    {
        return weapon != null;
    }

    public void SetWeapon(GameObject cpWeapon)
    {
        weapon = cpWeapon;
        weapon.transform.position = transform.position;
    }

    [PunRPC]
    public void Attack()
    {
        weapon.GetComponent<Weapon>().Attack();
    }
}
