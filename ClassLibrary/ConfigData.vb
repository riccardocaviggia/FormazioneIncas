<System.ComponentModel.TypeConverter(GetType(System.ComponentModel.ExpandableObjectConverter))>
Public Class ConfigData
    Public Property ConnDEV As String = ""
    Public Property ConnPROD As String = ""
    Public Property ConnHost As String = ""
End Class

Public Class AppConfig
    Public Property ConnectionStrings As New ConfigData()
    Public Property ServicePort As Integer = 8080
    Public Property LastUser As String = "northwind_user"
End Class
