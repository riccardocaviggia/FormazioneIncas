Imports System.Net
Imports System.Security.Authentication
Imports System.Text

Public Class BasicAuthenticator
    Private ReadOnly _username As String
    Private ReadOnly _password As String
    Public _logger As ServiceLogger

    Public Sub New(username As String, password As String)
        If String.IsNullOrWhiteSpace(username) Then Throw New ArgumentNullException(NameOf(username))
        If String.IsNullOrWhiteSpace(password) Then Throw New ArgumentNullException(NameOf(password))

        _username = username
        _password = password
    End Sub

    '-----------------------------------------------------------------------------
    '- Valida l'header Authorization della richiesta
    Public Function IsAuthenticated(request As HttpListenerRequest) As Boolean

        Dim authHeader = request.Headers("Authorization")
        If String.IsNullOrWhiteSpace(authHeader) Then Return False
        If Not authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase) Then Return False

        Try
            Dim encoded = authHeader.Substring("Basic ".Length).Trim()
            Dim decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded))

            Dim separatorIndex = decoded.IndexOf(":"c)
            If separatorIndex < 0 Then Return False

            Dim username = decoded.Substring(0, separatorIndex)
            Dim password = decoded.Substring(separatorIndex + 1)

            Return String.Equals(username, _username, StringComparison.Ordinal) AndAlso
                     String.Equals(password, _password, StringComparison.Ordinal)
        Catch ex As AuthenticationException
            Return False
        End Try
    End Function

    '-----------------------------------------------------------------------------
    '- Scrive una risposta 401 Unauthorized con l'header WWW-Authenticate
    Public Shared Sub WriteUnauthorizedResponse(response As HttpListenerResponse, Optional realm As String = "WareHouse")
        response.StatusCode = CInt(HttpStatusCode.Unauthorized)
        response.StatusDescription = "Unauthorized"
        response.AddHeader("WWW-Authenticate", $"Basic realm=""{realm}""")
        response.Close()
    End Sub

    '-----------------------------------------------------------------------------
    '- Crea il valore dell'header Authorization per le credenziali fornite
    Public Shared Function BuildHeaderValue(username As String, password As String) As String
        Dim credentials = Encoding.UTF8.GetBytes(username & ":" & password)
        Return "Basic " & Convert.ToBase64String(credentials)
    End Function
End Class
