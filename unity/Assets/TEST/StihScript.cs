using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StihScript : MonoBehaviour {
    public List<int> SignCheck;
    public List<int> ValueCheck;
    public bool isIncrementalStih;
    public bool isSignStih;
    public int LowValueToAdd;
    public int HighValueToAdd;
    public int StihSignStih;
    public List<int> CardValues;
	// Use this for initialization
	void Start () {
        //this.transform.SetAsLastSibling();
        StartCoroutine(GetStihType());
    }
	
    IEnumerator GetStihType()
    {
        yield return new WaitForSeconds(2);
        if (isIncremental())
        {
            isIncrementalStih = true;
            isSignStih = false;
        }
        else
        {
            isIncrementalStih = false;
            isSignStih = true;
        }
        
    }

    public bool isIncremental() {
        foreach (Transform card in this.transform)
        {
            if (SignCheck.Count == 0 && card.gameObject.GetComponent<CardScript>().CardSign != 0)
            {
                SignCheck.Add(card.gameObject.GetComponent<CardScript>().CardSign);
            }
            else if (SignCheck.Contains(card.gameObject.GetComponent<CardScript>().CardSign) && card.gameObject.GetComponent<CardScript>().CardSign != 0) 
            {
                StihSignStih = SignCheck[0];
                Debug.Log("Sign: " + StihSignStih.ToString());
                return true;
            }
        }

        return false;
    }
    public bool CheckIfCardCanBeAdded(int cardValue, int cardSign, GameObject card)
    {
        CardValues.Clear();
        foreach (Transform c in this.transform)
        {
            CardValues.Add(c.gameObject.GetComponent<CardScript>().CardValue);
        }
        CardValues.Sort();
        if (this.isIncrementalStih && StihSignStih == cardSign && ((cardValue == GetMinCard() - 1) || (cardValue == GetMaxCard() + 1))) 
        {
            return true;
        }
        else if (this.isSignStih && cardValue == GetCardValue() && SignContained(cardSign))
        {
            return true;
        }
        else
{
            return false;
        }
    }

    public void AddCardToStih(int cardValue, int cardSign, GameObject card)
    {
        CardValues.Clear();
        foreach (Transform c in this.transform)
        {
            CardValues.Add(c.gameObject.GetComponent<CardScript>().CardValue);
        }
        CardValues.Sort();
        if (this.isIncrementalStih && StihSignStih == cardSign && ((cardValue == GetMinCard() - 1) || (cardValue == GetMaxCard() + 1)));
        {
            if(cardValue == GetMinCard() - 1)
            {
                card.transform.SetParent(this.transform);
                card.transform.position = this.transform.position;
                card.transform.localScale = new Vector3(1, 1, 1);

            }
            if (cardValue == GetMaxCard() + 1) 
            {
                card.transform.SetParent(this.transform);
                card.transform.position = this.transform.position;
                card.transform.localScale = new Vector3(1, 1, 1);

            }
        }
        if(this.isSignStih && cardValue == GetCardValue() && SignContained(cardSign))
        {
                card.transform.SetParent(this.transform);
                card.transform.position = this.transform.position;
                card.transform.localScale = new Vector3(1, 1, 1);

        }
    }
    public int GetCardValue()
    {
        foreach (int c in CardValues)
        {
            if(c != 0)
            {
                return c;
            }
        }
        return 0;
    }
    public List<int> CardSignsInSigns;
    public bool SignContained(int cardSign)
    {
        CardSignsInSigns.Clear();
        foreach (Transform c in this.transform)
        {
            if(c.GetComponent<CardScript>().CardSign != 0)
            {
                CardSignsInSigns.Add(c.GetComponent<CardScript>().CardSign);
            }
        }
        if (CardSignsInSigns.Contains(cardSign))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public int GetMinCard()
    {
        foreach (int i in this.CardValues)
        {
            if (i != 0)
            {
                Debug.Log("Min Card: " + (i - 1).ToString());
                return i;
            }
        }
        return 0;
    }

    public int GetMaxCard()
    {
        for (int k = CardValues.Count - 1; k >= 0; k--)
        {
            if (CardValues[k] != 0)
            {
                Debug.Log("Max Card: " + (CardValues[k] + 1).ToString());
                return CardValues[k];
            }
        }
        return 0;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
