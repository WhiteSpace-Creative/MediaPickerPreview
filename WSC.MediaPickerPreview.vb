Imports Microsoft.VisualBasic
Imports Umbraco.Web.BaseRest

Namespace WSC.MediaPickerPreview
    <RestExtension("MediaPickerPreview")>
    Public Class Base
        <RestExtensionMethod(ReturnXml:=False)>
        Shared Function GetImageUrl(id As Integer, width As Integer) As String
            Dim url As String = String.Empty
            Dim format As String = "{0}?w={1}"
            If System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/imagegen.dll")) Then
                format = "/imagegen.ashx?width={0}&amp;image={1}"
            End If
            If System.IO.File.Exists(HttpContext.Current.Server.MapPath("~/bin/ImageResizer.dll")) Then
                format = "{0}?w={1}"
            End If
            Try
                Dim m As New Umbraco.cms.businesslogic.media.Media(id)
                url = String.Format(format, m.getProperty("umbracoFile").Value, width)
                Return url

            Catch ex As Exception
                Return String.Empty
            End Try
        End Function
    End Class

    Public Class HttpModule
        Implements IHttpModule

        Public Sub Dispose() Implements System.Web.IHttpModule.Dispose

        End Sub

        Public Sub Init(context As System.Web.HttpApplication) Implements System.Web.IHttpModule.Init
            AddHandler context.PostMapRequestHandler, AddressOf context_PostMapRequest
        End Sub

        Private Sub context_PostMapRequest(sender As Object, e As EventArgs)
            Dim umbracoPath As String = (umbraco.GlobalSettings.Path & "/")
            Dim application As HttpApplication = CType(sender, HttpApplication)
            Dim context As HttpContext = application.Context
            If context.Request.CurrentExecutionFilePath.StartsWith(umbracoPath, StringComparison.CurrentCultureIgnoreCase) Then
                Dim page As Page = TryCast(HttpContext.Current.CurrentHandler, Page)
                If ((Not page Is Nothing) AndAlso Not umbraco.Web.UmbracoContext.Current.InPreviewMode) Then
                    AddHandler page.Load, AddressOf umbracoPage_Load
                End If
            End If
        End Sub

        Public Sub umbracoPage_Load(sender As Object, e As EventArgs)
            Dim p As Page = DirectCast(sender, Page)
            Dim script As String = "$(function(){$('.treePickerTitle').each(function(i,e){var title = $(this);var container = title.parent().parent();var path = $('<div />',{ style: 'margin-top: 10px;'}).appendTo(container);var image = $('<img />',{ style: 'width:200px;'}).appendTo(container);var input = container.find('input');var _default = input.val();input.on('change', function(){image.hide().prop('src','');var url='/base/MediaPickerPreview/GetImageUrl/'+ input.val() +'/200';$.get(url, function(data){if (data != ''){ image.prop('src', data).show();}});path.html(title.attr('title'));}).trigger('change');setInterval(function () {if (input.val() !== _default ) { input.trigger('change'); _default  = input.val(); }}, 1000);});});"
            p.Page.ClientScript.RegisterClientScriptBlock(p.GetType, "MediaPickerPreview", script, True)
        End Sub

    End Class
End Namespace

