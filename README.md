# SSIS-Helpers
These are helpful functions to have in an SSIS data flow script task.  

1. Be sure to set SSIS data fields as read/write in properties of script task .  
2. Add Extensions.cs and ReplaceNullsResult.cs to script task project.

This function eliminates the need to check every data field for a null with _IsNull.  For projects with a lot of data fields, this can be a pain and makes the code hard to read.

Eliminates this error:  *"Script Component: RunTime Error  The column has NULL Value"*

You can edit the default value for each datatype in Extensions.cs.  

# Null Handling
#### Replaces nulls with default values from Extensions.cs
````c#
public override void Input0_ProcessInputRow(Input0Buffer Row)
{
  Row.ReplaceNulls();
  
  //More code
}
````

# Null Handling with Error Logging
#### Outputs which field had a problem to the package log.  Problem could be data type other than what is accounted for in Extensions.cs, or you forgot to set field as read/write in script task properties.
```c#
private bool Cancel;

public override void Input0_ProcessInputRow(Input0Buffer Row)
{
    ReplaceNullsResult[] results = Row.ReplaceNulls();
    foreach (var result in results)
        if (result.ReplaceNullsStatus == ReplaceNullsStatus.Fail)
            ComponentMetaData.FireError(1, "Failed to replace nulls", $"Failed to replace nulls on property: {result.Property}", "", 0, out Cancel);
            
    if (results[0].ReplaceNullsStatus == ReplaceNullsStatus.Fail)
        throw results[0].Exception;
        
    //More code
}
````
