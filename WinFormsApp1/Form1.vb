
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Linq
Imports System.Threading.Tasks
Imports Microsoft.Web.WebView2.Core

Public Class Form1
    Private _searchModule As SearchModule
    Private _currentSearchTask As Task(Of List(Of SearchResult))

    ' --- Windows API Code for extracting file icons ---



    ' SHGetFileInfo function flags
    <Flags()>
    Public Enum SHGFI As UInteger
        Icon = &H100
        DisplayName = &H200
        TypeName = &H400
        Attributes = &H800
        IconLocation = &H1000
        ExeType = &H2000
        SysIconIndex = &H4000
        LinkOverlay = &H8000
        Selected = &H10000
        Attr_Specified = &H20000
        LargeIcon = &H0
        SmallIcon = &H1
        OpenIcon = &H2
        ShellIconSize = &H4
        PIDL = &H8
        UseFileAttributes = &H10
    End Enum

    ' SHFILEINFO structure
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Public Structure SHFILEINFO
        Public hIcon As IntPtr
        Public iIcon As Integer
        Public dwAttributes As UInteger
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)>
        Public szDisplayName As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=80)>
        Public szTypeName As String
    End Structure

    ' The SHGetFileInfo function from the Shell32 library
    <DllImport("shell32.dll", CharSet:=CharSet.Auto)>
    Public Shared Function SHGetFileInfo(
    pszPath As String,
    dwFileAttributes As UInteger,
    ByRef psfi As SHFILEINFO,
    cbFileInfo As UInteger,
    uFlags As SHGFI
) As IntPtr
    End Function

    ' Add this with your other API declarations (like SHGetFileInfo)
    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Public Shared Function DestroyIcon(hIcon As IntPtr) As Boolean
    End Function
    ' --- End of API Code ---

    ' Dictionary to cache icons. The key is the file extension (e.g., ".txt")
    ' and the value is the index in the ImageList.
    Private iconCache As New Dictionary(Of String, Integer)()
    Private startingPath As String = "C:\Users"

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        ' Create and configure the file dialog
        Using openFileDialog As New OpenFileDialog
            openFileDialog.InitialDirectory = "c:\"
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*"
            openFileDialog.FilterIndex = 1
            openFileDialog.RestoreDirectory = True

            ' Show the dialog and check if the user selected a file
            If openFileDialog.ShowDialog = DialogResult.OK Then
                ' Get the path of specified file
                Dim filePath = openFileDialog.FileName

                ' Load the PDF file into the WebView2 control
                ' The URI must be an absolute path
                If filePath IsNot Nothing AndAlso File.Exists(filePath) Then
                    WebView21.Source = New Uri(filePath)
                End If
            End If
        End Using
    End Sub

    ' It's good practice to ensure the control is initialized, especially for more complex apps
    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' This ensures the underlying browser is ready before you try to use it.
        Await WebView21.EnsureCoreWebView2Async(Nothing)

        ' Initialize search module
        _searchModule = New SearchModule()
        AddHandler _searchModule.SearchProgress, AddressOf OnSearchProgress
        AddHandler _searchModule.SearchComplete, AddressOf OnSearchComplete

        ' Start by populating the TreeView with the computer's drives
        'PopulateDrives()


        ' Define your starting folder here
        Dim startPath As String = "C:\Users\amjad\OneDrive\Business\soap making\sds" ' <<< CHANGE THIS to your desired folder

        ' Load the TreeView starting from that path
        LoadTreeFromPath(startPath)

    End Sub

    Private Async Sub SearchButton_Click(sender As Object, e As EventArgs) Handles SearchButton.Click
        If String.IsNullOrWhiteSpace(SearchTextBox.Text) Then
            MessageBox.Show("Please enter a search term", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Check if we have a selected path
        Dim searchPath As String = ""
        If tvFolders.SelectedNode IsNot Nothing Then
            searchPath = CStr(tvFolders.SelectedNode.Tag)
        Else
            searchPath = "C:\"
        End If

        ' Disable search button and show progress
        SearchButton.Enabled = False
        SearchButton.Text = "Searching..."
        SearchProgressBar.Visible = True
        SearchProgressBar.Style = ProgressBarStyle.Marquee
        SearchStatusLabel.Text = "Starting search..."

        ' Clear previous results
        lvFiles.Items.Clear()

        Try
            ' Start the search
            _currentSearchTask = _searchModule.SearchAsync(
                searchPath,
                SearchTextBox.Text,
                SearchContentCheckBox.Checked,
                SearchPDFCheckBox.Checked
            )

            Dim results = Await _currentSearchTask

            ' Display results
            DisplaySearchResults(results)

        Catch ex As Exception
            MessageBox.Show($"Search error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ' Re-enable search button
            SearchButton.Enabled = True
            SearchButton.Text = "Search"
            SearchProgressBar.Visible = False
            SearchStatusLabel.Text = $"Search complete. {lvFiles.Items.Count} results found."
        End Try
    End Sub

    Private Sub OnSearchProgress(message As String, filesProcessed As Integer, totalFiles As Integer)
        If InvokeRequired Then
            Invoke(Sub() OnSearchProgress(message, filesProcessed, totalFiles))
            Return
        End If

        SearchStatusLabel.Text = message
        If totalFiles > 0 Then
            SearchProgressBar.Style = ProgressBarStyle.Continuous
            SearchProgressBar.Maximum = totalFiles
            SearchProgressBar.Value = Math.Min(filesProcessed, totalFiles)
        End If
    End Sub

    Private Sub OnSearchComplete(results As List(Of SearchResult))
        If InvokeRequired Then
            Invoke(Sub() OnSearchComplete(results))
            Return
        End If

        DisplaySearchResults(results)
    End Sub

    Private Sub DisplaySearchResults(results As List(Of SearchResult))
        lvFiles.Items.Clear()

        ' Group results by directory
        Dim groupedResults = results.GroupBy(Function(r) Path.GetDirectoryName(r.FilePath))

        For Each group In groupedResults
            ' Add directory header if multiple directories
            If groupedResults.Count() > 1 Then
                Dim dirItem = lvFiles.Items.Add($"📁 {group.Key}")
                dirItem.ForeColor = Color.DarkBlue
                dirItem.Font = New Font(dirItem.Font, FontStyle.Bold)
            End If

            For Each result In group
                Dim iconIdx = GetIconIndex(result.FilePath, False)
                Dim item As New ListViewItem(result.FileName, iconIdx)

                ' Add file size
                item.SubItems.Add(GetFileSizeText(result.FileSize))

                ' Add match context or modified date
                If result.MatchType = MatchType.FileContent Then
                    item.SubItems.Add($"Line {result.LineNumber}: {result.MatchContext}")
                    item.ForeColor = Color.DarkGreen
                Else
                    item.SubItems.Add(result.LastModified.ToString())
                End If

                item.Tag = result.FilePath
                lvFiles.Items.Add(item)
            Next
        Next
    End Sub

    Private Function GetFileSizeText(bytes As Long) As String
        If bytes < 1024 Then
            Return $"{bytes} B"
        ElseIf bytes < 1048576 Then
            Return $"{bytes / 1024:F1} KB"
        Else
            Return $"{bytes / 1048576:F1} MB"
        End If
    End Function

    Private Sub SearchTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles SearchTextBox.KeyPress
        If e.KeyChar = ChrW(Keys.Return) Then
            SearchButton.PerformClick()
            e.Handled = True
        End If
    End Sub

    Private Sub CancelSearchButton_Click(sender As Object, e As EventArgs)
        _searchModule?.CancelSearch()
        SearchButton.Enabled = True
        SearchButton.Text = "Search"
        SearchProgressBar.Visible = False
        SearchStatusLabel.Text = "Search cancelled."
    End Sub

    ' This subroutine navigates the TreeView to a specific path
    Private Sub LoadTreeFromPath(rootPath As String)
        ' Ensure the path exists before starting
        If Not Directory.Exists(rootPath) Then
            MessageBox.Show("The specified starting path does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        tvFolders.Nodes.Clear()

        ' Create the main root node from the starting path
        Dim rootDirInfo As New DirectoryInfo(rootPath)
        Dim rootNode As New TreeNode(rootDirInfo.Name)
        rootNode.Tag = rootPath

        ' Add the root node to the TreeView
        tvFolders.Nodes.Add(rootNode)

        ' Find and add all subdirectories of the root path
        Try
            For Each dirPath As String In Directory.GetDirectories(rootPath)
                Dim dirInfo As New DirectoryInfo(dirPath)

                ' Add only non-hidden and non-system folders
                If (dirInfo.Attributes And FileAttributes.Hidden) = 0 AndAlso
               (dirInfo.Attributes And FileAttributes.System) = 0 Then

                    Dim childNode As New TreeNode(dirInfo.Name)
                    childNode.Tag = dirPath
                    rootNode.Nodes.Add(childNode)
                    childNode.Nodes.Add("dummy") ' Add dummy node for on-demand expansion
                End If
            Next
        Catch ex As UnauthorizedAccessException
            ' Handle cases where we can't access the root path's subfolders
            rootNode.Nodes.Add("Access Denied").ForeColor = Color.Red
        End Try

        ' Expand the root node to show its children
        rootNode.Expand()
        tvFolders.SelectedNode = rootNode
    End Sub

    Private Sub PopulateDrives()
        tvFolders.Nodes.Clear()
        iconCache.Clear() ' Clear the cache when repopulating

        For Each drive As DriveInfo In DriveInfo.GetDrives()
            If drive.IsReady Then
                Dim drivePath As String = drive.RootDirectory.FullName
                ' Use the new function to get the correct icon index
                Dim iconIdx As Integer = GetIconIndex(drivePath, True)

                Dim driveNode As New TreeNode(drive.Name, iconIdx, iconIdx)
                driveNode.Tag = drivePath
                tvFolders.Nodes.Add(driveNode)
                driveNode.Nodes.Add("dummy")
            End If
        Next
    End Sub

    Private Sub tvFolders_BeforeExpand(sender As Object, e As TreeViewCancelEventArgs) Handles tvFolders.BeforeExpand
        Dim expandingNode As TreeNode = e.Node
        If expandingNode.Nodes.Count = 1 AndAlso expandingNode.Nodes(0).Text = "dummy" Then
            expandingNode.Nodes.Clear()
            Try
                Dim parentPath As String = CStr(expandingNode.Tag)
                For Each dirPath As String In Directory.GetDirectories(parentPath)
                    Dim dirInfo As New DirectoryInfo(dirPath)
                    If (dirInfo.Attributes And FileAttributes.Hidden) = 0 AndAlso
                   (dirInfo.Attributes And FileAttributes.System) = 0 Then

                        ' Use the new function to get the correct icon index
                        Dim iconIdx As Integer = GetIconIndex(dirPath, True)

                        Dim dirNode As New TreeNode(dirInfo.Name, iconIdx, iconIdx)
                        dirNode.Tag = dirPath
                        expandingNode.Nodes.Add(dirNode)
                        dirNode.Nodes.Add("dummy")
                    End If
                Next
            Catch ex As UnauthorizedAccessException
                expandingNode.Nodes.Add("Access Denied").ForeColor = Color.Red
            End Try
        End If
    End Sub

    Private Sub tvFolders_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvFolders.AfterSelect
        Dim selectedNode As TreeNode = e.Node
        If selectedNode Is Nothing OrElse selectedNode.Tag Is Nothing Then Return

        Dim selectedPath As String = CStr(selectedNode.Tag)
        lvFiles.Items.Clear()

        Try
            ' Add Subdirectories
            For Each dirPath As String In Directory.GetDirectories(selectedPath)
                Dim dirInfo As New DirectoryInfo(dirPath)
                If (dirInfo.Attributes And FileAttributes.Hidden) = 0 AndAlso
               (dirInfo.Attributes And FileAttributes.System) = 0 Then
                    ' Use the new function
                    Dim iconIdx As Integer = GetIconIndex(dirPath, True)
                    Dim item As New ListViewItem(dirInfo.Name, iconIdx)
                    item.SubItems.Add("<DIR>")
                    item.SubItems.Add(dirInfo.LastWriteTime.ToString())
                    lvFiles.Items.Add(item)
                End If
            Next

            ' Add Files
            For Each filePath As String In Directory.GetFiles(selectedPath)
                Dim fileInfo As New FileInfo(filePath)
                If (fileInfo.Attributes And FileAttributes.Hidden) = 0 AndAlso
               (fileInfo.Attributes And FileAttributes.System) = 0 Then
                    ' Use the new function
                    Dim iconIdx As Integer = GetIconIndex(filePath, False)
                    Dim item As New ListViewItem(fileInfo.Name, iconIdx)
                    item.SubItems.Add(String.Format("{0:N0} KB", (fileInfo.Length / 1024) + 1))
                    item.SubItems.Add(fileInfo.LastWriteTime.ToString())
                    lvFiles.Items.Add(item)
                End If
            Next
        Catch ex As Exception
            ' Handle errors
        End Try
    End Sub

    Private Sub lvFiles_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvFiles.SelectedIndexChanged
        ' Check if an item is selected in the ListView
        If lvFiles.SelectedItems.Count > 0 Then
            ' Get the selected item from the ListView
            Dim selectedItem = lvFiles.SelectedItems(0)
            Dim fullPath As String

            ' Check if this is a search result (has full path in Tag)
            If selectedItem.Tag IsNot Nothing Then
                fullPath = CStr(selectedItem.Tag)
            ElseIf tvFolders.SelectedNode IsNot Nothing Then
                ' Normal browsing - combine folder path with file name
                Dim itemName = selectedItem.Text
                Dim folderPath = CStr(tvFolders.SelectedNode.Tag)
                fullPath = Path.Combine(folderPath, itemName)
            Else
                Return
            End If

            ' Update title and display the file
            Text = fullPath

            ' Check if file exists and display it
            If File.Exists(fullPath) Then
                Try
                    ' WebView2 can handle PDF, Office documents, and many other formats natively
                    WebView21.Source = New Uri(fullPath)
                Catch ex As Exception
                    MessageBox.Show($"Cannot open file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End Try
            Else
                MessageBox.Show($"File not found: {fullPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Else
            ' No item is selected, so clear the title bar
            Text = "File Explorer - Enhanced Edition"
        End If
    End Sub


    Private Function GetIconIndex(path As String, isDirectory As Boolean) As Integer
        Dim key As String
        ' Use a special key for folders and drives, and the file extension for files
        If isDirectory Then
            key = "::FOLDER::"
            ' You could add a "::DRIVE::" key too, but the folder icon is usually the same.
        Else
            key = System.IO.Path.GetExtension(path).ToLower
            If String.IsNullOrEmpty(key) Then key = "::FILE::" ' Key for files with no extension
        End If

        ' If we've already cached this icon type, return its index immediately
        If iconCache.ContainsKey(key) Then
            Return iconCache(key)
        End If

        ' If not cached, get the icon from the Windows API
        Dim flags As SHGFI = SHGFI.Icon Or SHGFI.UseFileAttributes Or SHGFI.SmallIcon
        Dim shfi As New SHFILEINFO()
        Dim attributes As UInteger = If(isDirectory, &H10, 0) ' File or folder attribute

        SHGetFileInfo(path, attributes, shfi, CUInt(Marshal.SizeOf(shfi)), flags)

        ' Check if we got a valid icon handle
        If shfi.hIcon <> IntPtr.Zero Then
            ' Add the icon to our ImageList
            Dim icon As Icon = Icon.FromHandle(shfi.hIcon)
            ImageList1.Images.Add(key, icon.Clone())
            ' Clean up the handle
            DestroyIcon(shfi.hIcon)

            ' Get the index of the icon we just added
            Dim newIndex As Integer = ImageList1.Images.IndexOfKey(key)

            ' Cache the new index
            iconCache.Add(key, newIndex)

            Return newIndex
        Else
            ' Return a default "file" icon index if something goes wrong
            Return 0
        End If
    End Function


End Class

