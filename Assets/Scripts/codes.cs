using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Codes : MonoBehaviour
{
    public TMP_InputField input;
    public CashMoney moneyScript;
    public TMP_Text validCode;
    public TMP_Text invalidCode;
    public TMP_Text debug;
    public AudioSource claimed;
    public AudioSource invalid;
    public Store storeScript;
    public GameObject codeTheme;
    public GameObject codeTheme1;
    public GameObject codeTheme2;
    public TMP_Text infoText;
    public Traits traitScript;
    public levels levelScript;

    private float delay;

    private delegate void CodeAction();
    private Dictionary<string, CodeAction> codeActions;

    private void Start()
    {
        codeActions = new Dictionary<string, CodeAction>(StringComparer.OrdinalIgnoreCase)
        {
            { "BUGReportSEP6", () => ClaimOnce("HasClaimedBugCode", () => {
                moneyScript.totalCash += 5000;
                ShowInfo("+ $5000");
            }) },

            { "2415914", () => {
                moneyScript.totalCash += 9999999;
                levelScript.rebirthTokens += 999;
                traitScript.rerollShards += 999;
                ShowInfo("+ Max Boosts");
            }},

            { "SorryXavier!", () => ClaimOnce("HasClaimedSorryCode", () => {
                if (PlayerPrefs.GetString("UserID") == "DarthXayy") {
                    levelScript.rebirthCounter++;
                    levelScript.rebirthTokens++;
                    ShowInfo("+ 1 Rebirth, 1 Rebirth Token");
                }
            }) },

            { "WGameDev!", () => ClaimOnce("ClaimedGameDevCode", () => {
                moneyScript.totalCash += moneyScript.totalCash * 0.33f;
                ShowInfo("+ 33% of money");
            }) },

            { "Traits1.7!", () => ClaimOnce("Claimed1.7Code", () => {
                moneyScript.totalCash += moneyScript.totalCash * 0.33f;
                traitScript.rerollShards += 15;
                levelScript.rebirthTokens++;
                ShowInfo("+ 33% of money, 15 Reroll Shards, 1 Rebirth Token");
            }) },

            { "SavingisFixed!", () => ClaimOnce("ClaimedFixSavingCode", () => {
                traitScript.rerollShards += 20;
                ShowInfo("+ 20 Reroll Shards");
            }) },

            { "Banner1.8!", () => ClaimOnce("Claimed1.8Code", () => {
                moneyScript.totalCash += moneyScript.totalCash * 0.25f;
                traitScript.rerollShards += 10;
                levelScript.rebirthTokens++;
                storeScript.themeItem = codeTheme2.GetComponent<StoreItem>();
                storeScript.BuyTheme();
                ShowInfo("+ 25% of money, 10 Reroll Shards, 1 Rebirth Token");
            }) },

            { "1kPlays!", () => ClaimOnce("Claimed1kPlaysCode", () => {
                ApplyPlayBonus(0.5f, 15, 1, "+ 50% of money, 15 Reroll Shards, 1 Rebirth Token");
            }) },

            { "2kPlays!", () => ClaimOnce("Claimed2kPlaysCode", () => {
                ApplyPlayBonus(0.5f, 20, 2, "+ 50% of money, 20 Reroll Shards, 2 Rebirth Tokens");
            }) },

            { "5kPlays!", () => ClaimOnce("Claimed5kPlaysCode", () => {
                ApplyPlayBonus(1f, 50, 5, "+ 100% of money, 50 Reroll Shards, 5 Rebirth Tokens");
            }) },

            { "10kPlays!", () => ClaimOnce("Claimed10kPlaysCode", () => {
                ApplyPlayBonus(1f, 100, 10, "+ 100% of money, 100 Reroll Shards, 10 Rebirth Tokens");
            }) },

            { "25kPlays!", () => ClaimOnce("Claimed25kPlaysCode", () => {
                ApplyPlayBonus(2.5f, 250, 25, "+ 250% of money, 250 Reroll Shards, 25 Rebirth Tokens");
            }) },
        };
    }

    private void Update()
    {
        if (delay > 0f)
            delay -= Time.deltaTime;
    }

    public void CheckCode()
    {
        if (delay > 0f) return;

        string code = input.text;
        if (codeActions.TryGetValue(code, out CodeAction action))
        {
            action.Invoke();
            claimed.Play();
            delay = 0.5f;
            validCode.gameObject.SetActive(true);
            StartCoroutine(ValidCoroutine());
        }
        else
        {
            delay = 0.5f;
            invalid.Play();
            invalidCode.gameObject.SetActive(true);
            StartCoroutine(InvalidCoroutine());
        }
    }

    private void ClaimOnce(string key, Action onSuccess)
    {
        if (PlayerPrefs.GetFloat(key) != 0) return;

        onSuccess.Invoke();
        PlayerPrefs.SetFloat(key, 1.0f);
    }

    private void ApplyPlayBonus(float moneyMultiplier, int rerollShards, int rebirthTokens, string message)
    {
        moneyScript.totalCash += moneyScript.totalCash * moneyMultiplier;
        traitScript.rerollShards += rerollShards;
        levelScript.rebirthTokens += rebirthTokens;
        ShowInfo(message);
    }

    private void ShowInfo(string message)
    {
        infoText.text = message;
        infoText.gameObject.SetActive(true);
    }

    private IEnumerator ValidCoroutine()
    {
        yield return new WaitForSeconds(3);
        infoText.gameObject.SetActive(false);
        validCode.gameObject.SetActive(false);
    }

    private IEnumerator InvalidCoroutine()
    {
        yield return new WaitForSeconds(1);
        invalidCode.gameObject.SetActive(false);
    }
}
