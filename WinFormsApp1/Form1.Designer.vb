<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        ImageList1 = New ImageList(components)
        MainSplitContainer = New SplitContainer()
        LeftPanel = New Panel()
        FileExplorerSplitContainer = New SplitContainer()
        tvFolders = New TreeView()
        lvFiles = New ListView()
        ColumnHeader1 = New ColumnHeader()
        ColumnHeader2 = New ColumnHeader()
        ColumnHeader3 = New ColumnHeader()
        SearchPanel = New Panel()
        SearchStatusLabel = New Label()
        SearchProgressBar = New ProgressBar()
        SearchOptionsPanel = New Panel()
        SearchPDFCheckBox = New CheckBox()
        SearchContentCheckBox = New CheckBox()
        SearchButton = New Button()
        SearchTextBox = New TextBox()
        WebView21 = New Microsoft.Web.WebView2.WinForms.WebView2()
        CType(MainSplitContainer, ComponentModel.ISupportInitialize).BeginInit()
        MainSplitContainer.Panel1.SuspendLayout()
        MainSplitContainer.Panel2.SuspendLayout()
        MainSplitContainer.SuspendLayout()
        LeftPanel.SuspendLayout()
        CType(FileExplorerSplitContainer, ComponentModel.ISupportInitialize).BeginInit()
        FileExplorerSplitContainer.Panel1.SuspendLayout()
        FileExplorerSplitContainer.Panel2.SuspendLayout()
        FileExplorerSplitContainer.SuspendLayout()
        SearchPanel.SuspendLayout()
        SearchOptionsPanel.SuspendLayout()
        CType(WebView21, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        '
        ' ImageList1
        '
        ImageList1.ColorDepth = ColorDepth.Depth32Bit
        ImageList1.ImageSize = New Size(16, 16)
        ImageList1.TransparentColor = Color.Transparent
        '
        ' MainSplitContainer
        '
        MainSplitContainer.Dock = DockStyle.Fill
        MainSplitContainer.Location = New Point(0, 0)
        MainSplitContainer.Name = "MainSplitContainer"
        '
        ' MainSplitContainer.Panel1
        '
        MainSplitContainer.Panel1.Controls.Add(LeftPanel)
        '
        ' MainSplitContainer.Panel2
        '
        MainSplitContainer.Panel2.Controls.Add(WebView21)
        MainSplitContainer.Size = New Size(1352, 689)
        MainSplitContainer.SplitterDistance = 499
        MainSplitContainer.TabIndex = 0
        '
        ' LeftPanel
        '
        LeftPanel.BackColor = Color.LightSteelBlue
        LeftPanel.Controls.Add(FileExplorerSplitContainer)
        LeftPanel.Controls.Add(SearchPanel)
        LeftPanel.Dock = DockStyle.Fill
        LeftPanel.Location = New Point(0, 0)
        LeftPanel.Name = "LeftPanel"
        LeftPanel.Size = New Size(499, 689)
        LeftPanel.TabIndex = 0
        '
        ' FileExplorerSplitContainer
        '
        FileExplorerSplitContainer.Dock = DockStyle.Fill
        FileExplorerSplitContainer.Location = New Point(0, 120)
        FileExplorerSplitContainer.Name = "FileExplorerSplitContainer"
        '
        ' FileExplorerSplitContainer.Panel1
        '
        FileExplorerSplitContainer.Panel1.Controls.Add(tvFolders)
        '
        ' FileExplorerSplitContainer.Panel2
        '
        FileExplorerSplitContainer.Panel2.Controls.Add(lvFiles)
        FileExplorerSplitContainer.Size = New Size(499, 569)
        FileExplorerSplitContainer.SplitterDistance = 200
        FileExplorerSplitContainer.TabIndex = 1
        '
        ' tvFolders
        '
        tvFolders.Dock = DockStyle.Fill
        tvFolders.ImageIndex = 0
        tvFolders.ImageList = ImageList1
        tvFolders.Location = New Point(0, 0)
        tvFolders.Name = "tvFolders"
        tvFolders.SelectedImageIndex = 0
        tvFolders.Size = New Size(200, 569)
        tvFolders.TabIndex = 0
        '
        ' lvFiles
        '
        lvFiles.Columns.AddRange(New ColumnHeader() {ColumnHeader1, ColumnHeader2, ColumnHeader3})
        lvFiles.Dock = DockStyle.Fill
        lvFiles.FullRowSelect = True
        lvFiles.GridLines = True
        lvFiles.LargeImageList = ImageList1
        lvFiles.Location = New Point(0, 0)
        lvFiles.Name = "lvFiles"
        lvFiles.Size = New Size(295, 569)
        lvFiles.SmallImageList = ImageList1
        lvFiles.TabIndex = 0
        lvFiles.UseCompatibleStateImageBehavior = False
        lvFiles.View = View.Details
        '
        ' ColumnHeader1
        '
        ColumnHeader1.Text = "Name"
        ColumnHeader1.Width = 250
        '
        ' ColumnHeader2
        '
        ColumnHeader2.Text = "Size"
        ColumnHeader2.Width = 100
        '
        ' ColumnHeader3
        '
        ColumnHeader3.Text = "Date Modified"
        ColumnHeader3.Width = 150
        '
        ' SearchPanel
        '
        SearchPanel.BackColor = Color.White
        SearchPanel.BorderStyle = BorderStyle.FixedSingle
        SearchPanel.Controls.Add(SearchStatusLabel)
        SearchPanel.Controls.Add(SearchProgressBar)
        SearchPanel.Controls.Add(SearchOptionsPanel)
        SearchPanel.Controls.Add(SearchButton)
        SearchPanel.Controls.Add(SearchTextBox)
        SearchPanel.Dock = DockStyle.Top
        SearchPanel.Location = New Point(0, 0)
        SearchPanel.Name = "SearchPanel"
        SearchPanel.Size = New Size(499, 120)
        SearchPanel.TabIndex = 0
        '
        ' SearchStatusLabel
        '
        SearchStatusLabel.AutoSize = True
        SearchStatusLabel.ForeColor = Color.DimGray
        SearchStatusLabel.Location = New Point(10, 90)
        SearchStatusLabel.Name = "SearchStatusLabel"
        SearchStatusLabel.Size = New Size(0, 20)
        SearchStatusLabel.TabIndex = 4
        '
        ' SearchProgressBar
        '
        SearchProgressBar.Location = New Point(10, 75)
        SearchProgressBar.Name = "SearchProgressBar"
        SearchProgressBar.Size = New Size(460, 10)
        SearchProgressBar.TabIndex = 3
        SearchProgressBar.Visible = False
        '
        ' SearchOptionsPanel
        '
        SearchOptionsPanel.Controls.Add(SearchPDFCheckBox)
        SearchOptionsPanel.Controls.Add(SearchContentCheckBox)
        SearchOptionsPanel.Location = New Point(10, 40)
        SearchOptionsPanel.Name = "SearchOptionsPanel"
        SearchOptionsPanel.Size = New Size(460, 30)
        SearchOptionsPanel.TabIndex = 2
        '
        ' SearchPDFCheckBox
        '
        SearchPDFCheckBox.AutoSize = True
        SearchPDFCheckBox.Checked = True
        SearchPDFCheckBox.CheckState = CheckState.Checked
        SearchPDFCheckBox.Location = New Point(140, 5)
        SearchPDFCheckBox.Name = "SearchPDFCheckBox"
        SearchPDFCheckBox.Size = New Size(167, 24)
        SearchPDFCheckBox.TabIndex = 1
        SearchPDFCheckBox.Text = "Include PDF content"
        SearchPDFCheckBox.UseVisualStyleBackColor = True
        '
        ' SearchContentCheckBox
        '
        SearchContentCheckBox.AutoSize = True
        SearchContentCheckBox.Checked = True
        SearchContentCheckBox.CheckState = CheckState.Checked
        SearchContentCheckBox.Location = New Point(5, 5)
        SearchContentCheckBox.Name = "SearchContentCheckBox"
        SearchContentCheckBox.Size = New Size(122, 24)
        SearchContentCheckBox.TabIndex = 0
        SearchContentCheckBox.Text = "Search in files"
        SearchContentCheckBox.UseVisualStyleBackColor = True
        '
        ' SearchButton
        '
        SearchButton.BackColor = Color.SteelBlue
        SearchButton.FlatStyle = FlatStyle.Flat
        SearchButton.Font = New Font("Segoe UI", 9F, FontStyle.Bold)
        SearchButton.ForeColor = Color.White
        SearchButton.Location = New Point(370, 10)
        SearchButton.Name = "SearchButton"
        SearchButton.Size = New Size(100, 27)
        SearchButton.TabIndex = 1
        SearchButton.Text = "Search"
        SearchButton.UseVisualStyleBackColor = False
        '
        ' SearchTextBox
        '
        SearchTextBox.Font = New Font("Segoe UI", 10F)
        SearchTextBox.Location = New Point(10, 10)
        SearchTextBox.Name = "SearchTextBox"
        SearchTextBox.PlaceholderText = "Search files and content..."
        SearchTextBox.Size = New Size(350, 30)
        SearchTextBox.TabIndex = 0
        '
        ' WebView21
        '
        WebView21.AllowExternalDrop = True
        WebView21.CreationProperties = Nothing
        WebView21.DefaultBackgroundColor = Color.White
        WebView21.Dock = DockStyle.Fill
        WebView21.Location = New Point(0, 0)
        WebView21.Name = "WebView21"
        WebView21.Size = New Size(849, 689)
        WebView21.TabIndex = 0
        WebView21.ZoomFactor = 1.0R
        '
        ' Form1
        '
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.AliceBlue
        ClientSize = New Size(1352, 689)
        Controls.Add(MainSplitContainer)
        Name = "Form1"
        Text = "File Explorer - Enhanced Edition"
        MainSplitContainer.Panel1.ResumeLayout(False)
        MainSplitContainer.Panel2.ResumeLayout(False)
        CType(MainSplitContainer, ComponentModel.ISupportInitialize).EndInit()
        MainSplitContainer.ResumeLayout(False)
        LeftPanel.ResumeLayout(False)
        CType(FileExplorerSplitContainer, ComponentModel.ISupportInitialize).EndInit()
        FileExplorerSplitContainer.Panel1.ResumeLayout(False)
        FileExplorerSplitContainer.Panel2.ResumeLayout(False)
        FileExplorerSplitContainer.ResumeLayout(False)
        SearchPanel.ResumeLayout(False)
        SearchPanel.PerformLayout()
        SearchOptionsPanel.ResumeLayout(False)
        SearchOptionsPanel.PerformLayout()
        CType(WebView21, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents MainSplitContainer As SplitContainer
    Friend WithEvents LeftPanel As Panel
    Friend WithEvents FileExplorerSplitContainer As SplitContainer
    Friend WithEvents tvFolders As TreeView
    Friend WithEvents lvFiles As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Friend WithEvents SearchPanel As Panel
    Friend WithEvents SearchTextBox As TextBox
    Friend WithEvents SearchButton As Button
    Friend WithEvents SearchOptionsPanel As Panel
    Friend WithEvents SearchContentCheckBox As CheckBox
    Friend WithEvents SearchPDFCheckBox As CheckBox
    Friend WithEvents SearchProgressBar As ProgressBar
    Friend WithEvents SearchStatusLabel As Label
    Friend WithEvents WebView21 As Microsoft.Web.WebView2.WinForms.WebView2

End Class