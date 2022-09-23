using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMgr : MonoBehaviour
{
    public static StageMgr Instance // singlton     
    {
        get
        {
            if ( instance == null )
            {
                instance = FindObjectOfType<StageMgr> ( );
                if ( instance == null )
                {
                    var instanceContainer = new GameObject ( "StageMgr" );
                    instance = instanceContainer.AddComponent<StageMgr> ( );
                }
            }
            return instance;
        }
    }
    private static StageMgr instance;

    public GameObject Player;
    
    [System.Serializable]
    public class StartPositionArray
    {
        public List<Transform> StartPosition = new List<Transform> ( );
    }

    public StartPositionArray[] startPositionArrays; 

    public List<Transform> StartPositionAngel = new List<Transform> ( );
    public List<Transform> StartPositionBoss = new List<Transform> ( );
    public Transform StartPositionLastBoss;
   
    public int currentStage = 0;  //현재 방위치
    
    int LastStage = 20; // 막보방

    void Start ( )
    {
        Player = GameObject.FindGameObjectWithTag ( "Player" );
    }

    public void NextStage ( )
    {
        currentStage++;
        if (currentStage > LastStage)
        {
            return;
        }

        if ( currentStage % 5 != 0 )  //Normal Stage
        {
            int arrayIndex = currentStage / 10; //0이 나옴 0스테이지 1 2
            int randomIndex = Random.Range ( 0, startPositionArrays[arrayIndex].StartPosition.Count );
            Player.transform.position = startPositionArrays[arrayIndex].StartPosition[randomIndex].position;
            startPositionArrays[arrayIndex].StartPosition.RemoveAt ( randomIndex );
        }
        else    //BossRoom or Angel
        {
            if ( currentStage % 10 == 5 )   // Angel
            {
                int randomIndex = Random.Range ( 0, StartPositionAngel.Count );
                Player.transform.position = StartPositionAngel[randomIndex].position;
            }
            else    //Boss
            {
                if ( currentStage == LastStage )  //LastBoss
                {
                    Player.transform.position = StartPositionLastBoss.position;
                }
                else    //Mid Boss
                {
                    int randomIndex = Random.Range ( 0, StartPositionBoss.Count );
                    Player.transform.position = StartPositionBoss[randomIndex].position;
                    StartPositionBoss.RemoveAt ( currentStage / 10 );
                }
            }
        }
        CameraMovement.Instance.CarmeraNextRoom ( );
    }//NextStage
}