Imports System.IO
Imports System.Text
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Wordprocessing
Imports DocumentFormat.OpenXml.Spreadsheet
Imports ClosedXML.Excel

Module OfficeConverter

    Public Function ConvertWordToHtml(filePath As String) As String
        Try
            Dim html As New StringBuilder()
            html.AppendLine("<!DOCTYPE html>")
            html.AppendLine("<html><head>")
            html.AppendLine("<meta charset='utf-8'>")
            html.AppendLine("<style>")
            html.AppendLine("body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; }")
            html.AppendLine("p { margin: 10px 0; }")
            html.AppendLine("h1 { color: #2c3e50; }")
            html.AppendLine("h2 { color: #34495e; }")
            html.AppendLine("table { border-collapse: collapse; width: 100%; margin: 15px 0; }")
            html.AppendLine("td, th { border: 1px solid #ddd; padding: 8px; text-align: left; }")
            html.AppendLine("th { background-color: #f2f2f2; }")
            html.AppendLine(".document-info { background: #f0f0f0; padding: 10px; margin-bottom: 20px; border-radius: 5px; }")
            html.AppendLine("</style>")
            html.AppendLine("</head><body>")

            ' Add document info
            html.AppendLine("<div class='document-info'>")
            html.AppendLine($"<strong>Document:</strong> {Path.GetFileName(filePath)}<br>")
            html.AppendLine($"<strong>Type:</strong> Microsoft Word Document")
            html.AppendLine("</div>")

            Using wordDoc = WordprocessingDocument.Open(filePath, False)
                Dim body = wordDoc.MainDocumentPart.Document.Body

                For Each element In body.Elements()
                    If TypeOf element Is Paragraph Then
                        Dim para = CType(element, Paragraph)
                        Dim text = GetParagraphText(para)

                        ' Check for heading styles
                        Dim props = para.ParagraphProperties
                        If props IsNot Nothing AndAlso props.ParagraphStyleId IsNot Nothing Then
                            Dim styleId = props.ParagraphStyleId.Val
                            If styleId IsNot Nothing AndAlso styleId.Value.StartsWith("Heading") Then
                                Dim level = styleId.Value.Replace("Heading", "")
                                If Integer.TryParse(level, Nothing) Then
                                    html.AppendLine($"<h{level}>{text}</h{level}>")
                                Else
                                    html.AppendLine($"<p><strong>{text}</strong></p>")
                                End If
                            Else
                                html.AppendLine($"<p>{text}</p>")
                            End If
                        ElseIf Not String.IsNullOrWhiteSpace(text) Then
                            html.AppendLine($"<p>{text}</p>")
                        End If
                    ElseIf TypeOf element Is Table Then
                        html.AppendLine(ConvertTableToHtml(CType(element, Table)))
                    End If
                Next
            End Using

            html.AppendLine("</body></html>")
            Return html.ToString()

        Catch ex As Exception
            Return GenerateErrorHtml("Word Document", filePath, ex.Message)
        End Try
    End Function

    Private Function GetParagraphText(para As Paragraph) As String
        Dim text As New StringBuilder()
        For Each run In para.Elements(Of Run)()
            For Each textElement In run.Elements(Of Text)()
                text.Append(textElement.Text)
            Next
        Next
        Return text.ToString()
    End Function

    Private Function ConvertTableToHtml(table As Table) As String
        Dim html As New StringBuilder()
        html.AppendLine("<table>")

        For Each row In table.Elements(Of TableRow)()
            html.AppendLine("<tr>")
            For Each cell In row.Elements(Of TableCell)()
                Dim cellText As New StringBuilder()
                For Each para In cell.Elements(Of Paragraph)()
                    cellText.Append(GetParagraphText(para))
                    cellText.Append(" ")
                Next
                html.AppendLine($"<td>{cellText.ToString().Trim()}</td>")
            Next
            html.AppendLine("</tr>")
        Next

        html.AppendLine("</table>")
        Return html.ToString()
    End Function

    Public Function ConvertExcelToHtml(filePath As String) As String
        Try
            Dim html As New StringBuilder()
            html.AppendLine("<!DOCTYPE html>")
            html.AppendLine("<html><head>")
            html.AppendLine("<meta charset='utf-8'>")
            html.AppendLine("<style>")
            html.AppendLine("body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; }")
            html.AppendLine("table { border-collapse: collapse; margin: 15px 0; }")
            html.AppendLine("td, th { border: 1px solid #ddd; padding: 8px; text-align: left; min-width: 80px; }")
            html.AppendLine("th { background-color: #4CAF50; color: white; }")
            html.AppendLine("tr:nth-child(even) { background-color: #f2f2f2; }")
            html.AppendLine(".sheet-name { color: #2c3e50; margin-top: 20px; }")
            html.AppendLine(".document-info { background: #f0f0f0; padding: 10px; margin-bottom: 20px; border-radius: 5px; }")
            html.AppendLine(".numeric { text-align: right; }")
            html.AppendLine("</style>")
            html.AppendLine("</head><body>")

            ' Add document info
            html.AppendLine("<div class='document-info'>")
            html.AppendLine($"<strong>Document:</strong> {Path.GetFileName(filePath)}<br>")
            html.AppendLine($"<strong>Type:</strong> Microsoft Excel Spreadsheet")
            html.AppendLine("</div>")

            Using workbook As New XLWorkbook(filePath)
                For Each worksheet In workbook.Worksheets
                    html.AppendLine($"<h2 class='sheet-name'>Sheet: {worksheet.Name}</h2>")
                    html.AppendLine("<table>")

                    ' Get used range
                    Dim usedRange = worksheet.RangeUsed()
                    If usedRange IsNot Nothing Then
                        ' Header row
                        Dim firstRow = usedRange.FirstRow()
                        html.AppendLine("<tr>")
                        For Each cell In firstRow.Cells()
                            html.AppendLine($"<th>{cell.Value}</th>")
                        Next
                        html.AppendLine("</tr>")

                        ' Data rows
                        For rowNum = usedRange.FirstRow().RowNumber() + 1 To usedRange.LastRow().RowNumber()
                            Dim row = worksheet.Row(rowNum)
                            html.AppendLine("<tr>")
                            For colNum = usedRange.FirstColumn().ColumnNumber() To usedRange.LastColumn().ColumnNumber()
                                Dim cell = row.Cell(colNum)
                                Dim cellValue = cell.Value.ToString()
                                Dim cellClass = If(cell.DataType = XLDataType.Number, " class='numeric'", "")
                                html.AppendLine($"<td{cellClass}>{cellValue}</td>")
                            Next
                            html.AppendLine("</tr>")
                        Next
                    End If

                    html.AppendLine("</table>")
                Next
            End Using

            html.AppendLine("</body></html>")
            Return html.ToString()

        Catch ex As Exception
            Return GenerateErrorHtml("Excel Spreadsheet", filePath, ex.Message)
        End Try
    End Function

    Public Function ConvertPowerPointToHtml(filePath As String) As String
        Try
            ' For PowerPoint, we'll create a simple preview
            ' Full PowerPoint conversion requires more complex libraries
            Dim html As New StringBuilder()
            html.AppendLine("<!DOCTYPE html>")
            html.AppendLine("<html><head>")
            html.AppendLine("<meta charset='utf-8'>")
            html.AppendLine("<style>")
            html.AppendLine("body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; }")
            html.AppendLine(".document-info { background: #f0f0f0; padding: 10px; margin-bottom: 20px; border-radius: 5px; }")
            html.AppendLine(".slide { border: 2px solid #ddd; padding: 20px; margin: 20px 0; border-radius: 5px; }")
            html.AppendLine(".slide-number { color: #666; font-size: 0.9em; }")
            html.AppendLine("</style>")
            html.AppendLine("</head><body>")

            html.AppendLine("<div class='document-info'>")
            html.AppendLine($"<strong>Document:</strong> {Path.GetFileName(filePath)}<br>")
            html.AppendLine($"<strong>Type:</strong> Microsoft PowerPoint Presentation<br>")
            html.AppendLine("<strong>Note:</strong> Full presentation preview requires PowerPoint or online viewer")
            html.AppendLine("</div>")

            html.AppendLine("<p>PowerPoint presentations cannot be fully rendered in HTML. Please use one of these options:</p>")
            html.AppendLine("<ul>")
            html.AppendLine("<li>Open the file in Microsoft PowerPoint</li>")
            html.AppendLine("<li>Use the Office Online viewer (requires internet connection)</li>")
            html.AppendLine("<li>Export the presentation to PDF from PowerPoint for better compatibility</li>")
            html.AppendLine("</ul>")

            html.AppendLine("</body></html>")
            Return html.ToString()

        Catch ex As Exception
            Return GenerateErrorHtml("PowerPoint Presentation", filePath, ex.Message)
        End Try
    End Function

    Private Function GenerateErrorHtml(docType As String, filePath As String, errorMessage As String) As String
        Dim html As New StringBuilder()
        html.AppendLine("<!DOCTYPE html>")
        html.AppendLine("<html><head>")
        html.AppendLine("<meta charset='utf-8'>")
        html.AppendLine("<style>")
        html.AppendLine("body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; }")
        html.AppendLine(".error { background: #fee; padding: 20px; border-left: 4px solid #f44; border-radius: 5px; }")
        html.AppendLine("</style>")
        html.AppendLine("</head><body>")
        html.AppendLine("<div class='error'>")
        html.AppendLine($"<h2>Cannot Preview {docType}</h2>")
        html.AppendLine($"<p><strong>File:</strong> {Path.GetFileName(filePath)}</p>")
        html.AppendLine($"<p><strong>Error:</strong> {errorMessage}</p>")
        html.AppendLine("<p>Try opening this file in its native application.</p>")
        html.AppendLine("</div>")
        html.AppendLine("</body></html>")
        Return html.ToString()
    End Function

    Public Function ExtractTextFromWord(filePath As String) As String
        Try
            Dim text As New StringBuilder()
            Using wordDoc = WordprocessingDocument.Open(filePath, False)
                Dim body = wordDoc.MainDocumentPart.Document.Body
                For Each element In body.Elements()
                    If TypeOf element Is Paragraph Then
                        text.AppendLine(GetParagraphText(CType(element, Paragraph)))
                    End If
                Next
            End Using
            Return text.ToString()
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function ExtractTextFromExcel(filePath As String) As String
        Try
            Dim text As New StringBuilder()
            Using workbook As New XLWorkbook(filePath)
                For Each worksheet In workbook.Worksheets
                    text.AppendLine($"Sheet: {worksheet.Name}")
                    Dim usedRange = worksheet.RangeUsed()
                    If usedRange IsNot Nothing Then
                        For Each row In usedRange.Rows()
                            For Each cell In row.Cells()
                                text.Append(cell.Value.ToString() & " ")
                            Next
                            text.AppendLine()
                        Next
                    End If
                Next
            End Using
            Return text.ToString()
        Catch ex As Exception
            Return ""
        End Try
    End Function

End Module