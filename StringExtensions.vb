'=========================================================
' WWWeb Concepts wwwebconcepts.com
' James W. Threadgill james@wwwebconcepts.com
' Copyright 2017
' Version 1.0.0.0
'=========================================================

Imports System.Convert
Imports System.Web.HttpContext
Imports System.Text.RegularExpressions.Regex
Public Class StringExtensions

    'creates url slug from passed string
    ' From http://www.beansoftware.com/ASP.NET-FAQ/Create-Slug-From-Page-Title.aspx
    Public Shared Function ToSlug(ByVal SlugField As String) As String
        Dim Slug As String = SlugField.ToLower()

        ' Replace - with empty space
        Slug = Slug.Replace("-", " ")
        ' Replace unwanted characters with space
        Slug = Replace(Slug, "[^a-z0-9\s-]", " ")
        ' Replace multple white spaces with single space
        Slug = Replace(Slug, "\s+", " ").Trim()
        ' Replace white space with -
        Slug = Slug.Replace(" ", "-")

        Return Slug
    End Function

    ' Returns trimmed field 
    Public Overloads Shared Function TrimTextProperly(ByVal Field As String, ByVal Length As Integer, ByVal Properly As Boolean, ByVal Pointed As Boolean, ByVal Points As String) As String
        Dim strReturn As String
        ' clean the string of HTML, tabs, carriage returns and remove multiple, leading, and trailing spaces
        strReturn = RemoveHTML(Field)
        strReturn = strReturn.Replace(ToChar(13), " ")
        strReturn = strReturn.Replace(ToChar(9), " ")
        strReturn = Replace(strReturn, "\s+", " ").Trim()

        If (Len(strReturn) > Length) Then
            strReturn = Left(strReturn, Length)
            If Properly Then
                Dim TempArray As Array = Split(strReturn, " ")
                Dim n As Integer
                strReturn = ""
                For n = 0 To UBound(TempArray) - 1
                    strReturn = strReturn & " " & TempArray(n)
                Next
            End If

            If Pointed Then
                strReturn = strReturn & Points
            End If
        End If

        Return strReturn
    End Function

    ' Returns trimmed field with link at end
    Public Overloads Shared Function TrimTextProperly(ByVal Field As String, ByVal Length As Integer, ByVal Properly As Boolean, ByVal Pointed As Boolean, ByVal Points As String, ByVal LinkPage As String, ByVal LinkText As String) As String
        Dim strReturn As String
        ' clean the string of HTML, tabs, carriage returns and remove leading and trailing spaces
        strReturn = RemoveHTML(Field)
        strReturn = strReturn.Replace(ToChar(13), " ")
        strReturn = strReturn.Replace(ToChar(9), " ")
        strReturn = Replace(strReturn, "\s+", " ").Trim()

        If (Len(strReturn) > Length) Then
            strReturn = Left(strReturn, Length)
            If Properly Then
                Dim TempArray As Array = Split(strReturn, " ")
                Dim n As Integer
                strReturn = ""
                For n = 0 To UBound(TempArray) - 1
                    strReturn = strReturn & " " & TempArray(n)
                Next
            End If

            If Pointed Then
                strReturn = strReturn & Points & (" <a href = """ & LinkPage & """ > " & LinkText & "</a>")
            Else
                strReturn = strReturn & (" <a href = """ & LinkPage & """ > " & LinkText & "</a>")
            End If
        End If

        Return strReturn
    End Function

    ' Returns trimmed field with seach text higlighting and single link at end
    Public Overloads Shared Function HighLightText(ByVal Field As String, ByVal Length As Integer, ByVal Properly As Boolean, ByVal Pointed As Boolean, ByVal Points As String, ByVal LinkPage As String, ByVal LinkText As String, Query As String, StyleClass As String) As String
        Dim strReturn As String
        ' clean the string of HTML, tabs, carriage returns and remove leading and trailing spaces
        strReturn = RemoveHTML(Field)
        strReturn = strReturn.Replace(ToChar(13), " ")
        strReturn = strReturn.Replace(ToChar(9), " ")
        strReturn = Replace(strReturn, "\s+", " ").Trim()

        If (Len(strReturn) > Length) Then
            strReturn = Left(strReturn, Length)
            If Properly Then
                Dim TempArray As Array = Split(strReturn, " ")
                Dim n As Integer
                strReturn = ""
                For n = 0 To UBound(TempArray) - 1
                    strReturn = strReturn & " " & TempArray(n)
                Next
            End If

            If Pointed Then
                strReturn = HighlightText(strReturn, Query, StyleClass) & Points & (" <a href = """ & LinkPage & """ > " & LinkText & "</a>")
            Else
                strReturn = HighlightText(strReturn, Query, StyleClass) & (" <a href = """ & LinkPage & """ > " & LinkText & "</a>")
            End If
        End If

        Return strReturn
    End Function

    ' Returns field with all search text matches highlighted and linked
    Public Overloads Shared Function HighlightText(ByVal Field As String, ByVal Query As String, ByVal StyleClass As String, ByVal LinkPage As String) As String
        If Query = "" Then Query = "QXYZ"
        Dim strReturn As String = Field
        Dim input As String = Field
        Dim pattern As String = Query
        Dim options As RegexOptions = RegexOptions.IgnoreCase
        Dim match As Match = Regex.Match(input, pattern, options)

        If match.Length > 0 Then
            Dim strHighlight As String = match.Value.ToString
            strReturn = Field.Replace(strHighlight, "<span class=""" & StyleClass & """><a href=""" & LinkPage & Current.Server.UrlEncode(Query) & """>" & strHighlight & "</a></span>")
        End If

        Return strReturn
    End Function

    ' Returns field with search matches highlighted
    Public Overloads Shared Function HighlightText(ByVal Field As String, ByVal Query As String, ByVal StyleClass As String) As String
        If Query = "" Then Query = "QXYZ"
        Dim strReturn As String = Field
        Dim input As String = Field
        Dim pattern As String = Query
        Dim options As RegexOptions = RegexOptions.IgnoreCase
        Dim match As Match = Regex.Match(input, pattern, options)

        If match.Length > 0 Then
            Dim strHighlight As String = match.Value.ToString
            strReturn = Field.Replace(strHighlight, "<span class=""" & StyleClass & """>" & strHighlight & "</span>")
        End If

        Return strReturn
    End Function

    Public Shared Function RemoveHTML(ByVal Field As String) As String
        Dim strReturn As String
        strReturn = Replace(Field, "<[^>]*>", " ")
        Return strReturn
    End Function

End Class
