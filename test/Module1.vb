Imports System.Configuration

Module Module1

    Sub Main()
        Console.WriteLine(Environment.Is64BitProcess)
        Dim cs = ConfigurationManager.ConnectionStrings("WareHouseDB")
        If cs Is Nothing Then
            Console.WriteLine("Connection string 'WareHouseDB' not found.")
        Else
            Console.WriteLine("Connection string 'WareHouseDB' found: " & cs.ConnectionString)
        End If

        Console.WriteLine("Press any key to exit...")
        Console.ReadLine()
    End Sub

End Module
