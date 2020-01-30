using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineBehaviourScript : MonoBehaviour
{
    public static int objNum = 9;
    public int offsetSize = 30;   
    public GameObject lineUnit;

    public Object[] objList = new Object[objNum];


    // Start is called before the first frame update
    void Start()
    {
       for (int i = 0; i < objNum; i++)
        {
                        
            GameObject newObj = Instantiate(lineUnit, new Vector3 (transform.position.x, transform.position.y + offsetSize*-i, transform.position.z), transform.rotation);
            newObj.GetComponentInChildren<Text>().text = objList[i].name;

            //how to get color?....
            //newObj.GetComponentInChildren<Image>().color = objList[i]. 
            
            newObj.transform.SetParent(this.transform.parent);

        }
                      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
