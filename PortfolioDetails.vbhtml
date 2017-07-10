@Code
    ' Handle user request to swap theme
    If Not Request.QueryString("Theme") Is Nothing Then
        Dim swapTo As String = Request.QueryString("Theme")
        InitializeApplication.SwapTheme(swapTo)
    End If

    Layout = Helpers.SetTheme().ToString

    ' get values
    Dim searchQuery As String = ApplicationSecurity.ValidateInput(Request.QueryString("Query"))
    Dim item As Integer = ApplicationSecurity.Decode(Request.QueryString("ItemID"))

    ' Get data
    Dim userDatabase As String = InitializeApplication.DataConn()
    Dim datasource As Database = Database.Open(userDatabase)
    Dim sqlQuery As String = "SELECT Item_Title, Item_Tagline, Item_MetaKeywords, Item_MetaDescription, Item_FeaturedImage, Item_FeaturedImageAlign, Item_FeaturedImageCaption, Item_Image1, Item_Image2, Item_Image3, Item_Image4, Item_Image5, Item_Body, Item_Created FROM webpages_Portfolio_Items WHERE Item_Active = 'Y' And Item_ID = @0"
    Dim rs_portfolio = datasource.QuerySingle(sqlQuery, item)

    ' Get page meta
    Dim strTitle As String
    Dim strDecription As String
    Dim strKeywords As String
    If Not (rs_portfolio) Is Nothing Then
        strTitle = rs_portfolio("Item_Title")
        strKeywords = rs_portfolio("Item_MetaKeywords")
        strDecription = rs_portfolio("Item_MetaDescription")
    Else
        strTitle = HttpContext.Current.Application("DefaultKeywords")
        strKeywords = HttpContext.Current.Application("DefaultTitle")
        strDecription = HttpContext.Current.Application("DefaultDescription")
    End If

    PageData("Title") = strTitle
End Code
@Section Meta
        <meta name="Keywords" content="@strKeywords" />
        <meta name="Description" content="@strDecription" />
End Section
<div class="app-content">
    <div class="row">
        <div class="row-itemdetails">
            <div class="center-wrapper">
                @If (rs_portfolio) Is Nothing Then
                    @<h1>The item isn't available.</h1>
                Else
                    @<text><div class="col-md-12">
                            <div class="item-details">
                                <hgroup class="title">
                                    <h1>@rs_portfolio("Item_Title")</h1>
                                </hgroup>
                                @If rs_portfolio("Item_Tagline") <> "" Then
                                    @<h3>@rs_portfolio("Item_Tagline")</h3>
                                End If
                                <div class="item-date">@Strings.Format(rs_portfolio("Item_Created"), HttpContext.Current.Application("DateFormat"))</div>
                                <div class="item-featured_img @rs_portfolio("Item_FeaturedImageAlign")"><img src="Portfolio_Images/@rs_portfolio("Item_FeaturedImage")" alt="@rs_portfolio("Item_Title")" /><div class="item-featured-caption">@rs_portfolio("Item_FeaturedImageCaption")</div></div>
                                @Html.Raw(StringExtensions.HighlightText(rs_portfolio("Item_Body"), searchQuery, "searchtexthighlight", "Search?Query="))
                            </div>
                        </div>
                    </text>
                End If
            </div>
        </div>
    </div>
</div>