using UnityEngine;
using System.Collections;

public class Lighter : MonoBehaviour {

public GameObject LighterMesh;
public GameObject FlameFX;
public GameObject FlameLight;
public AudioSource LighterIgniteAudio;
public AudioSource LighterAudio;
  
void Start (){

    FlameFX.SetActive(false);
    FlameLight.SetActive(false);

}  
  
  
void Update (){
 
	if (Input.GetButtonDown("Fire1")) //check to see if the left mouse was pressed
    {
		StartCoroutine("LighterOn");
       
         
    }
    if (Input.GetButtonUp("Fire1"))
    {

    LighterMesh.GetComponent<Animation>().Play("Release Button");

    FlameFX.SetActive(false);

    FlameLight.SetActive(false);

    LighterAudio.Stop();

    }

            
 }
 
 
IEnumerator LighterOn (){

     LighterIgniteAudio.Play();

     LighterAudio.Play();

     LighterMesh.GetComponent<Animation>().Play("Push Button");

     FlameFX.SetActive(true);
    
     FlameLight.SetActive(true);

	 yield return new WaitForSeconds (2);

 }

}