Imports System
Imports System.Web.Services.Protocols

<AttributeUsage(AttributeTargets.Method Or AttributeTargets.Class)>
Public Class CommaToDotExtensionAttribute
    Inherits SoapExtensionAttribute

    Private _Priority As Integer

    Public Overrides ReadOnly Property ExtensionType As Type
        Get
            Return GetType(CommaToDotSoapExtension)
        End Get
    End Property

    Public Overrides Property Priority As Integer
        Get
            Return _Priority
        End Get
        Set(value As Integer)
            _Priority = value
        End Set
    End Property
End Class