using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WeaponItem : MonoBehaviour {

    public GameObject projectile;
    public Transform projectileSpawnPos;
    public float bulletTimer=0.0f,rateOfFire = 0.1f; 
    // Use this for initialization
    void Start (){

    }

    // Update is called once per frame
    void Update () {

    }

    public void fireWeapon()
    {
        bulletTimer -= Time.deltaTime;
        if (bulletTimer <= 0) {
            createProjectile ();
            bulletTimer = rateOfFire;
        }
    }

    public void createProjectile()
    {
        GameObject g = (GameObject) Instantiate (projectile, projectileSpawnPos.transform.position, this.transform.rotation);
        g.GetComponent<Projectile> ().myCreator = this.transform.parent.gameObject;
    }
}
