using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ReplaceNullsResult
{
    public ReplaceNullsStatus ReplaceNullsStatus;
    public string Property;
    public Exception Exception;
    
    public ReplaceNullsResult(ReplaceNullsStatus replaceNullsStatus, string property = "", Exception exception = null)
    {
        ReplaceNullsStatus = replaceNullsStatus;
        Property = property;
        Exception = exception;
    }
    
    public ReplaceNullsResult[] ToArray() => 
        new ReplaceNullsResult[1] { this };
}

public enum ReplaceNullsStatus
{
    Success,
    Fail
}
