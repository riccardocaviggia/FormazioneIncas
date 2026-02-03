Imports ClassLibrary
Module Module1

    Sub Main(ByVal args() As String)
        If args.Length <> 1 Then
            Console.WriteLine("Error: Provide only the configuration path as argument.")
            Return
        End If

        Dim connString As String = args(0)
        Dim _crud = New SqlCrudManager(connString)
        Dim dt As DataTable = _crud.ReadMethod("SELECT ProductID, ProductName, UnitPrice, CategoryID FROM Products")

        Dim products As New List(Of Product)
        For Each row As DataRow In dt.Rows
            products.Add(New Product With {
                .ProductID = CInt(row("ProductID")),
                .ProductName = row("ProductName").ToString(),
                .UnitPrice = CDec(row("UnitPrice")),
                .CategoryID = CInt(row("CategoryID"))
            })
        Next

        Console.WriteLine("--- LIST OF PRODUCTS ---")
        products.ForEach(Sub(p) Console.WriteLine(p.ToString()))

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
        Dim pGrouped = products.OrderBy(Function(b) b.CategoryID).GroupBy(Function(p) p.CategoryID)
        For Each p In pGrouped : Console.WriteLine($"Category: {p.Key} | Contains {p.Count} products") : Next

        Console.WriteLine(vbCrLf & "Press something to exit")
        Console.ReadKey()
    End Sub

End Module
