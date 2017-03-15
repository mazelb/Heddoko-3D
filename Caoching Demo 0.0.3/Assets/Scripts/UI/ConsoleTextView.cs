using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ConsoleTextView : MonoBehaviour
{

    public Text ConsoleText;
    public Text ScrollableText;
    private static ConsoleTextView sInstance;

    private static ConsoleTextView Instance
    {
        get
        {
            if (sInstance == null)
            {
                sInstance = GameObject.FindObjectOfType<ConsoleTextView>();                
            }
            return sInstance;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
        public static void UpdateScrollableConsole(string vText)
    {
        if (Instance != null)
        {
            if (Instance.ScrollableText != null)
            {
                Instance.ScrollableText.text += vText;
            }
            
        }
    }

    public static void UpdateStaticConsole(string vText)
    {
        if (Instance != null)
        {
            if (Instance.ConsoleText != null)
            {
                Instance.ConsoleText.text = vText;
            }
        }
    }
}
