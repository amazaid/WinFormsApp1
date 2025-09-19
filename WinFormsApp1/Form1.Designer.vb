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
        SplitContainer1 = New SplitContainer()
        Panel1 = New Panel()
        SearchPanel = New Panel()
        SearchTextBox = New TextBox()
        SearchButton = New Button()
        SearchOptionsPanel = New Panel()
        SearchContentCheckBox = New CheckBox()
        SearchPDFCheckBox = New CheckBox()
        SearchProgressBar = New ProgressBar()
        SearchStatusLabel = New Label()
        SplitContainer2 = New SplitContainer()
        tvFolders = New TreeView()
        lvFiles = New ListView()
        ColumnHeader1 = New ColumnHeader()
        ColumnHeader2 = New ColumnHeader()
        ColumnHeader3 = New ColumnHeader()
        WebView21 = New Microsoft.Web.WebView2.WinForms.WebView2()
        CType(SplitContainer1, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()
        Panel1.SuspendLayout()
        SearchPanel.SuspendLayout()
        SearchOptionsPanel.SuspendLayout()
        CType(SplitContainer2, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer2.Panel1.SuspendLayout()
        SplitContainer2.Panel2.SuspendLayout()
        SplitContainer2.SuspendLayout()
        CType(WebView21, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' ImageList1
        ' 
        ImageList1.ColorDepth = ColorDepth.Depth32Bit
        ImageList1.ImageSize = New Size(16, 16)
        ImageList1.TransparentColor = Color.Transparent
        ' 
        ' SplitContainer1
        ' 
        SplitContainer1.Dock = DockStyle.Fill
        SplitContainer1.Location = New Point(0, 0)
        SplitContainer1.Name = "SplitContainer1"
        ' 
        ' SplitContainer1.Panel1
        ' 
        SplitContainer1.Panel1.Controls.Add(Panel1)
        ' 
        ' SplitContainer1.Panel2
        ' 
        SplitContainer1.Panel2.Controls.Add(WebView21)
        SplitContainer1.Size = New Size(1352, 689)
        SplitContainer1.SplitterDistance = 499
        SplitContainer1.TabIndex = 5
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
        SearchPanel.TabIndex = 11
        '
        ' SearchTextBox
        '
        SearchTextBox.Font = New Font("Segoe UI", 10F)
        SearchTextBox.Location = New Point(10, 10)
        SearchTextBox.Name = "SearchTextBox"
        SearchTextBox.PlaceholderText = "Search files and content..."
        SearchTextBox.Size = New Size(350, 25)
        SearchTextBox.TabIndex = 0
        '
        ' SearchButton
        '
        SearchButton.BackColor = Color.SteelBlue
        SearchButton.FlatStyle = FlatStyle.Flat
        SearchButton.ForeColor = Color.White
        SearchButton.Location = New Point(370, 10)
        SearchButton.Name = "SearchButton"
        SearchButton.Size = New Size(100, 25)
        SearchButton.TabIndex = 1
        SearchButton.Text = "Search"
        SearchButton.UseVisualStyleBackColor = False
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
        ' SearchContentCheckBox
        '
        SearchContentCheckBox.AutoSize = True
        SearchContentCheckBox.Checked = True
        SearchContentCheckBox.CheckState = CheckState.Checked
        SearchContentCheckBox.Location = New Point(5, 5)
        SearchContentCheckBox.Name = "SearchContentCheckBox"
        SearchContentCheckBox.Size = New Size(120, 20)
        SearchContentCheckBox.TabIndex = 0
        SearchContentCheckBox.Text = "Search in files"
        SearchContentCheckBox.UseVisualStyleBackColor = True
        '
        ' SearchPDFCheckBox
        '
        SearchPDFCheckBox.AutoSize = True
        SearchPDFCheckBox.Checked = True
        SearchPDFCheckBox.CheckState = CheckState.Checked
        SearchPDFCheckBox.Location = New Point(140, 5)
        SearchPDFCheckBox.Name = "SearchPDFCheckBox"
        SearchPDFCheckBox.Size = New Size(150, 20)
        SearchPDFCheckBox.TabIndex = 1
        SearchPDFCheckBox.Text = "Include PDF content"
        SearchPDFCheckBox.UseVisualStyleBackColor = True
        '
        ' SearchProgressBar
        '
        SearchProgressBar.Location = New Point(10, 75)
        SearchProgressBar.Name = "SearchProgressBar"
        SearchProgressBar.Size = New Size(460, 10)
        SearchProgressBar.Style = ProgressBarStyle.Marquee
        SearchProgressBar.TabIndex = 3
        SearchProgressBar.Visible = False
        '
        ' SearchStatusLabel
        '
        SearchStatusLabel.AutoSize = True
        SearchStatusLabel.ForeColor = Color.DimGray
        SearchStatusLabel.Location = New Point(10, 90)
        SearchStatusLabel.Name = "SearchStatusLabel"
        SearchStatusLabel.Size = New Size(100, 16)
        SearchStatusLabel.TabIndex = 4
        SearchStatusLabel.Text = ""
        '
        ' Panel1
        '
        Panel1.BackColor = Color.LightSteelBlue
        Panel1.Controls.Add(SplitContainer2)
        Panel1.Controls.Add(SearchPanel)
        Panel1.Dock = DockStyle.Fill
        Panel1.Location = New Point(0, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(499, 689)
        Panel1.TabIndex = 0
        '
        ' SplitContainer2
        '
        SplitContainer2.Dock = DockStyle.Fill
        SplitContainer2.Location = New Point(0, 120)
        SplitContainer2.Name = "SplitContainer2"
        '
        ' SplitContainer2.Panel1
        '
        SplitContainer2.Panel1.Controls.Add(tvFolders)
        '
        ' SplitContainer2.Panel2
        '
        SplitContainer2.Panel2.Controls.Add(lvFiles)
        SplitContainer2.Size = New Size(499, 569)
        SplitContainer2.SplitterDistance = 200
        SplitContainer2.TabIndex = 10
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
        tvFolders.TabIndex = 6
        ' 
        ' lvFiles
        ' 
        lvFiles.Columns.AddRange(New ColumnHeader() {ColumnHeader1, ColumnHeader2, ColumnHeader3})
        lvFiles.Dock = DockStyle.Fill
        lvFiles.LargeImageList = ImageList1
        lvFiles.Location = New Point(0, 0)
        lvFiles.Name = "lvFiles"
        lvFiles.Size = New Size(295, 569)
        lvFiles.SmallImageList = ImageList1
        lvFiles.TabIndex = 9
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
        ' WebView21
        ' 
        WebView21.AllowExternalDrop = True
        WebView21.CreationProperties = Nothing
        WebView21.DefaultBackgroundColor = Color.White
        WebView21.Dock = DockStyle.Fill
        WebView21.Location = New Point(0, 0)
        WebView21.Name = "WebView21"
        WebView21.Size = New Size(849, 689)
        WebView21.TabIndex = 2
        WebView21.ZoomFactor = 1R
        '
        ' Form1
        '
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.AliceBlue
        ClientSize = New Size(1352, 689)
        Controls.Add(SplitContainer1)
        Name = "Form1"
        Text = "File Explorer - Enhanced Edition"
        SplitContainer1.Panel1.ResumeLayout(False)
        SplitContainer1.Panel2.ResumeLayout(False)
        CType(SplitContainer1, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer1.ResumeLayout(False)
        Panel1.ResumeLayout(False)
        SearchPanel.ResumeLayout(False)
        SearchPanel.PerformLayout()
        SearchOptionsPanel.ResumeLayout(False)
        SearchOptionsPanel.PerformLayout()
        SplitContainer2.Panel1.ResumeLayout(False)
        SplitContainer2.Panel2.ResumeLayout(False)
        CType(SplitContainer2, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer2.ResumeLayout(False)
        CType(WebView21, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents WebView21 As Microsoft.Web.WebView2.WinForms.WebView2
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lvFiles As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Friend WithEvents tvFolders As TreeView
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents SearchPanel As Panel
    Friend WithEvents SearchTextBox As TextBox
    Friend WithEvents SearchButton As Button
    Friend WithEvents SearchOptionsPanel As Panel
    Friend WithEvents SearchContentCheckBox As CheckBox
    Friend WithEvents SearchPDFCheckBox As CheckBox
    Friend WithEvents SearchProgressBar As ProgressBar
    Friend WithEvents SearchStatusLabel As Label

End Class
