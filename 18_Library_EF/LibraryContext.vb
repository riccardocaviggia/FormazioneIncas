Imports System.Data.Entity

Public Class LibraryContext
    Inherits DbContext

    Public Sub New(connString As String)
        MyBase.New(connString)
    End Sub

    Public Property Books As DbSet(Of Book)
    Public Property Authors As DbSet(Of Author)
    Public Property Users As DbSet(Of User)
    Public Property BookLoans As DbSet(Of BookLoan)

    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        modelBuilder.Entity(Of Book)() _
            .HasMany(Of Author)(Function(b) b.Authors) _
            .WithMany(Function(a) a.Books) _
            .Map(Sub(m)
                     m.ToTable("BookAuthors") ' Nome della tabella nel DB
                     m.MapLeftKey("BookID")   ' Chiave lato Libro
                     m.MapRightKey("AuthorID") ' Chiave lato Autore
                 End Sub)

        MyBase.OnModelCreating(modelBuilder)
    End Sub

End Class
