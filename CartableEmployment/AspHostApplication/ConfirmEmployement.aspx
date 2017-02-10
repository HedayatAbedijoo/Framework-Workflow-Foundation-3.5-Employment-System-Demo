<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="ConfirmEmployement.aspx.cs" Inherits="AspHostApplication.ConfirmEmployement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 400px">
        <tr>
            <td colspan="2" 
                style="font-size: x-large; font-weight: 700; font-style: italic; color: #0066CC;">
                Applican Succeed
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="CheckBox1" runat="server" Text=" " />
            </td>
            <td>
                ?Did you call him/her
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="CheckBox2" runat="server" Text=" " />
            </td>
            <td>
                ?Did you call resource manager
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnOk" runat="server" Text="اقدام" Width="95px" OnClick="btnOk_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
