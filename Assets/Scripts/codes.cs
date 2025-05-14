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
    private Dictionary<string, (CodeAction action, string key)> codeActions;

    private void Start()
    {
        codeActions = new Dictionary<string, (CodeAction, string)>(StringComparer.OrdinalIgnoreCase)
        {
            { "BUGReportSEP6", (() => {
                moneyScript.totalCash += 5000;
                ShowInfo("+ $5000");
            }, "HasClaimedBugCode") },

            { "2415914", (() => {
                moneyScript.totalCash += 9999999;
                levelScript.rebirthTokens += 999;
                traitScript.rerollShards += 999;
                ShowInfo("+ Max Boosts");
            }, "ClaimedDevCode_2415914") }, // Now only redeemable once

            { "SorryXavier!", (() => {
                levelScript.rebirthCounter++;
                levelScript.rebirthTokens++;
                ShowInfo("+ 1 Rebirth, 1 Rebirth Token");
            }, "HasClaimedSorryCode") },

            { "WGameDev!", (() => {
                moneyScript.totalCash += moneyScript.totalCash * 0.33f;
                ShowInfo("+ 33% of money");
            }, "ClaimedGameDevCode") },

            { "Traits1.7!", (() => {
                moneyScript.totalCash += moneyScript.totalCash * 0.33f;
                traitScript.rerollShards += 15;
                levelScript.rebirthTokens++;
                ShowInfo("+ 33% of money, 15 Reroll Shards, 1 Rebirth Token");
            }, "Claimed1.7Code") },

            { "SavingisFixed!", (() => {
                traitScript.rerollShards += 20;
                ShowInfo("+ 20 Reroll Shards");
            }, "ClaimedFixSavingCode") },

            { "Banner1.8!", (() => {
                moneyScript.totalCash += moneyScript.totalCash * 0.25f;
                traitScript.rerollShards += 10;
                levelScript.rebirthTokens++;
                storeScript.themeItem = codeTheme2.GetComponent<StoreItem>();
                storeScript.BuyTheme();
                ShowInfo("+ 25% of money, 10 Reroll Shards, 1 Rebirth Token");
            }, "Claimed1.8Code") },

            { "1kPlays!", (() => ApplyPlayBonus(0.5f, 15, 1, "+ 50% of money, 15 Reroll Shards, 1 Rebirth Token"), "Claimed1kPlaysCode") },
            { "2kPlays!", (() => ApplyPlayBonus(0.5f, 20, 2, "+ 50% of money, 20 Reroll Shards, 2 Rebirth Tokens"), "Claimed2kPlaysCode") },
            { "5kPlays!", (() => ApplyPlayBonus(1f, 50, 5, "+ 100% of money, 50 Reroll Shards, 5 Rebirth Tokens"), "Claimed5kPlaysCode") },
            { "10kPlays!", (() => ApplyPlayBonus(1f, 100, 10, "+ 100% of money, 100 Reroll Shards, 10 Rebirth Tokens"), "Claimed10kPlaysCode") },
            { "25kPlays!", (() => ApplyPlayBonus(2.5f, 250, 25, "+ 250% of money, 250 Reroll Shards, 25 Rebirth Tokens"), "Claimed25kPlaysCode") },
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
        if (codeActions.TryGetValue(code, out var entry))
        {
            if (entry.key == "HasClaimedSorryCode" && PlayerPrefs.GetString("UserID") != "DarthXayy")
            {
                // Don't allow special user code for others
                ShowInvalid();
                return;
            }

            if (PlayerPrefs.GetInt(entry.key, 0) == 0)
            {
                entry.action.Invoke();
                PlayerPrefs.SetInt(entry.key, 1);
                claimed.Play();
                delay = 0.5f;
                validCode.gameObject.SetActive(true);
                StartCoroutine(ValidCoroutine());
            }
            else
            {
                ShowInvalid();
            }
        }
        else
        {
            ShowInvalid();
        }
    }

    private void ShowInvalid()
    {
        delay = 0.5f;
        invalid.Play();
        invalidCode.gameObject.SetActive(true);
        StartCoroutine(InvalidCoroutine());
    }

    private static void ApplyPlayBonus(float moneyMultiplier, int rerollShards, int rebirthTokens, string message)
    {
        var instance = FindFirstObjectByType<Codes>();
        instance.moneyScript.totalCash += instance.moneyScript.totalCash * moneyMultiplier;
        instance.traitScript.rerollShards += rerollShards;
        instance.levelScript.rebirthTokens += rebirthTokens;
        instance.ShowInfo(message);
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
