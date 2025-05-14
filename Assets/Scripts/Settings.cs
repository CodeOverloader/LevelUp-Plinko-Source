using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class Settings : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider volume;
    public TMP_InputField input;
    public GameObject scamera;
    float delayValue; 
    public spawnBallYes spawnScript;
    public GameObject invalidVal;
    public GameObject confirmReset;
    public Slider shopVolume;
    public Store storeScript;
    public GameObject resetButton;
    public GameObject NamePage;
    public TMP_Text nameText;
    public CashMoney moneyScript;
    public Toggle ballTrails;
    public bool ballTrailsOn;
    public Codes codeScript;
    public TMP_Text infoText;
    public TMP_Text buttonText;
    public Menu menu;
    public bool listenForKey;
    public KeyCode dropKey;
    void Start()
    {
        if(PlayerPrefs.GetString("DropKey") == ""){
            PlayerPrefs.SetString("DropKey", "Space");
        }else{
            dropKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("DropKey"));
            buttonText.text = "Current Key: " + PlayerPrefs.GetString("DropKey");
        }
        if(PlayerPrefs.GetFloat("HasBeenReset1.7") == 0.0f){
            PlayerPrefs.DeleteAll();
            moneyScript.totalCash = 500;
            PlayerPrefs.SetFloat("Total Cash", moneyScript.totalCash);
            confirmReset.SetActive(false);
            resetButton.SetActive(true);
            SceneManager.LoadScene(0);
            PlayerPrefs.SetFloat("HasBeenReset1.7", 1.0f);
        }
        if(PlayerPrefs.GetFloat("HasChangedBallTrails") == 0){
            ballTrails.isOn = true;
            spawnScript.ballTrailsOn = true;
            PlayerPrefs.SetFloat("BallTrails", 1.0f);
            PlayerPrefs.SetFloat("HasChangedBallTrails", 1);
        }
        if(PlayerPrefs.GetFloat("HasChangedVolume") == 0){
            volume.value = 1.0f;
            storeScript.music.volume = 1.0f;
        }else{
            if(PlayerPrefs.GetFloat("BallTrails") == 0.0){
                ballTrails.isOn = false;
                UpdateBallTrails();
            }else{
                ballTrails.isOn = true;
                UpdateBallTrails();
            }
            volume.value = PlayerPrefs.GetFloat("VolumePreference");
            input.text = (PlayerPrefs.GetFloat("DelayPreference")).ToString();
            delayValue = PlayerPrefs.GetFloat("DelayPreference");
            shopVolume.value = PlayerPrefs.GetFloat("ShopMusicVolume");
        }
    }
    public void BackToGambling(){
        scamera.transform.position = new Vector3(-2,19.33f,-9);
    }
    public void ChangeName(){
        scamera.transform.Translate(130,0,0);
        NamePage.SetActive(true);
    }
    public void GoToSettings(){
        scamera.transform.position = new Vector3(-115,19.33f,-9);
        menu.toggle();
    }
    public void UpdateBallDelay(){
        float.TryParse(input.text, out delayValue);
        if(delayValue < 0.1){
            invalidVal.SetActive(true);
            StartCoroutine(TimerCoroutine());
        }else{
        spawnScript.delayAssign = delayValue;
        }
    }
    public void UpdateBallTrails(){
        spawnScript.ballTrailsOn = ballTrails.isOn;
    }
    public void ConfirmReset(){
        confirmReset.SetActive(true);
        resetButton.SetActive(false);
    }
    public void ResetConfirmed(){
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("ClaimedRebirthCOMPCode", 1);
        PlayerPrefs.SetFloat("ClaimedBustaCode", 1);
        PlayerPrefs.SetFloat("Claimed1.6Code", 1);
        PlayerPrefs.SetFloat("Claimed1.7Code", 1);
        PlayerPrefs.SetFloat("ClaimedFixSavingCode", 1);
        PlayerPrefs.SetFloat("HasBeenReset1.7", 1);
        PlayerPrefs.SetFloat("Claimed1kPlaysCode", 1);
        PlayerPrefs.SetFloat("Claimed2kPlaysCode", 1);
        PlayerPrefs.SetFloat("Claimed5kPlaysCode", 1);
        PlayerPrefs.SetFloat("Claimed10kPlaysCode", 1);
        PlayerPrefs.SetFloat("Claimed25kPlaysCode", 1);
        moneyScript.totalCash = 500;
        PlayerPrefs.SetFloat("Total Cash", moneyScript.totalCash);
        confirmReset.SetActive(false);
        resetButton.SetActive(true);
        SceneManager.LoadScene(0);
    }
    public void ResetCancelled(){
        confirmReset.SetActive(false);
        resetButton.SetActive(true);
    }
    public void ResetToDefault(){
        input.text = "0.25";
        PlayerPrefs.SetFloat("DelayPreference", 0.25f);
        EventSystem.current.SetSelectedGameObject(null);
    }
    // Update is called once per frame
    IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(2);
        invalidVal.SetActive(false);
    }
    public void GetNextInput(){
        listenForKey = true;
        infoText.text = "Press a key...";
    }
    void Update()
    {
        nameText.text = ("UserID: " + PlayerPrefs.GetString("UserID"));
        PlayerPrefs.SetFloat("VolumePreference", volume.value);
        PlayerPrefs.SetFloat("ShopMusicVolume", shopVolume.value);
        PlayerPrefs.SetFloat("HasChangedVolume", 1.0f);
        float.TryParse(input.text, out delayValue);
        PlayerPrefs.SetFloat("DelayPreference", spawnScript.delayAssign);
        if(ballTrails.isOn == false){
            PlayerPrefs.SetFloat("BallTrails", 0);
        }else{
           PlayerPrefs.SetFloat("BallTrails", 1); 
        }
        if(listenForKey){
            foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))){
                if(Input.GetKeyDown(vKey)){
                    PlayerPrefs.SetString("DropKey", vKey.ToString());
                    listenForKey = false;
                    infoText.text = "";
                    buttonText.text = "Current Key: " + vKey.ToString();
                    dropKey = vKey;
                }
            }
        }
    }
}
