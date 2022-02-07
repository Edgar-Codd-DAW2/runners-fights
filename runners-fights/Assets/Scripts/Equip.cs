using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equip : MonoBehaviour
{
    public GameObject weapon;
    private float horizontal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

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

    public bool IsWeaponSet()
    {
        return weapon != null;
    }

    public void SetWeapon(GameObject cpWeapon)
    {
        weapon = cpWeapon;
        weapon.transform.position = transform.position;
    }

    public void Attack(GameObject player)
    {
        weapon.GetComponent<Weapon>().Attack();
    }
}
