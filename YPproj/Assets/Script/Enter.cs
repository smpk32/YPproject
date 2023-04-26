using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Gravitons.UI.Modal;
public class Enter : MonoBehaviour
{
    string Check;
    string nmText;
    // Start is called before the first frame update
    void Start()
    { 
        //캐릭터 선택 체크
        if (GameManager.instance.firstCheck)
        {
            
            GameObject.Find("MainCanvas").transform.Find("PlayerSetPanel").transform.gameObject.SetActive(true);

            GameManager.instance.firstCheck = false;
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterRoom()
    {

        GameObject playerSetPanel = GameObject.Find("MainCanvas").transform.Find("PlayerSetPanel").gameObject;
        GameObject nf = playerSetPanel.transform.Find("Canvas").transform.Find("Form").transform.Find("NickNmInputField").gameObject;

        nmText = nf.GetComponent<TMP_InputField>().text;
        Check = Regex.Replace(nmText, @"[^a-zA-Z0-9가-힣]", "", RegexOptions.Singleline);
        Check = Regex.Replace(nmText, @"[^\w\.-]", "", RegexOptions.Singleline);
        
        if (nmText.Equals(Check) != true)
        {
           ModalManager.Show("알림", "입력값은 최대 10자리 이며 특수문자는 사용하실 수 없습니다. \n 다시입력하세요.",
                       new[] { new ModalButton() { Text = "확인" } });
            GameObject modal = GameObject.Find("MainCanvas").transform.Find("ModalManager").transform.gameObject;
            modal.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);

            Debug.Log("특수문자 사용.");

            nf.GetComponent<TMP_InputField>().text = "";
            nmText = "";
            Check = "";
            return;
        }
        else
        {
        GameManager.instance.CreateSingleChacracter();
            Debug.Log("캐릭터생성 성공");
        }
           
        




    }

}
