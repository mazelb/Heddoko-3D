/** 
* @file AppConfig.cs
* @brief Contains the AppConfig class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date August 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


/// <summary>
///  Provides a the configuration of the application
/// </summary>
public static class AppConfig
{
    /// <summary>
    /// The application version
    /// </summary>
    private static string sVersion = "1.0.0.0";

    /// <summary>
    /// Support email
    /// </summary>
    private static string sHeddokoSupportEmail = "support@heddoko.com";

    /// <summary>
    /// Sets the application version
    /// </summary>
    /// <param name="vVersion"></param>
    public static void SetVersion(string vVersion)
    {
        sVersion = "";
    }

    /// <summary>
    /// Return the application version
    /// </summary>
    public static string Version
    {
        get
        {
            return sVersion;
        }
    }

    /// <summary>
    /// returns the heddoko support email 
    /// </summary>
    public static string HeddokoSupportEmail
    {
        get
        {
            return sHeddokoSupportEmail;
        }
    }
}
