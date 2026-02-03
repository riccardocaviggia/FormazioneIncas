Module Module1

    Sub Main(ByVal args() As String)
        If args.Length <> 1 Then
            Console.WriteLine("Error: Provide only the configuration path as argument.")
        End If

        Dim connString As String = args(0)
        Dim products As List(Of Product)

        Using db As New Context(connString)
            Try
                products = db.Products.ToList()
            Catch ex As Exception
                Console.WriteLine("Error EF: " & ex.Message)
                Console.WriteLine(vbCrLf & "Press something to exit")
                Console.ReadKey()
            End Try
        End Using

        Console.WriteLine("--- LIST OF PRODUCTS ---")
        For Each p In products
            Console.WriteLine(p.ToString())
        Next

        Console.WriteLine(vbCrLf & "--- LIST OF PRODUCTS (SORTED)")
        Dim sorted = products.OrderBy(Function(p) p.ProductName)
        For Each p In sorted : Console.WriteLine(p.ToString()) : Next

        Console.WriteLine(vbCrLf & "--- PRODUCT WITH ID 10 ---")
        Dim productId5 = products.Find(Function(p) p.ProductID = 10)
        If productId5 IsNot Nothing Then Console.WriteLine(productId5.ToString())

        Console.WriteLine(vbCrLf & "--- PRODUCT WITH NAME 'QUESO'")
        Dim pQueso = products.FindAll(Function(p) p.ProductName.Contains("Queso"))
        For Each p In pQueso : Console.WriteLine(p.ToString()) : Next

        Console.WriteLine(vbCrLf & "--- PRODUCTS NAMES ---")
        Dim pNames = products.Select(Function(p) p.ProductName)
        For Each p In pNames : Console.WriteLine(p.ToString()) : Next

        Console.WriteLine(vbCrLf & "--- NUMBER OF PRODUCTS ---")
        Console.WriteLine(products.Count())

        Console.WriteLine(vbCrLf & "--- PRODUCT WITH A CERTAIN VALUE IN A CERTAIN PROPERTY ---")
        Dim price As Decimal = 9.5
        Dim pProperty = products.Find(Function(p) p.UnitPrice = price)
        If pProperty IsNot Nothing Then
            Console.WriteLine($"Product with price {price}: exists")
        Else Console.WriteLine($"Product with price {price}: doesn't exist")
        End If

        Console.WriteLine(vbCrLf & "--- PRODUCTS GROUPED BY CATEGORY")
        Dim pGrouped = products.GroupBy(Function(p) p.CategoryID)
        For Each p In pGrouped : Console.WriteLine($"Category: {p.Key} | Contains {p.Count} products") : Next

        Console.WriteLine(vbCrLf & "Press something to exit")
        Console.ReadKey()
    End Sub

End Module
