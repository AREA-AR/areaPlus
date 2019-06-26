using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{

    //Vector3 currentTargetDestination;
    //you can change the tolerance to whatever you need it to be

    void Start()
    {
        //		iTween.MoveBy(gameObject,iTween.Hash(
        //			"y"   , 22,
        //			"time", 0.2f
        //		));
        iTween.MoveTo(gameObject, iTween.Hash("y", 6, "time", 4, "loopType", "pingPong"
            , "delay", .4, "easeType", "easeInOutQuad"));
    }

}
