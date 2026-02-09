Module Module1

    Sub Main(ByVal args() As String)
        If args.Length <> 1 Then
            Console.WriteLine("Error: provide only the configuration path as argument.")
        End If

        Dim connString As String = args(0)
        Dim authors As List(Of Author)
        Dim books As List(Of Book)
        Dim users As List(Of User)
        Dim loans As New List(Of String)

        Using db As New LibraryContext(connString)
            If db.Database.CreateIfNotExists() Then
                Console.WriteLine("Database successfully created!")
            End If
            Console.WriteLine("Successfull database connection!")

            If Not db.Authors.Any() Then
                Dim eco As New Author With {.AuthorName = "Umberto", .AuthorSurname = "Eco"}
                Dim calvino As New Author With {.AuthorName = "Italo", .AuthorSurname = "Calvino"}
                Dim ginzburg As New Author With {.AuthorName = "Natalia", .AuthorSurname = "Ginzburg"}
                Dim levi As New Author With {.AuthorName = "Primo", .AuthorSurname = "Levi"}
                Dim leopardi As New Author With {.AuthorName = "Giacomo", .AuthorSurname = "Leopardi"}
                Dim deledda As New Author With {.AuthorName = "Grazia", .AuthorSurname = "Deledda"}
                Dim dante As New Author With {.AuthorName = "Dante", .AuthorSurname = "Alighieri"}
                Dim boccaccio As New Author With {.AuthorName = "Giovanni", .AuthorSurname = "Boccaccio"}
                Dim manzoni As New Author With {.AuthorName = "Alessandro", .AuthorSurname = "Manzoni"}
                Dim fruttero As New Author With {.AuthorName = "Carlo", .AuthorSurname = "Fruttero"}
                Dim lucentini As New Author With {.AuthorName = "Franco", .AuthorSurname = "Lucentini"}

                Dim b1 As New Book With {.BookTitle = "Il nome della rosa", .BookISBN = "9788845278655"}
                b1.Authors.Add(eco)
                b1.Authors.Add(calvino)

                Dim b2 As New Book With {.BookTitle = "Il pendolo di Foucault", .BookISBN = "9788845278656"}
                b2.Authors.Add(eco)

                Dim b3 As New Book With {.BookTitle = "Baudolino", .BookISBN = "9788845278657"}
                b3.Authors.Add(eco)

                Dim b4 As New Book With {.BookTitle = "Il cimitero di Praga", .BookISBN = "9788845278658"}
                b4.Authors.Add(eco)

                Dim b5 As New Book With {.BookTitle = "Il barone rampante", .BookISBN = "9788804668231"}
                b5.Authors.Add(calvino)

                Dim b6 As New Book With {.BookTitle = "Le città invisibili", .BookISBN = "9788804668232"}
                b6.Authors.Add(calvino)

                Dim b7 As New Book With {.BookTitle = "Lessico famigliare", .BookISBN = "9788804668233"}
                b7.Authors.Add(ginzburg)

                Dim b8 As New Book With {.BookTitle = "Se questo è un uomo", .BookISBN = "9788804668234"}
                b8.Authors.Add(levi)

                Dim b9 As New Book With {.BookTitle = "La tregua", .BookISBN = "9788804668235"}
                b9.Authors.Add(levi)

                Dim b10 As New Book With {.BookTitle = "Canti", .BookISBN = "9788804668236"}
                b10.Authors.Add(leopardi)

                Dim b11 As New Book With {.BookTitle = "Canne al vento", .BookISBN = "9788804668237"}
                b11.Authors.Add(deledda)

                Dim b12 As New Book With {.BookTitle = "Divina Commedia", .BookISBN = "9788804668238"}
                b12.Authors.Add(dante)

                Dim b13 As New Book With {.BookTitle = "Decameron", .BookISBN = "9788804668239"}
                b13.Authors.Add(boccaccio)

                Dim b14 As New Book With {.BookTitle = "I Promessi Sposi", .BookISBN = "9788804668240"}
                b14.Authors.Add(manzoni)

                Dim b15 As New Book With {.BookTitle = "La donna della domenica", .BookISBN = "9788804668241"}
                b15.Authors.Add(fruttero)
                b15.Authors.Add(lucentini)

                Dim u1 As New User With {.UserName = "Mario", .UserSurname = "Rossi", .UserEmail = "mario.rossi@email.it"}
                Dim u2 As New User With {.UserName = "Giulia", .UserSurname = "Verdi", .UserEmail = "giulia.verdi@email.it"}
                Dim u3 As New User With {.UserName = "Luca", .UserSurname = "Neri", .UserEmail = "luca.neri@email.it"}
                Dim u4 As New User With {.UserName = "Elena", .UserSurname = "Bianchi", .UserEmail = "elena.bianchi@email.it"}
                Dim u5 As New User With {.UserName = "Marco", .UserSurname = "Gialli", .UserEmail = "marco.gialli@email.it"}

                db.BookLoans.Add(New BookLoan With {.Book = b1, .User = u1, .LoanDate = New Date(2026, 1, 5), .ReturnDate = New Date(2026, 1, 20)})
                db.BookLoans.Add(New BookLoan With {.Book = b2, .User = u1, .LoanDate = New Date(2026, 1, 21), .ReturnDate = New Date(2026, 2, 5)})
                db.BookLoans.Add(New BookLoan With {.Book = b3, .User = u1, .LoanDate = New Date(2026, 2, 6), .ReturnDate = New Date(2026, 2, 28)})
                db.BookLoans.Add(New BookLoan With {.Book = b4, .User = u1, .LoanDate = New Date(2026, 3, 1), .ReturnDate = Nothing})
                db.BookLoans.Add(New BookLoan With {.Book = b5, .User = u2, .LoanDate = New Date(2026, 1, 10), .ReturnDate = New Date(2026, 1, 25)})
                db.BookLoans.Add(New BookLoan With {.Book = b6, .User = u2, .LoanDate = New Date(2026, 2, 1), .ReturnDate = Nothing})
                db.BookLoans.Add(New BookLoan With {.Book = b12, .User = u3, .LoanDate = New Date(2025, 12, 15), .ReturnDate = New Date(2026, 1, 10)})
                db.BookLoans.Add(New BookLoan With {.Book = b14, .User = u3, .LoanDate = New Date(2026, 1, 15), .ReturnDate = Nothing})
                db.BookLoans.Add(New BookLoan With {.Book = b8, .User = u4, .LoanDate = New Date(2026, 2, 10), .ReturnDate = New Date(2026, 3, 1)})
                db.BookLoans.Add(New BookLoan With {.Book = b9, .User = u4, .LoanDate = New Date(2026, 3, 5), .ReturnDate = Nothing})
                db.BookLoans.Add(New BookLoan With {.Book = b11, .User = u5, .LoanDate = New Date(2026, 1, 20), .ReturnDate = New Date(2026, 2, 15)})
                db.BookLoans.Add(New BookLoan With {.Book = b7, .User = u5, .LoanDate = New Date(2026, 2, 20), .ReturnDate = Nothing})
                db.BookLoans.Add(New BookLoan With {.Book = b13, .User = u1, .LoanDate = New Date(2026, 3, 10), .ReturnDate = Nothing})

                db.Books.Add(b10)
                db.Books.Add(b15)

                db.SaveChanges()

                Console.WriteLine("Data add correctly")
            Else
                Console.WriteLine("Data already exist!")
            End If

            Try
                authors = db.Authors.ToList()
                books = db.Books.ToList()
                users = db.Users.ToList()

                Console.WriteLine(vbCrLf & "--- Books currently in loan ---")
                For Each b In books
                    For Each loan In b.Loans
                        If loan.ReturnDate Is Nothing Then
                            Console.WriteLine($"{b.BookTitle}")
                        End If
                    Next
                Next

                Console.WriteLine(vbCrLf & "--- Users with more than 3 loans ---")
                For Each u In users
                    If u.BookLoans.Count > 3 Then
                        Console.WriteLine($"{u.UserName} {u.UserSurname}")
                    End If
                Next

                Console.WriteLine(vbCrLf & "--- Authors with more than 3 books ---")
                For Each a In authors
                    If a.Books.Count > 3 Then
                        Console.WriteLine($"{a.AuthorName} {a.AuthorSurname} ({a.Books.Count} Books)")
                    End If
                Next
            Catch ex As Exception
                Console.WriteLine("Errore: " & ex.Message)
                Console.WriteLine(vbCrLf & "Press something to exit")
                Console.ReadKey()
            End Try
        End Using



        Console.WriteLine(vbCrLf & "Press something to exit")
        Console.ReadKey()
    End Sub

End Module
