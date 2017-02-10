<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="CompleteDocumentState.aspx.cs" Inherits="AspHostApplication.CompleteDocumentState" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 400px">
        <tr>
            <td colspan="2" style="font-size: x-large; color: #0066CC">
                <i><b>Complete Document</b></i></td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="CheckBox1" runat="server" />
            </td>
            <td>
                ?Did you deliver documents
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnConfirm" runat="server" Text="اقدام" Width="105px" 
                    onclick="btnConfirm_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
