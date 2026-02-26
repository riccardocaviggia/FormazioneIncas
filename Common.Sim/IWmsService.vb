Imports System.ServiceModel

<ServiceContract>
Public Interface IWmsService
    <OperationContract>
    Function ProcessBarcode(request As WmsRequest) As WmsResponse
End Interface
