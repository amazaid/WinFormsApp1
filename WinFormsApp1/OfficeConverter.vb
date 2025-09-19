Imports System.IO
Imports System.Text
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Wordprocessing
Imports ClosedXML.Excel

Module OfficeConverter

    Public Function ExtractTextFromWord(filePath As String) As String
        Try
            Dim text As New StringBuilder()
            Using wordDoc = WordprocessingDocument.Open(filePath, False)
                Dim body = wordDoc.MainDocumentPart.Document.Body
                For Each element In body.Elements()
                    If TypeOf element Is Paragraph Then
                        text.AppendLine(GetParagraphText(CType(element, Paragraph)))
                    ElseIf TypeOf element Is DocumentFormat.OpenXml.Wordprocessing.Table Then
                        text.AppendLine(GetTableText(CType(element, DocumentFormat.OpenXml.Wordprocessing.Table)))
                    End If
                Next
            End Using
            Return text.ToString()
        Catch ex As Exception
            Return ""
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

    Private Function GetTableText(table As DocumentFormat.OpenXml.Wordprocessing.Table) As String
        Dim text As New StringBuilder()
        For Each row In table.Elements(Of TableRow)()
            For Each cell In row.Elements(Of TableCell)()
                For Each para In cell.Elements(Of Paragraph)()
                    text.Append(GetParagraphText(para))
                    text.Append(" ")
                Next
            Next
            text.AppendLine()
        Next
        Return text.ToString()
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