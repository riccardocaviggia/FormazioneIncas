Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://module1.com/calculator")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Calculator
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function Add(a As Double, b As Double) As Double
        Return a + b
    End Function

End Class