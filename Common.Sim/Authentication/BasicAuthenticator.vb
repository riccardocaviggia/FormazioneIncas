Imports System.Net
Imports System.Net.Http.Headers
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

        Dim authHeaderString = request.Headers("Authorization")
        If String.IsNullOrWhiteSpace(authHeaderString) Then Return False

        ' AuthenticationHeaderValue per validare il formato dell'header e decodificare le credenziali
        Dim authHeader = AuthenticationHeaderValue.Parse(authHeaderString)
        If Not authHeaderString.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase) Then Return False

        Try
            Dim credentialBytes = Convert.FromBase64String(authHeader.Parameter)
            Dim credentials = Encoding.UTF8.GetString(credentialBytes).Split(":"c)

            If credentials.Length <> 2 Then Return False

            Dim username = credentials(0)
            Dim password = credentials(1)

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
