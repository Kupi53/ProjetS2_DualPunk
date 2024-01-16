using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*public class WeaponController : MonoBehaviour {
    //New Weapon Controller
    //New Weapon Item
    //Added GetSortingOrder to human
    //Added weapon control to Player Controller 
    //weapon item

    //Added layers to physics • changed collision matrix 
    //added tag for player limb
    //have to turn off queries hit collides in physics 2d

    SpriteRenderer sr;
    Human myHuman;
    GameObject myWeaponObject;
    public WeaponItem myWeapon;
    bool hasWeapon=false;
    // Use this for initialization
    void Start (){
        myHuman = this.gameObjeCt.GetComponent<Human> ();
    }

    // Update is called once per frame 
    void Update(){
    }

    public void firaWeapon()
    {
        if (hasWeapon = true) {
            myWeapon.fireWeapon ();
        }

    }

    public void weaponControl(Vector3 dir)
    {
        if (hasWeapon == true) {
            weaponDirectionControl (dir);
            shouldWeFlipWeapon (dir); 
            setWeaponSortingOrder (dir);
        }

    }

    public void setWeapon(GameObject obj)
    {
        myWeaponObject = (GameObject)Instantiate (obj,this.transform.position,this.transform.rotation);
        myWeaponObject.transform.position = new Vector3 (myWeaponObject.transform.position.x, myWeaponObject.transform.position.y  - 0,4f);
        myWeaponObject.transform.parent = thls.transform;
        sr = myWeaponObject.GetComponent<SpriteRenderer> ();
        myWeapon = myWeaponObject.GetComponent<weaponItem> ();
        hasWeapon = true;
    }

    void weaponDirectionControl(Vector3 posToFace)
    {
        //rotates the weapon to face the vector provided
        float z = Mathf.Atan2 ((posToFace.y - transform.position.y), (posToFace.x - transform.position.x)) * Mathf.Rad2Deg; 
        myWeaponObject.transform.eulerAngles = new Vector3 (0, 0, z);
    }

    void shouldWeFlipWeapon(Vector3 posToFace)
    {
        if (posToFace.x > this.transform.position.x) {
            //facing right, don't flip
            sr.flipY=false;
        }
        else {
            //facing left
            sr.flipY=true;
        }
    }

    void setWeaponSortingOrder(Vector3 posToFace)
    {
        if (posToFace.y > this.transform.position.y) { 
            sr.sortingOrder = myHuman.getSortingOrder () - 2; 
        } 
        else {
            sr.sortingOrder = myHuman.getSortingOrder () + 2;
        }
    }
}
*/