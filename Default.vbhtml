@Code
    ' Handle user request to swap theme
    If Not Request.QueryString("Theme") Is Nothing Then
        Dim swapTo As String = Request.QueryString("Theme")
        InitializeApplication.SwapTheme(swapTo)
    End If
    ' set page theme and title properties
    Layout = Helpers.SetTheme().ToString
    PageData("Title") = HttpContext.Current.Application("DefaultTitle")

    ' Configure datasource connection
    Dim userDatabase As String = InitializeApplication.DataConn()
    ' Dimension variables for recordset paging 
    Dim pageSize As Integer = HttpContext.Current.Application("FeaturedNumRows") * HttpContext.Current.Application("FeaturedNumCols")
    Dim totalRows As Integer = RecordSetPaging.GetRowCount(userDatabase, "Item_ID", "webpages_Portfolio_Items", "Item_Active = 'Y' And Item_Featured > 0")
    If pageSize < 1 Then pageSize = totalRows
    Dim pageNum As String = Request.QueryString("Page")
    If Convert.ToInt32(pageNum) <= 0 Then pageNum = 1
    Dim offset As Integer = (pageNum - 1) * pageSize
    ' Get bootstrap column span from user column number setting
    Dim columnSpan As Integer = RecordSetPaging.GetColSpan(HttpContext.Current.Application("FeaturedNumCols"))

    ' Retrieve the dataset rows for this page
    Dim datasource As Database = Database.Open(userDatabase)
    Dim sqlQuery As String = "Select Item_ID, Item_Title, Item_Slug, Item_Tagline, Item_FeaturedImage, Item_FeaturedImageCaption, Item_Body, Item_Created FROM webpages_Portfolio_Items WHERE Item_Active = 'Y' And Item_Featured > 0 Order by Item_Featured Asc OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY"
    Dim rs_portfolio = datasource.Query(sqlQuery, offset, pageSize)
    Dim numItems As Integer = rs_portfolio.Count()
End Code

@Section Meta
   <meta name="Keywords" content="@HttpContext.Current.Application("DefaultKeywords")" />
   <meta name="Description" content="@HttpContext.Current.Application("DefaultDescription")" />
End Section

@Section featured
    <section class="featured">
        <div class="content-wrapper">
            @Helpers.SetTheme().ToString
            @If Not HttpContext.Current.Request.Cookies("ThemeCookie") Is Nothing Then
                Dim thecookie As String = HttpContext.Current.Request.Cookies("ThemeCookie").Value.ToString
                Dim session As String = HttpContext.Current.Session("SessionTheme")
                @<p>the cookie exists: @thecookie</p>
                @<p>the session is: @session</p>
                @<p>@ApplicationSecurity.Encode("22")</p>
                @<p>@ApplicationSecurity.Decode("MjIg")</p>      Else
                @<p>the cookie does not exist</p>
            End If
    Modify this template to jump-start your ASP.NET Web Pages application.
    <p>current : @pageNum total records: @totalRows page size: @pageSize </p>
</div>
    </section>
End Section

<div class="app-content">
    @If rs_portfolio.Any() Then
        Dim i As Integer = 1
        @<div Class="row">
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
        @<div Class="row">
            <div Class="row-homefeatured">
                <div Class="center-wrapper">
                    @For Each item In rs_portfolio
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
                                            <img src="~/Portfolio_Images/@item("Item_FeaturedImage")" alt="@item("Item_Title")" title="@item("Item_Title")" />
                                            <div class="item-featured-caption">@item("Item_FeaturedImageCaption")</div>
                                        </div>
                                    End If
                                    @Html.Raw(StringExtensions.TrimTextProperly(item("Item_Body"), HttpContext.Current.Application("FeaturedTextTrim"), -1, -1, ". . .", URL.Rewrite("PortfolioDetails?ItemID=" & ApplicationSecurity.Encode(item("Item_ID")) & "&amp;Title=" & item("Item_Slug")), "Continue Reading &raquo"))
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
                            @Html.Raw(RecordSetPaging.RecordsetNavBar(offset, pageSize, totalRows, "", "Page", HttpContext.Current.Application("RSNavStyle"), "rs-nav-wrapper"))
                        </div>
                    </div>
                </div>
            </div>
        End If
    Else
        @<div Class="row">
            <div Class="row-homefeatured">
                <div Class="center-wrapper">
                    <div Class="col-md-12"><h1>There Are No Records.</h1></div>
                </div>
            </div>
        </div>
    End If
</div>
