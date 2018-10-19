using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardScript : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler/*, IPointerEnterHandler, IPointerExitHandler*/
{
    public int CardValue;
    public int CardSign; // 1-srce 2-karo 3-tref 4-pik
    public bool isJoker;
    public bool isAce;
    public Color PickedColor;
    public Color FadedColor;
    public Color IdleColor;
    public int SiblingIndex;
    public GraphicRaycaster GR;
    public List<string> UIhits;
    private CoreGameplay CG;
    public GameObject SelectionToggleObject;

    public bool sentToGarbage;



    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = Input.mousePosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SelectionToggleObject.SetActive(false);
        SiblingIndex = this.transform.GetSiblingIndex();
        this.transform.SetParent(GameObject.FindGameObjectWithTag("FreeArea").transform);
        this.gameObject.GetComponent<Image>().color = PickedColor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Image>().color = IdleColor;
        UIhits.Clear();
        this.GetComponent<Image>().raycastTarget = false;
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        GR.Raycast(ped, results);
        foreach (RaycastResult resul in results) {
            UIhits.Add(resul.gameObject.name);
                }
        Debug.Log("Object hit: " + results[0].gameObject.tag);
        if (results[0].gameObject.tag == "Hand")
        {
            this.transform.SetParent(GameObject.FindGameObjectWithTag("Hand").transform);
            this.transform.SetSiblingIndex(SiblingIndex);
            this.GetComponent<Image>().raycastTarget = true;
            return;
        }
        else if (results[0].gameObject.tag == "Table") 
        {
            SelectionToggleObject.SetActive(false);
            if (CG.PoolCardDrawn && !CG.HandCardDiscarded)
            {               
                this.transform.SetParent(GameObject.FindGameObjectWithTag("Table").transform);
                int TableCount = GameObject.FindGameObjectWithTag("Table").transform.childCount;
                if (TableCount != 0) {
                    this.transform.SetSiblingIndex(TableCount + 1);
                }
                this.transform.position = GameObject.FindGameObjectWithTag("Table").transform.position;
                CG.DiscardHandCard(this.name);
                CG.NextTurn();
                //StartCoroutine(WaitForEndTurn());
                return;
            }
            else
            {
                SelectionToggleObject.SetActive(true);
                this.transform.SetParent(GameObject.FindGameObjectWithTag("Hand").transform);
                this.transform.SetSiblingIndex(SiblingIndex);
                this.GetComponent<Image>().raycastTarget = true;
                return;
            }
        }
        else if (results[0].gameObject.tag == "Card")
        {
            SelectionToggleObject.SetActive(true);
            this.transform.SetParent(GameObject.FindGameObjectWithTag("Hand").transform);
            this.transform.SetSiblingIndex(results[0].gameObject.transform.GetSiblingIndex() + 1);
            this.GetComponent<Image>().raycastTarget = true;
            return;
        }
        else if (results[0].gameObject.tag == "Stih" || results[0].gameObject.tag == "iStih")
        {
            SelectionToggleObject.SetActive(false);
            if (results[0].gameObject.GetComponent<StihScript>().CheckIfCardCanBeAdded(this.CardValue, this.CardSign, this.gameObject))
            {
                results[0].gameObject.GetComponent<StihScript>().AddCardToStih(this.CardValue, this.CardSign, this.gameObject);
                StartCoroutine(RefreshBoard());
                CG.RefreshStihsOnAddCard(results[0].gameObject.transform.parent.parent.name);
                this.GetComponent<Image>().raycastTarget = false;
                return;
            }
            else
            {
                SelectionToggleObject.SetActive(true);
                this.transform.SetParent(GameObject.FindGameObjectWithTag("Hand").transform);
                this.transform.SetSiblingIndex(SiblingIndex);
                this.GetComponent<Image>().raycastTarget = true;
            }
        }

        else
        {
            SelectionToggleObject.SetActive(true);
            this.transform.SetParent(GameObject.FindGameObjectWithTag("Hand").transform);
            this.transform.SetSiblingIndex(SiblingIndex);
            this.GetComponent<Image>().raycastTarget = true;
        }
        this.GetComponent<Image>().raycastTarget = true;

    }
    IEnumerator WaitForEndTurn()
    {
        yield return new WaitForSeconds(1);
        CG.NextTurn();
    }
    IEnumerator RefreshBoard()
    {
        yield return new WaitForSeconds(0.5f);
    }
    void GetCardSign(string cardname)
    {
        if (cardname.Contains("hearts"))
        {
            CardSign = 1;
        }
        if (cardname.Contains("diamonds"))
        {
            CardSign = 2;
        }
        if (cardname.Contains("clubs"))
        {
            CardSign = 3;
        }
        if (cardname.Contains("spades"))
        {
            CardSign = 4;
        }
    }

    void GetCardValue(string cardname)
    {
        if (cardname.Contains("jack"))
        {
            CardValue = 12;
            return;
        }
        if (cardname.Contains("queen"))
        {
            CardValue = 13;
            return;
        }
        if (cardname.Contains("king"))
        {
            CardValue = 14;
            return;
        }
        if (cardname.Contains("2"))
        {
            CardValue = 2;
            return;
        }
        if (cardname.Contains("3"))
        {
            CardValue = 3;
            return;
        }
        if (cardname.Contains("4"))
        {
            CardValue = 4;
            return;
        }
        if(cardname.Contains("5"))
        {
            CardValue = 5;
            return;
        }
        if (cardname.Contains("6"))
        {
            CardValue = 6;
            return;
        }
        if (cardname.Contains("7"))
        {
            CardValue = 7;
            return;
        }
        if (cardname.Contains("8"))
        {
            CardValue = 8;
            return;
        }
        if (cardname.Contains("9"))
        {
            CardValue = 9;
            return;
        }
        if (cardname.Contains("10"))
        {
            CardValue = 10;
            return;
        }
        if (cardname.Contains("ace"))
        {
            CardValue = 1;
            isAce = true;
            return;
        }
    }

    void IsCardAJoker(string cardname)
    {
        if (cardname.Contains("joker"))
        {
            CardValue = 0;
            CardSign = 0;
            isJoker = true;
            return;
        }
    }

    // Use this for initialization
    void Start () {
        SelectionToggleObject = this.transform.GetChild(0).gameObject;
        CG = GameObject.Find("Deck").GetComponent<CoreGameplay>();
        GR = GameObject.FindGameObjectWithTag("Canvas").GetComponent<GraphicRaycaster>();
        if (this.transform.parent.tag == "Hand")
        {
            SelectionToggleObject.SetActive(true);
            this.GetComponent<Image>().sprite = Resources.Load<Sprite>(CG.LocalCards[0]);
            this.name = CG.LocalCards[0];
            CG.LocalCards.RemoveAt(0);
        }
        if (this.transform.parent.tag == "Table")
        {
            SelectionToggleObject.SetActive(false);
            this.transform.localScale = new Vector3(2.5f, 1.25f, 1);
            this.GetComponent<Image>().sprite = Resources.Load<Sprite>(CG.TableCards[(CG.TableCards.Count - 1)]);
            this.name = CG.TableCards[CG.TableCards.Count - 1];
            //CG.TableCards.RemoveAt(0);
            this.GetComponent<Image>().raycastTarget = false;
        }
        if (this.transform.parent.tag == "Stih")
        {
            SelectionToggleObject.SetActive(false);
            this.transform.localScale = new Vector3(1, 1, 1);
            this.GetComponent<Image>().sprite = Resources.Load<Sprite>(CG.allSelectedCards[(CG.allSelectedCards.Count - 1)].gameObject.name.ToString());
            this.name = CG.allSelectedCards[(CG.allSelectedCards.Count - 1)].gameObject.name.ToString();
            CG.allSelectedCards.RemoveAt((CG.allSelectedCards.Count - 1));
            this.GetComponent<Image>().raycastTarget = false;
        }
        if (this.transform.parent.tag == "iStih")
        {
            SelectionToggleObject.SetActive(false);
            this.transform.localScale = new Vector3(1, 1, 1);
            this.GetComponent<Image>().sprite = Resources.Load<Sprite>(CG.iStihCards[0]);
            this.name = CG.iStihCards[0].ToString();
            CG.iStihCards.RemoveAt(0);
            this.GetComponent<Image>().raycastTarget = false;
        }

        string cardname = this.GetComponent<Image>().sprite.name;
        GetCardSign(cardname);
        GetCardValue(cardname);
        IsCardAJoker(cardname);

        GR = GameObject.FindGameObjectWithTag("Canvas").GetComponent<GraphicRaycaster>();
    }



    void Update()
    {
        if(this.gameObject.transform.parent == GameObject.FindGameObjectWithTag("Garbage").transform)
        {
            this.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

	
	// Update is called once per frame

}
