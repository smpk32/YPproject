using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class PlaceMove : MonoBehaviour
{
    // 행사정보리스트
    [Serializable]
    class EventListData
    {
        public string event_id;                 // 행사 id
        public string event_nm;                 // 행사 명
        public string event_image_atfl_id;      // 행사 포스터 이미지 파일 id
        public string event_dc;                 // 행사 설명
        public string event_bgng_dt;            // 행사 시작 일자
        public string event_end_dt;             // 행사 종료 일자
        public string event_place;              // 행사 장소
        public string event_hmpg_url;           // 행사 페이지 url

    }


    // 플레이어 생성장소 리스트
    public List<GameObject> spawnList;

    // 맵 리스트
    public List<GameObject> mapList;

    // 행사정보 리스트 오브젝트
    GameObject eventListObj;

    void Awake()
    {
        eventListObj = Instantiate(Resources.Load<GameObject>("EventList\\EventListObj"));
    }
    // Start is called before the first frame update
    void Start()
    {
       SetMapList();
    }


    // 초기 맵 리스트 세팅 함수
    void SetMapList()
    {
        GameObject MapGrp = GameObject.Find("Map");

        for(int i = 0; i< MapGrp.transform.childCount; i++)
        {
            mapList.Add(MapGrp.transform.GetChild(i).gameObject);
        }

        GameObject SpawnSpotGrp = GameObject.Find("SpawnSpot");

        for (int i = 0; i < SpawnSpotGrp.transform.childCount; i++)
        {
            spawnList.Add(SpawnSpotGrp.transform.GetChild(i).gameObject);
        }
        //MapChange(GameManager.instance.placeState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /* 맵 이동 이벤트 함수
     * 0 > 전시관 1
     * 1 > 전시관 2
     * 2 > 소통한마당 (싱글)
     * 3 > 소통한마당 (멀티)
     */
    public void MapChange(int num)
    {
        GameManager.instance.placeState = num;
        
        if (GameManager.instance.multiState == "Multi" && num == 3)                  // 강당 외의 장소에서 강당으로 이동 후 placeState를 0으로 세팅해줘야 SpawnSpot위치가 맞게 설정됨
        {
            
            GameManager.instance.placeState = 0;
            SetPlayerPos(GameManager.instance.placeState);

        }
        else if(GameManager.instance.multiState == "Multi" && num != 3)              // 강당에서 다른 장소로 이동할 때
        {
            GameObject.Find("MainCanvas").gameObject.transform.Find("LoadingImage").gameObject.SetActive(true);
            GameObject.Find("MainCanvas").GetComponent<Chat>().CloseSocket();
            /*Debug.Log("강당에서 다른 장소로 이동할 때");
            Debug.Log("Multi : Multi  &  num : !3");*/
            GameManager.instance.multiState = "Single";
            GameManager.instance.Disconnect();
        }
        else if(GameManager.instance.multiState == "Single" && num == 3)       // 강당 외의 장소에서 강당으로 이동할 때
        {
            /*Debug.Log("강당 외의 장소에서 강당으로 이동할 때");
            Debug.Log("Multi : Single  &  num : 3");*/
            GameManager.instance.multiState = "Multi";
            GameObject.Find("MainCanvas").gameObject.transform.Find("LoadingImage").gameObject.SetActive(true);

            // 상단 탑캔버스 SetActive False처리
            if (mapList.Count != 0)
            {
                mapList[1].transform.Find("FrameGrp").transform.Find("FloorCanvas").gameObject.SetActive(false);
            }

            GameObject.Find("MainSceneManager").GetComponent<MainSceneManager>().Enter();
            
            

        }
        else if (GameManager.instance.multiState == "Single" && num != 3)      // 강당 외의 장소에서 강당 외의 장소로 이동할 때
        {
            /*Debug.Log("강당 외의 장소에서 강당 외의 장소로 이동할 때");
            Debug.Log("Multi : Single  &  num : !3");*/
            //GameManager.instance.playerPrefab = GameObject.Find("PlayerObj");
            GameManager.instance.playerPrefab.transform.parent = null;
            GameManager.instance.playerPrefab.transform.Find("Player").GetComponent<PlayerController>().Sit(false, new Vector3(0, 0, 0), new Vector3(0, 0, 0), null);
            

            for (int i = 0; i < mapList.Count; i++)
            {
                mapList[i].SetActive(false);
            }

            GameObject.Find("MainCanvas").transform.Find("FrameDtlPanel").gameObject.SetActive(false);
            GameObject.Find("MenuImage").GetComponent<MenuEvent>().HideMenuPanel();
            GameObject.Find("DragPanel").transform.Find("SitupBtn").gameObject.SetActive(false);
            mapList[num].SetActive(true);

            if (mapList[num].name == "AuditoriumGrp")
            {
                //string urlHead = "http://192.168.1.113:8060/resources/unity/StreamingAssets/";
                string urlHead = "http://192.168.1.113:8080/files/";

                // 파일서버에서 영상 불러와 재생
                //mapList[num].transform.Find("FrameGrp").GetComponent<VideoCtrl>().LoadVideo(urlHead + "yangpyeongAD.mp4");

                // 프로젝트 내부 영상 불러와 재생
                mapList[num].transform.Find("FrameGrp").GetComponent<VideoCtrl>().LoadVideo2();
            }
            else if(mapList[num].name == "GalleryGrp (1)")
            {
                mapList[num].transform.Find("FrameGrp").GetComponent<FrameSet>().FloorChange(0);
            }

            SetPlayerPos(num);

        }

    }

    // 플레이어 생성, 장소 이동 시 플레이어 캐릭터, 카메라 세팅
    public void SetPlayerPos(int num)
    {
        Transform playerPos = GameManager.instance.playerPrefab.transform;
        //playerPos = GameObject.Find("PlayerObj").GetComponent<Transform>();
        playerPos.position = spawnList[num].transform.position;

        playerPos.rotation = spawnList[num].transform.rotation;
        playerPos.Find("Player").rotation = spawnList[num].transform.rotation;
        playerPos.Find("CameraObj").transform.rotation = spawnList[num].transform.rotation;
        if(GameManager.instance.multiState == "Single")
        {
            GameObject.Find("DragPanel").GetComponent<CameraRotateController>().Init();
        }

    }

    public void CloseEventListPanel()
    {
        GameObject.Find("MenuPanelGrp").transform.Find("PlaceMovePanel").gameObject.SetActive(true);
        GameObject.Find("MenuPanelGrp").transform.Find("EventListPanel").gameObject.SetActive(false);
    }

    // 장소 이동 > 전시관으로 이동 이벤트
    public void InitEventList()
    {
        StartCoroutine(SelectEventList("",(data) =>
        {
            Debug.Log(data);
            //var dataSet = JsonConvert.DeserializeObject<List<EventListData>>(data);

            /*DataTable eventListTable = new DataTable();
            eventListTable.Columns.Add(new DataColumn("event_id", typeof(string)));
            eventListTable.Columns.Add(new DataColumn("event_nm", typeof(string)));
            eventListTable.Columns.Add(new DataColumn("event_image_atfl_id", typeof(string)));
            eventListTable.Columns.Add(new DataColumn("event_dc", typeof(string)));
            eventListTable.Columns.Add(new DataColumn("event_bgng_dt", typeof(string)));
            eventListTable.Columns.Add(new DataColumn("event_end_dt", typeof(string)));
            eventListTable.Columns.Add(new DataColumn("event_place", typeof(string)));
            eventListTable.Columns.Add(new DataColumn("event_hmpg_url", typeof(string)));
*/
            GameObject.Find("MenuPanelGrp").transform.Find("PlaceMovePanel").gameObject.SetActive(false);
            GameObject.Find("MenuPanelGrp").transform.Find("EventListPanel").gameObject.SetActive(true);

            /*HorizontalScrollSnap scrollSnap = GameObject.Find("MainCanvas").transform.Find("MenuPanelGrp").transform.Find("MenuPanelGrp").Find("Canvas").transform.Find("Mask").transform.Find("HorizontalScrollSnap").GetComponent<HorizontalScrollSnap>();
            scrollSnap.RemoveAllChildren(out scrollSnap.ChildObjects);

            for (int i = 0; i <dataSet.Count; i++)
            {
                *//*DataRow row = eventListTable.NewRow();

                row["event_id"] = dataSet[i].event_id;
                row["event_nm"] = dataSet[i].event_nm;
                row["event_image_atfl_id"] = dataSet[i].event_image_atfl_id;
                row["event_dc"] = dataSet[i].event_dc;
                row["event_bgng_dt"] = dataSet[i].event_bgng_dt;
                row["event_end_dt"] = dataSet[i].event_end_dt;
                row["event_place"] = dataSet[i].event_place;
                row["event_hmpg_url"] = dataSet[i].event_hmpg_url;

                eventListTable.Rows.Add(row);*//*

                GameObject eventList = eventListObj;

                eventList.GetComponent<EventInfo>().eventDtlInfo = new EventInfo.EventDtlInfo(dataSet[i].event_id, dataSet[i].event_nm, dataSet[i].event_image_atfl_id, dataSet[i].event_dc, dataSet[i].event_bgng_dt, dataSet[i].event_end_dt, dataSet[i].event_place, dataSet[i].event_hmpg_url);


                // 패널 세팅
                LoadImageTexture(eventList.transform.Find("Poster").GetComponent<Image>(), dataSet[i].event_image_atfl_id);
                eventList.transform.Find("TitleText").GetComponent<TextMeshProUGUI>().text = dataSet[i].event_nm;
                eventList.transform.Find("DcText").GetComponent<TextMeshProUGUI>().text = dataSet[i].event_dc;
                eventList.transform.Find("DtText").GetComponent<TextMeshProUGUI>().text = dataSet[i].event_bgng_dt + " ~ " + dataSet[i].event_end_dt;
                eventList.transform.Find("PlaceText").GetComponent<TextMeshProUGUI>().text = dataSet[i].event_place;
                eventList.transform.Find("UrlText").GetComponent<TextMeshProUGUI>().text = dataSet[i].event_hmpg_url;

                scrollSnap.AddChild(eventList);

            }*/


        }));
    }

    public IEnumerator SelectEventList(string listType, Action<string> callback)
    {
        string GetDataUrl = GameManager.Instance.baseURL + "/event/list";

        using (UnityWebRequest www = UnityWebRequest.Post(GetDataUrl,""))
        {
            yield return www.SendWebRequest();
            // yield return System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) //불러오기 실패 시
            {
                Debug.Log(www.error);
                yield return "error";
            }
            else
            {
                if (www.isDone)
                {
                    Debug.Log(www.downloadHandler.data);

                    if (www.downloadHandler.data == null)
                    {

                        yield return null;

                    }
                    else
                    {
                        callback(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));
                        //yield return System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    }

                }
            }
        }

    }


    IEnumerator LoadImageTexture(Image rawImg, string fileId)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://192.168.1.113:8080/selectImg?file_nm="+ fileId);
        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            rawImg.preserveAspect = false;

        }
        else
        {
            //rawImg.transform.parent.transform.parent.gameObject.SetActive(true);

            Texture2D texture;
            texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            rawImg.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            rawImg.preserveAspect = true;


        }
    }
}
