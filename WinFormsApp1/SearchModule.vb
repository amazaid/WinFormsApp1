Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Text.RegularExpressions
Imports System.Data.SQLite
Imports iText.Kernel.Pdf
Imports iText.Kernel.Pdf.Canvas.Parser
Imports iText.Kernel.Pdf.Canvas.Parser.Listener

Public Class SearchModule
    Private _searchIndex As SearchIndex
    Private _cancellationTokenSource As CancellationTokenSource

    Public Event SearchProgress(message As String, filesProcessed As Integer, totalFiles As Integer)
    Public Event SearchComplete(results As List(Of SearchResult))

    Public Sub New()
        _searchIndex = New SearchIndex()
    End Sub

    Public Async Function SearchAsync(searchPath As String, searchTerm As String, searchInContent As Boolean, searchPDFs As Boolean) As Task(Of List(Of SearchResult))
        _cancellationTokenSource = New CancellationTokenSource()
        Dim results As New List(Of SearchResult)

        Try
            RaiseEvent SearchProgress("Collecting files...", 0, 0)

            Dim files = GetAllFiles(searchPath, searchTerm, _cancellationTokenSource.Token)
            Dim totalFiles = files.Count
            Dim filesProcessed = 0

            Dim tasks As New List(Of Task)
            Dim semaphore As New SemaphoreSlim(4) ' Process 4 files concurrently

            For Each filePath In files
                If _cancellationTokenSource.Token.IsCancellationRequested Then
                    Exit For
                End If

                tasks.Add(Task.Run(Async Function()
                    Await semaphore.WaitAsync()
                    Try
                        Dim result = ProcessFile(filePath, searchTerm, searchInContent, searchPDFs)
                        If result IsNot Nothing Then
                            SyncLock results
                                results.Add(result)
                            End SyncLock
                        End If

                        Interlocked.Increment(filesProcessed)
                        RaiseEvent SearchProgress($"Processing: {Path.GetFileName(filePath)}", filesProcessed, totalFiles)
                    Finally
                        semaphore.Release()
                    End Try
                End Function))
            Next

            Await Task.WhenAll(tasks)
            RaiseEvent SearchComplete(results)

        Catch ex As Exception
            RaiseEvent SearchProgress($"Error: {ex.Message}", 0, 0)
        End Try

        Return results
    End Function

    Public Sub CancelSearch()
        _cancellationTokenSource?.Cancel()
    End Sub

    Private Function GetAllFiles(searchPath As String, pattern As String, cancellationToken As CancellationToken) As List(Of String)
        Dim files As New List(Of String)
        Dim searchPattern = If(String.IsNullOrEmpty(pattern), "*", $"*{pattern}*")

        Try
            SearchDirectory(searchPath, searchPattern, files, cancellationToken)
        Catch ex As Exception
            ' Handle access denied exceptions
        End Try

        Return files
    End Function

    Private Sub SearchDirectory(path As String, pattern As String, files As List(Of String), cancellationToken As CancellationToken)
        If cancellationToken.IsCancellationRequested Then Return

        Try
            ' Add files matching pattern
            For Each file In Directory.GetFiles(path)
                If System.IO.Path.GetFileName(file).ToLower().Contains(pattern.Replace("*", "").ToLower()) Then
                    files.Add(file)
                End If
            Next

            ' Also add all files if we're searching content
            If pattern = "*" Then
                files.AddRange(Directory.GetFiles(path))
            End If

            ' Recursively search subdirectories
            For Each myDir In Directory.GetDirectories(path)
                SearchDirectory(myDir, pattern, files, cancellationToken)
            Next
        Catch ex As UnauthorizedAccessException
            ' Skip directories we can't access
        End Try
    End Sub

    Private Function ProcessFile(filePath As String, searchTerm As String, searchInContent As Boolean, searchPDFs As Boolean) As SearchResult
        Try
            Dim fileInfo As New FileInfo(filePath)
            Dim result As New SearchResult With {
                .FilePath = filePath,
                .FileName = fileInfo.Name,
                .FileSize = fileInfo.Length,
                .LastModified = fileInfo.LastWriteTime
            }

            ' Check filename
            If fileInfo.Name.ToLower().Contains(searchTerm.ToLower()) Then
                result.MatchType = MatchType.FileName
                result.MatchContext = fileInfo.Name
                Return result
            End If

            ' Check file content if enabled
            If searchInContent Then
                Dim content As String = ""
                Dim extension = fileInfo.Extension.ToLower()

                If extension = ".pdf" AndAlso searchPDFs Then
                    content = ExtractPDFText(filePath)
                ElseIf IsTextFile(extension) Then
                    content = File.ReadAllText(filePath)
                End If

                If Not String.IsNullOrEmpty(content) Then
                    Dim index = content.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase)
                    If index >= 0 Then
                        result.MatchType = MatchType.FileContent
                        result.MatchContext = GetContextAroundMatch(content, index, searchTerm.Length)
                        result.LineNumber = GetLineNumber(content, index)
                        Return result
                    End If
                End If
            End If

        Catch ex As Exception
            ' Skip files we can't process
        End Try

        Return Nothing
    End Function

    Private Function ExtractPDFText(pdfPath As String) As String
        Try
            Using reader As New PdfReader(pdfPath)
                Using pdfDoc As New PdfDocument(reader)
                    Dim text As New System.Text.StringBuilder()

                    For i = 1 To pdfDoc.GetNumberOfPages()
                        Dim page = pdfDoc.GetPage(i)
                        Dim strategy As New SimpleTextExtractionStrategy()
                        Dim pageText = PdfTextExtractor.GetTextFromPage(page, strategy)
                        text.AppendLine($"[Page {i}]")
                        text.AppendLine(pageText)
                    Next

                    Return text.ToString()
                End Using
            End Using
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Function IsTextFile(extension As String) As Boolean
        Dim textExtensions = {".txt", ".vb", ".cs", ".xml", ".json", ".config",
                             ".html", ".htm", ".css", ".js", ".ts", ".md",
                             ".yaml", ".yml", ".ini", ".log", ".bat", ".ps1"}
        Return textExtensions.Contains(extension.ToLower())
    End Function

    Private Function GetContextAroundMatch(content As String, matchIndex As Integer, matchLength As Integer) As String
        Dim contextBefore = 50
        Dim contextAfter = 50

        Dim startIndex = Math.Max(0, matchIndex - contextBefore)
        Dim endIndex = Math.Min(content.Length, matchIndex + matchLength + contextAfter)

        Dim context = content.Substring(startIndex, endIndex - startIndex)

        ' Add ellipsis if truncated
        If startIndex > 0 Then context = "..." & context
        If endIndex < content.Length Then context = context & "..."

        Return context.Replace(vbCrLf, " ").Replace(vbLf, " ").Replace(vbCr, " ")
    End Function

    Private Function GetLineNumber(content As String, index As Integer) As Integer
        Dim lineNumber = 1
        For i = 0 To Math.Min(index - 1, content.Length - 1)
            If content(i) = vbLf Then
                lineNumber += 1
            End If
        Next
        Return lineNumber
    End Function
End Class

Public Class SearchResult
    Public Property FilePath As String
    Public Property FileName As String
    Public Property FileSize As Long
    Public Property LastModified As DateTime
    Public Property MatchType As MatchType
    Public Property MatchContext As String
    Public Property LineNumber As Integer

    Public ReadOnly Property DisplayText As String
        Get
            Dim sizeText = GetFileSizeText(FileSize)
            If MatchType = MatchType.FileName Then
                Return $"{FileName} [{sizeText}]"
            Else
                Return $"{FileName} - Line {LineNumber}: {MatchContext} [{sizeText}]"
            End If
        End Get
    End Property

    Private Function GetFileSizeText(bytes As Long) As String
        If bytes < 1024 Then
            Return $"{bytes} B"
        ElseIf bytes < 1048576 Then
            Return $"{bytes / 1024:F1} KB"
        Else
            Return $"{bytes / 1048576:F1} MB"
        End If
    End Function
End Class

Public Enum MatchType
    FileName
    FileContent
End Enum

Public Class SearchIndex
    Private _dbPath As String
    Private _connection As SQLiteConnection

    Public Sub New()
        _dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FileExplorer", "searchindex.db")
        Directory.CreateDirectory(Path.GetDirectoryName(_dbPath))
        InitializeDatabase()
    End Sub

    Private Sub InitializeDatabase()
        _connection = New SQLiteConnection($"Data Source={_dbPath};Version=3;")
        _connection.Open()

        Using cmd As New SQLiteCommand(_connection)
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS FileIndex (
                FilePath TEXT PRIMARY KEY,
                FileName TEXT,
                Content TEXT,
                FileSize INTEGER,
                LastModified TEXT,
                LastIndexed TEXT
            )"
            cmd.ExecuteNonQuery()

            cmd.CommandText = "CREATE INDEX IF NOT EXISTS idx_filename ON FileIndex(FileName)"
            cmd.ExecuteNonQuery()

            ' Try to create FTS5 table, fall back to FTS4 or FTS3 if not available
            Try
                cmd.CommandText = "CREATE VIRTUAL TABLE IF NOT EXISTS FileContent USING fts5(FilePath, Content)"
                cmd.ExecuteNonQuery()
            Catch ex As SQLiteException
                Try
                    ' Try FTS4
                    cmd.CommandText = "CREATE VIRTUAL TABLE IF NOT EXISTS FileContent USING fts4(FilePath, Content)"
                    cmd.ExecuteNonQuery()
                Catch ex2 As SQLiteException
                    ' Skip FTS table creation - will use LIKE queries instead
                    Console.WriteLine("FTS not available, using standard search")
                End Try
            End Try
        End Using
    End Sub

    Public Sub IndexFile(filePath As String, content As String)
        Try
            Dim fileInfo As New FileInfo(filePath)

            Using cmd As New SQLiteCommand(_connection)
                cmd.CommandText = "INSERT OR REPLACE INTO FileIndex VALUES (@path, @name, @content, @size, @modified, @indexed)"
                cmd.Parameters.AddWithValue("@path", filePath)
                cmd.Parameters.AddWithValue("@name", fileInfo.Name)
                cmd.Parameters.AddWithValue("@content", content)
                cmd.Parameters.AddWithValue("@size", fileInfo.Length)
                cmd.Parameters.AddWithValue("@modified", fileInfo.LastWriteTime)
                cmd.Parameters.AddWithValue("@indexed", DateTime.Now)
                cmd.ExecuteNonQuery()

                ' Try to update FTS table if it exists
                Try
                    cmd.CommandText = "INSERT OR REPLACE INTO FileContent VALUES (@path, @content)"
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@path", filePath)
                    cmd.Parameters.AddWithValue("@content", content)
                    cmd.ExecuteNonQuery()
                Catch ex As SQLiteException
                    ' FTS table doesn't exist, skip FTS indexing
                End Try
            End Using
        Catch ex As Exception
            ' Handle indexing errors
        End Try
    End Sub

    Public Function SearchIndex(searchTerm As String) As List(Of SearchResult)
        Dim results As New List(Of SearchResult)

        Try
            Using cmd As New SQLiteCommand(_connection)
                ' First check if FTS table exists
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='FileContent'"
                Dim ftsExists = cmd.ExecuteScalar() IsNot Nothing

                If ftsExists Then
                    ' Use FTS search
                    cmd.CommandText = "SELECT FilePath, FileName, FileSize, LastModified, snippet(FileContent, 1, '...', '...', '...', 20) as Context
                                      FROM FileIndex
                                      JOIN FileContent ON FileIndex.FilePath = FileContent.FilePath
                                      WHERE FileContent MATCH @term
                                      LIMIT 100"
                Else
                    ' Fall back to LIKE search
                    cmd.CommandText = "SELECT FilePath, FileName, FileSize, LastModified,
                                      SUBSTR(Content, MAX(1, INSTR(LOWER(Content), LOWER(@term)) - 50), 150) as Context
                                      FROM FileIndex
                                      WHERE LOWER(Content) LIKE '%' || LOWER(@term) || '%'
                                      OR LOWER(FileName) LIKE '%' || LOWER(@term) || '%'
                                      LIMIT 100"
                End If

                cmd.Parameters.AddWithValue("@term", searchTerm)

                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        results.Add(New SearchResult With {
                            .FilePath = reader.GetString(0),
                            .FileName = reader.GetString(1),
                            .FileSize = reader.GetInt64(2),
                            .LastModified = DateTime.Parse(reader.GetString(3)),
                            .MatchType = MatchType.FileContent,
                            .MatchContext = If(reader.IsDBNull(4), "", reader.GetString(4))
                        })
                    End While
                End Using
            End Using
        Catch ex As Exception
            ' Handle search errors
            Console.WriteLine($"Search error: {ex.Message}")
        End Try

        Return results
    End Function

    Public Sub Close()
        _connection?.Close()
    End Sub
End Class