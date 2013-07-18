Imports Microsoft.VisualBasic
Imports umbraco.Web.BaseRest
Imports umbraco.interfaces
Imports umbraco.Web

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
                Dim m As New umbraco.cms.businesslogic.media.Media(id)
                url = String.Format(format, m.getProperty("umbracoFile").Value, width)
            Catch ex As Exception
            End Try
            Return url
        End Function
    End Class
    Public Class StartupHandlers
        Implements IApplicationEventHandler, IApplicationStartupHandler

        Public Sub OnApplicationInitialized(httpApplication As umbraco.Web.UmbracoApplication, applicationContext As umbraco.Core.ApplicationContext) Implements umbraco.Web.IApplicationEventHandler.OnApplicationInitialized

        End Sub

        Public Sub OnApplicationStarted(httpApplication As umbraco.Web.UmbracoApplication, applicationContext As umbraco.Core.ApplicationContext) Implements umbraco.Web.IApplicationEventHandler.OnApplicationStarted
            AddHandler umbraco.presentation.masterpages.umbracoPage.Load, AddressOf umbracoPage_Load
        End Sub

        Public Sub OnApplicationStarting(httpApplication As umbraco.Web.UmbracoApplication, applicationContext As umbraco.Core.ApplicationContext) Implements umbraco.Web.IApplicationEventHandler.OnApplicationStarting

        End Sub

        Private Sub umbracoPage_Load(sender As Object, e As EventArgs)
            Dim up As umbraco.presentation.masterpages.umbracoPage = DirectCast(sender, umbraco.presentation.masterpages.umbracoPage)

            Dim script As String = "$(function(){$('.treePickerTitle').each(function(i,e){var container = $(this).parents('.propertyItemContent');var image = $('<img width=""200"" style=""display:block; margin-top:10px;"" />').appendTo(container);var input = container.find('input');var _default = input.val();input.on('change', function(){image.hide().prop('src','');var url = '/base/MediaPickerPreview/GetImageUrl/'+ input.val() +'/200';$.get(url, function(data){ if (data != ''){ image.prop('src', data).show(); } });}).trigger('change');setInterval(function () {if (input.val() !== _default ) { input.trigger('change'); _default  = input.val(); }}, 1000);});});"
            up.Page.ClientScript.RegisterClientScriptBlock(up.GetType, "MediaPreview", script, True)
        End Sub

    End Class
End Namespace

