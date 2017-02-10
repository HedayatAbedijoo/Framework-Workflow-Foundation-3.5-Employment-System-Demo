<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="DeliverJobState.aspx.cs" Inherits="AspHostApplication.DeliverJobState" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 400px">
        <tr>
            <td colspan="2" style="font-size: x-large">
                <i><b>Contract</b></i>
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
                <asp:CheckBox ID="CheckBox1" runat="server" Text="Did You Sign the Contract" />
                &nbsp;
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
                <asp:Button ID="btnConfirm" runat="server" Text="اقدام" Width="112px" 
                    onclick="btnConfirm_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
