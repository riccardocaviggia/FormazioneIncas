Public Class BookLoan
    Public Property BookLoanID As Integer
    Public Property BookID As Integer
    Public Property UserID As Integer
    Public Property LoanDate As Date = Date.Now()
    Public Property ReturnDate As Date?

    Public Overridable Property Book As Book
    Public Overridable Property User As User
End Class
