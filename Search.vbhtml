@Code
    ' Handle user request to swap theme
    If Not Request.QueryString("Theme") Is Nothing Then
        Dim swapTo As String = Request.QueryString("Theme")
        InitializeApplication.SwapTheme(swapTo)
    End If

    ' set page properties
    Layout = Helpers.SetTheme().ToString
    PageData("Title") = HttpContext.Current.Application("SearchTitle")

    ' Get search query or set default if empty.
    Dim searchQuery As String = ApplicationSecurity.ValidateInput(Request.QueryString("Query"))
    Dim searchedFor As String = searchQuery
    If searchQuery = "" Then searchQuery = "QXYZ"

    ' Configure datasource connection
    Dim userDatabase As String = InitializeApplication.DataConn()
    ' Dimension variables for recordset paging
    Dim pageSize As Integer = HttpContext.Current.Application("SearchNumRows") * HttpContext.Current.Application("SearchNumCols")
    Dim totalRows As Integer = RecordSetPaging.GetRowCount(userDatabase, "Item_ID", "webpages_Portfolio_Items", "Item_Body Like '%" & searchQuery & "%' Or Item_Title Like '%" & searchQuery & "%' And Item_Active = 'Y'")
    ' Configure show all option using < 0 row value
    If pageSize < 1 Then pageSize = totalRows
    Dim pageNum As String = Request.QueryString("Page")
    If pageNum <= 0 Or pageNum = "" Then pageNum = 1
    pageNum = Convert.ToInt32(pageNum)
    Dim offset As Integer = (pageNum - 1) * pageSize
    ' Get bootstrap column span from page column setting
    Dim columnSpan As Integer = RecordSetPaging.GetColSpan(HttpContext.Current.Application("SearchNumCols"))

    ' retrieve the dataset rows for this page
    Dim datasource As Database = Database.Open(userDatabase)
    Dim sqlQuery As String = "Select Item_ID, Item_Title, Item_Slug, Item_Tagline, Item_FeaturedImage, Item_FeaturedImageCaption, Item_Body, Item_Created FROM webpages_Portfolio_Items WHERE Item_Body Like '%" & searchQuery & "%' Or Item_Title Like '%" & searchQuery & "%' And  Item_Active = 'Y' Order by Item_Featured Asc OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY"
    Dim rs_search = datasource.Query(sqlQuery, offset, pageSize)
    Dim numItems As Integer = rs_search.Count()
End Code

@Section Meta
    <meta name="Keywords" content="@HttpContext.Current.Application("DefaultKeywords")" />
    <meta name="Description" content="@HttpContext.Current.Application("DefaultDescription")" />
End Section

<div class="app-content">
    <div Class="row">
        <div Class="row-homefeatured">
            <div Class="center-wrapper">
                <div Class="col-md-12">
                    <hgroup class="title">
                        <h1>@PageData("Title")</h1>
                    </hgroup>
                    <div Class="content-count">
                        @RecordSetPaging.RecordSetStats(offset, pageNum, pageSize, totalRows)
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="formcontainer">
        <div class="forms">
            <form action="~/Search" method="get">
                <input id="Query" name="Query" type="text" />
                <input type="submit" value="Search" class="button" />
            </form>
        </div>
    </div>
    @If rs_search.Any() Then
        Dim i As Integer = 1
    @<div Class="row">
        <div Class="row-homefeatured">
            <div Class="center-wrapper">
                @For Each item In rs_search
                    @<text>
                        <div class="col-md-@columnSpan equal_height">
                            <div class="@IIf((i Mod 2), "alt_home-featured equal_height", "home-featured equal_height")">
                                <h2>@i.ToString. @item("Item_Title")</h2>
                                @If item("Item_Tagline") <> "" Then
                                    @<h3>@item("Item_Tagline")</h3>
                                End If
                                <div class="item-date">@Strings.Format(item("Item_Created"), HttpContext.Current.Application("DateFormat"))</div>
                                @If item("Item_FeaturedImage") <> "" Then
                                    @<div Class="thmb-featured">
                                        <img src="Portfolio_Images/@item("Item_FeaturedImage")" alt="@item("Item_Title")" title="@item("Item_Title")" />
                                        <div class="item-featured-caption">@item("Item_FeaturedImageCaption")</div>
                                    </div>
                                End If
                                @Html.Raw(StringExtensions.HighLightText(item("Item_Body"), HttpContext.Current.Application("FeaturedTextTrim"), -1, -1, ". . .", URL.Rewrite("PortfolioDetails?ItemID=" & ApplicationSecurity.Encode(item("Item_ID")) & "&amp;Title=" & item("Item_Slug") & "&amp;Query=" & Server.UrlEncode(searchQuery) & ""), "Continue Reading &raquo", searchQuery, "searchtexthighlight"))
                            </div>
                        </div>
                    </text>
                    i += 1
                Next
            </div>
        </div>
    </div>
    @If pageSize < totalRows Then
    @<div Class="row">
        <div Class="row-homefeatured">
            <div Class="center-wrapper">
                <div Class="col-md-12">
                    @Html.Raw(RecordSetPaging.RecordsetNavBar(offset, pageSize, totalRows, "Search?Query=" & Server.UrlEncode(searchQuery) & "", "Page", HttpContext.Current.Application("RSNavStyle"), "rs-nav-wrapper"))
                </div>
            </div>
        </div>
    </div>
    End If
    Else
    @<div Class="row">
        <div Class="row-homefeatured">
            <div Class="center-wrapper">
                <div Class="col-md-12">
                    @If Not String.IsNullOrEmpty(searchedFor) Then
                        @<h2>No matches found for @searchedFor.</h2>
                    End If
                </div>
            </div>
        </div>
    </div>
    End If
</div>