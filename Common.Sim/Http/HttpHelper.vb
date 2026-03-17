Imports System.IO
Imports System.Net

Public Module HttpHelper
    Public Function HandleReadyAsync(context As HttpListenerContext) As Boolean
        If context.Request.HttpMethod = "GET" AndAlso
               context.Request.Url.AbsolutePath.TrimEnd("/"c).EndsWith("/ready", StringComparison.OrdinalIgnoreCase) Then
            context.Response.StatusCode = CInt(HttpStatusCode.OK)
            context.Response.ContentType = "text/plain"

            Using writer As New StreamWriter(context.Response.OutputStream)
                writer.Write("OK")
            End Using
            Return True
        End If
        Return False
    End Function
End Module
