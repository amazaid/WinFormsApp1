
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Linq
Imports Microsoft.Web.WebView2.Core

Public Class Form1

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

        ' Start by populating the TreeView with the computer's drives
        'PopulateDrives()


        ' Define your starting folder here
        Dim startPath As String = "C:\Users\amjad\OneDrive\Business\soap making\sds" ' <<< CHANGE THIS to your desired folder

        ' Load the TreeView starting from that path
        LoadTreeFromPath(startPath)

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
        ' Check if an item is selected in the ListView AND a folder is selected in the TreeView
        If lvFiles.SelectedItems.Count > 0 AndAlso tvFolders.SelectedNode IsNot Nothing Then
            ' Get the selected item from the ListView
            Dim selectedItem = lvFiles.SelectedItems(0)
            Dim itemName = selectedItem.Text

            ' Get the path of the parent folder from the TreeView's selected node
            Dim folderPath = CStr(tvFolders.SelectedNode.Tag)

            ' Combine the folder path and the item name to get the full path
            Dim fullPath = Path.Combine(folderPath, itemName)

            ' Now you can use the full path, for example, display it in the title bar
            Text = fullPath
            WebView21.Source = New Uri(fullPath)

        Else
            ' No item is selected, so clear the title bar
            Text = "MyFileExplorer"
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

