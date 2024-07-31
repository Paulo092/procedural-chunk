using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GemBagUI : MonoBehaviour
{
    private TextMeshProUGUI _gemText;
    
    // Start is called before the first frame update
    void Start()
    {
        _gemText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateGemText(GemBag gemBag)
    {
        _gemText.text = "Gems: " + gemBag.NumberOfGems.ToString();
    }
}
