using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_BarracksMenu : UIScreen
{
    [SerializeField] private UI_ERROR uiError;

    [SerializeField] private List<TextMeshProUGUI> stackID;
    [SerializeField] private List<TextMeshProUGUI> stackTime;
    Barrack barrack;

    private void Update()
    {
        Displaying();
    }

    private void Displaying()
    {
        List<BeeRecruitingInformation> beeRecruitingInformation = barrack.GetRecruitingInformation();
        for (int n = 0; n < beeRecruitingInformation.Count && n < stackID.Count && n < stackTime.Count; n++)
        {
            if (beeRecruitingInformation[n].Empty)
            {
                stackID[n].text = "empty";
                stackTime[n].text = "";
            }
            else{
                stackID[n].text = beeRecruitingInformation[n].CurrentID.ToString();
                float currentTime = Mathf.Clamp((Mathf.Round(beeRecruitingInformation[n].CurrentTime * 100F) / 100F), 0F, Mathf.Infinity);
                float fullTime = Mathf.Round(beeRecruitingInformation[n].RecruitinTime * 100F) / 100F;
                stackTime[n].text = (currentTime + "/" + fullTime);
            }
        }
    }
    
    public void _CallMenu(GameObject _barrack)
    {
        barrack = _barrack.GetComponent<Barrack>();
    }
    
    public void _BuildingLVL_Up()
    {
        barrack.NextBuildingLevel();
    }

    public void _RecruitingWax()
    {
        uiError._ErrorCall(barrack.RecruitBees(BeesRecruitingID.Wasp));
    }
    
    public void _RecruitingBumblebee()
    {
        uiError._ErrorCall(barrack.RecruitBees(BeesRecruitingID.Bumblebee));
    }
}
