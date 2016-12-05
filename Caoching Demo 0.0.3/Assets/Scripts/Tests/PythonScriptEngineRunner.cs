using UnityEngine;
using System.Collections;
using System.Reflection;
using Microsoft.Scripting.Hosting;

public class PythonScriptEngineRunner : MonoBehaviour
{
    private const string FilePath = @"E:\Code\Heddoko3D\Caoching Demo 0.0.3\Assets\Scripts\Python\Message.py";
    private ScriptEngine mScriptEngine;
    public string Message;
    void Start()
    {
         //create a script engine
          mScriptEngine = IronPython.Hosting.Python.CreateEngine();
        //create a scop
        var vScriptScope = mScriptEngine.CreateScope();
 
        //execute a string in the interpreter and grab the variable
        //string vExample = "output = 'hello world'";
        //var vScriptSource = mScriptEngine.CreateScriptSourceFromString(vExample);
        //vScriptSource.Execute(vScriptScope);
        //string vCame = vScriptScope.GetVariable<string>("output");

    }

    void GetResultsFromPluginTest(string message)
    {
        ScriptSource vScript;
        vScript = mScriptEngine.CreateScriptSourceFromFile(FilePath);
        CompiledCode vCode = vScript.Compile();
        ScriptScope vSCope = mScriptEngine.CreateScope();
        vCode.Execute(vSCope);
        vSCope.SetVariable("Message", message);
        var vObj = vSCope.GetVariable<string>("Message");
        Debug.Log("Returned result:"+vObj);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetResultsFromPluginTest(Message);
        }
    }

}
