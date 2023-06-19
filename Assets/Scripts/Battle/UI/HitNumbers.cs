using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitNumbers : MonoBehaviour
{
    public GameObject damageNumberWithModPrefab;
    public float lifespan = 1f;
    public float moveSpeed = 1f;
    public float fadeSpeed = 1f;

    public Color redColor;
    public Color whiteColor;
    public Color blueColor;
    public Color greenColor;
    public Color yellowColor;

    private List<GameObject> nums = new List<GameObject>();

    public void SpawnDamageNumbersWithMod(int amount, int mod, string amountColor, string modColor)
    {
        GameObject damageNumberWMod = Instantiate(damageNumberWithModPrefab, transform.position, Quaternion.identity);
        nums.Add(damageNumberWMod);

        Color aColor = whiteColor;
        Color mColor = blueColor;

        if (amountColor == "Red")
        {
            aColor = redColor;
        }
        else if (amountColor == "Blue")
        {
            aColor = blueColor;
        }
        else if (amountColor == "Green")
        {
            aColor = greenColor;
        }
        else if (amountColor == "White")
        {
            aColor = whiteColor;
        }
        else if (amountColor == "Yellow")
        {
            aColor = yellowColor;
        }

        if (modColor == "Red")
        {
            mColor = redColor;
        }
        else if (modColor == "Blue")
        {
            mColor = blueColor;
        }
        else if (modColor == "Green")
        {
            mColor = greenColor;
        }
        else if (modColor == "White")
        {
            mColor = whiteColor;
        }
        else if (modColor == "Yellow")
        {
            mColor = yellowColor;
        }

        damageNumberWMod.GetComponent<DamageNumbersWMod>().Init(amount, mod, aColor, mColor);

        // Start the coroutine to move and fade the damage number
        StartCoroutine(MoveAndFadeWMod(damageNumberWMod.GetComponent<DamageNumbersWMod>()));
    }



    public void SpawnText(string txt, string color)
    {
        GameObject damageNumberWMod = Instantiate(damageNumberWithModPrefab, transform.position, Quaternion.identity);
        nums.Add(damageNumberWMod);

        Color aColor = whiteColor;

        if (color == "Red")
        {
            aColor = redColor;
        }
        else if (color == "Blue")
        {
            aColor = blueColor;
        }
        else if (color == "Green")
        {
            aColor = greenColor;
        }
        else if (color == "White")
        {
            aColor = whiteColor;
        }
        else if (color == "Yellow")
        {
            aColor = yellowColor;
        }

        damageNumberWMod.GetComponent<DamageNumbersWMod>().Init(txt, aColor);

        // Start the coroutine to move and fade the damage number
        StartCoroutine(MoveAndFadeWMod(damageNumberWMod.GetComponent<DamageNumbersWMod>()));
    }


    public void End()
    {
        for (int i = 0; i < nums.Count; i++)
        {
            Destroy(nums[i].gameObject);
        }

        nums = new List<GameObject>();
        StopAllCoroutines();
    }

    private IEnumerator MoveAndFadeWMod(DamageNumbersWMod damageNumber)
    {
        // Get the initial color of the damage number
        Color initialColor = damageNumber.text1.color;
        Color initialColor2 = damageNumber.text2.color;
        // Move the damage number upwards until its lifespan is over
        float timer = 0f;
        while (timer < lifespan)
        {
            damageNumber.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // Fade out the damage number
        timer = 0f;
        while (timer < fadeSpeed)
        {
            Color newColor = damageNumber.text1.color;
            Color newColor2 = damageNumber.text2.color;
            newColor.a = Mathf.Lerp(initialColor.a, 0f, timer / fadeSpeed);
            newColor2.a = Mathf.Lerp(initialColor2.a, 0f, timer / fadeSpeed);
            damageNumber.text1.color = newColor;
            damageNumber.text2.color = newColor2;
            timer += Time.deltaTime;
            yield return null;
        }

        // Destroy the damage number game object
        Destroy(damageNumber.gameObject);
    }

    
}
