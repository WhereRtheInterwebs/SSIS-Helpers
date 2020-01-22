using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public static class Extensions
{
    public static string MaxLength(this string Str, int MaxLength) =>
        Str.Length > MaxLength ? Str.Substring(0, MaxLength) : Str;
        
    public static string OnlyNumbers(this string Number)
    {
        //Format numbers to only contain digits
        var removeNonDigits = new Regex("(?![0-9]).");
        
        return removeNonDigits.Replace(Number, "");
    }
    
    public static string GetBlobText(this BlobColumn blob)
    {
        var removeLineBreaks = new Regex("\r\n?|\r?\n");
        var blobLength = Convert.ToInt32(blob.Length);
        var blobData = blob.GetBlobData(0, blobLength);
        var stringData = System.Text.Encoding.UTF8.GetString(blobData);
        var formattedStringData = removeLineBreaks.Replace(stringData, "^");
        return formattedStringData.MaxLength(999);
    }
    
    public static ReplaceNullsResult[] ReplaceNulls<T>(this T inputBuffer) where T : ScriptBuffer
    {
        PropertyInfo[] AllProperties = inputBuffer.GetType().GetProperties();
        PropertyInfo[] AllNullChecks = AllProperties.Where(x => x.PropertyType == typeof(bool) && 
                                                                x.Name.EndsWith("_IsNull"))
                                                    .ToArray();
        var Problems = new List<ReplaceNullsResult>();
        foreach (PropertyInfo NullCheck in AllNullChecks)
        {
            string ActualPropertyName = NullCheck.Name.Replace("_IsNull", "");
            PropertyInfo ActualProperty = AllProperties.Where(x => x.Name == ActualPropertyName).First();
            Type ActualPropertyType = ActualProperty.PropertyType;
            try
            {
                //If Null
                if ((bool)NullCheck.GetValue(inputBuffer, null))
                    if (ActualPropertyType == typeof(string))
                        ActualProperty.SetValue(inputBuffer, "", null);
                    else if (ActualPropertyType == typeof(Int32))
                        ActualProperty.SetValue(inputBuffer, 0, null);
                    else if (ActualPropertyType == typeof(DateTime))
                        ActualProperty.SetValue(inputBuffer, new DateTime(0), null);
                    else if (ActualPropertyType == typeof(bool))
                        ActualProperty.SetValue(inputBuffer, false, null);
            }
            catch (Exception ex)
            {
                Problems.Add(new ReplaceNullsResult(ReplaceNullsStatus.Fail, ActualPropertyName, ex));
            }
        }
        if (Problems.Count > 0)
            return Problems.ToArray();
        else 
            return new ReplaceNullsResult(ReplaceNullsStatus.Success).ToArray();
    }
}
