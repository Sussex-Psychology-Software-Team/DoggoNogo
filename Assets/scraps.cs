///// CHANGE COLOR
//if(rotten){
//    rotten=false;
//    gameObject.GetComponent<Renderer>().material.color = Color.green;
//} else {
//    rotten=true;
//    gameObject.GetComponent<Renderer>().material.color = Color.white;
//}

//let data = {
//    metadata:{
//        id:'ABC',
//        userAgent:'ABC',
//        date: ''
//    },
//    trials:[
//        {
//            isi: 0,
//            rt: 0,
//            score: 0,
//            early_presses: 0,
//        }
//    ]
//};
//
//
//public class Deserializer : MonoBehaviour
//{
//    [SerializeField] TextAsset jsonData;
//    [SerializeField] Container container;
// 
//    void Start()
//    {
//        container = JsonUtility.FromJson<Container>(jsonData.text);
//        Debug.Log("Container deserialized");
//    }
//}