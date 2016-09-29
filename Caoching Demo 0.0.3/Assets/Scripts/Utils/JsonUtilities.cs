
/**
* @file JsonUtilities.cs
* @brief Contains the JsonUtilities class
* @author Mohammed Haider(mohammed@heddoko.com)
* Copyright Heddoko(TM) 2015, all rights reserved
* @date June 2015
*/
using Newtonsoft.Json;
using System.IO;
using System; 

namespace Assets.Scripts.Utils
{

    /**
    * JsonUtilities class 
    * @brief Class that provides json utilities to the interested clients
    */

    public class JsonUtilities
    {

        /**
        * ConvertObjectToJson(string vPath, object vObj)
        * @param string vPath: the path where to save the file, and the object to serialize into json
        * @brief  Converts an object to a json string and saves it locally with the given path parameter.
        * @note  Will throw an exception if an object passed is null 
        */
        public static void ConvertObjectToJson(string vPath, object vObj)
        {
            if (vObj == null)
            {
                throw new NullValuePassedException();
            }
            JsonSerializer vSerializer = new JsonSerializer();
            StreamWriter vStreamWriter = new StreamWriter(vPath);
            vSerializer.NullValueHandling = NullValueHandling.Ignore;
            vSerializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            vSerializer.Formatting = Formatting.Indented;
            using (JsonWriter vJsonWriter = new JsonTextWriter(vStreamWriter))
            {
                vSerializer.Serialize(vJsonWriter, vObj);
            }
        }
        /// <summary>
        /// Serialize an object to a json string
        /// </summary>
        /// <param name="vObj">the object to serialize</param>
        /// <returns>the json formatted string</returns>
        public static string SerializeObjToJson(object vObj)
        {
             string vJsonString = JsonConvert.SerializeObject(vObj, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore

                });

            return vJsonString;//vStringBuilder.ToString();

        }
        /**
        * JsonFileToObject<T>(string vPath)
        * @param string vPath the data path where a local json file can be loaded
        * @brief  From the given file path, return an object of type T from a JSOn file
        * @return Returns a deserialized Json object of type T
        */
        public static T JsonFileToObject<T>(string vPath)
        {
            JsonSerializer vDeserializer = new JsonSerializer();
            JsonTextReader vJsonTxtReader = new JsonTextReader(File.OpenText(vPath));
            vDeserializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            T vDeserializedObj = (T)vDeserializer.Deserialize(vJsonTxtReader, typeof(T));
            return vDeserializedObj;
        }

    }
    /**
    * NullValuePassedException class 
    * @brief exception class that throws an exception when a null value was passed in
    */
    public class NullValuePassedException : Exception
    {
    }


}
