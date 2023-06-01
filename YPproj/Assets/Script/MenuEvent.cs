using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class MenuEvent : MonoBehaviour
{

    public Image menuBar;
    public GameObject btnGrp;

    //public GameObject[] panelList;

#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern int QuitGame();

#endif


    public void MenuClick()
    {
        bool chk = false;
        if (menuBar.fillAmount == 0)
        {
            chk = true;
        }

        StartCoroutine(FillMenuBar(chk));
    }

    IEnumerator FillMenuBar(bool chk)
    {
        menuBar.gameObject.SetActive(true);
        if (chk)
        {
            btnGrp.SetActive(true);
            float timer = 0.0f;
            menuBar.fillAmount = 1;

            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                menuBar.fillAmount = timer / 0.5f;
                yield return null;
            }

            if (timer >= 0.5f)
            {
                menuBar.fillAmount = 1;
            }

            //menuBar.gameObject.SetActive(false);
        }
        else
        {
            btnGrp.SetActive(false);
            float timer = 0.5f;
            menuBar.fillAmount = 0;
            
            while(timer > 0)
            {
                timer -= Time.deltaTime;
                menuBar.fillAmount = timer / 0.5f;
                yield return null;
            }

            if (timer <= 0)
            {
                menuBar.fillAmount = 0;
            }

            menuBar.gameObject.SetActive(false);

        }
    }

    public void ShowMenuPanel(int panelIdx)
    {

        GameManager.instance.SetState("setting");

        GameObject panelList = GameObject.Find("MainCanvas").transform.Find("MenuPanelGrp").gameObject;

        for (int i = 0; i < panelList.transform.childCount; i++)
        {
            panelList.transform.GetChild(i).gameObject.SetActive(false);
            //panelList[i].gameObject.SetActive(false);

        }
       
        GameObject.Find("MainCanvas").transform.Find("MenuPanelGrp").gameObject.SetActive(true);
        panelList.transform.GetChild(panelIdx).gameObject.SetActive(true);


    }

    public void HideMenuPanel()
    {
        GameManager.instance.SetState("normal");

        GameObject panelList = GameObject.Find("MainCanvas").transform.Find("MenuPanelGrp").gameObject;

        for (int i = 0; i < panelList.transform.childCount; i++)
        {
            
            panelList.transform.GetChild(i).gameObject.SetActive(false);
            //panelList[i].gameObject.SetActive(false);
            
        }

        GameObject.Find("MainCanvas").transform.Find("MenuPanelGrp").gameObject.SetActive(false);
    }

    public void QuitGameClick()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        QuitGame();
#else
        GameManager.instance.QuitGame();
#endif
    }
}
