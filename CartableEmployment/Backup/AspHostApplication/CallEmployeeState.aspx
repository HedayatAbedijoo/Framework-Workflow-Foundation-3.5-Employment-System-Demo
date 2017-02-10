<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true"
    CodeBehind="CallEmployeeState.aspx.cs" Inherits="AspHostApplication.CallEmployeeState" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 400px">
        <tr>
            <td colspan="2" style="color: #0066CC; font-size: x-large">
                <b><i>Call Employee</i></b></td>
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
                Did You Call Employee
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
                <asp:Button ID="btnConfirm" runat="server" Text="اقدام" 
                    onclick="btnConfirm_Click" Width="96px" />
            </td>
        </tr>
    </table>
</asp:Content>
