 /**
 * @file LocalizedString.cs
* @brief Contains the  LocalizedString class 
 * @author Mohammed Haider( mohammed @heddoko.com)
 * @date 09 2016
* Copyright Heddoko(TM) 2016,  all rights reserved
 */
namespace Assets.Scripts.Localization
{
    /// <summary>
    /// Model of localization
    /// </summary>
    public class LocalizedString
    {
        public string SingularForm;
        public string PluralForm;

        /// <summary>
        /// Returns  the string based whether it is plural or singular
        /// </summary>
        /// <param name="vIsPlural">do you want the plural form of the localized string?</param>
        /// <returns></returns>
        public string GetString(bool vIsPlural =false)
        {
            var vMsg = vIsPlural ? PluralForm : SingularForm;
            if (vIsPlural)
            {
                if (string.IsNullOrEmpty(PluralForm))
                {
                    vMsg = "";
                }
            }
            return vMsg;
        }
    }
}