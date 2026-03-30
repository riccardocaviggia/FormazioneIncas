Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Services.Protocols

Public Class CommaToDotSoapExtension
    Inherits SoapExtension

    Private oldStream As Stream
    Private newStream As MemoryStream

    Public Overrides Function ChainStream(stream As Stream) As Stream
        oldStream = stream
        newStream = New MemoryStream()
        Return newStream
    End Function

    Public Overrides Sub ProcessMessage(message As SoapMessage)
        Select Case message.Stage
            Case SoapMessageStage.BeforeDeserialize
                If oldStream Is Nothing Then
                    Return
                End If

                Try
                    ' Leggi tutti i byte dall'oldStream
                    If oldStream.CanSeek Then
                        oldStream.Position = 0
                    End If
                    Dim inBytes As Byte()
                    Using ms As New MemoryStream()
                        oldStream.CopyTo(ms)
                        inBytes = ms.ToArray()
                    End Using

                    Dim encoding As Encoding = DetectEncodingFromXmlProlog(inBytes)
                    Dim soap As String = encoding.GetString(inBytes)

                    ' Sostituisce solo le virgole tra cifre: 123,45 -> 123.45
                    Dim replaced As String = Regex.Replace(soap, "(?<=\d),(?=\d)", ".")

                    Dim outBytes As Byte() = encoding.GetBytes(replaced)
                    newStream.SetLength(0)
                    newStream.Write(outBytes, 0, outBytes.Length)
                    newStream.Position = 0

#If DEBUG Then
                    ' file di debug temporaneo (per ispezionare il corpo in uscita)
                    Try
                        File.WriteAllText(Path.Combine(Path.GetTempPath(), "soap_before_deserialize.txt"), replaced, Encoding.UTF8)
                    Catch
                    End Try
#End If
                Catch ex As Exception
                    ' Non interrompere il flusso in produzione: loggare se necessario
                End Try

            Case SoapMessageStage.AfterSerialize
                If newStream Is Nothing OrElse oldStream Is Nothing Then
                    Return
                End If

                Try
                    If newStream.CanSeek Then
                        newStream.Position = 0
                    End If

                    Dim outBytes As Byte()
                    Using ms As New MemoryStream()
                        newStream.CopyTo(ms)
                        outBytes = ms.ToArray()
                    End Using

                    Dim encoding As Encoding = DetectEncodingFromXmlProlog(outBytes)
                    Dim soapOut As String = encoding.GetString(outBytes)

#If DEBUG Then
                    Try
                        File.WriteAllText(Path.Combine(Path.GetTempPath(), "soap_after_serialize.txt"), soapOut, Encoding.UTF8)
                    Catch
                    End Try
#End If

                    Dim finalBytes As Byte() = encoding.GetBytes(soapOut)

                    ' Scrive sullo stream originale in modo robusto
                    Try
                        If oldStream.CanSeek Then
                            oldStream.SetLength(0)
                            oldStream.Position = 0
                        End If
                    Catch
                        ' se SetLength non è supportato, procederemo comunque a scrivere
                    End Try

                    oldStream.Write(finalBytes, 0, finalBytes.Length)
                    Try
                        If oldStream.CanSeek Then
                            oldStream.Position = 0
                        End If
                    Catch
                    End Try
                Catch ex As Exception
                    ' log/ignore
                End Try
        End Select
    End Sub

    Public Overrides Function GetInitializer(methodInfo As LogicalMethodInfo, attribute As SoapExtensionAttribute) As Object
        Return Nothing
    End Function

    Public Overrides Function GetInitializer(serviceType As Type) As Object
        Return Nothing
    End Function

    Public Overrides Sub Initialize(initializer As Object)
        ' niente da inizializzare
    End Sub

    Private Function DetectEncodingFromXmlProlog(bytes As Byte()) As Encoding
        If bytes Is Nothing OrElse bytes.Length = 0 Then
            Return Encoding.UTF8
        End If

        ' Proviamo a leggere i primi 1024 byte come ASCII per trovare il prolog
        Dim probeLen As Integer = Math.Min(bytes.Length, 1024)
        Dim probe As String = Encoding.ASCII.GetString(bytes, 0, probeLen)
        Dim m As Match = Regex.Match(probe, "encoding\s*=\s*[""'](?<enc>[^""']+)[""']", RegexOptions.IgnoreCase)
        If m.Success Then
            Try
                Dim encName As String = m.Groups("enc").Value
                Return Encoding.GetEncoding(encName)
            Catch
                Return Encoding.UTF8
            End Try
        End If

        ' fallback: se il prolog non contiene encoding, default UTF-8
        Return Encoding.UTF8
    End Function
End Class