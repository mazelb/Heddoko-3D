/**
* @file SuitsCommunicationMgr.cs
* @brief Contains the SuitsCommunicationManager class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/
using UnityEngine;
using System.Collections;
using Assets.Scripts.Interfaces;

/**
* SuitsCommunicationMgr class 
* @brief class to manage all connections with paired suits
*/
public class SuitsCommunicationMgr : MonoBehaviour
{
    #region Singleton definition
    private static readonly SuitsCommunicationMgr instance = new SuitsCommunicationMgr();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static SuitsCommunicationMgr()
    {
    }

    private SuitsCommunicationMgr()
    {
    }

    public static SuitsCommunicationMgr Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

}
